using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Raven.Abstractions.Data;
using Raven.Abstractions.Extensions;
using Raven.Assure.Fluent;
using Raven.Assure.Log;
using Raven.Assure.ResourceDocument;
using Raven.Client;
using Raven.Client.Connection;
using Raven.Client.Document;
using Raven.Imports.Newtonsoft.Json;
using Raven.Json.Linq;

namespace Raven.Assure.BackUp
{
   public class BackUpDatabase : AssureDataBase, IBackUpDatabase<BackUpDatabase>
   {
      public bool Incremental { get; private set; }
      public bool RemoveEncryptionKey { get; private set; }

      public IBackUpDatabase<BackUpDatabase> From(string fileSystemName)
      {
         this.DatabaseName = fileSystemName;

         return this;
      }

      public IBackUpDatabase<BackUpDatabase> To(string path)
      {
         this.BackupLocation = path;
         
         return this;
      }

      public IBackUpDatabase<BackUpDatabase> At(string url)
      {
         this.ServerUrl = url;

         return this;
      }

      public IBackUpDatabase<BackUpDatabase> Incrementally(bool incremental = true)
      {
         this.Incremental = incremental;

         return this;
      }

      public IBackUpDatabase<BackUpDatabase> WithoutEncryptionKey(bool removeEncryptionKey = true)
      {
         this.RemoveEncryptionKey = removeEncryptionKey;

         return this;
      }

      public bool Run()
      {
         var backupStartedOn = DateTime.Now;

         logger.Info($@"Running assure out...
   from {this.ServerUrl}/{this.DatabaseName}
   to {this.BackupLocation}
   with settings:
      incrementally: {this.Incremental}
      remove encryption key: {this.RemoveEncryptionKey}
");

         using (var store = new DocumentStore()
         {
            Url = this.ServerUrl,
            DefaultDatabase = this.DatabaseName
         }.Initialize())
         {
            store.DatabaseCommands.GlobalAdmin.StartBackup(
               backupLocation: this.BackupLocation,
               databaseDocument: null, // Gets the settings for specified database stored in the system database.
               incremental: this.Incremental,
               databaseName: this.DatabaseName
            );

            updateBackupStatus(store);
         }

         if (this.RemoveEncryptionKey)
         {
            TryRemoveEncryptionKey();
         }

         var backupEndedOn = DateTime.Now;
         var runTime = backupEndedOn - backupStartedOn;

         logger.NewLine();
         logger.Info("Backup completed!");
         logger.Info($"Total backup time: {runTime}");

         return true;
      }

      public new BackUpDatabase LogWith(ILogger logger)
      {
         return (BackUpDatabase) base.LogWith(logger);
      }

      public new BackUpDatabase On(IFileSystem fileSystem)
      {
         return (BackUpDatabase) base.On(fileSystem);
      }

      /// <summary>
      /// 
      /// </summary>
      /// <remarks>Modified from RavenDB@3.0.30155/Raven.Backup/AbstractBackupOperation.cs
      /// since this version of the StartBackup API doesn't provide status.</remarks>
      /// <param name="store"></param>
      private void updateBackupStatus(IDocumentStore store)
      {
         BackupStatus status = null;
         var messagesSeenSoFar = new HashSet<BackupStatus.BackupMessage>();

         while (status == null)
         {
            Thread.Sleep(100); // Allow the server to process the request
            status = getBackupStatus(store);
         }

         logger.Info("Backup operation has started, status is logged at Raven/BackUp/Status");

         while (status.IsRunning)
         {
            // Write out the messages as we poll for them, don't wait until the end, this allows "live" updates
            foreach (var msg in status.Messages)
            {
               if (messagesSeenSoFar.Add(msg))
               {
                  logStatusMessage(msg);
               }
            }

            Thread.Sleep(1000);
            status = getBackupStatus(store);
         }

         // After we've know it's finished, write out any remaining messages
         foreach (var msg in status.Messages)
         {
            if (messagesSeenSoFar.Add(msg))
            {
               logStatusMessage(msg);
            }
         }
      }

      private void logStatusMessage(BackupStatus.BackupMessage msg)
      {
         switch (msg.Severity)
         {
            case BackupStatus.BackupMessageSeverity.Error:
               logger.Error(msg.Message);
               break;
            case BackupStatus.BackupMessageSeverity.Informational:
            default:
               logger.WriteLineTimeStamped(msg.Timestamp, msg.Message);
               break;
         }
      }

      public IBackUpDatabase<BackUpDatabase> TryRemoveEncryptionKey()
      {
         var resourceDocumentService = new ResourceDocumentService<DatabaseDocument>(this.fileSystem);
         var databaseDocumentPath = resourceDocumentService.FindLatestDocumentPath(this.BackupLocation);
         var databaseDocument = resourceDocumentService.Load(databaseDocumentPath);

         if (databaseDocument == null)
         {
            logger.Warning($@"
Database.Document not found at {databaseDocumentPath} 
when trying to remove encryption key...should this explode?");
            return this;
         }

         var databaseDocumentUpdater = tryRemoveEncryptionKey(databaseDocument);

         if (!databaseDocumentUpdater.Updated)
         {
            logger.Info($@"
Encryption key not found in 
   {databaseDocumentPath}
...nothing to remove.");
            return this;
         }

         saveDatabaseDocument(databaseDocumentUpdater.Document, databaseDocumentPath);
         logger.Info($@"
Removed encryption key from 
   {databaseDocumentPath}");

         return this;
      }

      private ResourceDocumentUpdate<DatabaseDocument> tryRemoveEncryptionKey(DatabaseDocument databaseDocument)
      {
         const string encryptionKeySettingKey = "Raven/Encryption/Key";
         if (!databaseDocument.SecuredSettings.ContainsKey(encryptionKeySettingKey))
         {
            return new ResourceDocumentUpdate<DatabaseDocument>
            {
               Document = databaseDocument,
               Updated = false
            };
         }

         // Remove encryption key
         databaseDocument.SecuredSettings[encryptionKeySettingKey] = null;


         return new ResourceDocumentUpdate<DatabaseDocument>
         {
            Document = databaseDocument,
            Updated = true
         };
      }



      private void saveDatabaseDocument(DatabaseDocument databaseDocument, string databaseDocumentPath)
      {
         var databaseDocumentText = JsonConvert.SerializeObject(databaseDocument);
         fileSystem.File.WriteAllText(databaseDocumentPath, databaseDocumentText);
      }

      private BackupStatus getBackupStatus(IDocumentStore store)
      {
         var backupStatusDocRequest = store.DatabaseCommands.Get(BackupStatus.RavenBackupStatusDocumentKey);

         return backupStatusDocRequest.DataAsJson.Deserialize<BackupStatus>(store.Conventions);
      }
   }
}