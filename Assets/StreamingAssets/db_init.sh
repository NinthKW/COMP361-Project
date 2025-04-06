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
    resource_id INT PRIMARY KEY,
    name VARCHAR(255),
    current_amount INT
);

CREATE TABLE Soldier (
    soldier_id INT PRIMARY KEY,
    name VARCHAR(255) NOT NULL, 
    level INT DEFAULT 1,
    hp INT,
    max_hp INT,
    atk INT,
    def INT,
    role VARCHAR(50)   
);

CREATE TABLE Weapon (
    weapon_id INT PRIMARY KEY,
    name VARCHAR(255),
    description VARCHAR(1000),
    damage INT, 
    cost INT,
    resource_amount INT,
    resource_type INT,
    unlocked BOOL,
    FOREIGN KEY (resource_type) REFERENCES Resource(resource_id)
);

CREATE TABLE Equipment (
    equipment_id INT PRIMARY KEY,
    name VARCHAR(255),
    hp INT,
    def INT,
    atk INT, 
    cost INT,
    resource_amount INT,
    resource_type INT,
    unlocked BOOL,
    FOREIGN KEY (resource_type) REFERENCES Resource(resource_id)
);

CREATE TABLE Infrastructure (
    building_id INT PRIMARY KEY,
    name VARCHAR(255),
    description VARCHAR(1000),
    level INT, 
    cost INT, 
    resource_amount INT,
    resource_type INT,
    unlocked BOOL,
    placed BOOL,
    x INT,
    y INT,
    FOREIGN KEY (resource_type) REFERENCES Resource(resource_id)
);

CREATE TABLE Weather ( 
    name VARCHAR(255) PRIMARY KEY,
    atk_effect INT,
    def_effect INT,
    hp_effect INT
);

CREATE TABLE Terrain ( 
    name VARCHAR(255) PRIMARY KEY,
    atk_effect INT,
    def_effect INT,
    hp_effect INT
);

CREATE TABLE Mission (
    mission_id INT PRIMARY KEY,
    name VARCHAR(200),
    description VARCHAR(1000),
    difficulty INT,
    reward_money INT,
    reward_amount INT,
    reward_resource INT,
    terrain VARCHAR(50),
    weather VARCHAR(50),
    unlocked BOOL,
    FOREIGN KEY (reward_resource) REFERENCES Resource(resource_id),
    FOREIGN KEY(terrain) REFERENCES Terrain(name),
    FOREIGN KEY(weather) REFERENCES Weather(name)
);

CREATE TABLE TECHNOLOGY (
    tech_id INT PRIMARY KEY,
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
    unlocked BOOL,
    FOREIGN KEY (cost_resources_id) REFERENCES Resource(resource_id),
    FOREIGN KEY (prerequisite_id) REFERENCES TECHNOLOGY(tech_id),
    FOREIGN KEY (unlocks_role_id) REFERENCES Soldier(soldier_id),
    FOREIGN KEY (unlocks_weapon_id) REFERENCES Weapon(weapon_id),
    FOREIGN KEY (unlocks_equipment_id) REFERENCES Equipment(equipment_id)
);

CREATE TABLE MISSION_ASSIGNMENT (
    mission_id INT,
    soldier_id INT,
    PRIMARY KEY (mission_id, soldier_id),
    FOREIGN KEY (mission_id) REFERENCES Mission(mission_id),
    FOREIGN KEY (soldier_id) REFERENCES Soldier(soldier_id)
);

CREATE TABLE MISSION_ENEMY (
    mission_id INT,
    et_id INT,
    count INT DEFAULT 0,
    PRIMARY KEY (mission_id, et_id),
    FOREIGN KEY (mission_id) REFERENCES Mission(mission_id),
    FOREIGN KEY (et_id) REFERENCES ENEMY_TYPES(et_id)
);

CREATE TABLE ENEMY_TYPES (
    et_ID INT PRIMARY KEY,
    et_name VARCHAR(255) NOT NULL,
    HP INT NOT NULL,
    base_ATK INT NOT NULL,
    base_DPS INT NOT NULL
);

-- Insert into Resource
INSERT INTO Resource VALUES
(1, 'Iron', 1000),
(2, 'Wood', 800),
(3, 'Gold', 600),
(4, 'Stone', 900),
(5, 'Crystal', 500),
(6, 'Copper', 700),
(7, 'Silver', 400),
(8, 'Titanium', 350),
(9, 'Uranium', 250),
(10, 'Platinum', 150);

