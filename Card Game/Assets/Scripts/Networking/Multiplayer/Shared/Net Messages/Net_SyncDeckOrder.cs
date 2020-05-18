[System.Serializable]
public class Net_SyncDeckOrder : NetMsg
{
    public Net_SyncDeckOrder()
    {
        OP = NetOP.SyncDeckOrder;
    }

    public int[] cardIds { get; set; }
    public byte deckCpId { get; set; }
}
