using static NetInterface;

[System.Serializable]
public class Net_InstantiateCard : NetMsg
{
    public Net_InstantiateCard()
    {
        OP = NetOP.InstantiateCard;
    }

    public SerializeableCard card { set; get; }
    public bool ownerIsP1 { set; get; }
}
