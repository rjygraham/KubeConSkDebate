var builder = DistributedApplication.CreateBuilder(args);

// Azure OpenAI
var deployment = new AzureOpenAIDeployment("gpt-4o", "gpt-4o", "2024-08-06", "GlobalStandard", 450);
var aoai = builder.AddAzureOpenAI("aoai")
    .AddDeployment(deployment);

// Add the resources which you will use for Orleans clustering and
// grain state storage.
var storage = builder.AddAzureStorage("storage");
var clusteringTable = storage.AddTables("clustering");
var grainStorage = storage.AddBlobs("grain-state");

// Add the Orleans resource to the Aspire DistributedApplication
// builder, then configure it with Azure Table Storage for clustering
// and Azure Blob Storage for grain storage.
var orleans = builder.AddOrleans("default")
    .WithClustering(clusteringTable)
    .WithGrainStorage("Lobby", grainStorage)
    .WithGrainStorage("Debates", grainStorage)
    .WithGrainStorage("Topics", grainStorage)
    .WithGrainStorage("Agents", grainStorage);

var debateHost = builder.AddProject<Projects.KubeCon_Sk_Debate_Host>("debate-host")
    .WithReference(orleans)
    .WithReference(aoai)
    .WithEnvironment("AOAI_DEPLOYMENT_NAME", deployment.Name)
    .WithExternalHttpEndpoints();

builder.AddProject<Projects.KubeCon_Sk_Debate_DefaultAgents>("default-agents")
    .WithReference(orleans)
    .WithReference(aoai)
    .WithEnvironment("AOAI_DEPLOYMENT_NAME", deployment.Name);

builder.AddProject<Projects.KubeCon_Sk_Debate_Leaderboard>("leaderboard")
    .WithReference(orleans)
    .WithExternalHttpEndpoints();

builder.Build().Run();
