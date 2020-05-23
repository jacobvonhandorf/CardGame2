[System.Serializable]
public class Net_EnterMMPool : NetMsg
{
    public Net_EnterMMPool()
    {
        OP = NetOP.EnterMMPool;
    }

    public bool inPool { get; set; }
}
