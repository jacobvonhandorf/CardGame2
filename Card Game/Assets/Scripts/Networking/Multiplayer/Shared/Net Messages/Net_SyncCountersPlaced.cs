[System.Serializable]
public class Net_SyncCountersPlaced : NetMsg
{
    public Net_SyncCountersPlaced()
    {
        OP = NetOP.SyncCountersPlaced;
    }

    public int counterId { get; set; }
    public int amount { get; set; }
    public int targetCardId { get; set; }
}


