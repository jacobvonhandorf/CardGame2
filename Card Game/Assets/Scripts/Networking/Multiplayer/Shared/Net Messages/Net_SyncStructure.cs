[System.Serializable]
public class Net_SyncStructure : NetMsg
{
    public Net_SyncStructure()
    {
        OP = NetOP.SyncStructure;
    }

    public int sourceCardId { get; set; }
    public bool controllerIsP1 { get; set; }
    public int health { get; set; }
    public int baseHealth { get; set; }
}
