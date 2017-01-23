namespace Raven.Assure.Fluent
{
   public class BackUp : IBackUp
   {
      public string DatabaseName { get; private set; }

      public IBackUp From(string databaseName)
      {
         this.DatabaseName = databaseName;
         return this;
      }

      public IBackUp To(string path)
      {
         throw new System.NotImplementedException();
      }

      public IBackUp At(string url)
      {
         throw new System.NotImplementedException();
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