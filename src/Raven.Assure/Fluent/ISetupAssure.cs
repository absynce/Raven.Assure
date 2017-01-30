using System.IO.Abstractions;
using Raven.Assure.Log;

namespace Raven.Assure.Fluent
{
   public interface ISetupAssure<out T>
   {
      T LogWith(ILogger logger);
      T On(IFileSystem fileSystem);
   }
}