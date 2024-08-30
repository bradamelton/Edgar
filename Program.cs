using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

using log4net;
using log4net.Core;
using log4net.Config;

using ConsoleTools;

namespace EdgarC
{
    public class Program
    {
        static void Main(string[] args)
        {


            /* need to look at this for tag reading: https://github.com/Arelle/Arelle

            */



            ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            var _ui = _log;
            //var logRepository = LogManager.GetRepository(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString());
            //XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

            //ILog _log = (ILog)LogManager.GetLogger("console");
            //ILog _ui = (ILog)LogManager.GetLogger("ui");

            var _configurationAdapter = new DBu.ConfigurationAdapter();
            var _dbuProvider = new DBu.DBuMongoProvider(_configurationAdapter);

            var downloader = new Downloader(new ConfigurationAdapter(), new FileProvider(), _log);
            var em = new EdgarCManager(downloader, _log, _ui, _dbuProvider);

            var menu = new ConsoleMenu(args, level: 0)
                .Add("Search Companies", () =>
                {
                    Console.Write("Enter Company Search Value: ");
                    var search = Console.ReadLine();
                    var ret = em.CompanySearch(search);

                    if (ret != null && ret.Count > 0)
                    {
                        ret.ForEach(r => { Console.WriteLine($"{r.Name}"); });
                    }
                    else
                    {
                        Console.Write("Not found...");
                        Console.ReadKey();
                    }
                })
                .Add("Download Current Quarter", () =>
                {
                    em.DownloadQuarterCurrent();
                })
                .Add("Exit", () =>
                {
                    Console.WriteLine("Bye!");
                    Environment.Exit(0);
                }).Configure(config =>
                {
                    config.Selector = "--> ";
                    config.EnableFilter = false;
                    config.Title = "Main menu";
                    config.EnableWriteTitle = true;
                    config.EnableBreadcrumb = false;
                });

            menu.Show();

            // want to get all companies numbers
            // need to link these to another db.

            // do'nt need to do that immediately. It's not as applicable to what I'm trying to do here.

            // load a range of indexes. (Just daily master.)





            // build and or update comprehensive list of available files and records.

            // need to compare which ones we have and which ones not.

            // maybe compare filecounts in the directories...
            /// that can be the checksum for now.

        }
    }
}