namespace Raven.Assure.BackUp
{
   public interface IBackUp<out TInterface>
   {
      bool Incremental { get; }
      bool RemoveEncryptionKey { get; }
      TInterface From(string resourceName);
      TInterface To(string path);
      TInterface At(string url);
      TInterface Incrementally(bool incremental = true);
      TInterface WithoutEncryptionKey(bool removeEncryptionKey = true);
      TInterface TryRemoveEncryptionKey();
   }
}