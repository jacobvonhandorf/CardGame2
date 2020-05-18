[System.Serializable]
public class Net_DoneSendingCards : NetMsg
{
    public Net_DoneSendingCards()
    {
        OP = NetOP.DoneSendingCards;
    }
}
