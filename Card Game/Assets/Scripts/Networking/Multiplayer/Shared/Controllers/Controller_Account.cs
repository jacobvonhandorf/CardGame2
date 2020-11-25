using Google.Cloud.Datastore.V1;
using System;

public class Controller_Account
{
    public Model_Account model { get; set; }
    public static class Enum
    {
        public const string account = "Account";
        public const string username = "username";
        public const string hashedPassword = "hashedPassword";
        public const string salt = "salt";
        public const string email = "email";
        public const string discriminator = "discriminator";
        public const string activeConnection = "activeConnection";
        public const string status = "status";
        public const string token = "token";
        public const string lastLogin = "lastLogin";
    }

    private Controller_Account() { }

    public static Controller_Account BuildController(Entity e)
    {
        Controller_Account controller = new Controller_Account();
        Model_Account model = new Model_Account();
        Value v;

        model.Username = e.Properties[Enum.username].StringValue;
        model.HashedPassword = e.Properties[Enum.hashedPassword].StringValue;
        model.Discriminator = e.Properties[Enum.discriminator].StringValue;

        if (e.Properties.TryGetValue(Enum.activeConnection, out v))
            model.ActiveConnection = (int)v.IntegerValue;
        if (e.Properties.TryGetValue(Enum.status, out v))
            model.Status = (byte)v.IntegerValue;
        if (e.Properties.TryGetValue(Enum.token, out v))
            model.Token = v.StringValue;
        if (e.Properties.TryGetValue(Enum.lastLogin, out v))
            model.LastLogin = new DateTime(v.IntegerValue);
        if (e.Properties.TryGetValue(Enum.salt, out v))
            model.Salt = v.StringValue;


        controller.model = model;
        return controller;
    }

    public static Entity BuildEntity(Model_Account model)
    {
        Key key = Datastore.ds.keyBuilder(Enum.account).SetId(model.Email).Build();

        var e = new Entity
        {
            Key = key,
            [Enum.username] = model.Username,
            [Enum.email] = model.Email,
            [Enum.discriminator] = model.Discriminator,
            [Enum.activeConnection] = model.ActiveConnection,
            [Enum.status] = model.Status,
            [Enum.token] = model.Token,
            [Enum.lastLogin] = model.LastLogin.Ticks,
            [Enum.hashedPassword] = model.HashedPassword,
            [Enum.salt] = model.Salt
        };

        return e;
    }
}
