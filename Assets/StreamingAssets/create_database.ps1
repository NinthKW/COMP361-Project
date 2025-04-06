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
CREATE TABLE IF NOT EXISTS Resource (
    resource_id INTEGER PRIMARY KEY,
    name TEXT,
    current_amount INTEGER
);

CREATE TABLE IF NOT EXISTS Soldier (
    soldier_id INTEGER PRIMARY KEY,
    name TEXT NOT NULL,
    level INTEGER DEFAULT 1,
    hp INTEGER,
    max_hp INTEGER,
    atk INTEGER,
    def INTEGER,
    role TEXT
);

CREATE TABLE IF NOT EXISTS Weapon (
    weapon_id INTEGER PRIMARY KEY,
    name TEXT,
    description TEXT,
    damage INTEGER,
    cost INTEGER,
    resource_amount INTEGER,
    resource_type INTEGER,
    unlocked BOOLEAN,
    FOREIGN KEY (resource_type) REFERENCES Resource(resource_id)
);


CREATE TABLE IF NOT EXISTS Equipment (
    equipment_id INTEGER PRIMARY KEY,
    name TEXT,
    hp INTEGER,
    def INTEGER,
    atk INTEGER,
    cost INTEGER,
    resource_amount INTEGER,
    resource_type INTEGER,
    unlocked BOOLEAN,
    FOREIGN KEY (resource_type) REFERENCES Resource(resource_id)
);

