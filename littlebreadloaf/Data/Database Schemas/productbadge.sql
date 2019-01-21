CREATE TABLE `productbadge` (
  `ProductBadgeID` varchar(36) NOT NULL,
  `ProductID` varchar(36) NOT NULL,
  `Title` varchar(50) NOT NULL,
  `Description` varchar(255) NOT NULL,
  PRIMARY KEY (`ProductBadgeID`),
  KEY `ProductID` (`ProductID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
