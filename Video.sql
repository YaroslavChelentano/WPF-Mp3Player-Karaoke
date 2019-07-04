CREATE TABLE IF NOT EXISTS Video (    
  VideoID bigserial PRIMARY KEY,
  ArtistID bigint,
  VideoName text,
  VideoDuration text,
  VideoPath text,
  FOREIGN KEY (ArtistID) REFERENCES Artist (ArtistID)
);