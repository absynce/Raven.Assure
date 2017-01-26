using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Moq;
using Raven.Assure.Fluent;
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
            var mockBackUpper = new Mock<IBackUp<BackUp>>();
            var mockRestorer = new Mock<IRestore<Restore>>();
            var program = new Program(mockLogger.Object, mockBackUpper.Object, mockRestorer.Object);

            program.ParseCommands(new ReadOnlyCollection<string>(new List<string>()));

            mockLogger.Verify(logger => logger.Info(It.Is<string>(message => message.Contains("usage"))));
         }

         [Fact]
         public void ShouldPrintManualWhenHelpArgumentPassed()
         {
            var mockLogger = new Mock<ILogger>();
            var mockBackUpper = new Mock<IBackUp<BackUp>>();
            var mockRestorer = new Mock<IRestore<Restore>>();

            var program = new Program(mockLogger.Object, mockBackUpper.Object, mockRestorer.Object);

            program.ParseCommands(new ReadOnlyCollection<string>(new List<string>() {"help"}));

            mockLogger.Verify(logger => logger.Info(It.Is<string>(message => message.Contains("usage"))));
         }

         [Fact]
         public void ShouldCallBackupOutWithPassedConfigParams()
         {
            var mockLogger = new Mock<ILogger>();
            var mockBackUpper = new Mock<IBackUp<BackUp>>();
            var mockRestorer = new Mock<IRestore<Restore>>();

            mockBackUpper
               .Setup(backup => backup.From(It.IsAny<string>()))
               .Returns(() => mockBackUpper.Object);

            mockBackUpper
               .Setup(backup => backup.At(It.IsAny<string>()))
               .Returns(() => mockBackUpper.Object);

            mockBackUpper
               .Setup(backup => backup.To(It.IsAny<string>()))
               .Returns(() => mockBackUpper.Object);

            var program = new Program(mockLogger.Object, mockBackUpper.Object, mockRestorer.Object);

            program.ParseCommands(new ReadOnlyCollection<string>(new List<string>() {"out", "test.qa"}));

            mockBackUpper.Verify(backUpper => backUpper.From("Test"));
            mockBackUpper.Verify(backUpper => backUpper.At("http://localhost:8080/"));
            mockBackUpper.Verify(backUpper => backUpper.To("test.raven.incremental.bak"));
            mockBackUpper.Verify(backUpper => backUpper.Incrementally(true));
            mockBackUpper.Verify(backUpper => backUpper.Run());
         }

         [Fact]
         public void ShouldCallBackupInWithPassedConfigParams()
         {
            var mockLogger = new Mock<ILogger>();
            var mockBackUpper = new Mock<IBackUp<BackUp>>();
            var mockRestorer = new Mock<IRestore<Restore>>();

            mockRestorer
               .Setup(restore => restore.From(It.IsAny<string>()))
               .Returns(() => mockRestorer.Object);

            mockRestorer
               .Setup(restore => restore.At(It.IsAny<string>()))
               .Returns(() => mockRestorer.Object);

            mockRestorer
               .Setup(restore => restore.To(It.IsAny<string>()))
               .Returns(() => mockRestorer.Object);

            var program = new Program(mockLogger.Object, mockBackUpper.Object, mockRestorer.Object);

            program.ParseCommands(new ReadOnlyCollection<string>(new List<string>() { "in", "test.qa" }));

            mockRestorer.Verify(restorer => restorer.From(@"C:\temp\test.raven.incremental.bak"));
            mockRestorer.Verify(restorer => restorer.To("TestRestored"));
            mockRestorer.Verify(restorer => restorer.At("http://localhost:8080/"));
            mockRestorer.Verify(restorer => restorer.Run());
         }
      }

      public class GetConfigFromArgs
      {
         public class WhenPassedValidEnvironment
         {
            private readonly string _validEnvironment;

            public WhenPassedValidEnvironment()
            {
               _validEnvironment = "test.qa";
            }

            [Fact]
            public void ShouldReturnConfigValue()
            {
               var expectedConfig = JsonConfig.Config.ApplyJsonFromPath($"config/{_validEnvironment}.json",
                  JsonConfig.Config.Default);
               var args = new List<string>() {"out", _validEnvironment};

               var mockLogger = new Mock<ILogger>();
               var mockBackUpper = new Mock<IBackUp<BackUp>>();
               var mockRestorer = new Mock<IRestore<Restore>>();
               var program = new Program(mockLogger.Object, mockBackUpper.Object, mockRestorer.Object);

               var actualConfig = program.GetConfigFromArgs(args);

               Assert.Equal(expectedConfig.Out.Database.ToString(), actualConfig.Out.Database.ToString());
            }
         }

         public class WhenPassedInvalidEnvironment
         {
            private readonly string _invalidEnvironment;

            public WhenPassedInvalidEnvironment()
            {
               _invalidEnvironment = "bad-config.non-env";
            }

            [Fact]
            public void ShouldThrowInvalidArgumentException()
            {
               var expectedExceptionMessage = $"Environment '{_invalidEnvironment}' not recognized.";

               var args = new List<string>() {"out", _invalidEnvironment};

               var mockLogger = new Mock<ILogger>();
               var mockBackUpper = new Mock<IBackUp<BackUp>>();
               var mockRestorer = new Mock<IRestore<Restore>>();
               var program = new Program(mockLogger.Object, mockBackUpper.Object, mockRestorer.Object);

               try
               {
                  program.GetConfigFromArgs(args);
                  Assert.True(false, "ArgumentException should have been thrown.");
               }
               catch (ArgumentException exception)
               {
                  Assert.Equal(expectedExceptionMessage, exception.Message);
               }
            }
         }
      }

   }
}
