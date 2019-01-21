CREATE TABLE `cartitem` (
  `CartItemID` varchar(36) NOT NULL,
  `CartID` varchar(36) NOT NULL,
  `ProductID` varchar(36) NOT NULL,
  `Quantity` int(11) NOT NULL,
  `Price` decimal(13,2) NOT NULL,
  `Created` datetime NOT NULL,
  PRIMARY KEY (`CartItemID`),
  KEY `CartID` (`CartID`) /*!80000 INVISIBLE */,
  KEY `ProductID` (`ProductID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
