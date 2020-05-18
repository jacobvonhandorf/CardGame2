[System.Serializable]
public class Net_EndTurn : NetMsg
{
    public Net_EndTurn()
    {
        OP = NetOP.EndTurn;
    }
}
