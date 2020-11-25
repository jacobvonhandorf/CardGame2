using System;

public class Model_Account
{
    public string Email { set; get; }
    public string Username { set; get; }
    public string Discriminator { set; get; }
    public string HashedPassword { set; get; }
    public string Salt { set; get; }
    public int ActiveConnection { set; get; }
    public byte Status { set; get; } // user is online, offline or other status
    public string Token { set; get; }
    public DateTime LastLogin { set; get; }
}
