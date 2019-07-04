CREATE TABLE IF NOT EXISTS Learn (    
  LearnID bigserial PRIMARY KEY,
  DevisesID bigint,
  LearnName text,
  LearnPath text,
  FOREIGN KEY (DevisesID) REFERENCES Devises (DevisesID)
);
ALTER SEQUENCE Learn_LearnID_seq RESTART WITH 4294967296; /* 2^32 */