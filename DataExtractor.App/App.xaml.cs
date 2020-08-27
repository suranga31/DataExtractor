using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace DataExtractor.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        { 

             var logger = new LoggerConfiguration()
                  .ReadFrom.AppSettings()
                  .CreateLogger();

             base.OnStartup(e);

             
        }
    }
}
