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
    resource_id int PRIMARY KEY,
    name VARCHAR(255),
    current_amount int
);

CREATE TABLE IF NOT EXISTS Soldier (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL,
    role TEXT NOT NULL,
    level INTEGER DEFAULT 1,
    exp INTEGER DEFAULT 0,
    health INTEGER NOT NULL,
    attack INTEGER NOT NULL,
    defense INTEGER NOT NULL
);

CREATE TABLE Weapon (
    weapon_id int PRIMARY KEY,
    name VARCHAR(255),
    description VARCHAR(1000),
    damage int, 
    cost int,
    resource_amount int,
    resource_type int,
    FOREIGN KEY (resource_type) REFERENCES Resource(resource_id)
);

CREATE TABLE Ability (
    ability_id int PRIMARY KEY,
    name VARCHAR(255),
    description VARCHAR(255)
);

CREATE TABLE Equipment (
    equipment_id int PRIMARY KEY,
    name VARCHAR(255),
    hp int,
    def int,
    atk int, 
    cost int,
    resource_amount int,
    resource_type int,
    FOREIGN KEY (resource_type) REFERENCES Resource(resource_id)
);

CREATE TABLE Infrastructure (
    building_id int PRIMARY KEY,
    name VARCHAR(255),
    description VARCHAR(1000),
    level int, 
    cost int, 
    resource_amount,
    resource_type int,
    FOREIGN KEY (resource_type) REFERENCES Resource(resource_id)
);

CREATE TABLE Weather ( 
    name VARCHAR(255) PRIMARY KEY,
    atk_effect int,
    def_effect int,
    hp_effect int
);

CREATE TABLE Terrain ( 
    name VARCHAR(255) PRIMARY KEY,
    atk_effect int,
    def_effect int,
    hp_effect int
);

CREATE TABLE Mission (
    mission_id int PRIMARY KEY,
    name VARCHAR(200),
    description VARCHAR(1000),
    difficulty int,
    reward_money int,
    reward_amount int,
    reward_resource int,
    terrain VARCHAR(50),
    weather VARCHAR(50),
    FOREIGN KEY (reward_resource) REFERENCES Resource(resource_id),
    FOREIGN KEY(terrain) REFERENCES Terrain(name),
    FOREIGN KEY(weather) REFERENCES Weather(name)
);


CREATE TABLE TECHNOLOGY (
    tech_id SERIAL PRIMARY KEY,
    tech_name VARCHAR(255) NOT NULL,
    description TEXT,
    cost_money DECIMAL(10,2) DEFAULT 0,
    cost_resources_id INT,
    cost_resources_amount INT DEFAULT 0,
    cost_points INT DEFAULT 0,
    prerequisite_id INT,
    unlocks_role_id INT,
    unlocks_weapon_id INT,
    unlocks_equipment_id INT,
    FOREIGN KEY (cost_resources_id) REFERENCES RESOURCES(resource_id),
    FOREIGN KEY (prerequisite_id) REFERENCES TECHNOLOGY(tech_id),
    FOREIGN KEY (unlocks_role_id) REFERENCES ROLE(role_id),
    FOREIGN KEY (unlocks_weapon_id) REFERENCES WEAPON(weapon_id),
    FOREIGN KEY (unlocks_equipment_id) REFERENCES EQUIPMENT(equipment_id)
);

CREATE TABLE MISSION_ASSIGNMENT (
    mission_id INT,
    soldier_id INT,
    PRIMARY KEY (mission_id, soldier_id),
    FOREIGN KEY (mission_id) REFERENCES MISSION(mission_id),
    FOREIGN KEY (soldier_id) REFERENCES SOLDIER(soldier_id)
);

CREATE TABLE MISSION_ENEMY (
    mission_id INT,
    et_id INT,
    count INT DEFAULT 0,
    PRIMARY KEY (mission_id, et_id),
    FOREIGN KEY (mission_id) REFERENCES MISSION(mission_id),
    FOREIGN KEY (et_id) REFERENCES ENEMY_TYPES(et_id)
);

CREATE TABLE ENEMY_TYPES (
    et_ID SERIAL PRIMARY KEY,
    et_name VARCHAR(255) NOT NULL,
    HP INT NOT NULL,
    base_ATK INT NOT NULL,
    base_DPS INT NOT NULL
);

-- Insert into Resource
INSERT INTO Resource VALUES
(1, 'Iron', 1000),
(2, 'Wood', 500),
(3, 'Gold', 300),
(4, 'Stone', 700),
(5, 'Crystal', 200);

