using System.Threading.Tasks;
using Raven.Abstractions.Data;
using Raven.Assure.Log;
using Raven.Client.Document;

namespace Raven.Assure.Fluent
{
   public class BackUp : AssureBase, IBackUp<BackUp>
   {
      public string BackupLocation { get; private set; }
      public string DatabaseName { get; private set; }
      public string ServerUrl { get; private set; }
      public bool Incremental { get; private set; }

      public IBackUp<BackUp> From(string databaseName)
      {
         this.DatabaseName = databaseName;

         return this;
      }

      public IBackUp<BackUp> To(string path)
      {
         this.BackupLocation = path;
         
         return this;
      }

      public IBackUp<BackUp> At(string url)
      {
         this.ServerUrl = url;

         return this;
      }

      public IBackUp<BackUp> Incrementally(bool incremental = true)
      {
         this.Incremental = incremental;

         return this;
      }

      public IBackUp<BackUp> RemoveEncryptionKey(bool removeEncryptionKey = true)
      {
         throw new System.NotImplementedException();
      }

      public Task<bool> RunAsync()
      {
         throw new System.NotImplementedException();
      }

      public bool Run()
      {
         logger.Info($@"Running smuggle out...
   from {this.ServerUrl}/{this.DatabaseName}
   to {this.BackupLocation}
   with settings:
      incrementally: {this.Incremental}
");

         using (var store = new DocumentStore()
         {
            Url = this.ServerUrl,
            DefaultDatabase = this.DatabaseName
         }.Initialize())
         {
            store.DatabaseCommands.GlobalAdmin.StartBackup(
               backupLocation: this.BackupLocation,
               databaseDocument: new DatabaseDocument(),
               incremental: this.Incremental,
               databaseName: this.DatabaseName
            );

            logger.Info("No idea when it will end...:-s");

            return true;
         }
      }

      public new BackUp LogWith(ILogger logger)
      {
         return (BackUp) base.LogWith(logger);
      }
   }
}