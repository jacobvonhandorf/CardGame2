using Google.Cloud.Datastore.V1;
using UnityEngine;

public class Datastore
{
    private const string projectId = "blanked out for privacy";
    private const string namespaceId = "TestingEnvironment";
    //private const string namespaceId = "Production";

    private DatastoreDb db;
    public static Datastore ds;

    public void Init()
    {
        db = DatastoreDb.Create(projectId);
        if (ds != null)
            Debug.LogError("Trying to initialize a Datastore instance when one already exists");
        ds = this;
    }

    public class KeyBuilder
    {
        private string kind;
        private Entity parentEntity;
        private Key parentKey;
        private long id;
        private string stringId;

        public KeyBuilder(string kind)
        {
            this.kind = kind;
        }
        public KeyBuilder SetParent(Entity e)
        {
            parentEntity = e;
            return this;
        }
        public KeyBuilder SetParent(Key k)
        {
            parentKey = k;
            return this;
        }
        public KeyBuilder SetId(long id)
        {
            this.id = id;
            return this;
        }
        public KeyBuilder SetId(string id)
        {
            stringId = id;
            return this;
        }
        public Key Build()
        {
            KeyFactory kf;
            if (parentEntity == null && parentKey == null)
                kf = new KeyFactory(projectId, namespaceId, kind);
            else if (parentKey != null)
                kf = new KeyFactory(parentKey, kind);
            else
                kf = new KeyFactory(parentEntity, kind);
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
            return Controller_Account.BuildController(e).model;
    }
    public Model_Account FindAccount(string username, string discriminator)
    {
        Query query = new Query(Controller_Account.Enum.account)
        {
            Filter = Filter.And(Filter.Equal(Controller_Account.Enum.username, username),
                                Filter.Equal(Controller_Account.Enum.discriminator, discriminator))
        };
        DatastoreQueryResults results = db.RunQuery(query);
        if (results.Entities.Count == 0)
            return null;
        else
            return Controller_Account.BuildController(results.Entities[0]).model;
    }
    public Entity GetOneResult(Query q)
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
    public Model_Account LoginAccount(string usernameOrEmail, string password, int cnnId, string token)
    {
        Model_Account accountModel = null;
        Query q = null;
        
        if (AccountUtils.IsEmail(usernameOrEmail))
        {
            // login via email
            q = new Query(Controller_Account.Enum.account)
            {
                Filter = Filter.Equal(Controller_Account.Enum.email, usernameOrEmail)
            };
        }
        else
        {
            // login with username + discriminator
            string[] userDiscriminator = usernameOrEmail.Split('#');
            if (userDiscriminator[1] != null)
            {
                q = new Query(Controller_Account.Enum.account)
                {
                    Filter = Filter.And(
                        Filter.Equal(Controller_Account.Enum.username, userDiscriminator[0]),
                        Filter.Equal(Controller_Account.Enum.discriminator, userDiscriminator[1]))
                };
            }
        }
        // perform query to find the account
        Entity accountEntity = GetOneResult(q);
        if (accountEntity != null)
        {
            accountModel = Controller_Account.BuildController(accountEntity).model;
            if (!BCryptImplementation.ValidatePassword(accountModel, password))
                return null;

            // perform login
            accountModel.ActiveConnection = cnnId;
            accountModel.Token = token;
            accountModel.Status = 1; // status of 1 means logged in
            accountModel.LastLogin = System.DateTime.Now;
            StoreEntity(Controller_Account.BuildEntity(accountModel));
        }
        return accountModel;
    }
    public byte CreateAccount(string username, string password, string email)
    {
        if (!AccountUtils.IsEmail(email))
            return CreateAccountResponseCode.invalidEmail;
        if (!AccountUtils.IsUsername(username))
            return CreateAccountResponseCode.invalidUsername;
        if (FindAccountByEmail(email) != null) // if account already exists
            return CreateAccountResponseCode.emailAlreadyUsed;
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
        model.Username = username;
        model.Discriminator = discriminator;
        model.Email = email;
        model.Salt = salt;
        model.HashedPassword = hashedPassword;

        StoreEntity(Controller_Account.BuildEntity(model));

        return CreateAccountResponseCode.success;
    }
    public void StoreEntity(Entity e)
    {
        e.Key.PartitionId = new PartitionId(projectId, namespaceId);
        using (DatastoreTransaction transaction = db.BeginTransaction())
        {
            transaction.Upsert(e);
            transaction.Commit();
        }
    }
    #endregion

    #region Delete
    #endregion
}
