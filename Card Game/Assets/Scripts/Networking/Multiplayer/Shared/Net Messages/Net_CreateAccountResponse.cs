[System.Serializable]
public class Net_CreateAccountResponse : NetMsg
{
    public Net_CreateAccountResponse()
    {
        OP = NetOP.CreateAccountResponse;
    }

    public byte success { set; get; }
}

public static class CreateAccountResponseCode
{
    public const int success = 0;
    public const int invalidEmail = 1;
    public const int invalidUsername = 2;
    public const int emailAlreadyUsed = 3;
    public const int overUsedUsername = 4;
}