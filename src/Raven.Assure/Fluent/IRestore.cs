namespace Raven.Assure.Fluent
{
   public interface IRestore<out T> : IRunAssure, ISetupAssure<T>
   {
   }
}