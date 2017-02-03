using Raven.Assure.Fluent;

namespace Raven.Assure.BackUp
{
   public interface IBackUpFileSystem<out T> : IBackUp<IBackUpFileSystem<T>>, IAssureFileSystem, IRunAssure, ISetupAssure<T>
   {
      bool Incremental { get; }
      bool RemoveEncryptionKey { get; }
      IBackUpFileSystem<T> From(string fileSystemName);
      IBackUpFileSystem<T> To(string path);
      IBackUpFileSystem<T> WithoutEncryptionKey(bool removeEncryptionKey = true);
      IBackUpFileSystem<T> TryRemoveEncryptionKey();
   }
}