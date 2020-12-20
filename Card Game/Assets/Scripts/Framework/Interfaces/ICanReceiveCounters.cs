public interface ICanReceiveCounters
{
    void OnCountersAdded(CounterType counterType, int amount);
}
