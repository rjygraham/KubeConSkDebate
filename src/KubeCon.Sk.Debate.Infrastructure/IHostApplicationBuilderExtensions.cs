using KubeCon.Sk.Debate.ServiceDefaults;

namespace Microsoft.AspNetCore.Builder;

public static class IHostApplicationBuilderExtensions
{
    public static IHostApplicationBuilder AddSkDebateOrleans(this IHostApplicationBuilder builder, Action<ISiloBuilder>? action = null)
    {
        builder.AddKeyedAzureTableClient(AppHostConstants.OrleansClusteringName, _ => _.DisableTracing = true);
        builder.AddKeyedAzureBlobClient(AppHostConstants.OrleansGrainStateName, _ => _.DisableTracing = true);
        builder.UseOrleans(siloBuilder =>
        {
            action?.Invoke(siloBuilder);
        });

        return builder;
    }
}
