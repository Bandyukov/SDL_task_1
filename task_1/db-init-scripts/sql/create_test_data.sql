INSERT INTO Franchises (name, code) VALUES
('Foodmania', 'F123'),
('Appetizer', 'A456'),
('Muchachos', 'I789'),
('Uncle Pepe', 'U101');

INSERT INTO Cafe (franchise_id, name, city, type, popularity, raiting) VALUES
(1, 'Cafeteria', 'Moscow', 'average', 4322, 4.3),
(3, 'Maros', 'Moscow', 'average', 7654, 4.6),
(3, 'PastaPizza', 'Kazan', 'cheap', 3245, 3.7),
(2, 'Wine and Go', 'Sochi', 'expensive', 5478, 4.9),
(1, 'Kavkaz', 'Sochi', 'average', 7890, 4.1),
(1, 'Kremlin bufet', 'Moscow', 'expensive', 9435, 4.5);

INSERT INTO Cities (name) VALUES
('Moscow'),
('Kazan'),
('Sochi');

INSERT INTO Types (name) VALUES
('cheap'),
('average'),
('expensive');