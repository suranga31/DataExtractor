using DataExtractor.Service.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace DataExtractor.Service.Service
{
    public class DataExtractService
    {
        
        private string _outputFolderPathBase;
        private string _currentOutputFolderPath;

        public delegate void NotificationHandler(string message);
        public event NotificationHandler RaiseNotification;
        private List<string> _failedFileList = new List<string>();
        public  void ExtractDataFromDB(string scriptPath)
        {
            RaiseNotification("Extraction Starts");
            _outputFolderPathBase = scriptPath + "\\Output";
            if (Directory.Exists(_outputFolderPathBase))
            {
                var backupName = _outputFolderPathBase + DateTime.Now.Ticks.ToString();
                Directory.Move(_outputFolderPathBase, backupName);
            }
            var folderList = GetFolderList(scriptPath);
            
            ExtractDataFromFolders(folderList);
            var failedFilePath = _outputFolderPathBase + "\\failedFiles.txt";

            using (StreamWriter outputFile = new StreamWriter(failedFilePath))
            {
                outputFile.WriteLine(String.Join(Environment.NewLine,_failedFileList.ToArray()));
            }

            RaiseNotification("Extraction Completed");


        }

        private void ExtractDataFromFolders(List<string> folderList)
        {
            foreach (string folder in folderList)
            {
                if (!folder.Contains(_outputFolderPathBase))
                {
                    var folderInfo = new DirectoryInfo(folder);
                    RaiseNotification(@"Start Extractons for : " + folderInfo.Name);
                    
                    _currentOutputFolderPath = _outputFolderPathBase +"\\" + folderInfo.Name;
                    ExtractScriptsFromFolder(folder);
                    RaiseNotification(@"Complete extracton for : " + folderInfo.Name);
                }
            }
        }
        

        private List<String> GetFolderList(string scriptPath)
        {
            return Directory.GetDirectories(scriptPath, "*", SearchOption.TopDirectoryOnly).ToList();

        }

        private void ExtractScriptsFromFolder(string folder)
        {
            var fileList = Directory.GetFiles(folder);
            foreach (var file in fileList)
            {
                try
                {
                    ExtractDataFromFile(file);
                } catch(Exception ex)
                {
                    RaiseNotification("Data extraction failed for :" + file);
                    _failedFileList.Add(file);
                }
            }
            RaiseNotification(@"Number of files processd : " + fileList.Length);
        }

        private void ExtractDataFromFile(string file)
        {
             
            string script = File.ReadAllText(file);
            var dataSet=  GetDataFromDataBase(script);
            string csv = ConvertDataSetToCSV(dataSet);
            var fileInfo = new FileInfo(file);
            if(!Directory.Exists(_currentOutputFolderPath))
            Directory.CreateDirectory(_currentOutputFolderPath);
            var datafilePath = _currentOutputFolderPath + "\\" + fileInfo.Name.Replace(fileInfo.Extension, ".csv");

            using (StreamWriter outputFile = new StreamWriter(datafilePath))
            {
                  outputFile.WriteLine(csv);
            }

            RaiseNotification(@"Data extraction completed for " + fileInfo.Name);


        }

        private DataSet GetDataFromDataBase(string script)
        {
            var dbConnectionString = ConfigurationManager.AppSettings["DBConnection"];
            var dataset = new DataSet();
            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = new SqlCommand(
                    script, connection);
                adapter.Fill(dataset);
                
            }
            return dataset;
        }


        public static string ConvertDataSetToCSV(DataSet ds)
        {
            const string LC_COMMA = ",";
            const string LC_DBLQUOTE = "\"";
            const string LC_DBLQUOTE_ESC = "\"\"";

            StringBuilder csv = new StringBuilder();

            foreach (DataTable tbl in ds.Tables)
            {
                // Append the table's column headers.
                foreach (DataColumn col in tbl.Columns)
                {
                    csv.Append(LC_DBLQUOTE + col.ColumnName + LC_DBLQUOTE + LC_COMMA);
                }
                csv.Length -= 1;
                csv.Append(Environment.NewLine);

                // Append the table's data.
                foreach (DataRow row in tbl.Rows)
                {
                    foreach (object val in row.ItemArray)
                    {
                        csv.Append(LC_DBLQUOTE + val.ToString().Replace(LC_DBLQUOTE, LC_DBLQUOTE_ESC)
                        + LC_DBLQUOTE + LC_COMMA);
                    }
                    csv.Length -= 1;
                    csv.Append(Environment.NewLine);
                }

                // Add an empty line between this and the next table.
                csv.Append(Environment.NewLine);
            }

            return csv.ToString();
        }
    }
}
