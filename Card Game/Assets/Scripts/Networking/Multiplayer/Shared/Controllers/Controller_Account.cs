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

    public static Controller_Account Build(Entity e)
    {
        Controller_Account controller = new Controller_Account();
        Model_Account model = new Model_Account();
        Value v;

        model.username = e.Properties[Enum.username].StringValue;
        model.hashedPassword = e.Properties[Enum.hashedPassword].StringValue;
        model.discriminator = e.Properties[Enum.discriminator].StringValue;
        if (e.Properties.TryGetValue(Enum.activeConnection, out v))
            model.activeConnection = (int)v.IntegerValue;
        if (e.Properties.TryGetValue(Enum.status, out v))
            model.status = (byte)v.IntegerValue;
        if (e.Properties.TryGetValue(Enum.token, out v))
            model.token = v.StringValue;
        if (e.Properties.TryGetValue(Enum.lastLogin, out v))
            model.lastLogin = new DateTime(v.IntegerValue);
        if (e.Properties.TryGetValue(Enum.salt, out v))
            model.salt = v.StringValue;


        controller.model = model;
        return controller;
    }

    public static Entity Build(Model_Account model)
    {
        Key key = Datastore.ds.keyBuilder(Enum.account).setId(model.email).build();

        var e = new Entity
        {
            Key = key,
            [Enum.username] = model.username,
            [Enum.email] = model.email,
            [Enum.discriminator] = model.discriminator,
            [Enum.activeConnection] = model.activeConnection,
            [Enum.status] = model.status,
            [Enum.token] = model.token,
            [Enum.lastLogin] = model.lastLogin.Ticks,
            [Enum.hashedPassword] = model.hashedPassword,
            [Enum.salt] = model.salt
        };

        return e;
    }
}
