[System.Serializable]
public class Net_SyncPermanentPlaced : NetMsg
{
    public Net_SyncPermanentPlaced()
    {
        OP = NetOP.SyncPermanentPlaced;
    }

    public int sourceCardId { set; get; }
    public int x { set; get; }
    public int y { set; get; }
}

