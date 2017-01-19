using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Moq;
using Raven.Assure.Log;
using Xunit;

namespace Raven.Assure.Test
{
   // see example explanation on xUnit.net website:
   // https://xunit.github.io/docs/getting-started-dotnet-core.html
   public class ProgramTests
   {
      public class ParseCommand
      {
         [Fact]
         public void ShouldPrintManualWhenNoArgumentsPassed()
         {
            var mockLogger = new Mock<ILogger>();

            var program = new Program(mockLogger.Object);

            program.ParseCommands(new ReadOnlyCollection<string>(new List<string>()));

            mockLogger.Verify(logger => logger.Info(It.Is<string>(message => message.Contains("usage"))));
         }
         [Fact]
         public void ShouldPrintManualWhenHelpArgumentPassed()
         {
            var mockLogger = new Mock<ILogger>();

            var program = new Program(mockLogger.Object);

            program.ParseCommands(new ReadOnlyCollection<string>(new List<string>() { "help" }));

            mockLogger.Verify(logger => logger.Info(It.Is<string>(message => message.Contains("usage"))));
         }
      }

   }
}
