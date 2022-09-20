using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;
using Google.Ads.GoogleAds;
using Google.Ads.GoogleAds.Lib;
using Google.Ads.GoogleAds.V11.Services;
using Google.Api.Ads.Common.Util;
using Microsoft.Win32;

namespace GoogleQueryExporter
{
    public static class GoogleHelper
    {

        public static GoogleAdsClient GetClient()
        {
            var googleAdsClient = new GoogleAdsClient();
            return googleAdsClient;
        }
        
        public  static void ExecuteQueryCreateCsv(long customerId, string query)
        {
            try
            {
                var client = GetClient();

                var googleAdsServiceClient = client.GetService(Services.V11.GoogleAdsService);

                googleAdsServiceClient.SearchStream(customerId.ToString(), query,
                    delegate (SearchGoogleAdsStreamResponse response)
                    {
                        if (response.Results.Count() == 0)
                        {
                            MessageBox.Show("No Results");
                            return;
                        }

                        CreateCsvFromResponse(response);

                    }
                );
            }
            catch (Exception e)
            {
                MessageBox.Show("Error during query execution, check if the query written is valid");
                MessageBox.Show(e.Message);
            }
        }

        public static void CreateCsvFromResponse(SearchGoogleAdsStreamResponse response)
        {
            string path;

            var sfd = new SaveFileDialog
            {
                FileName = "ExportGoogleAds" + DateTime.Now.ToString("ddMMyyyyHHmmss"),
                Filter = "CSV (*.csv)|*.csv",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };
            var result = sfd.ShowDialog();
            if (result == true && !string.IsNullOrWhiteSpace(sfd.FileName))
            {
                path = sfd.FileName;
            }
            else
            {
                return;
            }

            var csvFile = new CsvFile();
            csvFile.Headers.AddRange(response.FieldMask.Paths);

            foreach (var responseResult in response.Results)
            {
                var row = GetCsvRowFromResponseRow(responseResult, response.FieldMask.Paths.ToArray());
                csvFile.Records.Add(row);
            }

            csvFile.Write(path);

            var p = new Process();
            p.StartInfo = new ProcessStartInfo(path)
            { 
                UseShellExecute = true 
            };
            p.Start();
        }

        public static string[] GetCsvRowFromResponseRow(GoogleAdsRow row, string[] headers)
        {
            var result = new string[headers.Length];

            for (int i = 0; i < headers.Length; i++)
            {
                result[i] = GetValueFromField(row, headers[i], typeof(GoogleAdsRow));
            }

            return result;
        }

        public static string GetValueFromField(object obj, string field, Type type)
        {
            if (string.IsNullOrWhiteSpace(field)) return string.Empty;

            if (field.Contains("."))
            {
                var split = field.Split('.', StringSplitOptions.RemoveEmptyEntries);

                var property = type.FindProperty(split[0]);
                if (property == null)
                    return $"Propiertà {split[0]} non trovata";

                var value = property.GetValue(obj, null);
                if (value == null) return "NULL";

                var subType = value.GetType();


                return GetValueFromField(value, string.Join('.',split[1..]), subType);
            }
            else
            {

                var property = type.FindProperty(field);
                if (property == null)
                    return $"Propiertà {field} non trovata";

                var value = property.GetValue(obj, null);
                return value?.ToString() ?? "NULL";
            }
        }

        public static PropertyInfo? FindProperty(this Type type, string name)
        {
            return type.GetProperties().FirstOrDefault(x => x.Name.ToLower() == name.Replace("_", "").ToLower());
        }
    }
}
