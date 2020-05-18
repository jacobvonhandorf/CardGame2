[System.Serializable]
public class Net_SyncPlayerResources : NetMsg
{
    public Net_SyncPlayerResources()
    {
        OP = NetOP.SyncPlayerStats;
    }

    public bool isPlayerOne { get; set; }
    public int gold { get; set; }
    public int goldPTurn { get; set; }
    public int mana { get; set; }
    public int manaPTurn { get; set; }
    public int actions { get; set; }
    public int actionsPTurn { get; set; }
}
