CREATE TABLE `product` (
  `ProductID` varchar(36) NOT NULL,
  `Name` varchar(45) NOT NULL,
  `Description` varchar(255) NOT NULL,
  `Price` decimal(13,2) NOT NULL,
  `Unit` varchar(45) NOT NULL,
  `Created` datetime NOT NULL,
  `LastUpdated` datetime NOT NULL,
  PRIMARY KEY (`ProductID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
