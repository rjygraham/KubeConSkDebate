using KubeCon.Sk.Debate.Abstractions;
using KubeCon.Sk.Debate.Abstractions.Models;

namespace KubeCon.Sk.Debate.Host;

public class DebateEngine(IGrainFactory grainFactory, DebateEngineStateController debateEngineStateController, ILogger<DebateEngine> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!DebateEngineStateController.IsSiloReady)
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
        }

        var currentDebateGrain = grainFactory.GetGrain<IDebateGrain>(Guid.NewGuid());
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Send a system status update
                var grainCount = await grainFactory.GetGrain<IManagementGrain>(0).GetTotalActivationCount();
                var leaderboardGrain = grainFactory.GetGrain<ILeaderboardGrain>(Guid.Empty);
                await leaderboardGrain.UpdateSystemStatus(new SystemStatusUpdate
                {
                    DateStarted = debateEngineStateController.ServerStartedTime,
                    DebatesCompleted = debateEngineStateController.DebatesCompleted,
                    TimeUp = debateEngineStateController.Uptime.Elapsed,
                    GrainsActive = grainCount
                });
            }
            catch (Exception ex)
            {
                if (!stoppingToken.IsCancellationRequested)
                {
                    logger.LogError(ex, "SkD: DebateEngine SystemStatusUpdate error.");
                }
            }

            if (debateEngineStateController.IsStarted)
            {
                try
                {
                    var currentDebate = await currentDebateGrain.GetDebate();
                    debateEngineStateController.CurrentDebateId = currentDebate.Id;

                    // Select a topic if it's unselected so far.
                    if (string.IsNullOrEmpty(currentDebate.Topic))
                    {
                        await currentDebateGrain.SelectTopicAsync();
                    }

                    // Select debaters if they're unselected so far.
                    if (currentDebate.Moderator is null || currentDebate.Debater1 is null || currentDebate.Debater2 is null)
                    {
                        await currentDebateGrain.SelectAgentsAsync();
                    }

                    currentDebate = await currentDebateGrain.GetDebate();

                    // Start the debate if it's ready and not complete.
                    if (currentDebate.IsReady && !currentDebate.IsComplete)
                    {
                        if (currentDebate.Started == DateTime.MinValue)
                        {
                            await currentDebateGrain.StartDebateAsync();
                        }

                        await currentDebateGrain.DebateAsync();
                    }

                    if (currentDebate.IsComplete)
                    {
                        debateEngineStateController.IncrementDebateCount();
                        debateEngineStateController.CurrentDebateId = Guid.Empty;
                        debateEngineStateController.Stop();

                        currentDebateGrain = grainFactory.GetGrain<IDebateGrain>(Guid.NewGuid());
                    }
                }
                catch (Exception ex)
                {
                    if (!stoppingToken.IsCancellationRequested)
                    {
                        logger.LogError(ex, "SkD: DebateEngine Debate error.");
                    }
                }
            }

            await Task.Delay(debateEngineStateController.DebateLoopDelayInMilliseconds, stoppingToken);
        }
    }
}
