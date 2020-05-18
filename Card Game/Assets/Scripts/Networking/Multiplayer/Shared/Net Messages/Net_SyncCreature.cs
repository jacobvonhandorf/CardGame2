[System.Serializable]
public class Net_SyncCreature : NetMsg
{
    public Net_SyncCreature()
    {
        OP = NetOP.SyncCreature;
    }

    public int sourceCardId { get; set; }

    public bool controllerIsP1 { get; set; }
    public int baseHealth { get; set; }
    public int baseAttack { get; set; }
    public int baseRange { get; set; }
    public int baseMovement { get; set; }
    public int health { get; set; }
    public int attack { get; set; }
    public int range { get; set; }
    public int movement { get; set; }
}
