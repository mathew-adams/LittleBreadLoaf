CREATE TABLE `cart` (
  `CartID` varchar(36) NOT NULL,
  `UserID` varchar(36) DEFAULT NULL,
  `Created` datetime NOT NULL,
  `CheckedOut` datetime NOT NULL,
  PRIMARY KEY (`CartID`),
  KEY `UserID` (`UserID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
