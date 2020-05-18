using Google.Cloud.Datastore.V1;
using UnityEngine;

public class Datastore
{
    private const string projectId = "bubbly-stone-272515";
    private const string namespaceId = "TestingEnvironment";
    //private const string namespaceId = "Production";

    private DatastoreDb db;
    public static Datastore ds;

    public void init()
    {
        // Instantiates a db
        db = DatastoreDb.Create(projectId);
        if (ds != null)
            Debug.LogError("Trying to initialize a Datastore instance when one already exists");
        ds = this;
    }

    private void exampleStore()
    {
        // The kind for the new entity
        string kind = "Task";
        // The name/ID for the new entity
        string name = "sampletask2";
        KeyFactory keyFactory = db.CreateKeyFactory(kind);
        // The Cloud Datastore key for the new entity
        Key key = keyFactory.CreateKey(name);

        var task = new Entity
        {
            Key = key,
            ["description"] = "aaaa"
        };
        using (DatastoreTransaction transaction = db.BeginTransaction())
        {
            // Saves the task
            transaction.Upsert(task);
            transaction.Commit();

            Debug.Log($"Saved {task.Key.Path[0].Name}: {(string)task["description"]}");
        }
    }

    public class KeyBuilder
    {
        private string kind;
        private Entity parentE;
        private Key parentK;
        private long id;
        private string stringId;

        public KeyBuilder(string kind)
        {
            this.kind = kind;
        }
        public KeyBuilder setParent(Entity e)
        {
            parentE = e;
            return this;
        }
        public KeyBuilder setParent(Key k)
        {
            parentK = k;
            return this;
        }
        public KeyBuilder setId(long id)
        {
            this.id = id;
            return this;
        }
        public KeyBuilder setId(string id)
        {
            stringId = id;
            return this;
        }
        public Key build()
        {
            KeyFactory kf;
            if (parentE == null && parentK == null)
                kf = new KeyFactory(projectId, namespaceId, kind);
            else if (parentK != null)
                kf = new KeyFactory(parentK, kind);
            else
                kf = new KeyFactory(parentE, kind);
            if (id > 0)
                return kf.CreateKey(id);
            else if (stringId != null)
                return kf.CreateKey(stringId);
            else
                return kf.CreateIncompleteKey();
        }
    }
    public KeyBuilder keyBuilder(string kind)
    {
        return new KeyBuilder(kind);
    }

#region Fetch
    public Model_Account FindAccountByEmail(string email)
    {
        Key k = db.CreateKeyFactory("Account").CreateKey(email);
        k.PartitionId = new PartitionId(projectId, namespaceId);

        Entity e = db.Lookup(k);
        if (e == null)
            return null;
        else
            return Controller_Account.Build(e).model;
    }
    public Model_Account FindAccount(string username, string discriminator)
    {
        Query query = new Query(Controller_Account.Enum.account)
        {
            Filter = Filter.And( Filter.Equal(Controller_Account.Enum.username, username),
                Filter.Equal(Controller_Account.Enum.discriminator, discriminator))
        };
        DatastoreQueryResults results = db.RunQuery(query);
        if (results.Entities.Count == 0)
            return null;
        else
            return Controller_Account.Build(results.Entities[0]).model;
    }
    public Entity FindOne(Query q)
    {
        DatastoreQueryResults results = db.RunQuery(q);
        if (results.Entities.Count == 0)
            return null;
        else
            return results.Entities[0];
    }
#endregion

#region Update
#endregion

#region Insert
    public Model_Account loginAccount(string usernameOrEmail, string shaPassword, int cnnId, string token)
    {
        Model_Account myAccount = null;
        Entity accountEntity = null;
        Query q = null;

        if (AccountUtils.IsEmail(usernameOrEmail))
        {
            // login via email
            q = new Query(Controller_Account.Enum.account)
            {
                Filter = Filter.And(
                    Filter.Equal(Controller_Account.Enum.username, usernameOrEmail),
                    Filter.Equal(Controller_Account.Enum.hashedPassword, shaPassword))
            };
        }
        else
        {
            // login with username + discriminator
            string[] data = usernameOrEmail.Split('#');
            if (data[1] != null)
            {
                q = new Query(Controller_Account.Enum.account)
                {
                    Filter = Filter.And(
                        Filter.Equal(Controller_Account.Enum.username, data[0]),
                        Filter.Equal(Controller_Account.Enum.discriminator, data[1]),
                        Filter.Equal(Controller_Account.Enum.hashedPassword, shaPassword))
                };
            }
        }
        // perform query to find the account
        accountEntity = FindOne(q);
        if (accountEntity != null)
        {
            // account found, perform login
            myAccount = Controller_Account.Build(accountEntity).model;
            myAccount.activeConnection = cnnId;
            myAccount.token = token;
            myAccount.status = 1;
            myAccount.lastLogin = System.DateTime.Now;
            storeEntity(Controller_Account.Build(myAccount));
        }
        return myAccount;
    }

    // 0 = success
    // 1 = invalid email
    // 2 = invalid username
    // 3 = email already in use
    // 4 = popular username, try again
    public byte createAccount(string username, string password, string email)
    {
        if (!AccountUtils.IsEmail(email))
        {
            Debug.Log(email + " is not a email");
            return CreateAccountResponseCode.invalidEmail;
        }
        if (!AccountUtils.IsUsername(username))
        {
            Debug.Log(username + " is not a username");
            return CreateAccountResponseCode.invalidUsername;
        }
        if (FindAccountByEmail(email) != null) // if account already exists
        {
            Debug.Log(email + " is already being used");
            return CreateAccountResponseCode.emailAlreadyUsed;
        }
        // account credentials are valid

        string salt = BCryptImplementation.GetRandomSalt();
        string hashedPassword = BCryptImplementation.HashPassword(password, salt);


        // roll for a unique discriminator
        int rollCount = 0;
        string discriminator = "0000";
        while (FindAccount(username, discriminator) != null)
        {
            discriminator = Random.Range(0, 9999).ToString("000");
            rollCount++;
            if (rollCount > 100)
            {
                Debug.Log("Rolled over 100 times for account");
                return CreateAccountResponseCode.overUsedUsername;
            }
        }

        Model_Account model = new Model_Account();
        model.username = username;
        model.discriminator = discriminator;
        model.email = email;
        model.salt = salt;
        model.hashedPassword = hashedPassword;

        storeEntity(Controller_Account.Build(model));

        return CreateAccountResponseCode.success;
    }
    public void storeEntity(Entity e)
    {
        e.Key.PartitionId = new PartitionId(projectId, namespaceId);
        using (DatastoreTransaction transaction = db.BeginTransaction())
        {
            transaction.Upsert(e);
            transaction.Commit();
            Debug.Log("Stored new entity");
        }
    }

#endregion

#region Delete
#endregion
}
