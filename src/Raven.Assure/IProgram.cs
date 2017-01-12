using System.Collections.Generic;

namespace Raven.Assure
{
   public interface IProgram
   {
      void ParseCommands(IReadOnlyList<string> args);
   }
}