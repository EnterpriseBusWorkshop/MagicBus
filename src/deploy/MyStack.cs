using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using MediatR;
using Pulumi;
using Pulumi.Azure.AppInsights;
using Pulumi.Azure.AppService;
using Pulumi.Azure.AppService.Inputs;
using Pulumi.Azure.Blueprint;
using Pulumi.Azure.ContainerService;
using Pulumi.Azure.Core;
using Pulumi.Azure.ServiceBus;
using Pulumi.Azure.Storage;
using Pulumi.Azure.CosmosDB;
using Pulumi.Azure.CosmosDB.Inputs;
using Pulumi.Azure.Network;
using Pulumi.Docker;
using Account = Pulumi.Azure.Storage.Account;

namespace MagicBus.Deploy
{
    public class MyStack : Stack
    {
        private readonly ResourceGroup _resourceGroup;
        private readonly Insights _appInsights;
        private Namespace _serviceBusNamespace;
        private Topic _serviceBusTopic;


        public MyStack()
        {
            // Create an Azure Resource Group
            _resourceGroup = new ResourceGroup("magic-bus-rg");

            var config = new Pulumi.Config();
			string prefix = config.Get("prefix") ?? string.Empty;
			Console.WriteLine("Using PREFIX: " + prefix);

			_appInsights = AppInsights(prefix);

			// the ultimate app insights config - everything turned on (maybe scale back for Prod in case it slows the app)
			var appInsightsConfig = new InputMap<string>()
			{
				{"APPINSIGHTS_INSTRUMENTATIONKEY", _appInsights.InstrumentationKey},
				{"APPLICATIONINSIGHTS_CONNECTION_STRING", Output.Format($"InstrumentationKey={_appInsights.InstrumentationKey}")},
				{"ApplicationInsightsAgent_EXTENSION_VERSION", "~2"},
				{"APPINSIGHTS_PROFILERFEATURE_VERSION", "1.0.0"},
				{"APPINSIGHTS_SNAPSHOTFEATURE_VERSION", "1.0.0"},
				{"DiagnosticServices_EXTENSION_VERSION", "~3"},
				{"InstrumentationEngine_EXTENSION_VERSION", "~1"},
				{"SnapshotDebugger_EXTENSION_VERSION", "~1"},
				{"XDT_MicrosoftApplicationInsights_BaseExtensions", "~1"},
				{"XDT_MicrosoftApplicationInsights_Mode", "recommended"},
				{"XDT_MicrosoftApplicationInsights_PreemptSdk", "disabled"},
			};

			// CORE - SERVICE BUS
			ServiceBus(prefix);
			var serviceBusConfig = new InputMap<string>()
			{
				{"ServiceBusConnectionString", _serviceBusNamespace.DefaultPrimaryConnectionString},
				{"ServiceBusTopic", _serviceBusTopic.Name}
			};

			// COMMON APP SETTINGS
			var commonAppSettings = InputMap<string>.Merge(appInsightsConfig, serviceBusConfig);

			// SHOP
			var shopifySettings = new InputMap<string>()
			{
				{"ShopUrl", config.Get("ShopUrl") ?? string.Empty}, // SET this with :> pulumi config set MagicBus.Deploy:ShopUrl [value]
				{"ShopAccessToken", config.Get("ShopAccessToken") ?? string.Empty} // SET this secret with :> pulumi config set MagicBus.Deploy:ShopAccessToken [value] --secret
			};

			var shopFunctionApp = FunctionApp($"{prefix}shop",
				InputMap<string>.Merge(commonAppSettings, shopifySettings),
				new SubscriptionFilterConfig() { AssemblyToScan = typeof(MagicBus.Shop.Startup).Assembly });

			// HEALTHCHECK
			var healthCheckFunctionApp = FunctionApp($"{prefix}healthcheck", commonAppSettings);

			// MESSAGE STORE
            var messageStoreCosmosAccount = MessageStoreCosmosDb(prefix);
            var messageStoreCosmosDbAppSettings = new InputMap<string>()
            {
	            {"CosmosDbEndpoint", messageStoreCosmosAccount.Endpoint },
	            {"CosmosDbAuthKey", messageStoreCosmosAccount.PrimaryMasterKey }
            };

            var messageStoreFunctionApp = FunctionApp($"{prefix}messagestore",
	            InputMap<string>.Merge(commonAppSettings, messageStoreCosmosDbAppSettings));

            var smtpSettings = new InputMap<string>()
            {
                {"SmtpHost", config.Get("SmtpHost") ?? string.Empty },
                {"SmtpPort", config.Get("SmtpPort") ?? string.Empty },
                {"SmtpUsername", config.Get("SmtpUsername") ?? string.Empty },
                {"SmtpPassword", config.Get("SmtpPassword") ?? string.Empty }, // SET this secret with :> pulumi config set MagicBus.Deploy:SmtpPassword [value] --secret
            };

            var fulfillmentFunctionApp = FunctionApp($"{prefix}fulfillment",
                InputMap<string>.Merge(commonAppSettings, smtpSettings),
                new SubscriptionFilterConfig() { AssemblyToScan = typeof(MagicBus.Fulfilment.Startup).Assembly} );

            var mappingServiceCosmosAccount = MappingServiceCosmosDb(prefix);
            var mappingServiceCosmosDbAppSettings = new InputMap<string>()
            {
	            {"CosmosDbEndpoint", mappingServiceCosmosAccount.Endpoint },
	            {"CosmosDbAuthKey", mappingServiceCosmosAccount.PrimaryMasterKey }
            };
            var mappingServiceFunctionApp = FunctionApp($"{prefix}mappingservice",
	            InputMap<string>.Merge(commonAppSettings, mappingServiceCosmosDbAppSettings),
	            new SubscriptionFilterConfig() { AssemblyToScan = typeof(MagicBus.MappingService.Startup).Assembly } );

			// ADMIN PORTAL
            var adminPortalApp = AdminPortal(prefix, commonAppSettings, messageStoreCosmosAccount);

            // set outputs
            this.ResourceGroupName = _resourceGroup.Name;
            this.AppInsightInstrumentationKey = _appInsights.InstrumentationKey;
            this.ServiceBusConnectionString = _serviceBusNamespace.DefaultPrimaryConnectionString;
            this.ServiceBusTopic = _serviceBusTopic.Name;
            this.ShopName = shopFunctionApp.Name;
            this.HealthCheckName = healthCheckFunctionApp.Name;

            this.MessageStoreName = messageStoreFunctionApp.Name;
            this.MessageStoreDbEndpoint = messageStoreCosmosAccount.Endpoint;
            this.MessageStoreDbKey = messageStoreCosmosAccount.PrimaryMasterKey;

            this.MappingServiceName = mappingServiceFunctionApp.Name;
            this.MappingServiceDbEndpoint = mappingServiceCosmosAccount.Endpoint;
            this.MappingServiceDbKey = mappingServiceCosmosAccount.PrimaryMasterKey;
            this.AdminPortalUrl = Output.Format($"https://{adminPortalApp.DefaultSiteHostname}");
        }