-- Insert into Soldier
INSERT INTO Soldier VALUES
(1, 'John', 5, 100, 100, 20, 15, 'Infantry'),
(2, 'Alice', 3, 80, 80, 15, 10, 'Sniper'),
(3, 'Bob', 7, 120, 120, 25, 20, 'Tank'),
(4, 'Eve', 4, 90, 90, 18, 12, 'Scout'),
(5, 'Charlie', 6, 110, 110, 22, 17, 'Engineer'),
(6, 'David', 5, 95, 95, 19, 14, 'Medic'),
(7, 'Sophia', 4, 85, 85, 16, 11, 'Assault'),
(8, 'James', 6, 115, 115, 23, 18, 'HeavyGunner'),
(9, 'Olivia', 3, 75, 75, 14, 9, 'Recon'),
(10, 'Henry', 7, 130, 130, 27, 22, 'SpecialForces');

-- Insert into Weapon
INSERT INTO Weapon VALUES
(1, 'Rifle', 'Standard issue rifle', 30, 100, 10, 1, 1),
(2, 'Sniper', 'Long-range precision rifle', 50, 150, 15, 2, 1),
(3, 'Shotgun', 'Close-range heavy impact weapon', 40, 120, 12, 3, 1),
(4, 'Pistol', 'Lightweight sidearm', 20, 80, 8, 4, 1),
(5, 'Machine Gun', 'High-rate-of-fire weapon', 35, 200, 20, 5, 1),
(6, 'Rocket Launcher', 'Anti-armor weapon', 70, 300, 25, 6, 1),
(7, 'Energy Blaster', 'Futuristic energy weapon', 60, 250, 18, 7, 1),
(8, 'Crossbow', 'Silent ranged weapon', 25, 110, 10, 8, 1),
(9, 'Flamethrower', 'Burn enemies with fire', 45, 180, 22, 9, 1),
(10, 'Plasma Rifle', 'High-tech plasma weapon', 65, 350, 30, 10, 1);

-- Insert into Terrain
INSERT INTO Terrain VALUES
('Plains', 5, 5, 10),
('Forest', 10, 15, -5),
('Mountains', 15, 20, -10),
('Desert', 20, -5, -15),
('Swamp', -10, 10, 5),
('Caves', 10, 5, 0),
('Frozen Wasteland', -5, 15, -20),
('Alien Ruins', 15, 10, 10);

-- Insert into Weather
INSERT INTO Weather VALUES
('Sunny', 5, 5, 0),
('Rainy', -5, 10, 5),
('Stormy', -10, 15, -5),
('Foggy', 0, 10, 0),
('Snowy', -10, 5, -10),
('Windy', 5, -5, 0),
('Heatwave', 10, -10, -5),
('Asteroid Shower', -15, 20, -20);

-- Insert into Mission
INSERT INTO Mission VALUES
(1, 'Recon', 'Scout enemy territory', 2, 100, 10, 1, 'Plains', 'Sunny', 1),
(2, 'Sabotage', 'Destroy enemy supplies', 4, 200, 15, 2, 'Forest', 'Rainy', 1),
(3, 'Rescue', 'Save hostages from enemy capture', 3, 150, 12, 3, 'Mountains', 'Stormy', 1),
(4, 'Assault', 'Attack and capture an enemy outpost', 5, 300, 20, 4, 'Desert', 'Foggy', 1),
(5, 'Defense', 'Hold the frontline against enemy attacks', 4, 180, 14, 5, 'Swamp', 'Snowy', 1),
(6, 'Supply Raid', 'Seize enemy supply convoys', 3, 120, 10, 6, 'Plains', 'Sunny', 1),
(7, 'Infiltration', 'Gather intel from enemy base', 4, 250, 18, 7, 'Forest', 'Rainy', 1),
(8, 'Base Defense', 'Defend our main operations base', 5, 350, 25, 8, 'Mountains', 'Stormy', 1),
(9, 'Elimination', 'Hunt down a high-value target', 6, 400, 30, 9, 'Desert', 'Foggy', 1),
(10, 'Final Assault', 'Massive attack on enemy headquarters', 7, 500, 40, 10, 'Swamp', 'Snowy', 1);

-- Insert into ENEMY_TYPES
INSERT INTO ENEMY_TYPES VALUES
(1, 'Grunt', 100, 10, 5),
(2, 'Sniper', 80, 20, 10),
(3, 'Tank', 200, 30, 15),
(4, 'Elite', 150, 25, 12),
(5, 'Commander', 250, 40, 20),
(6, 'Scout', 90, 15, 7),
(7, 'Heavy Gunner', 180, 35, 18),
(8, 'Warrior', 160, 28, 14),
(9, 'Assassin', 110, 22, 11),
(10, 'Boss', 500, 50, 25);

