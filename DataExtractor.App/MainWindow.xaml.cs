using DataExtractor.Service.Service;
using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DataExtractor.App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Logger _logger;
        public MainWindow()
        {
            InitializeComponent();
            _logger = new LoggerConfiguration()
                  .ReadFrom.AppSettings()
                  .CreateLogger();
        }

        private async void btnExtractData_Click(object sender, RoutedEventArgs e)
        {
            DataExtractService sv = new DataExtractService();
            sv.RaiseNotification += new DataExtractService.NotificationHandler(NotificationFound);
            var folderPath = txtFolderPath.Text.Trim();

            await Task.Run(() =>
            {
                try
                {
                    sv.ExtractDataFromDB(folderPath);
                } catch(Exception ex)
                {
                    NotificationFound("Data extraction failed : " + ex.Message);
                }
            });
        }


        private void NotificationFound(string message)
        {

            // ;
            Dispatcher.Invoke(() =>
            {
                txtLog.AppendText(message + Environment.NewLine);
                try
                {
                    _logger.Debug(message);
                } catch(Exception ex)
                {
                    var s = ex.Message;
                }
            });
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txtLog.Clear();
        }
    }
}
