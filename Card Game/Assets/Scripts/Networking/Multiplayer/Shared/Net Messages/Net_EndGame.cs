[System.Serializable]
public class Net_EndGame : NetMsg
{
    public Net_EndGame()
    {
        OP = NetOP.EndGame;
    }

    public EndGameCode endGameCode { get; set; }
}

public enum EndGameCode
{
    Disconnect,
    Quit
}
