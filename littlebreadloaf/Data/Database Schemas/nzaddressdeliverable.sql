CREATE TABLE `nzaddressdeliverable` (
  `address_id` int(11) DEFAULT NULL,
  `suburb_locality` text,
  `address_number` int(11) DEFAULT NULL,
  `full_road_name` text,
  `address_number_high` text,
  `full_address_number` text,
  `full_address` text,
  `address_number_suffix` text,
  `town_city` text,
  `unit_value` text,
  `address_type` text,
  FULLTEXT KEY `Full Address` (`full_address`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
