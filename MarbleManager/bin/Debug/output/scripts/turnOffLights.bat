@echo off
setlocal
set nanoleafIp=192.168.68.66
set nanoleafApiKey=lM0MBR0meZtfSr1ml4s8AzSMMkP6cy8k
set lifxSelector=d073d56ed33a
set lifxAuthKey=c2cf1f99a25aadcaff85cb5e6e9574dddc60999c07b0a057411905820892e482

echo Turning Lights Off
curl -X PUT "https://api.lifx.com/v1/lights/%lifxSelector%/state" -H "Authorization: Bearer %lifxAuthKey%" -d "power=off" --ssl-no-revoke
curl -X PUT -H "Content-Type: application/json" -d "{\"on\":{\"value\":false}}" http://%nanoleafIp%:16021/api/v1/%nanoleafApiKey%/state

endlocal