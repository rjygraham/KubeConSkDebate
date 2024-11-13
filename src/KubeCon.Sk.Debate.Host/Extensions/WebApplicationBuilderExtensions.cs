using KubeCon.Sk.Debate.Infrastructure.PipelinePolicies;
using Microsoft.Extensions.Azure;
using Microsoft.SemanticKernel;
using System.ClientModel.Primitives;

namespace KubeCon.Sk.Debate.Host.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddApplicationComponents(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<DebateEngineStateController>();
        builder.Services.AddHostedService<DebateEngine>();
        builder.Services.AddTransient<ILifecycleParticipant<ISiloLifecycle>, SiloLifecycleObserver>();

        builder.Services.AddEndpointsApiExplorer();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        builder.AddAzureOpenAIClient("aoai", configureClientBuilder: clientBuilder =>
        {
            clientBuilder.ConfigureOptions(options =>
            {
                options.AddPolicy(new SingleAuthorizationHeaderPolicy(), PipelinePosition.PerTry);
            });
        });

        var aoaiDeploymentName = builder.Configuration.GetValue<string>("AOAI_DEPLOYMENT_NAME");
        builder.Services.AddKernel()
            .AddAzureOpenAIChatCompletion(aoaiDeploymentName);

        // Add the resources which you will use for Orleans clustering and Grain Storage.
        builder.AddKeyedAzureTableClient("clustering");
        builder.AddKeyedAzureBlobClient("grain-state");
        builder.UseOrleans();

        return builder;
    }
}