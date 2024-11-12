using KubeCon.Sk.Debate.Abstractions;

namespace KubeCon.Sk.Debate.Infrastructure;

public sealed class AgentWorker<TAgent>(IGrainFactory grainFactory, ILogger<AgentWorker<TAgent>> logger)
    : BackgroundService where TAgent : IAgentGrain
{
    private IAgentSessionGrain? agentSessionGrain;

    private async Task SignAgentIn(CancellationToken cancellationToken)
    {
        bool keepTrying = true;
        var agentType = typeof(TAgent);

        while (keepTrying)
        {
            try
            {
                agentSessionGrain = grainFactory.GetGrain<IAgentSessionGrain>(agentType.Name);
                await agentSessionGrain.SignIn(agentType.FullName).ConfigureAwait(false);
                keepTrying = false;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Error registering agent {AgentType}. Debate Engine may not be up yet. Trying again in 1 second.", agentType);
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (agentSessionGrain == null)
            {
                await SignAgentIn(stoppingToken);
            }
            else
            {
                var isAgentOnline = await agentSessionGrain.IsAgentOnline();
                var isAgentKicked = await agentSessionGrain.IsAgentKicked();

                if (!isAgentOnline && !isAgentKicked)
                {
                    await SignAgentIn(stoppingToken);
                }
            }

            await Task.Delay(TimeSpan.FromMinutes(1));
        }
    }
}
