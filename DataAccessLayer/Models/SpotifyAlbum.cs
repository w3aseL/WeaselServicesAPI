﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class SpotifyAlbum
{
    public string Id { get; set; }

    public string Title { get; set; }

    public string Url { get; set; }

    public string ArtworkUrl { get; set; }

    public virtual ICollection<SpotifyArtistAlbum> SpotifyArtistAlbums { get; set; } = new List<SpotifyArtistAlbum>();

    public virtual ICollection<SpotifySongAlbum> SpotifySongAlbums { get; set; } = new List<SpotifySongAlbum>();
}