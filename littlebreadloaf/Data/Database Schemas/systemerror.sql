CREATE TABLE `systemerror` (
  `ErrorID` varchar(36) NOT NULL,
  `RequestID` varchar(255) NOT NULL,
  `Path` varchar(255) NOT NULL,
  `Error` longtext NOT NULL,
  `Occurred` datetime NOT NULL,
  PRIMARY KEY (`ErrorID`),
  KEY `RequestID` (`RequestID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
