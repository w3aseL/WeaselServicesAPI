﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class SpotifySong
{
    public string Id { get; set; }

    public string Title { get; set; }

    public int Duration { get; set; }

    public string Url { get; set; }

    public virtual ICollection<SpotifySongAlbum> SpotifySongAlbums { get; set; } = new List<SpotifySongAlbum>();

    public virtual ICollection<SpotifySongArtist> SpotifySongArtists { get; set; } = new List<SpotifySongArtist>();

    public virtual ICollection<SpotifyTrackPlay> SpotifyTrackPlays { get; set; } = new List<SpotifyTrackPlay>();
}