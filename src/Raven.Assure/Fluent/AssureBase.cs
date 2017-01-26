using Raven.Assure.Log;

namespace Raven.Assure.Fluent
{
   public class AssureBase : IAssureBase, ISetupAssure<AssureBase>
   {
      protected ILogger logger;

      public string BackupLocation { get; protected set; }
      public string DatabaseName { get; protected set; }
      public string ServerUrl { get; protected set; }

      public AssureBase()
      {
         this.logger = new ConsoleLogger();
      }

      public AssureBase LogWith(ILogger logger)
      {
         this.logger = logger;

         return this;
      }
   }
}