        private Insights AppInsights(string prefix)
        {
	        return new Pulumi.Azure.AppInsights.Insights($"{prefix}magicbus-ai", new InsightsArgs()
	        {
				ApplicationType = "web",
				ResourceGroupName = _resourceGroup.Name,
				Location = _resourceGroup.Location
			});
        }

        private AppService AdminPortal(string prefix, InputMap<string> commonAppSettings, Pulumi.Azure.CosmosDB.Account messageStoreCosmosAccount)
        {
            var registry = new Pulumi.Azure.ContainerService.Registry($"{prefix}magicbusregistry", new RegistryArgs()
            {
                ResourceGroupName = _resourceGroup.Name,
                Sku = "Basic",
                AdminEnabled = true
            });

            var dockerImage = new Pulumi.Docker.Image("adminportal", new ImageArgs()
            {
                ImageName = Output.Format($"{registry.LoginServer}/adminportal:latest"),
                Build = new DockerBuild()
                {
                    Context = "../",
                },
                Registry = new ImageRegistry()
                {
                    Server = registry.LoginServer,
                    Username = registry.AdminUsername,
                    Password = registry.AdminPassword
                }
            });
            var appServicePlan = new Plan("admin-portal-plan", new PlanArgs()
            {
                ResourceGroupName = _resourceGroup.Name,
                Location = "australiasoutheast",
                Kind = "linux",
                Reserved = true,
                Sku = new PlanSkuArgs()
                {
                    Tier = "Basic",
                    Size = "B1",
                    Capacity = 1
                }
            });
            var app = new AppService("magic-bus-admin-portal", new AppServiceArgs()
            {
                ResourceGroupName = _resourceGroup.Name,
                AppServicePlanId = appServicePlan.Id,
                Location = "australiasoutheast",
                AppSettings = InputMap<string>.Merge(commonAppSettings, new InputMap<string>()
                {
	                {"WEBSITES_ENABLE_APP_SERVICE_STORAGE", "false"},
                    {"DOCKER_REGISTRY_SERVER_URL", Output.Format($"https://{registry.LoginServer}")},
                    {"DOCKER_REGISTRY_SERVER_USERNAME",registry.AdminUsername},
                    {"DOCKER_REGISTRY_SERVER_PASSWORD", registry.AdminPassword},
                    {"WEBSITES_PORT", "80,443"},
                    {"CosmosDbEndpoint", messageStoreCosmosAccount.Endpoint },
                    {"CosmosDbAuthKey", messageStoreCosmosAccount.PrimaryMasterKey }
				}),
                SiteConfig = new AppServiceSiteConfigArgs()
                {
                    AlwaysOn = true,
                    LinuxFxVersion = Output.Format($"DOCKER|{dockerImage.ImageName}")
                },
                HttpsOnly = true
            });

            return app;
        }


