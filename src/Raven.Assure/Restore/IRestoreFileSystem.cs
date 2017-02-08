using Raven.Assure.Fluent;

namespace Raven.Assure.Restore
{
   public interface IRestoreFileSystem<out T> : IRestore<IRestoreFileSystem<T>>, IAssureFileSystem, IRunAssure, ISetupAssure<T>
   {
      new IRestoreFileSystem<T> To(string fileSystemName);
   }
}