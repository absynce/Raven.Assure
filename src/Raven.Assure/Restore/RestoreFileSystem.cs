using System.IO.Abstractions;
using Raven.Assure.Fluent;
using Raven.Assure.Log;

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
         throw new System.NotImplementedException();
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