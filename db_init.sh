#!/bin/bash

DB_FILE="database.db"

# Check if the database file exists
if [ ! -f "$DB_FILE" ]; then
    echo "Database file not found. Creating $DB_FILE..."
    sqlite3 "$DB_FILE" "VACUUM;"
    echo "Database created successfully."
else
    echo "Database already exists."
fi

echo "Creating Database tables"

sqlite3 database.db <<EOF

CREATE TABLE Resource (
    resourceID int PRIMARY KEY,
    name VARCHAR(50),
    current_amount int
);

CREATE TABLE Soldier (
    soldierID int PRIMARY KEY,
    name VARCHAR(50) NOT NULL, 
    level int DEFAULT 1,
    hp int,
    atk int,
    def int,
    role VARCHAR(50)   
);

CREATE TABLE Weapon (
    weaponID int PRIMARY KEY,
    damage int, 
    name VARCHAR(100),
    cost int,
    resource_amount int,
    resource_type int,
    FOREIGN KEY (resource_type) REFERENCES Resource(resourceID)
);

CREATE TABLE Ability (
    abilityID int PRIMARY KEY,
    name VARCHAR(100)
);

CREATE TABLE Equipment (
    equipmentID int PRIMARY KEY,
    name VARCHAR(100),
    hp int,
    def int,
    atk int, 
    cost int,
    resource_amount int,
    resource_type int,
    FOREIGN KEY (resource_type) REFERENCES Resource(resourceID)
);

CREATE TABLE Infrastructure (
    buildingID int PRIMARY KEY,
    name VARCHAR(100),
    level int, 
    cost int, 
    resource_amount,
    resource_type int,
    FOREIGN KEY (resource_type) REFERENCES Resource(resourceID)
);

CREATE TABLE Weather ( 
    name VARCHAR(50) PRIMARY KEY,
    atk_effect int,
    def_effect int,
    hp_effect int
);

CREATE TABLE Terrain ( 
    name VARCHAR(50) PRIMARY KEY,
    atk_effect int,
    def_effect int,
    hp_effect int
);

CREATE TABLE Mission (
    missionID int PRIMARY KEY,
    name VARCHAR(200),
    description VARCHAR(1000),
    difficulty int,
    reward_money int,
    reward_amount int,
    reward_resource int,
    terrain VARCHAR(50),
    weather VARCHAR(50),
    FOREIGN KEY (reward_resource) REFERENCES Resource(resourceID),
    FOREIGN KEY(terrain) REFERENCES Terrain(name),
    FOREIGN KEY(weather) REFERENCES Weather(name)
);

EOF

echo "Done"