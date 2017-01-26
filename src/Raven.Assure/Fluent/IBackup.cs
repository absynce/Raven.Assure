namespace Raven.Assure.Fluent
{
   public interface IBackUp<out T> : IAssureBase, IRunAssure, ISetupAssure<T>
   {
      bool Incremental { get; }
      IBackUp<T> From(string databaseName);
      IBackUp<T> To(string path);
      IBackUp<T> At(string url);
      IBackUp<T> Incrementally(bool incremental = true);
      IBackUp<T> RemoveEncryptionKey(bool removeEncryptionKey = true);
   }
}