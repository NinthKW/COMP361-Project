# Define the database file path
$DB_FILE = "database.db"
# Define the path to the sqlite3 executable if it's not in your system PATH
# $sqliteExePath = "C:\path\to\sqlite3.exe" 

# Check if the database file exists
if (-not (Test-Path -Path $DB_FILE -PathType Leaf)) {
    Write-Host "Database file '$DB_FILE' not found."
    # Optional: Exit the script if the file is required
    # exit 1
} else {
    Write-Host "Database file '$DB_FILE' found."

    Write-Host "Dropping all tables..."

    # Define the SQL commands using a PowerShell Here-String
    # Using "IF EXISTS" makes the script more robust - it won't fail if a table doesn't exist.
    $sqlCommands = @"
DROP TABLE IF EXISTS Resource;
DROP TABLE IF EXISTS Soldier;
DROP TABLE IF EXISTS Weapon;
DROP TABLE IF EXISTS Equipment;
DROP TABLE IF EXISTS Infrastructure;
DROP TABLE IF EXISTS Weather;
DROP TABLE IF EXISTS Terrain;
DROP TABLE IF EXISTS Mission;
DROP TABLE IF EXISTS TECHNOLOGY;
DROP TABLE IF EXISTS MISSION_ASSIGNMENT;
DROP TABLE IF EXISTS MISSION_ENEMY;
DROP TABLE IF EXISTS ENEMY_TYPES;
DROP TABLE SOLDIER_EQUIPMENT;
VACUUM;
"@

    # --- Option 1: Using sqlite3.exe (Direct Translation) ---
    # Ensure sqlite3.exe is in your system's PATH or provide the full path.
    try {
        # Pipe the SQL commands string to the standard input of sqlite3.exe
        $sqlCommands | sqlite3.exe $DB_FILE

        # Check the exit code of the last command
        if ($LASTEXITCODE -eq 0) {
            Write-Host "All tables dropped successfully via sqlite3.exe."
        } else {
            Write-Error "sqlite3.exe reported an error (Exit Code: $LASTEXITCODE)."
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
    #     # Split commands as Invoke-SqliteQuery often handles one statement better,
    #     # though it can sometimes handle multiple separated by semicolons.
    #     # VACUUM needs to be run separately usually.
    #     $dropStatements = $sqlCommands -split ';' | Where-Object { $_.Trim() -ne '' -and $_.Trim().ToUpper() -ne 'VACUUM' }
    #
    #     foreach ($statement in $dropStatements) {
    #         Write-Verbose "Executing: $statement;"
    #         Invoke-SqliteQuery -DataSource $DB_FILE -Query "$($statement.Trim());" -ErrorAction Stop
    #     }
    #
    #     Write-Verbose "Executing: VACUUM;"
    #     Invoke-SqliteQuery -DataSource $DB_FILE -Query "VACUUM;" -ErrorAction Stop
    #
    #     Write-Host "All tables dropped successfully via PSSQLite module."
    #
    # } catch {
    #     Write-Error "Error executing SQLite commands using PSSQLite module."
    #     Write-Error "Error details: $_"
    #     # Optional: Exit with a non-zero code on error
    #     # exit 1
    # }
}

# Optional: Add a final message
Write-Host "Script finished."