        /// <summary>
        /// create a cosmos db instance for the message store
        /// </summary>
        private Pulumi.Azure.CosmosDB.Account MappingServiceCosmosDb(string prefix)
        {
            var cosmosAccount = new Pulumi.Azure.CosmosDB.Account($"{prefix}mapping-service",
                new Pulumi.Azure.CosmosDB.AccountArgs()
                {
                    ResourceGroupName = _resourceGroup.Name,
                    GeoLocations = new InputList<AccountGeoLocationArgs>() {
                        new AccountGeoLocationArgs()
                        {
                            Location = _resourceGroup.Location,
                            FailoverPriority = 0
                        },
                        new AccountGeoLocationArgs()
                        {
                            Location = "Australia Southeast",
                            FailoverPriority = 1
                        }
                    },
                    ConsistencyPolicy = new AccountConsistencyPolicyArgs()
                    {
                        ConsistencyLevel = "Session",
                        MaxStalenessPrefix = 100,
                        MaxIntervalInSeconds = 5
                    },
                    OfferType = "Standard",
                    EnableAutomaticFailover = false
                });

            var mappingDb = new Pulumi.Azure.CosmosDB.SqlDatabase("mapping-service", new Pulumi.Azure.CosmosDB.SqlDatabaseArgs()
            {
                Name = "mapping-service",
                ResourceGroupName = _resourceGroup.Name,
                AccountName = cosmosAccount.Name,
                Throughput = 400
            });

			var mappingContainer = new Pulumi.Azure.CosmosDB.SqlContainer("mappings", new SqlContainerArgs()
			{
				ResourceGroupName = _resourceGroup.Name,
				AccountName = cosmosAccount.Name,
				DatabaseName = mappingDb.Name,
				PartitionKeyPath = "/id",
				Throughput = 400
			});

			return cosmosAccount;
        }

        /// <summary>
        /// create a cosmos db instance for the message store
        /// </summary>
        private Pulumi.Azure.CosmosDB.Account MessageStoreCosmosDb(string prefix)
        {
            var cosmosAccount = new Pulumi.Azure.CosmosDB.Account($"{prefix}message-store",
                new Pulumi.Azure.CosmosDB.AccountArgs()
                {
                    ResourceGroupName = _resourceGroup.Name,
                    GeoLocations = new InputList<AccountGeoLocationArgs>() {
                        new AccountGeoLocationArgs()
                        {
                            Location = _resourceGroup.Location,
                            FailoverPriority = 0
                        },
                        new AccountGeoLocationArgs()
                        {
                            Location = "Australia Southeast",
                            FailoverPriority = 1
                        }
                    },
                    ConsistencyPolicy = new AccountConsistencyPolicyArgs()
                    {
                        ConsistencyLevel = "Session",
                        MaxStalenessPrefix = 100,
                        MaxIntervalInSeconds = 5
                    },
                    OfferType = "Standard",
                    EnableAutomaticFailover = false
                });
            var messageStoreDb = new Pulumi.Azure.CosmosDB.SqlDatabase("message-store", new Pulumi.Azure.CosmosDB.SqlDatabaseArgs()
            {
                Name = "message-store",
                ResourceGroupName = _resourceGroup.Name,
                AccountName = cosmosAccount.Name,
                Throughput = 400
            });

			var messageStoreContainer = new Pulumi.Azure.CosmosDB.SqlContainer("messages", new SqlContainerArgs()
			{
				ResourceGroupName = _resourceGroup.Name,
				AccountName = cosmosAccount.Name,
				DatabaseName = messageStoreDb.Name,
				PartitionKeyPath = "/id",
				Throughput = 400
			});

			return cosmosAccount;
        }


        [Output] public Output<string> ResourceGroupName { get; set; }
		[Output] public Output<string> AppInsightInstrumentationKey { get; set; }
		[Output] public Output<string> ServiceBusConnectionString { get; set; }
        [Output] public Output<string> ServiceBusTopic { get; set; }
        [Output] public Output<string> MessageStoreName { get; set; }
        [Output] public Output<string> MessageStoreDbEndpoint { get; set; }
        [Output] public Output<string> MessageStoreDbKey { get; set; }

        [Output] public Output<string> HealthCheckName { get; set; }
        [Output] public Output<string> ShopName { get; set; }
        [Output] public Output<string> MappingServiceName { get; set; }
        [Output] public Output<string> MappingServiceDbEndpoint { get; set; }
        [Output] public Output<string> MappingServiceDbKey { get; set; }

