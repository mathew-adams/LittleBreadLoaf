
CREATE TABLE `categorytoblog` (
  `CategoryToBlogID` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL,
  `BlogCategoryID` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL,
  `BlogID` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL,
  PRIMARY KEY (`CategoryToBlogID`),
  KEY `CategoryID` (`BlogCategoryID`),
  KEY `BlogID` (`BlogID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
