
$root = $PSScriptRoot

Write-Warning "==== MessageStore ===="
cd ../MagicBus.MessageStore
func azure functionapp publish magicbus-messagestore
Write-Warning "==== MessageStore Complete ===="

Write-Warning "==== HeralthCheckService ===="
cd ../MagicBus.HealthCheckService
func azure functionapp publish magicbus-healthcheck
Write-Warning "==== HeralthCheckService Complete ===="

Write-Warning "==== Shop ===="
cd ../MagicBus.Shop
func azure functionapp publish magicbus-shop
Write-Warning "==== ShopComplete ===="

Write-Warning "==== MappingService ===="
cd ../MagicBus.MappingService
func azure functionapp publish magicbus-mappingservice
Write-Warning "==== MappingServiceComplete ===="

Write-Warning "==== Fulfillment ===="
cd ../MagicBus.Fulfilment
func azure functionapp publish magicbus-fulfillment
Write-Warning "==== FulfillmentComplete ===="

cd $root