CREATE TABLE IF NOT EXISTS Infrastructure (
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

CREATE TABLE IF NOT EXISTS Weather (
    name TEXT PRIMARY KEY,
    atk_effect INTEGER,
    def_effect INTEGER,
    hp_effect INTEGER
);

CREATE TABLE IF NOT EXISTS Terrain (
    name TEXT PRIMARY KEY,
    atk_effect INTEGER,
    def_effect INTEGER,
    hp_effect INTEGER
);

CREATE TABLE IF NOT EXISTS Mission (
    mission_id INTEGER PRIMARY KEY,
    name TEXT,
    description TEXT,
    difficulty INTEGER,
    reward_money INTEGER,
    reward_amount INTEGER,
    reward_resource INTEGER,
    terrain TEXT,
    weather TEXT,
    unlocked BOOLEAN,
    FOREIGN KEY (reward_resource) REFERENCES Resource(resource_id),
    FOREIGN KEY(terrain) REFERENCES Terrain(name),
    FOREIGN KEY(weather) REFERENCES Weather(name)
);

CREATE TABLE IF NOT EXISTS TECHNOLOGY (
    tech_id INTEGER PRIMARY KEY,
    tech_name TEXT NOT NULL,
    description TEXT,
    cost_money REAL DEFAULT 0, -- Changed DECIMAL to REAL for SQLite compatibility
    cost_resources_id INTEGER,
    cost_resources_amount INTEGER DEFAULT 0,
    cost_points INTEGER DEFAULT 0,
    prerequisite_id INTEGER,
    unlocks_role_id INTEGER,
    unlocks_weapon_id INTEGER,
    unlocks_equipment_id INTEGER,
    unlocked BOOLEAN,
    FOREIGN KEY (cost_resources_id) REFERENCES Resource(resource_id),
    FOREIGN KEY (prerequisite_id) REFERENCES TECHNOLOGY(tech_id),
    FOREIGN KEY (unlocks_role_id) REFERENCES Soldier(soldier_id),
    FOREIGN KEY (unlocks_weapon_id) REFERENCES Weapon(weapon_id),
    FOREIGN KEY (unlocks_equipment_id) REFERENCES Equipment(equipment_id)
);

CREATE TABLE IF NOT EXISTS MISSION_ASSIGNMENT (
    mission_id INTEGER,
    soldier_id INTEGER,
    PRIMARY KEY (mission_id, soldier_id),
    FOREIGN KEY (mission_id) REFERENCES Mission(mission_id),
    FOREIGN KEY (soldier_id) REFERENCES Soldier(soldier_id)
);

CREATE TABLE IF NOT EXISTS MISSION_ENEMY (
    mission_id INTEGER,
    et_id INTEGER,
    count INTEGER DEFAULT 0,
    PRIMARY KEY (mission_id, et_id),
    FOREIGN KEY (mission_id) REFERENCES Mission(mission_id),
    FOREIGN KEY (et_id) REFERENCES ENEMY_TYPES(et_id)
);

CREATE TABLE IF NOT EXISTS ENEMY_TYPES (
    et_ID INTEGER PRIMARY KEY,
    et_name TEXT NOT NULL,
    HP INTEGER NOT NULL,
    base_ATK INTEGER NOT NULL,
    base_DPS INTEGER NOT NULL, -- Assuming DPS means Damage Per Second, keeping INT
    exp_reward INTEGER NOT NULL 
);

-- Insert data (using INSERT OR IGNORE to prevent errors if data already exists)
INSERT OR IGNORE INTO Resource (resource_id, name, current_amount) VALUES
(1, 'Iron', 1000), (2, 'Wood', 800), (3, 'Gold', 600), (4, 'Stone', 900), (5, 'Crystal', 500),
(6, 'Copper', 700), (7, 'Silver', 400), (8, 'Titanium', 350), (9, 'Uranium', 250), (10, 'Platinum', 150);

INSERT OR IGNORE INTO Soldier (soldier_id, name, level, hp, max_hp, atk, def, role) VALUES
(1, 'John', 5, 100, 100, 20, 15, 'Infantry'),
(2, 'Alice', 3, 80, 80, 15, 10, 'Sniper'),
(3, 'Bob', 7, 120, 120, 25, 20, 'Tank'),
(4, 'Charlie', 6, 110, 110, 22, 17, 'Engineer'),
(5, 'David', 20, 95, 95, 19, 14, 'Medic'),
(6, 'Henry', 7, 130, 130, 27, 22, 'Infantry');

INSERT OR IGNORE INTO Weapon (weapon_id, name, description, damage, cost, resource_amount, resource_type, unlocked) VALUES
(1, 'Rifle', 'Standard issue rifle', 30, 100, 10, 1, 1), (2, 'Sniper', 'Long-range precision rifle', 50, 150, 15, 2, 1),
(3, 'Shotgun', 'Close-range heavy impact weapon', 40, 120, 12, 3, 1), (4, 'Pistol', 'Lightweight sidearm', 20, 80, 8, 4, 1),
(5, 'Machine Gun', 'High-rate-of-fire weapon', 35, 200, 20, 5, 1), (6, 'Rocket Launcher', 'Anti-armor weapon', 70, 300, 25, 6, 1),
(7, 'Energy Blaster', 'Futuristic energy weapon', 60, 250, 18, 7, 1), (8, 'Crossbow', 'Silent ranged weapon', 25, 110, 10, 8, 1),
(9, 'Flamethrower', 'Burn enemies with fire', 45, 180, 22, 9, 1), (10, 'Plasma Rifle', 'High-tech plasma weapon', 65, 350, 30, 10, 1);

INSERT OR IGNORE INTO Terrain (name, atk_effect, def_effect, hp_effect) VALUES
('Plains', 5, 5, 10), ('Forest', 10, 15, -5), ('Mountains', 15, 20, -10), ('Desert', 20, -5, -15),
('Swamp', -10, 10, 5), ('Caves', 10, 5, 0), ('Frozen Wasteland', -5, 15, -20), ('Alien Ruins', 15, 10, 10);

INSERT OR IGNORE INTO Weather (name, atk_effect, def_effect, hp_effect) VALUES
('Sunny', 5, 5, 0), ('Rainy', -5, 10, 5), ('Stormy', -10, 15, -5), ('Foggy', 0, 10, 0),
('Snowy', -10, 5, -10), ('Windy', 5, -5, 0), ('Heatwave', 10, -10, -5), ('Asteroid Shower', -15, 20, -20);

INSERT OR IGNORE INTO Mission (mission_id, name, description, difficulty, reward_money, reward_amount, reward_resource, terrain, weather, unlocked) VALUES
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

INSERT OR IGNORE INTO ENEMY_TYPES (et_ID, et_name, HP, base_ATK, base_DPS, exp_reward) VALUES
(1, 'Grunt', 100, 10, 5, 20), (2, 'Sniper', 80, 20, 10, 20), (3, 'Tank', 200, 30, 15, 30), (4, 'Elite', 150, 25, 12, 50),
(5, 'Commander', 250, 40, 20, 60), (6, 'Scout', 90, 15, 7, 30), (7, 'Heavy Gunner', 180, 35, 18, 60), (8, 'Warrior', 160, 28, 14, 80),
(9, 'Assassin', 110, 22, 11, 90), (10, 'Boss', 500, 50, 25, 100);

INSERT OR IGNORE INTO MISSION_ENEMY (mission_id, et_id, count) VALUES
(1, 1, 10), (1, 2, 5), (2, 2, 15), (2, 3, 7), (3, 3, 20), (3, 4, 8), (4, 4, 25), (4, 5, 12),
(5, 5, 30), (5, 6, 10), (6, 6, 12), (6, 7, 5), (7, 7, 15), (7, 8, 7), (8, 8, 18), (8, 9, 9),
(9, 9, 20), (9, 10, 15), (10, 10, 30), (10, 1, 20);

INSERT OR IGNORE INTO MISSION_ASSIGNMENT (mission_id, soldier_id) VALUES
(1, 1), (1, 2), (2, 3), (2, 4), (3, 5), (3, 6), (4, 7), (4, 8), (5, 9), (5, 10),
(6, 1), (6, 3), (7, 2), (7, 4), (8, 5), (8, 7), (9, 6), (9, 8), (10, 9), (10, 10);

INSERT OR IGNORE INTO TECHNOLOGY (tech_id, tech_name, description, cost_money, cost_resources_id, cost_resources_amount, cost_points, prerequisite_id, unlocks_role_id, unlocks_weapon_id, unlocks_equipment_id, unlocked) VALUES
(1, 'Advanced Tactics', 'Enhance soldier strategies', 100.00, 1, 10, 50, NULL, 1, 1, 1, 1),
(2, 'Enhanced Armor', 'Increase soldier durability', 200.00, 2, 20, 100, 1, 2, 2, 2, 1),
(3, 'Laser Weapons', 'Unlock advanced laser weaponry', 300.00, 3, 30, 150, 2, 3, 3, 3, 1),
(4, 'Cybernetics', 'Augment soldiers with cybernetic enhancements', 400.00, 4, 40, 200, 3, 4, 4, 4, 1),
(5, 'Stealth Tech', 'Improve stealth and infiltration capabilities', 500.00, 5, 50, 250, 4, 5, 5, 5, 1),
(6, 'Drone Warfare', 'Deploy drones for recon and combat', 350.00, 6, 25, 180, 1, 6, 6, 6, 1),
(7, 'Exoskeletons', 'Enhance soldiers physical abilities', 450.00, 7, 35, 220, 2, 7, 7, 7, 1),
(8, 'Railgun', 'Develop high-energy railguns', 550.00, 8, 45, 300, 3, 8, 8, 8, 1),
(9, 'AI Combat Assist', 'Integrate AI-assisted targeting', 600.00, 9, 55, 350, 4, 9, 9, 9, 1),
(10, 'Orbital Strike', 'Unlock powerful satellite-based attacks', 700.00, 10, 65, 400, 5, 10, 10, 10, 1);


INSERT OR IGNORE INTO Equipment (equipment_id, name, hp, def, atk, cost, resource_amount, resource_type, unlocked) VALUES
(1, 'Combat Armor', 50, 10, 5, 150, 10, 1, 1),
(2, 'Stealth Suit', 20, 5, 15, 200, 15, 2, 1),
(3, 'Exo-Skeleton', 80, 20, 10, 300, 25, 3, 1),
(4, 'Power Gauntlets', 30, 10, 25, 180, 12, 4, 1),
(5, 'Reinforced Helmet', 10, 5, 5, 100, 8, 5, 1),
(6, 'Kinetic Boots', 25, 5, 10, 120, 10, 6, 1),
(7, 'Personal Shield Generator', 40, 15, 5, 250, 20, 7, 1),
(8, 'Nano-Fiber Vest', 60, 12, 8, 220, 18, 8, 1);

INSERT OR IGNORE INTO Infrastructure (building_id, name, description, level, cost, resource_amount, resource_type, unlocked, placed, x, y) VALUES
(1, 'HQ', 'Central hub for military operations', 3, 1000, 50, 1, 1, 1, 40, -25),
(2, 'Barracks', 'Housing and training facility for soldiers', 2, 800, 40, 2, 1, 0, 0, 0),
(3, 'Armory', 'Storage for weapons and ammunition', 2, 600, 30, 3, 1, 0, 0, 0),
(4, 'Research Lab', 'Facility for developing new technologies', 4, 1200, 60, 4, 1, 0, 0, 0),
(5, 'Power Station', 'Generates energy for the base', 3, 900, 45, 5, 1, 0, 0, 0),
(6, 'Hospital', 'Provides healthcare and recovery for soldiers', 2, 700, 35, 6, 1, 0, 0, 0),
(7, 'Radar Station', 'Monitors enemy movements and signals', 3, 1000, 50, 7, 1, 0, 0, 0),
(8, 'Shield Generator', 'Defensive structure providing energy shields', 5, 1500, 75, 8, 1, 0, 0, 0);

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