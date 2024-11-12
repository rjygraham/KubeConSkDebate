using KubeCon.Sk.Debate.DefaultAgents;
using KubeCon.Sk.Debate.DefaultAgents.Plugins;
using Microsoft.SemanticKernel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddServiceDefaults();

builder.AddAzureOpenAIClient("aoai");

//builder.AddAzureOpenAIClient("aoai", configureClientBuilder: builder =>
//{
//    builder.ConfigureOptions(options =>
//    {
//        options.AddPolicy(new SingleAuthorizationHeaderPolicy(), PipelinePosition.PerTry);
//    });
//});

var aoaiDeploymentName = builder.Configuration.GetValue<string>("AOAI_DEPLOYMENT_NAME");
builder.Services.AddKernel()
    .AddAzureOpenAIChatCompletion(aoaiDeploymentName)
    .Plugins.AddFromType<CoinFlipPlugin>();

builder.AddSkDebateOrleans(siloBuilder =>
    siloBuilder
        .AddAgent<DefaultModeratorAgentGrain>()
        .AddAgent<DefaultAgainstDebaterAgentGrain>()
        .AddAgent<DefaultForDebaterAgentGrain>()
);

var app = builder.Build();

app.MapDefaultEndpoints();

app.Run();