using Raven.Assure.Fluent;

namespace Raven.Assure.Restore
{
   public interface IRestoreDatabase<out T> : IAssureDataBase, IRunAssure, ISetupAssure<T>
   {
      string DatabaseLocation { get; }
      IRestoreDatabase<T> From(string path);
      IRestoreDatabase<T> To(string databaseName);
      IRestoreDatabase<T> At(string url);
      IRestoreDatabase<T> In(string databaseLocation);
   }
}