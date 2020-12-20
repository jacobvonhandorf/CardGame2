[System.Serializable]
public class Net_SyncCard : NetMsg
{
    public Net_SyncCard()
    {
        OP = NetOP.SyncCard;
    }

    public int sourceCardId { get; set; }

    public int baseGoldCost { get; set; }
    public int goldCost { get; set; }
    public int baseManaCost { get; set; }
    public int manaCost { get; set; }
    public bool ownerIsP1 { get; set; }
    public ElementIdentity elementalIdentity { get; set; }
}
