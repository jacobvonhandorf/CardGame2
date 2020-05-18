[System.Serializable]
public class Net_InGameRelay : NetMsg
{
    public Net_InGameRelay()
    {
        OP = NetOP.InGameRelay;
    }

    public NetMsg msg { set; get; }
}
