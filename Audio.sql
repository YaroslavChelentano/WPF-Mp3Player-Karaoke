CREATE TABLE IF NOT EXISTS Audio (    
  AudioID bigserial PRIMARY KEY,
  AlbumID bigint,
  ArtistID bigint,
  AudioName text,
  AudioDuration text,
  AudioPath text,
  FOREIGN KEY (AlbumID) REFERENCES Album (AlbumID),
  FOREIGN KEY (ArtistID) REFERENCES Artist (ArtistID)
);