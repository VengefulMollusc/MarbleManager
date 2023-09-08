@echo off
setlocal enabledelayedexpansion

set nanoleafIp=<nanoleafIp>
set apiKey=<nanoleafApiKey>

set nanoleafUrl=http://%nanoleafIp%:16021/api/v1/%apiKey%

endlocal&set %~1=%nanoleafUrl%