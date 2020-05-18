[System.Serializable]
public class Net_SyncAttack : NetMsg
{
    public Net_SyncAttack()
    {
        OP = NetOP.SyncAttack;
    }
    
    public int attackerId { get; set; }
    public int defenderId { get; set; }
    public int damageRoll { get; set; }
}
