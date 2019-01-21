CREATE TABLE `productingredient` (
  `ProductIngredientID` varchar(36) NOT NULL,
  `ProductID` varchar(36) NOT NULL,
  `Description` varchar(100) NOT NULL,
  `Created` datetime NOT NULL,
  PRIMARY KEY (`ProductIngredientID`),
  KEY `ProductID` (`ProductID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
