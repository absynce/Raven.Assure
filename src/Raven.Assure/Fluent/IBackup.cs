namespace Raven.Assure.Fluent
{
   public interface IBackUp
   {
      string BackupLocation { get; }
      string DatabaseName { get; }
      bool Incremental { get; }
      string ServerUrl { get; }
      IBackUp From(string databaseName);
      IBackUp To(string path);
      IBackUp At(string url);
      IBackUp Incrementally(bool incremental = true);
      IBackUp RemoveEncryptionKey(bool removeEncryptionKey = true);
   }
}