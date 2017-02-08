using Raven.Assure.Fluent;

namespace Raven.Assure.Restore
{
   public interface IRestoreDatabase<out T> : IRestore<IRestoreDatabase<T>>, IAssureDataBase, IRunAssure, ISetupAssure<T>
   {
      new IRestoreDatabase<T> To(string databaseName);
   }
}