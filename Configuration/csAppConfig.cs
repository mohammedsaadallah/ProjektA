using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

//Configuration namespace is base layer
//used for all classed needed to configure the application and it's components
namespace Configuration;

//csAppConfig will contain all configuration in our example
//but it could be segmented into several classes, each with its own
//configuration responisibily
public sealed class csAppConfig
{
    //use the right appsettings file depending on Debug or Release Build
#if DEBUG
    public const string Appsettingfile = "appsettings.Development.json";
#else
        public const string Appsettingfile = "appsettings.json";
#endif

    #region Singleton design pattern
    private static readonly object instanceLock = new();

    private static csAppConfig _instance = null;
    private static IConfigurationRoot _configuration = null;
    #endregion

    //All the DB Connections in the appsetting file
    private static DbSetDetail _dbSetActive = new DbSetDetail();
    private static List<DbSetDetail> _dbSets = new List<DbSetDetail>();
    private static PasswordSaltDetails _passwordSaltDetails = new PasswordSaltDetails();
    private static JwtConfig _jwtConfig = new JwtConfig();

    private csAppConfig()
    {
        //Lets get the credentials access Azure KV and set them as Environment variables
        //During Development this will come from User Secrets,
        //After Deployment it will come from appsettings.json

        string s = Directory.GetCurrentDirectory();

        #region Include Azure Key Vault
#if DEBUG
        //Transfer Azure Key Vault access Keys to Environment Variables
        //Only debug, not after deployment
        var _azureAccess = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile(Appsettingfile, optional: true, reloadOnChange: true)
                            .AddUserSecrets("33525b1b-5a79-489a-b362-6bbb39b99013", reloadOnChange: true)
                            .Build();


        Environment.SetEnvironmentVariable("AZURE_KeyVaultUri", _azureAccess["AZURE_KeyVaultUri"]);
        Environment.SetEnvironmentVariable("AZURE_CLIENT_SECRET", _azureAccess["AZURE_CLIENT_SECRET"]);
        Environment.SetEnvironmentVariable("AZURE_CLIENT_ID", _azureAccess["AZURE_CLIENT_ID"]);
        Environment.SetEnvironmentVariable("AZURE_TENANT_ID", _azureAccess["AZURE_TENANT_ID"]);
#endif

        //Open the AZKV
        var _kvuri = Environment.GetEnvironmentVariable("AZURE_KeyVaultUri");
        var client = new SecretClient(new Uri(_kvuri), new DefaultAzureCredential(
            new DefaultAzureCredentialOptions { AdditionallyAllowedTenants = { "*" } }));

        //Get user-secrets from AZKV and flatten it into a Dictionary<string, string>
        var secret = client.GetSecret("user-secrets");
        var message = secret.Value.Value;
        var userSecretsAKV = JsonFlatToDictionary(message);
        #endregion

        //Create final ConfigurationRoot which includes also AzureKeyVault
        var builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile(Appsettingfile, optional: true, reloadOnChange: true)
                            .AddUserSecrets("33525b1b-5a79-489a-b362-6bbb39b99013", reloadOnChange: true);

        #region Include Azure Key Vault
                            //Instead of loading user-secrets, load the secrets from AZKV
                            //.AddInMemoryCollection(userSecretsAKV);
        #endregion

        _configuration = builder.Build();

        //get DbSet details
        _configuration.Bind("DbSets", _dbSets);  //Need the NuGet package Microsoft.Extensions.Configuration.Binder

        //Set the active db set and fill in location and server into Login Details
        var i = int.Parse(_configuration["DbSetActiveIdx"]);
        _dbSetActive = _dbSets[i];
        _dbSetActive.DbLogins.ForEach(i =>
        {
            i.DbLocation = _dbSetActive.DbLocation;
            i.DbServer = _dbSetActive.DbServer;
        });
        
        //get user password details
        _configuration.Bind("PasswordSaltDetails", _passwordSaltDetails);

        //get jwt configurations
        _configuration.Bind("JwtConfig", _jwtConfig);

    }

