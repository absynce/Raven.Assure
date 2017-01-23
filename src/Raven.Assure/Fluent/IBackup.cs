namespace Raven.Assure.Fluent
{
   public interface IBackUp
   {
      string DatabaseName { get; }
      IBackUp From(string databaseName);
      IBackUp To(string path);
      IBackUp At(string url);
      IBackUp Incremental(bool incremental = true);
      IBackUp RemoveEncryptionKey(bool removeEncryptionKey = true);
   }
}