-- Insert into Soldier
INSERT INTO Soldier VALUES
(1, 'John', 'Engineer', 1, 0, 100, 10, 5),
(2, 'Jane', 'Sniper', 1, 0, 80, 20, 10),
(3, 'Jack', 'Tank', 1, 0, 200, 30, 15),
(4, 'Jill', 'Medic', 1, 0, 150, 25, 12),
(5, 'Boss', 'Scout', 1, 0, 70, 50, 25);

-- Insert into Weapon
INSERT INTO Weapon VALUES
(1, 'Rifle', 'Standard rifle', 30, 100, 10, 1),
(2, 'Sniper', 'Long-range weapon', 50, 150, 15, 2),
(3, 'Shotgun', 'Close combat weapon', 40, 120, 12, 3),
(4, 'Pistol', 'Sidearm', 20, 80, 8, 4),
(5, 'Machine Gun', 'Rapid fire', 35, 200, 20, 5);

-- Insert into Ability
INSERT INTO Ability VALUES
(1, 'Sprint', 'Move faster for 10s'),
(2, 'Shield', 'Reduce damage taken for 5s'),
(3, 'Heal', 'Restore 50 HP'),
(4, 'Camouflage', 'Avoid detection'),
(5, 'Berserk', 'Increase attack power for 8s');

-- Insert into Equipment
INSERT INTO Equipment VALUES
(1, 'Helmet', 10, 5, 2, 50, 5, 1),
(2, 'Armor', 30, 15, 5, 100, 10, 2),
(3, 'Boots', 5, 2, 1, 30, 3, 3),
(4, 'Gloves', 3, 1, 1, 20, 2, 4),
(5, 'Shield', 20, 10, 3, 80, 8, 5);

-- Insert into Infrastructure
INSERT INTO Infrastructure VALUES
(1, 'Barracks', 'Train soldiers', 1, 500, 50, 1),
(2, 'Armory', 'Store weapons', 2, 600, 60, 2),
(3, 'Factory', 'Produce equipment', 3, 700, 70, 3),
(4, 'HQ', 'Command center', 4, 800, 80, 4),
(5, 'Research Lab', 'Develop technologies', 5, 900, 90, 5);

-- Insert into Weather
INSERT INTO Weather VALUES
('Sunny', 5, 5, 0),
('Rainy', -2, 3, 2),
('Stormy', -5, 5, -3),
('Foggy', 0, 0, 0),
('Snowy', -3, 2, -1);

-- Insert into Terrain
INSERT INTO Terrain VALUES
('Plains', 5, 5, 0),
('Mountains', -2, 3, 2),
('Forest', -3, 4, 1),
('Desert', -5, -3, -2),
('Swamp', -3, -2, -1);

-- Insert into Mission
INSERT INTO Mission VALUES
(1, 'Recon', 'Scout enemy territory', 2, 100, 10, 1, 'Plains', 'Sunny'),
(2, 'Sabotage', 'Destroy enemy supplies', 4, 200, 15, 2, 'Forest', 'Rainy'),
(3, 'Rescue', 'Save hostages', 3, 150, 12, 3, 'Mountains', 'Stormy'),
(4, 'Assault', 'Attack enemy base', 5, 300, 20, 4, 'Desert', 'Foggy'),
(5, 'Defense', 'Hold the line', 3, 180, 14, 5, 'Swamp', 'Snowy');

-- Insert into TECHNOLOGY
INSERT INTO TECHNOLOGY VALUES
(1, 'Advanced Tactics', 'Enhance soldier strategies', 100, 1, 10, 50, NULL, 1, 1, 1),
(2, 'Enhanced Armor', 'Increase soldier durability', 200, 2, 20, 100, 1, 2, 2, 2),
(3, 'Laser Weapons', 'Unlock laser guns', 300, 3, 30, 150, 2, 3, 3, 3),
(4, 'Cybernetics', 'Augment soldiers', 400, 4, 40, 200, 3, 4, 4, 4),
(5, 'Stealth Tech', 'Improve infiltration', 500, 5, 50, 250, 4, 5, 5, 5);

-- Insert into MISSION_ASSIGNMENT
INSERT INTO MISSION_ASSIGNMENT VALUES
(1, 1),
(2, 2),
(3, 3),
(4, 4),
(5, 5);

-- Insert into MISSION_ENEMY
INSERT INTO MISSION_ENEMY VALUES
(1, 1, 10),
(2, 2, 15),
(3, 3, 20),
(4, 4, 25),
(5, 5, 30);

-- Insert into ENEMY_TYPES
INSERT INTO ENEMY_TYPES VALUES
(1, 'Grunt', 100, 10, 5),
(2, 'Sniper', 80, 20, 10),
(3, 'Tank', 200, 30, 15),
(4, 'Elite', 150, 25, 12),
(5, 'Boss', 500, 50, 25);
EOF



echo "Done"
