CREATE TABLE `cartitem` (
  `CartItemID` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL,
  `CartID` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL,
  `ProductID` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL,
  `Quantity` int(11) NOT NULL,
  `Price` decimal(13,2) NOT NULL,
  `Created` datetime NOT NULL,
  PRIMARY KEY (`CartItemID`),
  KEY `CartID` (`CartID`) /*!80000 INVISIBLE */,
  KEY `ProductID` (`ProductID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_bin;
