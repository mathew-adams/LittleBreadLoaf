CREATE TABLE `productsuggestion` (
  `ProductSuggestionID` varchar(36) NOT NULL,
  `ProductID` varchar(36) NOT NULL,
  `Description` varchar(255) NOT NULL,
  `Created` datetime NOT NULL,
  PRIMARY KEY (`ProductSuggestionID`),
  KEY `ProductID` (`ProductID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
