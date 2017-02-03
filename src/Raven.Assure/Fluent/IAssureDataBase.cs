namespace Raven.Assure.Fluent
{
   public interface IAssureDataBase : IAssureBase
   {
      string DatabaseName { get; }
   }
}