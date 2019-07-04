CREATE TABLE IF NOT EXISTS Karaoke (    
  KaraokeID bigserial PRIMARY KEY,
  VideoID bigint,
  ArtistID bigint,
  VideoName text,
  FOREIGN KEY (VideoID) REFERENCES Video (VideoID),
  FOREIGN KEY (ArtistID) REFERENCES Artist (ArtistID)
);
