using DbUp;
using DbUp.Engine;
using DBUp.Exceptions;
using DBUp.Utils;
using System;
using System.IO;
using System.Reflection;

namespace DBUp.Services
{
    public class DBMigration
    {
        //public string physicalPathScripts = "../../SQLScripts";
        #region Public Methods
        public string ExecuteMigration(string pathScriptFolder, string connectionString)
        {
            var logger = new Logger();
            var path = pathScriptFolder + @"\ChangeInDB";
            UpgradeEngine upgrader = GetDbUpgrader(path, logger, connectionString);
            var result = upgrader.PerformUpgrade();
            if (result.Successful)
            {
                return logger.GetLog();
            }
            else
            {
                // there were errors when updating the DB so we need to throw an exception to stop the execution of the process
                throw new DBMigrationException(result.Error.Message);
            }
        }
        #endregion

        #region private DbUp Engine UpgradeEngine Methods
        private static UpgradeEngine GetDbUpgrader(string pathScriptFolder, Logger logger, string connectionString)
        {
            var AllPathToSqlScripts = Directory.GetDirectories(pathScriptFolder);
            var upgrader = DeployChanges.To
                .MySqlDatabase(connectionString)
                .WithTransactionPerScript()
                .LogTo(logger);
            foreach ( string path in AllPathToSqlScripts )
            {
                upgrader.WithScriptsFromFileSystem(path);
            }
            return upgrader.Build(); ;
        }
        #endregion

        public static bool CheckIfUpgradeRequired(string pathScriptFolder, string connectionString)
        {
            // pathScriptFolder = string.IsNullOrEmpty(pathScriptFolder) ? HttpRuntime.AppDomainAppPath + @"bin\SqlScripts\MainDB\" : pathScriptFolder;
            var logger = new Logger();
            
            // check if the schemaversions exixsts, if not create it
            CreateDBUpSchemaVersion(logger, connectionString);

            //Check for schema, expertdb, some default values
            if (!CheckIfAddressTableExists(connectionString))
            {
                // in case the default DB is not created we need to create it, if the others DBs fails 
                // there is nothing we can do about it
                CreateDBCustomerSchema(connectionString);
                return true;
            }
            return GetDbUpgrader(pathScriptFolder+@"\ChangeInDB", logger, connectionString).IsUpgradeRequired();
        }

        private static void CreateDBUpSchemaVersion(Logger logger, string connectionString)
        {
            var upgradeEngine = DeployChanges.To
                .MySqlDatabase(connectionString)
                .WithScriptsFromFileSystem(GetBlankScriptsPath())
                .LogTo(logger)
                .Build();

            //Checking for schemaversions table - If not - created.
            var existSchemaVersions = ExistsSchemaVersionTable(upgradeEngine);
            if (!existSchemaVersions)
            {
                var builder1 = DeployChanges.To
                                .MySqlDatabase(connectionString)
                                .WithScript("CreateSchema.sql", $@"CREATE TABLE `schemaversions` (
                                  `schemaversionid` INT(11) NOT NULL AUTO_INCREMENT,
                                  `scriptname` VARCHAR(255) NOT NULL,
                                  `applied` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
                                  PRIMARY KEY (`schemaversionid`));"
                                )
                                .LogTo(logger)
                                .LogScriptOutput()
                                .Build();

                builder1.PerformUpgrade();
            }
        }

        private static bool CheckIfAddressTableExists(string connectionString)
        {
            //var connectionString = ExpertDB.Utils.Utils.GetTransformedConnectionString(Globals.DBConnectionString, Guid.Empty);
            var upgrader = DeployChanges.To
                .MySqlDatabase(connectionString)
                .WithScript("CreateAddress.sql", "Select * From `Address`")
                .Build();

            var result = upgrader.PerformUpgrade();
            return result.Successful;
        }

        private static bool ExistsSchemaVersionTable(UpgradeEngine upgradeEngine)
        {
            try
            {
                return upgradeEngine.IsUpgradeRequired();
            }
            catch (Exception)
            {
                return false;
            }
        }
       
        private static string GetBlankScriptsPath()
        {
            // based on the location of the server we will use different scripts for the creation of the Database
            // because the SLU tables data doesn't match on both servers and if the wrong scripts are used it will fail

            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            var currentDirectory = System.IO.Path.GetDirectoryName(path);
            //var solutionPath = currentDirectory.Replace("VCLWebAPI\\bin", "");
            var scriptFolderPath = currentDirectory + @"\SQLScripts";

            return scriptFolderPath + @"\BlankDB\";                  
        }

        public static void CreateDBCustomerSchema(string connectionString)
        {
            var logger = new Logger();

            try
            {
                //TBD
                //dbContext.VCL.ExecuteSqlCommand(string.Format("CREATE DATABASE {0}", $"ExpertDB_{customerId.ToString().Replace("-", string.Empty)}"));
                //var connectionString = "Server=localhost;Database=VCLDesignDB;user id=root;password=SchucoUSA1234!;";

                EnsureDatabase.For.MySqlDatabase(connectionString);

                var scriptFolderPath = GetBlankScriptsPath();
                
                var runScripts = DeployChanges.To
                .MySqlDatabase(connectionString)
                .WithScriptsFromFileSystem(scriptFolderPath)
                .LogTo(logger)
                .Build();

                var result = runScripts.PerformUpgrade();

                if (!result.Successful)
                {
                    throw new Exception();
                }
            }
            catch (Exception e)
            {

                throw;
            }
        }


    }
}