using System;

public class Model_Account
{
    public string email;
    public string username;
    public string discriminator;
    public string hashedPassword;
    public string salt;
    public int activeConnection;
    public byte status; // user is online, offline or other status
    public string token;
    public DateTime lastLogin;
}
