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
    building_id INTEGER PRIMARY KEY,
    name TEXT,
    description TEXT,
    level INTEGER,
    cost INTEGER,
    resource_amount INTEGER,
    resource_type INTEGER,
    unlocked BOOLEAN,
    placed BOOLEAN,
    x INTEGER,
    y INTEGER,
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
    cleared BOOL,
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
    et_ID INTEGER PRIMARY KEY,
    et_name TEXT NOT NULL,
    HP INTEGER NOT NULL,
    base_ATK INTEGER NOT NULL,
    base_DPS INTEGER NOT NULL, -- Assuming DPS means Damage Per Second, keeping INT
    exp_reward INTEGER NOT NULL 
);

CREATE TABLE SOLDIER_EQUIPMENT (
    soldier_ID INTEGER,
    weapon_ID INTEGER,
    equipment_ID INTEGER,
    PRIMARY KEY (soldier_ID, weapon_ID),
    FOREIGN KEY (soldier_ID) REFERENCES Soldier(soldier_id),
    FOREIGN KEY (weapon_ID) REFERENCES Weapon(weapon_id),
    FOREIGN KEY (equipment_ID) REFERENCES Equipment(equipment_id)
);

-- Insert into Resource
INSERT INTO Resource VALUES
(0, 'Food', 1000),
(1, 'Money', 1000),
(2, 'Iron', 1000),
(3, 'Wood', 800),
(4, 'Titanium', 350),
(5, 'Healing', 100);


-- Insert into Soldier
INSERT INTO Soldier VALUES
(1, 'John', 10, 100, 100, 20, 15, 'Infantry'),
(2, 'Alice', 5, 80, 80, 15, 10, 'Sniper'),
(3, 'Bob', 7, 120, 120, 25, 20, 'Tank'),
(4, 'Charlie', 10, 110, 110, 22, 17, 'Engineer'),
(5, 'David', 20, 95, 95, 19, 14, 'Medic'),
(6, 'Henry', 7, 130, 130, 27, 22, 'Infantry');

-- Insert into Weapon
INSERT INTO Weapon VALUES
(1, 'Rifle', 'Standard issue rifle', 30, 100, 10, 1, 1),
(2, 'Sniper2', 'Long-range precision rifle', 50, 150, 15, 2, 1),
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
(1, 'Shadow Recon', 'Infiltrate a Black Horizon outpost and gather intelligence.', 3, 120, 15, 1, 'Forest', 'Rainy', 1, 0),
(2, 'Data Extraction', 'Steal crucial data from a secret research lab.', 4, 180, 20, 2, 'Alien Ruins', 'Foggy', 1, 0),
(3, 'Supply Interdiction', 'Destroy Black Horizon''s resource supply lines.', 4, 150, 18, 3, 'Plains', 'Sunny', 1, 0),
(4, 'Elite Guard Assault', 'Attack and eliminate a Black Horizon elite squad.', 5, 250, 22, 4, 'Mountains', 'Snowy', 1, 0),
(5, 'Weapon Cache Raid', 'Seize advanced weapon samples and destroy the storage facility.', 6, 300, 25, 5, 'Desert', 'Heatwave', 1, 0),
(6, 'Facility Destruction', 'Sabotage a research facility to halt enemy progress.', 7, 400, 28, 6, 'Swamp', 'Stormy', 1, 0),
(7, 'Stealth Infiltration', 'Sneak into and investigate the Black Horizon command center.', 6, 350, 26, 7, 'Caves', 'Windy', 1, 0),
(8, 'The Gauntlet', 'Endure the enemy''s desperate counterattack and defend the facility.', 8, 500, 30, 8, 'Alien Ruins', 'Asteroid Shower', 1, 0),
(9, 'Final Showdown', 'Assault the Black Horizon main lab and end their operations.', 9, 600, 35, 9, 'Mountains', 'Stormy', 1, 0),
(10, 'Clean Sweep', 'Search and eliminate all remaining Black Horizon forces.', 10, 800, 50, 10, 'Plains', 'Sunny', 1, 0);

-- Insert into ENEMY_TYPES
INSERT INTO ENEMY_TYPES VALUES
(1, 'Recon Drone', 50, 18, 4, 10),
(2, 'Heavy Guard', 120, 25, 7, 20),
(3, 'Experimental Tank', 180, 38, 10, 20),
(4, 'Black Ops Sniper', 60, 35, 8, 30),
(5, 'Mech Soldier', 100, 44, 7, 50),
(6, 'Cyber Assassin', 90, 48, 9, 50),
(7, 'Bioengineered Beast', 150, 50, 12, 70),
(8, 'Psyker', 80, 30, 10, 70),
(9, 'Prototype AI', 200, 45, 15, 90),
(10, 'Black Horizon Commander', 300, 60, 20, 100);

-- Insert into MISSION_ENEMY (Each mission has multiple enemies)
INSERT INTO MISSION_ENEMY VALUES
(1, 1, 5), (1, 2, 2),
(2, 2, 3), (2, 3, 1),
(3, 1, 4), (3, 2, 2),
(4, 2, 3), (4, 4, 2),
(5, 3, 2), (5, 4, 1),
(6, 7, 2), (6, 8, 1),
(7, 6, 3), (7, 8, 1),
(8, 3, 2), (8, 9, 1),
(9, 9, 1), (9, 10, 1),
(10, 2, 5), (10, 1, 4), (10, 3, 3);

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


INSERT INTO Infrastructure VALUES
(1, 'HQ', 'Central hub for military operations, will generate money', 3, 1000, 50, 1, 1, 0, 0, 0),
(2, 'Training Room', 'Level your soldiers', 3, 900, 45, 5, 1, 0, 0, 0),
(3, 'Hospital', 'Provides healthcare and recovery for soldiers', 2, 700, 35, 4, 1, 0, 0, 0),
(4, 'Restaurant', 'Generates food for the base', 3, 1000, 50, 4, 1, 0, 0, 0),
(5, 'Pharmacy', 'Generates healing for the base', 5, 1500, 75, 4, 1, 0, 0, 0),
(6, 'Lumber Yard', 'Generates wood for the base', 3, 1000, 50, 4, 1, 0, 0, 0),
(7, 'Mine', 'Generates iron for the base', 3, 1000, 50, 4, 1, 0, 0, 0),
(8, 'Forgery', 'Generates titanium for the base', 3, 1000, 50, 4, 1, 0, 0, 0),
(9, 'Loadout Room', 'Equip weapons and armor onto your soldiers', 3, 1000, 50, 4, 1, 0, 0, 0);

INSERT OR IGNORE INTO Soldier_Equipment (soldier_ID, weapon_ID, equipment_ID) VALUES
(1, 1, 1),
(2, 2, 2),
(3, 3, 3);

EOF
echo "Finished inserting data"
echo "Done"
