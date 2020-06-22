// client sends message with version null and gets back
// a version to check vs its the same as stored one
[System.Serializable]
public class Net_CheckVersion : NetMsg
{
    public Net_CheckVersion()
    {
        OP = NetOP.CheckVersion;
    }

    public string version { get; set; }
}
