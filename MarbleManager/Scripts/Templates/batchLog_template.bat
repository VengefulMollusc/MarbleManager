rem Create timestamp string for logging
for /f "tokens=2-4 delims=/ " %%a in ('date /t') do (
    set "month=%%a"
    set "day=%%b"
    set "year=%%c"
)
set "timestamp=!year!-!month!-!day!_%time%"
rem Create log string and write to file
set "logLine=!timestamp! - Run .bat command <command>"
set "filePath=<filePath>"
if not exist "!filePath!" (
    type nul > "!filePath!"
)
echo !logLine! >> "!filePath!"