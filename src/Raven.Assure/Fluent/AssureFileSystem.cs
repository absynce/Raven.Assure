using System.IO.Abstractions;
using Raven.Assure.Log;

namespace Raven.Assure.Fluent
{
   public class AssureFileSystem : IAssureFileSystem, ISetupAssure<AssureFileSystem>
   {
      protected IFileSystem fileSystem;
      protected ILogger logger;

      public string BackupLocation { get; protected set; }
      public string FileSystemName { get; protected set; }
      public string ServerUrl { get; protected set; }

      public AssureFileSystem()
      {
         this.fileSystem = new FileSystem();
         this.logger = new ConsoleLogger();
      }

      public AssureFileSystem LogWith(ILogger logger)
      {
         this.logger = logger;

         return this;
      }

      public AssureFileSystem On(IFileSystem fileSystem)
      {
         this.fileSystem = fileSystem;

         return this;
      }
   }
}
