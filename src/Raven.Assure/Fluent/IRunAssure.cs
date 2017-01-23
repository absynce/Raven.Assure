using System.Threading.Tasks;

namespace Raven.Assure.Fluent
{
   public interface IRunAssure
   {
      Task<bool> RunAsync();
      bool Run();
   }
}