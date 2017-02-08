using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Moq;
using Raven.Assure.BackUp;
using Raven.Assure.Fluent;
using Raven.Assure.Log;
using Raven.Assure.Restore;
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
            var mockDatabaseBackUpper = new Mock<IBackUpDatabase<BackUpDatabase>>();
            var mockDatabaseRestorer = new Mock<IRestoreDatabase<RestoreDatabase>>();
            var program = new Program(mockLogger.Object, mockDatabaseBackUpper.Object, mockDatabaseRestorer.Object, Mock.Of<BackUpFileSystem>());

            program.ParseCommands(new ReadOnlyCollection<string>(new List<string>()));

            mockLogger.Verify(logger => logger.Info(It.Is<string>(message => message.Contains("usage"))));
         }

         [Fact]
         public void ShouldPrintManualWhenHelpArgumentPassed()
         {
            var mockLogger = new Mock<ILogger>();
            var mockDatabaseBackUpper = new Mock<IBackUpDatabase<BackUpDatabase>>();
            var mockDatabaseRestorer = new Mock<IRestoreDatabase<RestoreDatabase>>();

            var program = new Program(mockLogger.Object, mockDatabaseBackUpper.Object, mockDatabaseRestorer.Object, Mock.Of<BackUpFileSystem>());

            program.ParseCommands(new ReadOnlyCollection<string>(new List<string>() {"help"}));

            mockLogger.Verify(logger => logger.Info(It.Is<string>(message => message.Contains("usage"))));
         }

         [Fact]
         public void ShouldCallBackupDatabaseOutWithPassedConfigParams()
         {
            var mockLogger = new Mock<ILogger>();
            var mockDatabaseBackUpper = new Mock<IBackUpDatabase<BackUpDatabase>>();
            var mockDatabaseRestorer = new Mock<IRestoreDatabase<RestoreDatabase>>();

            mockDatabaseBackUpper
               .Setup(backup => backup.From(It.IsAny<string>()))
               .Returns(() => mockDatabaseBackUpper.Object);

            mockDatabaseBackUpper
               .Setup(backup => backup.At(It.IsAny<string>()))
               .Returns(() => mockDatabaseBackUpper.Object);

            mockDatabaseBackUpper
               .Setup(backup => backup.To(It.IsAny<string>()))
               .Returns(() => mockDatabaseBackUpper.Object);

            mockDatabaseBackUpper
               .Setup(backup => backup.Incrementally(It.IsAny<bool>()))
               .Returns(() => mockDatabaseBackUpper.Object);

            mockDatabaseBackUpper
               .Setup(backup => backup.WithoutEncryptionKey(It.IsAny<bool>()))
               .Returns(() => mockDatabaseBackUpper.Object);

            var program = new Program(mockLogger.Object, mockDatabaseBackUpper.Object, mockDatabaseRestorer.Object, Mock.Of<BackUpFileSystem>());

            program.ParseCommands(new ReadOnlyCollection<string>(new List<string>() {"out", "test.qa"}));

            mockDatabaseBackUpper.Verify(backUpper => backUpper.From("Test"));
            mockDatabaseBackUpper.Verify(backUpper => backUpper.At("http://localhost:8080/"));
            mockDatabaseBackUpper.Verify(backUpper => backUpper.To("C:\\temp\\test.raven.incremental.bak"));
            mockDatabaseBackUpper.Verify(backUpper => backUpper.Incrementally(true));
            mockDatabaseBackUpper.Verify(backUpper => backUpper.WithoutEncryptionKey(true));
            mockDatabaseBackUpper.Verify(backUpper => backUpper.Run());
         }

         [Fact]
         public void ShouldCallBackupFileSystemOutWithPassedConfigParams()
         {
            var mockLogger = new Mock<ILogger>();
            var mockDatabaseBackUpper = new Mock<IBackUpDatabase<BackUpDatabase>>();
            var mockFileSystemBackUpper = new Mock<IBackUpFileSystem<BackUpFileSystem>>();
            var mockDatabaseRestorer = new Mock<IRestoreDatabase<RestoreDatabase>>();

            mockFileSystemBackUpper
               .Setup(backup => backup.From(It.IsAny<string>()))
               .Returns(() => mockFileSystemBackUpper.Object);

            mockFileSystemBackUpper
               .Setup(backup => backup.At(It.IsAny<string>()))
               .Returns(() => mockFileSystemBackUpper.Object);

            mockFileSystemBackUpper
               .Setup(backup => backup.To(It.IsAny<string>()))
               .Returns(() => mockFileSystemBackUpper.Object);

            mockFileSystemBackUpper
               .Setup(backup => backup.Incrementally(It.IsAny<bool>()))
               .Returns(() => mockFileSystemBackUpper.Object);

            mockFileSystemBackUpper
               .Setup(backup => backup.WithoutEncryptionKey(It.IsAny<bool>()))
               .Returns(() => mockFileSystemBackUpper.Object);

            var program = new Program(mockLogger.Object, Mock.Of<BackUpDatabase>(), mockDatabaseRestorer.Object, mockFileSystemBackUpper.Object);

            program.ParseCommands(new ReadOnlyCollection<string>(new List<string>() {"out", "test.qa.files"}));

            mockFileSystemBackUpper.Verify(backUpper => backUpper.From("Test.Files"));
            mockFileSystemBackUpper.Verify(backUpper => backUpper.At("http://localhost:8080/"));
            mockFileSystemBackUpper.Verify(backUpper => backUpper.To("C:\\temp\\test.files.raven.incremental.bak"));
            mockFileSystemBackUpper.Verify(backUpper => backUpper.Incrementally(true));
            mockFileSystemBackUpper.Verify(backUpper => backUpper.WithoutEncryptionKey(true));
            mockFileSystemBackUpper.Verify(backUpper => backUpper.Run());
         }

         [Fact]
         public void ShouldCallBackupFileSystemInWithPassedConfigParams()
         {
            var mockLogger = new Mock<ILogger>();
            var mockDatabaseBackUpper = new Mock<IBackUpDatabase<BackUpDatabase>>();
            var mockDatabaseRestorer = new Mock<IRestoreDatabase<RestoreDatabase>>();

            mockDatabaseRestorer
               .Setup(restore => restore.From(It.IsAny<string>()))
               .Returns(() => mockDatabaseRestorer.Object);

            mockDatabaseRestorer
               .Setup(restore => restore.At(It.IsAny<string>()))
               .Returns(() => mockDatabaseRestorer.Object);

            mockDatabaseRestorer
               .Setup(restore => restore.To(It.IsAny<string>()))
               .Returns(() => mockDatabaseRestorer.Object);

            var program = new Program(mockLogger.Object, mockDatabaseBackUpper.Object, mockDatabaseRestorer.Object, Mock.Of<BackUpFileSystem>());

            program.ParseCommands(new ReadOnlyCollection<string>(new List<string>() { "in", "test.qa" }));

            mockDatabaseRestorer.Verify(restorer => restorer.From(@"C:\temp\test.raven.incremental.bak"));
            mockDatabaseRestorer.Verify(restorer => restorer.To("TestRestored"));
            mockDatabaseRestorer.Verify(restorer => restorer.At("http://localhost:8080/"));
            mockDatabaseRestorer.Verify(restorer => restorer.Run());
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
               var mockDatabaseBackUpper = new Mock<IBackUpDatabase<BackUpDatabase>>();
               var mockDatabaseRestorer = new Mock<IRestoreDatabase<RestoreDatabase>>();
               var program = new Program(mockLogger.Object, mockDatabaseBackUpper.Object, mockDatabaseRestorer.Object, Mock.Of<BackUpFileSystem>());

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
               var mockDatabaseBackUpper = new Mock<IBackUpDatabase<BackUpDatabase>>();
               var mockDatabaseRestorer = new Mock<IRestoreDatabase<RestoreDatabase>>();
               var program = new Program(mockLogger.Object, mockDatabaseBackUpper.Object, mockDatabaseRestorer.Object, Mock.Of<BackUpFileSystem>());

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
