#!/usr/bin/env bash
set -euo pipefail

sqlcmd=(/opt/mssql-tools18/bin/sqlcmd -S db,1433 -U sa -C -b -l 10 -f 65001)

"${sqlcmd[@]}" -Q "IF DB_ID(N'OrisDb') IS NULL CREATE DATABASE [OrisDb];"
"${sqlcmd[@]}" -i /database/create_tables.sql

seeded=$("${sqlcmd[@]}" -d OrisDb -h -1 -W -Q "
SET NOCOUNT ON;
SELECT COUNT(*) FROM sys.extended_properties WHERE class = 0 AND name = N'OrisSeedV1';
")

if [[ "$seeded" == 0 ]]; then
    cd /database
    "${sqlcmd[@]}" -d OrisDb -i /database/seed_once.sql
fi
