using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Raven.Abstractions.Data;
using Raven.Assure.Log;
using Raven.Client;
using Raven.Client.Connection;
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
         var backupStartedOn = DateTime.Now;

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

            updateBackupStatus(store);
         }

         var backupEndedOn = DateTime.Now;
         var runTime = backupEndedOn - backupStartedOn;

         logger.NewLine();
         logger.Info("Backup completed!");
         logger.Info($"Total backup time: {runTime}");

         return true;
      }

      public new BackUp LogWith(ILogger logger)
      {
         return (BackUp) base.LogWith(logger);
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

         logger.Info("Backup operation has started, status is logged at Raven/Backup/Status");

         while (status.IsRunning)
         {
            // Write out the messages as we poll for them, don't wait until the end, this allows "live" updates
            foreach (var msg in status.Messages)
            {
               if (messagesSeenSoFar.Add(msg))
               {
                  LogStatusMessage(msg);
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
               LogStatusMessage(msg);
            }
         }
      }

      private void LogStatusMessage(BackupStatus.BackupMessage msg)
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

      private BackupStatus getBackupStatus(IDocumentStore store)
      {
         var backupStatusDocRequest = store.DatabaseCommands.Get(BackupStatus.RavenBackupStatusDocumentKey);

         return backupStatusDocRequest.DataAsJson.Deserialize<BackupStatus>(store.Conventions);
      }
   }
}