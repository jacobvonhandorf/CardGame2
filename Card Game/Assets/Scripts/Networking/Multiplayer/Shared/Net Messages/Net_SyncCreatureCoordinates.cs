﻿[System.Serializable]
public class Net_SyncCreatureCoordinates : NetMsg
{
    public Net_SyncCreatureCoordinates()
    {
        OP = NetOP.SyncCreatureCoordinates;
    }

    public int creatureCardId { get; set; }
    public int sourceCardId { get; set; }
    public byte x { get; set; }
    public byte y { get; set; }
}
