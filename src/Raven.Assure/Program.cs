using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Raven.Assure.Log;

namespace Raven.Assure
{
   public class Program : IProgram
   {
      private ILogger logger;

      public static void Main(string[] args)
      {
      }

      public void ParseCommands(IReadOnlyList<string> args)
      {
      }

      public Program(ILogger logger)
      {
         this.logger = logger;
      }

   }
}
