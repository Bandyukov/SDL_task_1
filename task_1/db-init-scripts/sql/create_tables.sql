CREATE TABLE Franchises(
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) UNIQUE,
    code VARCHAR(255)
);

CREATE TABLE Cafe(
    id SERIAL PRIMARY KEY,
    franchise_id INT,
    name VARCHAR(255),
    city VARCHAR(255),
    type VARCHAR(255),
    popularity FLOAT,
    raiting FLOAT,
    FOREIGN KEY (franchise_id) REFERENCES Franchises(id)
);

CREATE TABLE Cities(
    id SERIAL PRIMARY KEY,
    name VARCHAR(255)
);

CREATE TABLE Types(
    id SERIAL PRIMARY KEY,
    name VARCHAR(255)
);