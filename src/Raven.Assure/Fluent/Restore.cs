using System.Resources;
using System.Threading.Tasks;
using Raven.Assure.Log;

namespace Raven.Assure.Fluent
{
   public class Restore : IRestore<Restore>
   {
      public Task<bool> RunAsync()
      {
         throw new System.NotImplementedException();
      }

      public bool Run()
      {
         throw new System.NotImplementedException();
      }

      public Restore LogWith(ILogger logger)
      {
         throw new System.NotImplementedException();
      }
   }
}