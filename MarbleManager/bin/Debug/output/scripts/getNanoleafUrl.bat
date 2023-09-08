@echo off
setlocal enabledelayedexpansion

set nanoleafIp=192.168.68.66
set apiKey=lM0MBR0meZtfSr1ml4s8AzSMMkP6cy8k

set nanoleafUrl=http://%nanoleafIp%:16021/api/v1/%apiKey%

endlocal&set %~1=%nanoleafUrl%