[System.Serializable]
public class Net_LoginRequestResponse : NetMsg
{
    public Net_LoginRequestResponse()
    {
        OP = NetOP.LoginRequestResponse;
    }

    public byte success { set; get; } // 0 = success, 1 = failure
    public string information { set; get; }
    public string username { set; get; }
    public string discriminator { set; get; }
    public string token { set; get; }
    public int connectionId { set; get; }
    
}
