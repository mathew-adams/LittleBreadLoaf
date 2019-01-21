CREATE TABLE `productimage` (
  `ProductImageID` varchar(36) NOT NULL,
  `ProductID` varchar(36) NOT NULL,
  `Title` varchar(255) NOT NULL,
  `FileLocation` varchar(1000) NOT NULL,
  `PrimaryImage` tinyint(1) NOT NULL,
  PRIMARY KEY (`ProductImageID`),
  KEY `ProductID` (`ProductID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
