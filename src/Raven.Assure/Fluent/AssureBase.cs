using Raven.Assure.Log;

namespace Raven.Assure.Fluent
{
   public class AssureBase : ISetupAssure<AssureBase>
   {
      protected ILogger logger;

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
