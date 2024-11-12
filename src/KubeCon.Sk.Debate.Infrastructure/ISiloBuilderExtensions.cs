using KubeCon.Sk.Debate.Abstractions;
using KubeCon.Sk.Debate.Infrastructure;

namespace Orleans.Hosting;

public static class ISiloBuilderExtensions
{
    public static ISiloBuilder AddAgent<TAgentGrain>(this ISiloBuilder builder) where TAgentGrain : IAgentGrain
    {
        builder.ConfigureServices(services =>
        {
            services.AddHostedService<AgentWorker<TAgentGrain>>();
        });

        return builder;
    }

}