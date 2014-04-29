using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommandLine;
using DbScriptDeploy.UI.Services;
using StructureMap;
using NLog;

namespace DbScriptDeploy.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                logger.Info("Application startup");
                BootStrapper.Boot();
                base.OnStartup(e);

                bool isCommandLine = false;
                CommandOptions options = new CommandOptions();
                if (CommandLine.Parser.Default.ParseArguments(e.Args, options))
                {
                    if (!String.IsNullOrWhiteSpace(options.ConnString) && !String.IsNullOrWhiteSpace(options.ScriptFolder))
                    {
                        isCommandLine = true;

                        try
                        {
                            IScriptExecutionService executionService = ObjectFactory.GetInstance<IScriptExecutionService>();
                            executionService.ExecuteScripts(options.ConnString, options.ScriptFolder);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            Console.WriteLine(ex.StackTrace);
                            throw;
                        }
                    }
                }

                if (!isCommandLine)
                {
                    new MainWindow().ShowDialog();
                }

                this.Shutdown();
                logger.Info("Application shutdown");
            }
            catch (Exception ex)
            {
                logger.FatalException(ex.Message, ex);
            }
        }
    }

	public class CommandOptions
	{
		[Option("conn", Required = false, HelpText = "A connection string for the database.")]
		public string ConnString { get; set; }
		
		[Option("scripts", Required = false, HelpText = "The folder where the scripts to be run are stored.")]
		public string ScriptFolder { get; set; }

		[HelpOption]
		public string GetUsage()
		{
			var usage = new StringBuilder();
			usage.AppendLine("DbScriptDeploy Command Line Reference");
			usage.AppendLine("-conn : Connection string to the database");
			usage.AppendLine("-scripts : The folder where the scripts to be run are stored");
			return usage.ToString();
		}
	}

}
