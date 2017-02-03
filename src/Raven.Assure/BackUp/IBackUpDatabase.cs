using Raven.Assure.Fluent;

namespace Raven.Assure.BackUp
{
   public interface IBackUpDatabase<out T> : IBackUp<IBackUpDatabase<T>>, IAssureDataBase, IRunAssure, ISetupAssure<T>
   {
      bool Incremental { get; }
      bool RemoveEncryptionKey { get; }
      IBackUpDatabase<T> From(string databaseName);
      IBackUpDatabase<T> To(string path);
      IBackUpDatabase<T> WithoutEncryptionKey(bool removeEncryptionKey = true);
      IBackUpDatabase<T> TryRemoveEncryptionKey();
   }
}