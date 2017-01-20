using System;
using System.Collections.Generic;
using System.IO;
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
         var program = new Program(new ConsoleLogger());
         program.ParseCommands(args);
      }

      public Program(ILogger logger)
      {
         this.logger = logger;
      }

      public void ParseCommands(IReadOnlyList<string> args)
      {
         if (args.Count == 0 || args[0] == "help")
         {
            PrintManual();
            return;
         }
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
            databaseConfig = JsonConfig.Config.ApplyJsonFromPath($"config/{configArgument}.json", JsonConfig.Config.Default);
         }
         catch (FileNotFoundException)
         {
            throw new ArgumentException($"Environment '{configArgument}' not recognized.");// Please use one of { string.Join(Environment.NewLine, (Array)JsonConfig.Config.Global.GetType().GetProperties()) }");
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
