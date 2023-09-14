@echo off
setlocal
set nanoleafIp=<nanoleafIp>
set nanoleafApiKey=<nanoleafApiKey>
set lifxSelector=<lifxSelector>
set lifxAuthKey=<lifxAuthKey>

echo Turning Lights Off
curl -X PUT "https://api.lifx.com/v1/lights/%lifxSelector%/state" -H "Authorization: Bearer %lifxAuthKey%" -d "power=off" --ssl-no-revoke
curl -X PUT -H "Content-Type: application/json" -d "{\"on\":{\"value\":false}}" http://%nanoleafIp%:16021/api/v1/%nanoleafApiKey%/state

endlocal