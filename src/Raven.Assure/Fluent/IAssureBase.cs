namespace Raven.Assure.Fluent
{
   public interface IAssureBase
   {
      string BackupLocation { get; }
      string DatabaseName { get; }
      string ServerUrl { get; }
   }
}