
namespace KubeCon.Sk.Debate.Host;

public class SiloLifecycleObserver : ILifecycleParticipant<ISiloLifecycle>, ILifecycleObserver
{
    public Task OnStart(CancellationToken cancellationToken = default)
    {
        DebateEngineStateController.IsSiloReady = true;
        return Task.CompletedTask;
    }

    public Task OnStop(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public void Participate(ISiloLifecycle observer)
    {
        observer.Subscribe(20_000, this);
    }
}
