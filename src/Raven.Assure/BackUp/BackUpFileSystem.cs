using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Raven.Abstractions.Data;
using Raven.Abstractions.Extensions;
using Raven.Abstractions.FileSystem;
using Raven.Assure.Fluent;
using Raven.Assure.Log;
using Raven.Assure.ResourceDocument;
using Raven.Client;
using Raven.Client.Connection;
using Raven.Client.Document;
using Raven.Client.FileSystem;
using Raven.Imports.Newtonsoft.Json;
using Raven.Json.Linq;

namespace Raven.Assure.BackUp
{
   public class BackUpFileSystem : AssureFileSystem, IBackUpFileSystem<BackUpFileSystem>
   {
      public bool Incremental { get; private set; }
      public bool RemoveEncryptionKey { get; private set; }

      public IBackUpFileSystem<BackUpFileSystem> From(string fileSystemName)
      {
         this.FileSystemName = fileSystemName;

         return this;
      }

      public IBackUpFileSystem<BackUpFileSystem> To(string path)
      {
         this.BackupLocation = path;
         
         return this;
      }

      public IBackUpFileSystem<BackUpFileSystem> At(string url)
      {
         this.ServerUrl = url;

         return this;
      }

      public IBackUpFileSystem<BackUpFileSystem> Incrementally(bool incremental = true)
      {
         this.Incremental = incremental;

         return this;
      }

      public IBackUpFileSystem<BackUpFileSystem> WithoutEncryptionKey(bool removeEncryptionKey = true)
      {
         this.RemoveEncryptionKey = removeEncryptionKey;

         return this;
      }

      public bool Run()
      {
         var backupStartedOn = DateTime.Now;

         logger.Info($@"Running assure out...
   from {this.ServerUrl}/{this.FileSystemName}
   to {this.BackupLocation}
   with settings:
      incrementally: {this.Incremental}
      remove encryption key: {this.RemoveEncryptionKey}
");

         using (var store = new FilesStore()
         {
            Url = this.ServerUrl,
            DefaultFileSystem = this.FileSystemName
         }.Initialize())
         {
            store.AsyncFilesCommands.Admin.StartBackup(
               backupLocation: this.BackupLocation,
               fileSystemDocument: null, // Gets the settings for specified database stored in the system database.
               incremental: this.Incremental,
               fileSystemName: this.FileSystemName
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

      public new BackUpFileSystem LogWith(ILogger logger)
      {
         return (BackUpFileSystem) base.LogWith(logger);
      }

      public new BackUpFileSystem On(IFileSystem fileSystem)
      {
         return (BackUpFileSystem) base.On(fileSystem);
      }

      /// <summary>
      /// 
      /// </summary>
      /// <remarks>Modified from RavenDB@3.0.30155/Raven.Backup/AbstractBackupOperation.cs
      /// since this version of the StartBackup API doesn't provide status.</remarks>
      /// <param name="store"></param>
      private void updateBackupStatus(IFilesStore store)
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

      public IBackUpFileSystem<BackUpFileSystem> TryRemoveEncryptionKey()
      {
         throw new NotImplementedException("Not sure how to do this for file system yet.");
         var databaseDocumentPath = findLatestDatabaseDocument(this.BackupLocation);
         var databaseDocument = loadDatabaseDocument(databaseDocumentPath);

         if (databaseDocument == null)
         {
            logger.Warning($@"
Database.Document not found at {databaseDocumentPath} 
when trying to remove encryption key...should this explode?");
            return this;
         }

         //var databaseDocumentUpdater = tryRemoveEncryptionKey(databaseDocument);

//         if (!databaseDocumentUpdater.Updated)
//         {
//            logger.Info($@"
//Encryption key not found in 
//   {databaseDocumentPath}
//...nothing to remove.");
//            return this;
//         }

//         saveDatabaseDocument(databaseDocumentUpdater.Document, databaseDocumentPath);
//         logger.Info($@"
//Removed encryption key from 
//   {databaseDocumentPath}");

         return this;
      }

      private static ResourceDocumentUpdate<FileSystemDocument> tryRemoveEncryptionKey(FileSystemDocument fileSystemDocument)
      {
         const string encryptionKeySettingKey = "Raven/Encryption/Key";
         if (!fileSystemDocument.SecuredSettings.ContainsKey(encryptionKeySettingKey))
         {
            return new ResourceDocumentUpdate<FileSystemDocument>
            {
               Document = fileSystemDocument,
               Updated = false
            };
         }

         // Remove encryption key
         fileSystemDocument.SecuredSettings[encryptionKeySettingKey] = null;


         return new ResourceDocumentUpdate<FileSystemDocument>
         {
            Document = fileSystemDocument,
            Updated = true
         };
      }

      private DatabaseDocument loadDatabaseDocument(string databaseDocumentPath)
      {
         if (!fileSystem.File.Exists(databaseDocumentPath))
         {
            return null;
         }

         var databaseDocumentText = fileSystem.File.ReadAllText(databaseDocumentPath);

         return RavenJObject.Parse(databaseDocumentText).JsonDeserialization<Abstractions.Data.DatabaseDocument>();
      }

      /// <summary>
      /// </summary>
      /// <remarks>Adapted from RavenDB@3.0.30155 - Raven.Database.Actions.MaintenanceActions.FindDatabaseDocument.</remarks>
      /// <param name="backupLocation"></param>
      /// <returns></returns>
      private string findLatestDatabaseDocument(string backupLocation)
      {
         var backupPath = fileSystem.Directory.GetDirectories(backupLocation, "Inc*")
                           .OrderByDescending(dir => dir)
                           .Select(dir => Path.Combine(dir, Constants.DatabaseDocumentFilename))
                           .FirstOrDefault();

         return backupPath ?? Path.Combine(backupLocation, Constants.DatabaseDocumentFilename);
      }

      private void saveDatabaseDocument(DatabaseDocument databaseDocument, string databaseDocumentPath)
      {
         var databaseDocumentText = JsonConvert.SerializeObject(databaseDocument);
         fileSystem.File.WriteAllText(databaseDocumentPath, databaseDocumentText);
      }

      private static BackupStatus getBackupStatus(IFilesStore store)
      {
         return store.AsyncFilesCommands.Configuration.GetKeyAsync<BackupStatus>(BackupStatus.RavenBackupStatusDocumentKey).Result;
      }
   }
}