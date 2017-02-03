using System;
using System.IO.Abstractions;
using System.Threading.Tasks;
using Raven.Abstractions.Data;
using Raven.Assure.Fluent;
using Raven.Assure.Log;
using Raven.Client.Document;

namespace Raven.Assure.Restore
{
   public class RestoreDatabase : AssureDataBase, IRestoreDatabase<RestoreDatabase>
   {
      public string DatabaseLocation { get; private set; }

      public IRestoreDatabase<RestoreDatabase> From(string path)
      {
         this.BackupLocation = path;

         return this;
      }

      public IRestoreDatabase<RestoreDatabase> To(string databaseName)
      {
         this.DatabaseName = databaseName;

         return this;
      }

      public IRestoreDatabase<RestoreDatabase> At(string url)
      {
         this.ServerUrl = url;

         return this;
      }

      public IRestoreDatabase<RestoreDatabase> In(string databaseLocation)
      {
         this.DatabaseLocation = databaseLocation;

         return this;
      }

      public Task<bool> RunAsync()
      {
         throw new System.NotImplementedException();
      }

      public bool Run()
      {
         var restoreStartedOn = DateTime.Now;

         logger.Info($@"Running assure in...
   from {this.BackupLocation}
   to {this.ServerUrl}/{this.DatabaseName}
   in {this.DatabaseLocation}
");

         using (var store = new DocumentStore()
         {
            Url = this.ServerUrl
         }.Initialize())
         {
            var restoreOperation = store.DatabaseCommands.GlobalAdmin.StartRestore(new DatabaseRestoreRequest() {
               BackupLocation = this.BackupLocation,
               DatabaseName = this.DatabaseName
            });

            logger.Info("RavenDB provides no status updates on the restore operation...just wait until it completes :-s");

            restoreOperation.WaitForCompletion();
         }

         var restoreEndedOn = DateTime.Now;

         var timeToRestore = restoreEndedOn - restoreStartedOn;

         logger.NewLine();
         logger.Info($"RestoreDatabase from {this.BackupLocation} to {this.DatabaseName} completed!");
         logger.Info($"Total restore time: {timeToRestore}");

         return true;
      }

      public new RestoreDatabase LogWith(ILogger logger)
      {
         return (RestoreDatabase) base.LogWith(logger);
      }

      public new RestoreDatabase On(IFileSystem fileSystem)
      {
         return (RestoreDatabase) base.On(fileSystem);
      }
   }
}