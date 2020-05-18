public static class NetOP
{
    // operation code enum
    public const int None = 0;
    public const int CreateAccount = 1;
    public const int CreateAccountResponse = 2;
    public const int LoginRequest = 3;
    public const int LoginRequestResponse = 4;
    public const int InGameRelay = 5;
    public const int NotifyClientOfMMPairing = 6;
    public const int InstantiateCard = 7;
    public const int MoveCardToPile = 8;
    public const int SyncDeckOrder = 9;
    public const int SyncCreatureCoordinates = 10;
    public const int EnterMMPool = 11;
    public const int SyncAttack = 12;
    public const int SyncPlayerStats = 13;
    public const int EndTurn = 14;
    public const int SyncStructure = 15;
    public const int SyncCreature = 16;
    public const int SyncCard = 17;
    public const int DoneSendingCards = 18;
    public const int DoneWithSetup = 19;
    public const int SyncPermanentPlaced = 20;
    public const int SyncCountersPlaced = 21;
}

[System.Serializable]
public abstract class NetMsg
{
    public byte OP { set; get; } // operation code

    public NetMsg()
    {
        OP = NetOP.None;
    }
}
