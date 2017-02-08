using System;
using System.Collections.Generic;
using System.IO;
using JsonConfig;
using Raven.Assure.BackUp;
using Raven.Assure.Fluent;
using Raven.Assure.Log;
using Raven.Assure.Restore;
using Raven.Client.Linq;

namespace Raven.Assure
{
   public class Program : IProgram
   {
      private readonly ILogger logger;
      private readonly IBackUpDatabase<BackUpDatabase> databaseBackUpper;
      private readonly IRestoreDatabase<RestoreDatabase> databaseRestorer;
      private readonly IBackUpFileSystem<BackUpFileSystem> fileSystemBackUpper;

      public static void Main(string[] args)
      {
         var logger = new ConsoleLogger();
         var databaseBackUpper = new BackUpDatabase()
            .LogWith(logger);
         var databaseRestorer = new RestoreDatabase()
            .LogWith(logger);
         var fileSystemBackUpper = new BackUpFileSystem()
            .LogWith(logger);
         var program = new Program(logger, databaseBackUpper, databaseRestorer, fileSystemBackUpper);
         program.ParseCommands(args);
      }

      public Program(
         ILogger logger, 
         IBackUpDatabase<BackUpDatabase> databaseBackUpper,
         IRestoreDatabase<RestoreDatabase> databaseRestorer,
         IBackUpFileSystem<BackUpFileSystem> fileSystemBackUpper
        ) 
      {
         this.logger = logger;
         this.databaseBackUpper = databaseBackUpper;
         this.databaseRestorer = databaseRestorer;
         this.fileSystemBackUpper = fileSystemBackUpper;
      }

      public void ParseCommands(IReadOnlyList<string> args)
      {
         if (args.Count == 0 || args[0] == "help")
         {
            PrintManual();
            return;
         }

         var command = args[0];

         switch (command)
         {
            case "in":
               var inEnvironment = GetConfigFromArgs(args);
               RunRestore(inEnvironment);
               break;
            case "out":
               var outEnvironment = GetConfigFromArgs(args);
               RunBackup(outEnvironment);
               break;
            default:
               logger.Info($"Command '{command}' not recognized.");
               PrintManual();
               break;
         }
      }

      private void RunBackup(dynamic outEnvironment)
      {
         if (outEnvironment.Out == Config.Default.Out)
         {
            logger.Warning("using the default environment Out config (probably not what you want).");
         }

         if (outEnvironment.IsFileSystem)
         {
            runFileSystemBackup(outEnvironment);
         }
         else
         {
            runDatabaseBackup(outEnvironment);
         }
      }

      private void runDatabaseBackup(dynamic outEnvironment)
      {
         databaseBackUpper
            .From(outEnvironment.Out.From.Server.Database)
            .At(outEnvironment.Out.From.Server.Url)
            .To(outEnvironment.Out.To.FilePath)
            .WithoutEncryptionKey(outEnvironment.Out.RemoveEncryptionKey);

         if (outEnvironment.Out.Incremental)
         {
            databaseBackUpper.Incrementally();
         }

         databaseBackUpper.Run();
      }

      private void runFileSystemBackup(dynamic outEnvironment)
      {
         fileSystemBackUpper
            .From(outEnvironment.Out.From.Server.FileSystem)
            .At(outEnvironment.Out.From.Server.Url)
            .To(outEnvironment.Out.To.FilePath)
            .WithoutEncryptionKey(outEnvironment.Out.RemoveEncryptionKey)
            .Incrementally(outEnvironment.Out.Incremental)
            .Run();
      }

      private void RunRestore(dynamic inEnvironment)
      {
         databaseRestorer
            .From(inEnvironment.In.From.FilePath)
            .To(inEnvironment.In.To.Server.Database)
            .At(inEnvironment.In.To.Server.Url)
            .Run();
      }

      public dynamic GetConfigFromArgs(IReadOnlyList<string> args)
      {
         if (args.Count < 2)
         {
            throw new ArgumentException("Please specify an environment: ");
         }

         var configArgument = args[1];

         dynamic databaseConfig;
         try
         {
            databaseConfig = JsonConfig.Config
               .ApplyJsonFromPath($"config/{configArgument}.json", JsonConfig.Config.Default);
         }
         catch (FileNotFoundException)
         {
            throw new ArgumentException($"Environment '{configArgument}' not recognized.");
            // Please use one of { string.Join(Environment.NewLine, (Array)JsonConfig.Config.Global.GetType().GetProperties()) }");
         }

         return databaseConfig;
      }

      private void PrintManual()
      {
         var manual = @"
======================================================
 _______  _______  _______           _______  _______ 
(  ___  )(  ____ \(  ____ \|\     /|(  ____ )(  ____ \
| (   ) || (    \/| (    \/| )   ( || (    )|| (    \/
| (___) || (_____ | (_____ | |   | || (____)|| (__    
|  ___  |(_____  )(_____  )| |   | ||     __)|  __)   
| (   ) |      ) |      ) || |   | || (\ (   | (      
| )   ( |/\____) |/\____) || (___) || ) \ \__| (____/\
|/     \|\_______)\_______)(_______)|/   \__/(_______/
                                                      
======================================================

usage: assure <command> [<args>]

commands:
- help            Print this page.
//- help [command]  Print details about each command.
- out [db.env]    
- in  [db.env]    
";
         logger.Info(manual);
      }
   }
}
