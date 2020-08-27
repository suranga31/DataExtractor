using DataExtractor.Service.Service;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataExtactor.Win
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void btnExtract_ClickAsync(object sender, EventArgs e)
        {
            DataExtractService sv = new DataExtractService();
            sv.RaiseNotification += new DataExtractService.NotificationHandler(NotificationFound);

           await Task.Run(() =>
            {
                sv.ExtractDataFromDB(txtScriptFolderPath.Text);
            });
        }

        private void NotificationFound(string message)
        {
     
           // ;
            txtOutput.Invoke((MethodInvoker)(() => txtOutput.AppendText(System.Environment.NewLine + message)));
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
