using Raven.Assure.Fluent;

namespace Raven.Assure.BackUp
{
   public interface IBackUpFileSystem<out T> : IBackUp<IBackUpFileSystem<T>>, IAssureFileSystem, IRunAssure, ISetupAssure<T>
   {
      new IBackUpFileSystem<T> From(string fileSystemName);
   }
}