[System.Serializable]
public class Net_MoveCardToPile : NetMsg
{
    public Net_MoveCardToPile()
    {
        OP = NetOP.MoveCardToPile;
    }

    public byte cpId { get; set; }
    public int cardId { get; set; }
    public int sourceId { get; set; }
}
