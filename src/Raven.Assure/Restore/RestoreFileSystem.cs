using System;
using System.IO.Abstractions;
using Raven.Abstractions.Data;
using Raven.Assure.Fluent;
using Raven.Assure.Log;
using Raven.Client.FileSystem;

namespace Raven.Assure.Restore
{
   public class RestoreFileSystem : AssureFileSystem, IRestoreFileSystem<RestoreFileSystem>, IRunAssure, ISetupAssure<RestoreFileSystem>
   {
      public string RestoreLocation { get; protected set; }

      public IRestoreFileSystem<RestoreFileSystem> To(string fileSystemName)
      {
         this.FileSystemName = fileSystemName;

         return this;
      }

      public IRestoreFileSystem<RestoreFileSystem> From(string path)
      {
         this.BackupLocation = path;

         return this;
      }

      public IRestoreFileSystem<RestoreFileSystem> At(string url)
      {
         this.ServerUrl = url;

         return this;
      }

      public IRestoreFileSystem<RestoreFileSystem> In(string restoreLocation)
      {
         this.RestoreLocation = restoreLocation;

         return this;
      }

      public bool Run()
      {
         var restoreStartedOn = DateTime.Now;

         logger.Info($@"Running assure in...
   from {this.BackupLocation}
   to {this.ServerUrl}/{this.FileSystemName}
   in {this.RestoreLocation}
");

         using (var store = new FilesStore()
         {
            Url = this.ServerUrl,
            DefaultFileSystem = this.FileSystemName
         }.Initialize())
         {
            var restoreOperation = store.AsyncFilesCommands.Admin.StartRestore(new FilesystemRestoreRequest() {
               BackupLocation = this.BackupLocation,
               FilesystemName = this.FileSystemName,
               FilesystemLocation = this.RestoreLocation
            });

            logger.Info("RavenDB provides no status updates on the restore operation...just wait until it completes :-s");

            restoreOperation.Wait();
         }

         var restoreEndedOn = DateTime.Now;

         var timeToRestore = restoreEndedOn - restoreStartedOn;

         logger.NewLine();
         logger.Info($"Restore file system from {this.BackupLocation} to {this.FileSystemName} completed!");
         logger.Info($"Total restore time: {timeToRestore}");

         return true;
      }

      public new RestoreFileSystem LogWith(ILogger logger)
      {
         return (RestoreFileSystem) base.LogWith(logger);
      }

      public new RestoreFileSystem On(IFileSystem fileSystem)
      {
         return (RestoreFileSystem) base.On(fileSystem);
      }
   }
}