using KubeCon.Sk.Debate.DefaultAgents;
using KubeCon.Sk.Debate.DefaultAgents.Plugins;
using KubeCon.Sk.Debate.Infrastructure.PipelinePolicies;
using Microsoft.Extensions.Azure;
using Microsoft.SemanticKernel;
using System.ClientModel.Primitives;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddServiceDefaults();

builder.AddAzureOpenAIClient("aoai", configureClientBuilder: clientBuilder =>
{
    clientBuilder.ConfigureOptions(options =>
    {
        options.AddPolicy(new SingleAuthorizationHeaderPolicy(), PipelinePosition.PerTry);
    });
});

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