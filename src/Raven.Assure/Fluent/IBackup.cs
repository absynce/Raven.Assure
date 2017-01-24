namespace Raven.Assure.Fluent
{
   public interface IBackUp<out T> : IRunAssure, ISetupAssure<T>
   {
      string BackupLocation { get; }
      string DatabaseName { get; }
      bool Incremental { get; }
      string ServerUrl { get; }
      IBackUp<T> From(string databaseName);
      IBackUp<T> To(string path);
      IBackUp<T> At(string url);
      IBackUp<T> Incrementally(bool incremental = true);
      IBackUp<T> RemoveEncryptionKey(bool removeEncryptionKey = true);
   }
}