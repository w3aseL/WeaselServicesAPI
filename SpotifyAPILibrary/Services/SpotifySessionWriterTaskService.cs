using DataAccessLayer;
using DataAccessLayer.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyAPILibrary
{
    public class SpotifySessionJob
    {
        public string JobType { get; set; }
        public object JobData { get; set; }

        public SpotifySessionJob(string jobType, object jobData)
        {
            JobType = jobType;
            JobData = jobData;
        }
    }

    public class SpotifySessionJobQueue
    {
        public ConcurrentQueue<SpotifySessionJob> Queue { get; set; }

        public SpotifySessionJobQueue()
        {
            Queue = new ConcurrentQueue<SpotifySessionJob>();
        }
    }

    public class SpotifySessionWriterTaskService : BackgroundService
    {
        private readonly TimeSpan timespan = TimeSpan.FromSeconds(30);
        private readonly IServiceProvider _serviceProvider;
        private ILogger<SpotifySessionWriterTaskService> _logger;
        private SpotifySessionJobQueue _queue;

        public SpotifySessionWriterTaskService(IServiceProvider serviceProvider, ILogger<SpotifySessionWriterTaskService> logger, SpotifySessionJobQueue queue)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _queue = queue;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using PeriodicTimer timer = new PeriodicTimer(timespan);
            using IServiceScope scope = _serviceProvider.CreateAsyncScope();

            var ctx = scope.ServiceProvider.GetRequiredService<ServicesAPIContext>();

            while (!stoppingToken.IsCancellationRequested &&
                await timer.WaitForNextTickAsync(stoppingToken))
            {
                while (_queue.Queue.TryDequeue(out var job))
                {
                    if (job.JobType == "AddSession")
                    {
                        using var tx = ctx.Database.BeginTransaction();

                        try
                        {
                            var currentState = job.JobData as SpotifyPlayerSessionModel;

                            if (currentState is null || currentState is not SpotifyPlayerSessionModel)
                                throw new Exception("The data provided is not in the active state model format!");

                            var session = new SpotifySession
                            {
                                AccountId = currentState.SpotifyAccountId,
                                StartTime = currentState.StartTime,
                                EndTime = currentState.EndTime,
                                SongCount = currentState.SongList.Count,
                                TimeListening = (int) currentState.SessionLength
                            };
                            ctx.SpotifySessions.Add(session);
                            ctx.SaveChanges();

                            foreach (var songRec in currentState.SongList)
                            {
                                var song = songRec.Song;

                                var dbSong = ctx.SpotifySongs.FirstOrDefault(s => s.Id == song.Id);

                                if (dbSong is null)
                                {
                                    // create song database object
                                    dbSong = new SpotifySong
                                    {
                                        Id = song.Id,
                                        Title = song.Name,
                                        Url = song.Uri,
                                        Duration = song.DurationMs / 1000
                                    };
                                    ctx.SpotifySongs.Add(dbSong);
                                    ctx.SaveChanges();

                                    // fetch artists or create artist objects and add song associations
                                    foreach (var artist in song.Artists)
                                    {
                                        var dbArtist = ctx.SpotifyArtists.FirstOrDefault(a => a.Id == artist.Id);

                                        if (dbArtist is null)
                                        {
                                            dbArtist = new SpotifyArtist()
                                            {
                                                Id = artist.Id,
                                                Name = artist.Name,
                                                Url = artist.Uri
                                            };
                                            ctx.SpotifyArtists.Add(dbArtist);
                                            ctx.SaveChanges();
                                        }

                                        ctx.SpotifySongArtists.Add(new SpotifySongArtist()
                                        {
                                            SongId = dbSong.Id,
                                            ArtistId = dbArtist.Id
                                        });
                                        ctx.SaveChanges();
                                    }

                                    // fetch album or create album object and add song associations
                                    var dbAlbum = ctx.SpotifyAlbums.FirstOrDefault(a => a.Id == song.Album.Id);

                                    if (dbAlbum is null)
                                    {
                                        dbAlbum = new SpotifyAlbum()
                                        {
                                            Id = song.Album.Id,
                                            Title = song.Album.Name,
                                            Url = song.Album.Uri,
                                            ArtworkUrl = song.Album.ArtworkURL
                                        };
                                        ctx.SpotifyAlbums.Add(dbAlbum);
                                        ctx.SaveChanges();

                                        // fetch artists or create artist objects and add artist-album associations
                                        foreach (var artist in song.Album.Artists)
                                        {
                                            var dbArtist = ctx.SpotifyArtists.FirstOrDefault(a => a.Id == artist.Id);

                                            if (dbArtist is null)
                                            {
                                                dbArtist = new SpotifyArtist()
                                                {
                                                    Id = artist.Id,
                                                    Name = artist.Name,
                                                    Url = artist.Uri
                                                };
                                                ctx.SpotifyArtists.Add(dbArtist);
                                                ctx.SaveChanges();
                                            }

                                            ctx.SpotifyArtistAlbums.Add(new SpotifyArtistAlbum()
                                            {
                                                AlbumId = dbAlbum.Id,
                                                ArtistId = dbArtist.Id
                                            });
                                            ctx.SaveChanges();
                                        }
                                    }

                                    ctx.SpotifySongAlbums.Add(new SpotifySongAlbum()
                                    {
                                        SongId = dbSong.Id,
                                        AlbumId = dbAlbum.Id
                                    });
                                    ctx.SaveChanges();
                                }

                                // create track play for sesison
                                var trackPlay = new SpotifyTrackPlay
                                {
                                    SongId = dbSong.Id,
                                    SessionId = session.Id,
                                    TimePlayed = songRec.TimePlayed
                                };
                                ctx.SpotifyTrackPlays.Add(trackPlay);
                                ctx.SaveChanges();
                            }

                            tx.Commit();
                            _logger.LogInformation($"Session created for user with ID { currentState.SpotifyAccountId }. Listening time: { currentState.SessionLength } seconds");
                        }
                        catch (Exception e)
                        {
                            _logger.LogError($"An error has occurred while trying to generate a session. Msg: {e.Message}", e);
                            tx.Rollback();
                        }
                    }
                }
            }
        }
    }
}