        [Output] public Output<string> AdminPortalUrl { get; set; }



        private FunctionApp FunctionApp(string name, InputMap<string> appSettings, SubscriptionFilterConfig? subscriptionFilterConfig = null)
        {
            var storageAccount = new Pulumi.Azure.Storage.Account(name, new Pulumi.Azure.Storage.AccountArgs
            {
                ResourceGroupName = _resourceGroup.Name,
                Location = _resourceGroup.Location,
                AccountTier = "Standard",
                AccountReplicationType = "LRS",
            });

            var plan = new Plan(name, new PlanArgs
            {
                Location = _resourceGroup.Location,
                ResourceGroupName = _resourceGroup.Name,
                Kind = "FunctionApp",
                Sku = new Pulumi.Azure.AppService.Inputs.PlanSkuArgs
                {
                    Tier = "Dynamic",
                    Size = "Y1",
                },
            });

            var serviceBusSubscription = new Subscription(name, new SubscriptionArgs()
            {
                Name = name,
                NamespaceName = _serviceBusNamespace.Name,
                TopicName = _serviceBusTopic.Name,
                ResourceGroupName = _resourceGroup.Name,
                MaxDeliveryCount = 10
            });

            if (subscriptionFilterConfig != null)
            {
                var typesList = new List<string>();
                typesList.AddRange(subscriptionFilterConfig.MessageTypes);
                if (subscriptionFilterConfig.AssemblyToScan != null)
                {
                    var handlerTypes = subscriptionFilterConfig.AssemblyToScan.GetTypes()
                        .Where(t => t.GetInterfaces()
                            .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<>))).ToList();

                    Console.Out.WriteLine("Got handler Types: "+string.Join(' ', handlerTypes.Select(t => t.Name)));

                    var messageTypes = handlerTypes.Select(ht =>
                        ht.GetInterfaces()
                            .FirstOrDefault(i => i.GetGenericTypeDefinition() == typeof(IRequestHandler<>))
                            .GenericTypeArguments.First()
                        )
                        .ToList();
                    Console.Out.WriteLine("Got message Types: "+string.Join(' ', messageTypes.Select(t => t.Name)));

                    typesList.AddRange(messageTypes.Select(t => t.Name));
                }

                if (typesList.Any())
                {
                    var filterExpression = string.Join(" OR ", typesList.Select(t => $"sys.Label='{t}'"));
                    Console.Out.WriteLine("Creating Filter Expression: "+filterExpression);
                    var subFilter = new SubscriptionRule(name, new SubscriptionRuleArgs()
                    {
                        ResourceGroupName = _resourceGroup.Name,
                        NamespaceName = _serviceBusNamespace.Name,
                        TopicName = _serviceBusTopic.Name,
                        SubscriptionName = name,
                        FilterType = "SqlFilter",
                        SqlFilter = filterExpression
                    });
                }
            }

			// create a new app settings input map - avoids adding the exception for having the same key but different value on the next function app
            var funcAppSettings = InputMap<string>.Merge(appSettings, new InputMap<string>()
            {
	            {"ServiceBusSubscription", serviceBusSubscription.Name},
	            {"WEBSITE_RUN_FROM_PACKAGE", "1"}
            });

            var functionApp = new FunctionApp($"magicbus-{name}", new FunctionAppArgs()
            {
                Name = $"magicbus-{name}",
                ResourceGroupName = _resourceGroup.Name,
                HttpsOnly = true,
                Version = "~3",
                AppServicePlanId = plan.Id,
                StorageAccountName = storageAccount.Name,
                StorageAccountAccessKey = storageAccount.PrimaryAccessKey,
                AppSettings = funcAppSettings,
            });
            return functionApp;

        }

        private void ServiceBus(string prefix)
        {
            // create ServiceBus namespace
            _serviceBusNamespace = new Namespace($"{prefix}magic-bus", new NamespaceArgs()
            {
                Capacity = 0,
                Sku = "standard",
                ResourceGroupName = _resourceGroup.Name,
                Location = _resourceGroup.Location
            });

            // service bus topic
            _serviceBusTopic = new Pulumi.Azure.ServiceBus.Topic("magic-bus-topic", new TopicArgs()
            {
                Name = "magic-bus-topic",
                NamespaceName = _serviceBusNamespace.Name,
                ResourceGroupName = _resourceGroup.Name,
            });
        }




    }


    public class SubscriptionFilterConfig
    {
        /// <summary>
        /// if set, scan this assembly for handler types and register a fitler for each message tye handled
        /// </summary>
        public Assembly? AssemblyToScan { get; set; } = null;

        /// <summary>
        /// create a filter for each of these name message types.
        /// </summary>
        public IList<string> MessageTypes { get; set; } = new List<string>();

    }




}
