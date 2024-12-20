﻿using SpotifyAPI.Web;
using SpotifyAPILibrary.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyAPILibrary
{
    public class SpotifySessionSongRecord
    {
        public SpotifySongModel Song { get; set; }
        public int TimePlayed { get; set; }
    }

    public class SpotifyPlayerActiveState : ICloneable
    {
        private const int INACTIVE_TIME_SEC = 300;  // 5 min

        public bool IsPlaying { get; set; }
        public bool IsSessionActive { get; set; }
        public int SkipCount { get; set; }
        public DateTime SessionStartTime { get; set; }
        public DateTime SessionEndTime { get; set; }
        private Stopwatch InactivityStopwatch { get; set; }
        public SpotifySongModel CurrentSong { get; set; }
        public SpotifySongModel PreviousSong { get; set; }
        public List<SpotifySessionSongRecord> SongList { get; set; }

        private Stopwatch SessionStopwatch { get; set; }
        private Stopwatch SongStopwatch { get; set; }
        public decimal Position { get; set; }
        private int SpotifyAccountId { get; set; }
        private SpotifySessionJobQueue _queue;

        public SpotifyPlayerActiveState(int userId, SpotifySessionJobQueue queue)
        {
            SpotifyAccountId = userId;
            IsPlaying = false;
            SongList = new List<SpotifySessionSongRecord>();
            SessionStopwatch = new Stopwatch();
            SongStopwatch = new Stopwatch();
            InactivityStopwatch = new Stopwatch();
            _queue = queue;
            SkipCount = 0;
            Position = 0;
        }

        private bool CheckIfSongIsToBeAdded()
        {
            if (SongStopwatch.ElapsedMilliseconds > (CurrentSong.DurationMs / 2))
            {
                SongList.Add(new SpotifySessionSongRecord()
                {
                    Song = CurrentSong,
                    TimePlayed = (int) SongStopwatch.ElapsedMilliseconds / 1000
                });

                return true;
            }

            return false;
        }

        public (bool, string) UpdateActiveState(CurrentlyPlayingContext ctx)
        {
            var stateChanged = false;
            var stateChangeMessage = "";

            // check session is active
            if (IsPlaying && IsSessionActive)
            {
                if (InactivityStopwatch.IsRunning)
                    InactivityStopwatch.Reset();

                if (!SongStopwatch.IsRunning)
                    SongStopwatch.Start();

                if (!SessionStopwatch.IsRunning)
                {
                    SessionStopwatch.Start();

                    stateChanged = true;
                    stateChangeMessage = $"Spotify session for user {SpotifyAccountId} has resumed!";
                }
            }

            // check session is inactive
            if (!IsPlaying && IsSessionActive)
            {
                if (!InactivityStopwatch.IsRunning)
                {
                    InactivityStopwatch.Start();

                    stateChanged = true;
                    stateChangeMessage = $"Spotify session for user {SpotifyAccountId} has paused!";
                }

                if (SongStopwatch.IsRunning)
                    SongStopwatch.Stop();

                if (SessionStopwatch.IsRunning)
                    SessionStopwatch.Stop();

                if (InactivityStopwatch.Elapsed.TotalSeconds >= INACTIVE_TIME_SEC)
                {
                    InactivityStopwatch.Reset();
                    SessionStopwatch.Stop();
                    SongStopwatch.Stop();

                    CheckIfSongIsToBeAdded();

                    
                    SessionEndTime = DateTime.Now;

                    _queue.Queue.Enqueue(new SpotifySessionJob("AddSession",
                        new SpotifyPlayerSessionModel(SpotifyAccountId, SessionStartTime, SessionEndTime, SessionStopwatch.ElapsedMilliseconds / 1000, SongList, SkipCount)));

                    // reset session
                    IsSessionActive = false;
                    SongList = new List<SpotifySessionSongRecord>();
                    SkipCount = 0;

                    stateChanged = true;
                    stateChangeMessage = $"Spotify session for user {SpotifyAccountId} has ended! Session lasted {SessionStopwatch.Elapsed.TotalSeconds} seconds.";

                    SessionStopwatch.Reset();
                    SongStopwatch.Reset();
                }
            }

            // handle checking current song
            var wasPlaying = IsPlaying;

            IsPlaying = ctx?.IsPlaying ?? false;

            if (IsPlaying && ctx.Item is FullTrack)
            {
                var song = ctx.Item as FullTrack;

                if (CurrentSong is null || CurrentSong.Id != song.Id)
                {
                    var prevSong = PreviousSong;
                    var addedSong = false;

                    if (CurrentSong is not null && CurrentSong.Id != song.Id)
                    {
                        prevSong = CurrentSong;

                        addedSong = CheckIfSongIsToBeAdded();
                        SongStopwatch.Restart();

                        stateChanged = true;
                    }

                    if (CurrentSong is null && song is not null)
                        SongStopwatch.Start();

                    if (addedSong)
                        PreviousSong = prevSong;
                    else if (!addedSong && CurrentSong != null)
                        SkipCount++;

                    CurrentSong = new SpotifySongModel(song);

                    stateChangeMessage = $"Spotify session for user {SpotifyAccountId} has changed songs! {(prevSong != null ? $"Previous song: \"{prevSong.Name}\", " : "")}Current Song: \"{CurrentSong.Name}\".";
                }

                Position = ctx.ProgressMs;
            }

            if (IsPlaying && !IsSessionActive)
            {
                IsSessionActive = true;
                SessionStopwatch.Start();
                SessionStartTime = DateTime.Now;

                stateChanged = true;
                stateChangeMessage = $"Spotify session for user {SpotifyAccountId} has started! Current song: {CurrentSong.Name}";
            }

            return (stateChanged, stateChangeMessage);
        }

        public double GetTimeInactive()
        {
            return InactivityStopwatch.Elapsed.TotalSeconds;
        }

        public double GetSessionTime()
        {
            return SessionStopwatch.Elapsed.TotalSeconds;
        }

        public double GetActiveSongTime()
        {
            return SongStopwatch.Elapsed.TotalMilliseconds;
        }

        public SpotifyPlayerStateModel SerializePlayerState()
        {
            return new SpotifyPlayerStateModel(this);
        }

        public SpotifyPlayerActiveState ShallowClone()
        {
            return (SpotifyPlayerActiveState) this.MemberwiseClone();
        }

        object ICloneable.Clone()
        {
            throw new NotImplementedException();
        }
    }

    public class SpotifyPlayerStateModel
    {
        public bool IsPlaying { get; set; }
        public bool IsSessionActive { get; set; }
        public DateTime StartTime { get; set; }
        public SpotifySongModel CurrentSong { get; set; }
        public SpotifySongModel PreviousSong { get; set; }
        public List<SpotifySessionSongRecord> SongList { get; set; }
        public long TimeInactive { get; set; }
        public long SessionLength { get; set; }
        public long SongPositionMs { get; set; }
        public long ProgressMs { get; set; }
        public int SkipCount { get; set; }

        public SpotifyPlayerStateModel(SpotifyPlayerActiveState state)
        {
            IsPlaying = state.IsPlaying;
            IsSessionActive = state.IsSessionActive;
            CurrentSong = state.CurrentSong;
            PreviousSong = state.PreviousSong;
            SongList = state.SongList;
            StartTime = state.SessionStartTime;
            TimeInactive = (long) state.GetTimeInactive();
            SessionLength = (long) state.GetSessionTime();
            SongPositionMs = (long) state.GetActiveSongTime();
            ProgressMs = (long) state.Position;
            SkipCount = state.SkipCount;
        }
    }

    public class SpotifyPlayerSessionModel
    {
        public int SpotifyAccountId { get; set; }
        public long SessionLength { get; set; }
        public List<SpotifySessionSongRecord> SongList { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int SkipCount { get; set; }

        public SpotifyPlayerSessionModel(int accountId, DateTime startTime, DateTime endTime, long sessionLength, List<SpotifySessionSongRecord> list, int skipCount=0)
        {
            SpotifyAccountId = accountId;
            StartTime = startTime;
            EndTime = endTime;
            SessionLength = sessionLength;
            SongList = list;
            SkipCount = skipCount;
        }
    }
}
