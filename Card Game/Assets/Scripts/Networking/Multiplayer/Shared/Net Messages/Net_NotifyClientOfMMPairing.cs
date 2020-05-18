[System.Serializable]
public class Net_NotifyClientOfMMPairing : NetMsg
{
    public Net_NotifyClientOfMMPairing()
    {
        OP = NetOP.NotifyClientOfMMPairing;
    }

    public bool isPlayer1{ set; get; }

}
