public static class BCryptImplementation
{
    public static string GetRandomSalt()
    {
        return BCrypt.Net.BCrypt.GenerateSalt(12);
    }

    public static string HashPassword(string password, string salt)
    {
        password += salt;
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public static bool ValidatePassword(string password, string correctHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, correctHash);
    }

    public static bool ValidatePassword(Model_Account account, string password)
    {
        password = HashPassword(password, account.Salt);
        return ValidatePassword(password, account.HashedPassword);
    }
}
