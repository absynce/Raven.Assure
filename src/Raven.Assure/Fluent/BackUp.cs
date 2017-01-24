namespace Raven.Assure.Fluent
{
   public class BackUp : IBackUp
   {
      public string BackupLocation { get; private set; }
      public string DatabaseName { get; private set; }
      public string ServerUrl { get; private set; }

      public IBackUp From(string databaseName)
      {
         this.DatabaseName = databaseName;
         return this;
      }

      public IBackUp To(string path)
      {
         this.BackupLocation = path;
         return this;
      }

      public IBackUp At(string url)
      {
         this.ServerUrl = url;
         return this;
      }

      public IBackUp Incremental(bool incremental = false)
      {
         throw new System.NotImplementedException();
      }

      public IBackUp RemoveEncryptionKey(bool removeEncryptionKey = true)
      {
         throw new System.NotImplementedException();
      }
   }
}