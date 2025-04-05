#!/bin/bash

DB_FILE="database.db"

if [ ! -f "$DB_FILE" ]; then
    echo "Database file not found."
else
    echo "Database found."
fi

echo "Dropping all tables..."

sqlite3 "$DB_FILE" <<EOF
DROP TABLE Resource;
DROP TABLE Soldier;
DROP TABLE Weapon;
DROP TABLE Ability;
DROP TABLE Equipment;
DROP TABLE Infrastructure;
DROP TABLE Weather;
DROP TABLE Terrain;
DROP TABLE Mission;
DROP TABLE TECHNOLOGY;
DROP TABLE MISSION_ASSIGNMENT;
DROP TABLE MISSION_ENEMY;
DROP TABLE ENEMY_TYPES;
VACUUM;

EOF

echo "All tables dropped successfully."
