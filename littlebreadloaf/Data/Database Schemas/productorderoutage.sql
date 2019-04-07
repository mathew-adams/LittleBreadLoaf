CREATE TABLE `productorderoutage` (
  `OutageID` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL,
  `Start` datetime NOT NULL,
  `Stop` datetime NOT NULL,
  PRIMARY KEY (`OutageID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_bin;
