namespace Waves.Core;

public class GameTick
{
    private Timer _timer;
    public event Action OnTick;

    public GameTick(int tickRateMilliseconds)
    {
        _timer = new(Tick, null, 0, tickRateMilliseconds);
    }

    private void Tick(object state)
    {
        OnTick.Invoke();
    }

    public void Subscribe(Action action)
    { 
        OnTick += action;
    }

    public void Unsubscribe(Action action)
    {
        OnTick -= action;
    }
}
