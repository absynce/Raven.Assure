using System.Threading.Tasks;
using Raven.Assure.Log;

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
         throw new System.NotImplementedException();
      }

      public new BackUp LogWith(ILogger logger)
      {
         return (BackUp) base.LogWith(logger);
      }
   }
}