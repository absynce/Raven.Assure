namespace Raven.Assure.Restore
{
   public interface IRestore<out TInterface>
   {
      string RestoreLocation { get; }
      TInterface To(string fileSystemName);
      TInterface From(string path);
      TInterface At(string url);
      TInterface In(string restoreLocation);
   }
}