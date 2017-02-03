using Raven.Assure.Fluent;

namespace Raven.Assure.BackUp
{
   public interface IBackUpDatabase<out T> : IAssureDataBase, IRunAssure, ISetupAssure<T>
   {
      bool Incremental { get; }
      bool RemoveEncryptionKey { get; }
      IBackUpDatabase<T> From(string databaseName);
      IBackUpDatabase<T> To(string path);
      IBackUpDatabase<T> At(string url);
      IBackUpDatabase<T> Incrementally(bool incremental = true);
      IBackUpDatabase<T> WithoutEncryptionKey(bool removeEncryptionKey = true);
      IBackUpDatabase<T> TryRemoveEncryptionKey();
   }
}