    #region Include Azure Key Vault
    private static Dictionary<string, string> JsonFlatToDictionary(string json)
    {
        IEnumerable<(string Path, JsonProperty P)> GetLeaves(string path, JsonProperty p)
            => p.Value.ValueKind != JsonValueKind.Object
                ? new[] { (Path: path == null ? p.Name : path + ":" + p.Name, p) }
                : p.Value.EnumerateObject().SelectMany(child => GetLeaves(path == null ? p.Name : path + ":" + p.Name, child));

        using (JsonDocument document = JsonDocument.Parse(json)) // Optional JsonDocumentOptions options
            return document.RootElement.EnumerateObject()
                .SelectMany(p => GetLeaves(null, p))
                .ToDictionary(k => k.Path, v => v.P.Value.Clone().ToString()); //Clone so that we can use the values outside of using
    }
    #endregion

    public static IConfigurationRoot ConfigurationRoot
    {
        get
        {
            lock (instanceLock)
            {
                if (_instance == null)
                {
                    _instance = new csAppConfig();
                }
                return _configuration;
            }
        }
    }

    public static DbSetDetail DbSetActive
    {
        get
        {
            lock (instanceLock)
            {
                if (_instance == null)
                {
                    _instance = new csAppConfig();
                }
                return _dbSetActive;
            }
        }
    }
    public static DbLoginDetail DbLoginDetails (string DbLogin)
    {
        if (string.IsNullOrEmpty(DbLogin) || string.IsNullOrWhiteSpace(DbLogin))
            throw new ArgumentNullException();

        lock (instanceLock)
        {
            if (_instance == null)
            {
                _instance = new csAppConfig();
            }

            var conn = _dbSetActive.DbLogins.First(m => m.DbUserLogin.Trim().ToLower() == DbLogin.Trim().ToLower());
            if (conn == null)
                throw new ArgumentException("Database connection not found");

            return conn;
        }
    }
    public static string SecretMessage => ConfigurationRoot["SecretMessage"];

    public static string DataSource => ConfigurationRoot["DataSource"];
    public static Uri WebApiBaseUri => new Uri(ConfigurationRoot["WebApiBaseUri"]);

    public static PasswordSaltDetails PasswordSalt
    {
        get
        {
            lock (instanceLock)
            {
                if (_instance == null)
                {
                    _instance = new csAppConfig();
                }
                return _passwordSaltDetails;
            }
        }
    }

    public static JwtConfig JwtConfig
    {
        get
        {
            lock (instanceLock)
            {
                if (_instance == null)
                {
                    _instance = new csAppConfig();
                }
                return _jwtConfig;
            }
        }
    }
}

public class DbSetDetail
{
    public string DbLocation { get; set; }
    public string DbServer { get; set; }

    public List<DbLoginDetail> DbLogins { get; set; }
}

public class DbLoginDetail
{
    //set after reading in the active DbSet
    
    public string DbLocation { get; set; } = null;
    public string DbServer { get; set; } = null;

    public string DbUserLogin { get; set; }
    public string DbConnection { get; set; }
    public string DbConnectionString => csAppConfig.ConfigurationRoot.GetConnectionString(DbConnection);
}


public class PasswordSaltDetails
{
    public string Salt { get; set; }
    public int Iterations { get; set; }
}

public class JwtConfig
{
    public int LifeTimeMinutes { get; set; }

    public bool ValidateIssuerSigningKey { get; set; }
    public string IssuerSigningKey { get; set; }

    public bool ValidateIssuer { get; set; } = true;
    public string ValidIssuer { get; set; }

    public bool ValidateAudience { get; set; } = true;
    public string ValidAudience { get; set; }

    public bool RequireExpirationTime { get; set; }
    public bool ValidateLifetime { get; set; } = true;
}


