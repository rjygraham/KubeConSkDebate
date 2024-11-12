using KubeCon.Sk.Debate.Abstractions;
using KubeCon.Sk.Debate.Abstractions.Models;

namespace KubeCon.Sk.Debate.Host;

public class DebateEngine(IGrainFactory grainFactory, DebateEngineStateController debateEngineStateController, ILogger<DebateEngine> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var currentDebateGrain = grainFactory.GetGrain<IDebateGrain>(Guid.NewGuid());

        while (!stoppingToken.IsCancellationRequested)
        {
            if (debateEngineStateController.IsStarted)
            {
                try
                {
                    var currentDebate = await currentDebateGrain.GetDebate();

                    // Select debaters if they're unselected so far
                    if (currentDebate.Moderator is null || currentDebate.Debater1 is null || currentDebate.Debater2 is null)
                    {
                        await currentDebateGrain.SelectAgents();
                    }
                    else
                    {
                        if (currentDebate.Winner is null)
                        {
                            await currentDebateGrain.Go();
                            //await currentDebateGrain.ScoreTurn();
                        }
                        else
                        {
                            //await currentDebateGrain.ScoreDebate();
                            debateEngineStateController.IncrementDebateCount();

                            // Start a new game.
                            currentDebateGrain = grainFactory.GetGrain<IDebateGrain>(Guid.NewGuid());
                        }
                    }

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
                        logger.LogError(ex, "SkD: DebateEngine error.");
                    }
                }
            }

            await Task.Delay(debateEngineStateController.DebateLoopDelayInMilliseconds, stoppingToken);
        }
    }
}
