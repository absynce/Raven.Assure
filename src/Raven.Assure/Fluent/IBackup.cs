namespace Raven.Assure.Fluent
{
   public interface IBackUp<out T> : IAssureBase, IRunAssure, ISetupAssure<T>
   {
      bool Incremental { get; }
      bool RemoveEncryptionKey { get; }
      IBackUp<T> From(string databaseName);
      IBackUp<T> To(string path);
      IBackUp<T> At(string url);
      IBackUp<T> Incrementally(bool incremental = true);
      IBackUp<T> WithoutEncryptionKey(bool removeEncryptionKey = true);
      IBackUp<T> TryRemoveEncryptionKey();
   }
}