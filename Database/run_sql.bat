echo off

rem batch file to run a sql script with sqlexpress

sqlcmd -S localhost -E -i  liberary_db.sql
rem sqlcmd -S 12.0.0.1 -E -i %1

echo .
echo if no error messages appear, the DB was created (but check) 
pause
