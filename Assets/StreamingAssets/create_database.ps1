# Define the database file path
$DB_FILE = "database.db"
# Define the path to the sqlite3 executable if it's not in your system PATH
# $sqliteExePath = "C:\path\to\sqlite3.exe"

# Check if the database file exists
if (-not (Test-Path -Path $DB_FILE -PathType Leaf)) {
    Write-Host "Database file '$DB_FILE' not found. Creating..."
    try {
        # Creating an empty file first can sometimes help, though VACUUM on non-existent often works.
        # New-Item -Path $DB_FILE -ItemType File -Force | Out-Null

        # Run VACUUM using sqlite3.exe to initialize the database file structure
        # Option 1a: Using sqlite3.exe (Direct Translation)
        & sqlite3.exe $DB_FILE "VACUUM;"

        # Option 1b: Using sqlite3.exe with full path (if not in PATH)
        # & $sqliteExePath $DB_FILE "VACUUM;"

        # Check the exit code of the last command
        if ($LASTEXITCODE -ne 0) {
            Write-Error "sqlite3.exe reported an error creating the database (Exit Code: $LASTEXITCODE)."
            # Optional: Exit the script if creation failed
            # exit $LASTEXITCODE
        } else {
             Write-Host "Database '$DB_FILE' created successfully."
        }
    } catch {
        Write-Error "Failed to execute sqlite3.exe to create database. Make sure it's installed and in your PATH."
        Write-Error "Error details: $_"
        # Optional: Exit the script if creation failed
        # exit 1
    }
} else {
    Write-Host "Database file '$DB_FILE' already exists."
}