-- Insert into MISSION_ENEMY (Each mission has multiple enemies)
INSERT INTO MISSION_ENEMY VALUES
(1, 1, 10), (1, 2, 5),
(2, 2, 15), (2, 3, 7),
(3, 3, 20), (3, 4, 8),
(4, 4, 25), (4, 5, 12),
(5, 5, 30), (5, 6, 10),
(6, 6, 12), (6, 7, 5),
(7, 7, 15), (7, 8, 7),
(8, 8, 18), (8, 9, 9),
(9, 9, 20), (9, 10, 15),
(10, 10, 30), (10, 1, 20);

-- Insert into MISSION_ASSIGNMENT (Ensuring each mission has soldiers assigned)
INSERT INTO MISSION_ASSIGNMENT VALUES
(1, 1), (1, 2), 
(2, 3), (2, 4),
(3, 5), (3, 6),
(4, 7), (4, 8),
(5, 9), (5, 10),
(6, 1), (6, 3),
(7, 2), (7, 4),
(8, 5), (8, 7),
(9, 6), (9, 8),
(10, 9), (10, 10);

-- Insert into TECHNOLOGY
INSERT INTO TECHNOLOGY VALUES
(1, 'Advanced Tactics', 'Enhance soldier strategies', 100, 1, 10, 50, NULL, 1, 1, 1, 1),
(2, 'Enhanced Armor', 'Increase soldier durability', 200, 2, 20, 100, 1, 2, 2, 2, 1),
(3, 'Laser Weapons', 'Unlock advanced laser weaponry', 300, 3, 30, 150, 2, 3, 3, 3, 1),
(4, 'Cybernetics', 'Augment soldiers with cybernetic enhancements', 400, 4, 40, 200, 3, 4, 4, 4, 1),
(5, 'Stealth Tech', 'Improve stealth and infiltration capabilities', 500, 5, 50, 250, 4, 5, 5, 5, 1),
(6, 'Drone Warfare', 'Deploy drones for recon and combat', 350, 6, 25, 180, 1, 6, 6, 6, 1),
(7, 'Exoskeletons', 'Enhance soldiers physical abilities', 450, 7, 35, 220, 2, 7, 7, 7, 1),
(8, 'Railgun', 'Develop high-energy railguns', 550, 8, 45, 300, 3, 8, 8, 8, 1),
(9, 'AI Combat Assist', 'Integrate AI-assisted targeting', 600, 9, 55, 350, 4, 9, 9, 9, 1),
(10, 'Orbital Strike', 'Unlock powerful satellite-based attacks', 700, 10, 65, 400, 5, 10, 10, 10, 1);


-- Insert into Equipment
INSERT INTO Equipment VALUES
(1, 'Combat Armor', 50, 10, 5, 150, 10, 1, 1),
(2, 'Stealth Suit', 20, 5, 15, 200, 15, 2, 1),
(3, 'Exo-Skeleton', 80, 20, 10, 300, 25, 3, 1),
(4, 'Power Gauntlets', 30, 10, 25, 180, 12, 4, 1),
(5, 'Reinforced Helmet', 10, 5, 5, 100, 8, 5, 1),
(6, 'Kinetic Boots', 25, 5, 10, 120, 10, 6, 1),
(7, 'Personal Shield Generator', 40, 15, 5, 250, 20, 7, 1),
(8, 'Nano-Fiber Vest', 60, 12, 8, 220, 18, 8, 1);

-- Insert into Infrastructure
INSERT INTO Infrastructure VALUES
(1, 'HQ', 'Central hub for military operations', 3, 1000, 50, 1, 1, 1, 40, -25),
(2, 'Barracks', 'Housing and training facility for soldiers', 2, 800, 40, 2, 1, 0, 0, 0),
(3, 'Armory', 'Storage for weapons and ammunition', 2, 600, 30, 3, 1, 0, 0, 0),
(4, 'Research Lab', 'Facility for developing new technologies', 4, 1200, 60, 4, 1, 0, 0, 0),
(5, 'Power Station', 'Generates energy for the base', 3, 900, 45, 5, 1, 0, 0, 0),
(6, 'Hospital', 'Provides healthcare and recovery for soldiers', 2, 700, 35, 6, 1, 0, 0, 0),
(7, 'Radar Station', 'Monitors enemy movements and signals', 3, 1000, 50, 7, 1, 0, 0, 0),
(8, 'Shield Generator', 'Defensive structure providing energy shields', 5, 1500, 75, 8, 1, 0, 0, 0);


EOF
echo "Finished inserting data"
echo "Done"
