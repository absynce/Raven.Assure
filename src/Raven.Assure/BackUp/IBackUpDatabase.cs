using Raven.Assure.Fluent;

namespace Raven.Assure.BackUp
{
   public interface IBackUpDatabase<out T> : IBackUp<IBackUpDatabase<T>>, IAssureDataBase, IRunAssure, ISetupAssure<T>
   {
      new IBackUpDatabase<T> From(string databaseName);
   }
}