# Proceed only if the database file exists or was successfully created
if (Test-Path -Path $DB_FILE -PathType Leaf) {

    Write-Host "Creating Database tables and inserting data..."

    # Define the SQL commands using a PowerShell Here-String
    $sqlCommands = @"
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
(1, 'John', 1, 100, 100, 20, 15, 'Infantry'),
(2, 'Alice', 1, 80, 80, 15, 10, 'Sniper'),
(3, 'Bob', 1, 120, 120, 25, 20, 'Tank'),
(4, 'Charlie', 1, 110, 110, 22, 17, 'Engineer'),
(5, 'David', 1, 95, 95, 19, 14, 'Medic'),
(6, 'Henry', 1, 130, 130, 27, 22, 'Infantry');

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
INSERT OR IGNORE INTO ENEMY_TYPES (et_ID, et_name, HP, base_ATK, base_DPS, exp_reward) VALUES
(1, 'Recon Drone', 50, 40, 4, 10),
(2, 'Heavy Guard', 120, 55, 40, 20),
(3, 'Experimental Tank', 480, 68, 100, 20),
(4, 'Black Ops Sniper', 260, 70, 25, 30),
(5, 'Mech Soldier', 500, 90, 60, 50),
(6, 'Cyber Assassin', 50, 100000, 0, 50),
(7, 'Bioengineered Beast', 450, 50, 150, 70),
(8, 'Psyker', 280, 60, 10, 160),
(9, 'Prototype AI', 500, 90, 15, 300),
(10, 'Black Horizon Commander', 3000, 150, 200, 1000);


INSERT OR IGNORE INTO MISSION_ENEMY (mission_id, et_id, count) VALUES
-- Easy Recon: mostly drones with a stray beast
(1, 1, 6),  (1, 2, 6),

-- Data Extraction: mix of snipers, mechs and a few drones
(2, 4, 9),  (2, 5, 3),  (2, 1, 3),

-- Supply Interdiction: tanks backed by guards and assassins
(3, 3, 10),  (3, 2, 3),  (3, 6, 1),

-- Elite Guard Assault: guards, tanks, beasts and a psyker
(4, 2, 2),  (4, 3, 2),  (4, 7, 1),  (4, 8, 1),

-- Weapon Cache Raid: snipers, drones and mech soldiers
(5, 4, 2),  (5, 1, 4),  (5, 5, 6),

-- Facility Destruction: prototype AI leading mechs and drones
(6, 9, 1),  (6, 5, 2),  (6, 1, 15),

-- Stealth Infiltration: assassins, snipers and guards in the shadows
(7, 6, 2),  (7, 4, 2),  (7, 2, 3),

-- The Gauntlet: heavy mechs flanked by AI and tanks
(8, 5, 3),  (8, 9, 1),  (8, 3, 1),  (8, 8, 1),

-- Final Showdown Prep: AI overlord with tanks and snipers
(9, 9, 2),  (9, 3, 2),  (9, 4, 1),  (9, 1, 3), (9, 7, 1),

-- Clean Sweep: commander supported by AI, beasts and psykers
(10, 10, 1),  (10, 9, 2),  (10, 7, 1),  (10, 8, 1);


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
(1, 'Training Room', 'Enhance soldier levels', 100.00, 2, 10, 50, NULL, 1, 1, 1, 1),
(2, 'Hospital', 'Healing station for soldier', 200.00, 2, 20, 100, 1, 2, 2, 2, 1),
(3, 'Restaurant', 'Generate the energy for soldiers to level up', 300.00, 3, 30, 150, 2, 3, 3, 3, 1),
(4, 'Lumber Yard', 'Generate wood for the base', 400.00, 4, 40, 200, 3, 4, 4, 4, 1),
(5, 'Mine', 'Generate iron for the base', 500.00, 3, 50, 250, 4, 5, 5, 5, 1),
(6, 'Forgery', 'Generate titanium for the base', 350.00, 4, 25, 180, 1, 6, 6, 6, 1),
(7, 'Loadout Room', 'Equip your soldiers with weapons and armor', 450.00, 2, 35, 220, 2, 7, 7, 7, 1),
(8, 'HQ', 'Generate money for the base', 50.00, 2, 5, 220, 2, 7, 7, 7, 1),
(9, 'Pharmacy', 'Generate medecine for the base', 50.00, 2, 10, 220, 2, 7, 7, 7, 1);


INSERT OR IGNORE INTO Equipment (equipment_id, name, hp, def, atk, cost, resource_amount, resource_type, unlocked) VALUES
(1, 'Combat Armor', 40, 12, 4, 120, 8, 1, 1),
(2, 'Stealth Suit', 25, 7, 12, 180, 12, 2, 1),
(3, 'Exo-Skeleton', 70, 25, 8, 280, 22, 3, 1),
(4, 'Power Gauntlets', 25, 8, 22, 160, 10, 4, 1),
(5, 'Reinforced Helmet', 8, 4, 4, 80, 6, 5, 1),
(6, 'Kinetic Boots', 20, 6, 8, 100, 8, 6, 1),
(7, 'Personal Shield Generator', 35, 18, 4, 230, 18, 7, 1),
(8, 'Nano‑Fiber Vest', 55, 14, 7, 200, 16, 8, 1);

INSERT INTO Infrastructure VALUES
(1, 'HQ', 'Central hub for military operations, will generate money', 3, 1000, 50, 1, 0, 0, 0, 0),
(2, 'Training Room', 'Level your soldiers', 3, 900, 45, 5, 0, 0, 0, 0),
(3, 'Hospital', 'Provides healthcare and recovery for soldiers', 2, 700, 35, 4, 0, 0, 0, 0),
(4, 'Restaurant', 'Generates food for the base', 3, 1000, 50, 4, 0, 0, 0, 0),
(5, 'Pharmacy', 'Generates healing for the base', 5, 1500, 75, 4, 0, 0, 0, 0),
(6, 'Lumber Yard', 'Generates wood for the base', 3, 1000, 50, 4, 0, 0, 0, 0),
(7, 'Mine', 'Generates iron for the base', 3, 1000, 50, 4, 0, 0, 0, 0),
(8, 'Forgery', 'Generates titanium for the base', 3, 1000, 50, 4, 0, 0, 0, 0),
(9, 'Loadout Room', 'Equip weapons and armor onto your soldiers', 3, 1000, 50, 4, 0, 0, 0, 0);


"@ # End of Here-String

    # --- Option 1: Using sqlite3.exe (Direct Translation) ---
    # Ensure sqlite3.exe is in your system's PATH or provide the full path.
    try {
        # Pipe the SQL commands string to the standard input of sqlite3.exe
        $sqlCommands | sqlite3.exe $DB_FILE
        # Or use full path: $sqlCommands | & $sqliteExePath $DB_FILE

        # Check the exit code of the last command
        if ($LASTEXITCODE -eq 0) {
            Write-Host "Finished inserting data via sqlite3.exe."
        } else {
            Write-Error "sqlite3.exe reported an error processing SQL commands (Exit Code: $LASTEXITCODE)."
            # Optional: Exit with a non-zero code on error
            # exit $LASTEXITCODE
        }
    } catch {
        Write-Error "Failed to execute sqlite3.exe. Make sure it's installed and in your PATH."
        Write-Error "Error details: $_"
        # Optional: Exit with a non-zero code on error
        # exit 1
    }

    # --- Option 2: Using PSSQLite Module (More PowerShell Idiomatic) ---
    # This requires installing the module first: Install-Module -Name PSSQLite -Scope CurrentUser
    # Uncomment the block below and comment out Option 1 if you prefer this method.
    #
    # try {
    #     Import-Module PSSQLite -ErrorAction Stop
    #
    #     # Invoke-SqliteQuery can often handle multiple statements separated by semicolons directly from a string
    #     Invoke-SqliteQuery -DataSource $DB_FILE -Query $sqlCommands -ErrorAction Stop
    #
    #     Write-Host "Finished inserting data via PSSQLite module."
    #
    # } catch {
    #     Write-Error "Error executing SQLite commands using PSSQLite module."
    #     Write-Error "Error details: $_"
    #     # Optional: Exit with a non-zero code on error
    #     # exit 1
    # }

} else {
    Write-Error "Cannot proceed. Database file '$DB_FILE' does not exist or could not be created."
    # Optional: Exit with a non-zero code
    # exit 1
}

Write-Host "Script finished."