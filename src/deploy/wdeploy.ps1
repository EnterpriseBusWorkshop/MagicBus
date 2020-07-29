$root = $PSScriptRoot

$apps = @(
	@{
		Name="MessageStore";
		ProjectPath="../MagicBus.MessageStore";
		FuncAppName="magicbus-wmessagestore";
	},
	@{
		Name="HealthCheck";
		ProjectPath="../MagicBus.HealthCheckService";
		FuncAppName="magicbus-whealthcheck";
	},
	@{
		Name="MappingService";
		ProjectPath="../MagicBus.MappingService";
		FuncAppName="magicbus-wmappingservice";
	},
	@{
		Name="Shop";
		ProjectPath="../MagicBus.Shop";
		FuncAppName="magicbus-wshop";
	},
	@{
		Name="Fulfillment";
		ProjectPath="../MagicBus.Fulfilment";
		FuncAppName="magicbus-wfulfillment";
	}
)

$apps | ForEach-Object -ThrottleLimit 5 -Parallel {		
		Write-Host "Publishing $($_.Name)" -ForegroundColor Green		
		$appPath = Resolve-Path -Path $_.ProjectPath;
		Set-Location -Path $appPath;
		func azure functionapp publish $_.FuncAppName --csharp;
	}

cd $root