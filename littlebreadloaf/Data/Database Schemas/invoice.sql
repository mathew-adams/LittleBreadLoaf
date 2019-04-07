CREATE TABLE `invoice` (
  `InvoiceID` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL,
  `ProductOrderID` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL,
  `Created` datetime NOT NULL,
  `Due` datetime NOT NULL,
  PRIMARY KEY (`InvoiceID`),
  KEY `ProductOrderID` (`ProductOrderID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_bin;
