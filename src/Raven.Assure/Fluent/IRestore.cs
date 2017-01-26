namespace Raven.Assure.Fluent
{
   public interface IRestore<out T> : IAssureBase, IRunAssure, ISetupAssure<T>
   {
      string DatabaseLocation { get; }
      IRestore<T> From(string path);
      IRestore<T> To(string databaseName);
      IRestore<T> At(string url);
      IRestore<T> In(string databaseLocation);
   }
}