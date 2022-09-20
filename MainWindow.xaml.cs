using Microsoft.Win32;
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

namespace GoogleQueryExporter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ExecuteCsvClick(object sender, RoutedEventArgs e)
        {
            var customerIdText = TextBoxCustomer.Text;
            if (customerIdText.Contains("-"))
                customerIdText = customerIdText.Replace("-", "");

            if (!long.TryParse(customerIdText, out var customerId))
            {
                MessageBox.Show("Customer Id must be a numeric value");
                return;
            }

            GoogleHelper.ExecuteQueryCreateCsv(customerId, TextBoxQuery.Text);
        }

        private void ExampleYesterdayCampaign(object sender, RoutedEventArgs e)
        {
            TextBoxQuery.Text = $@"SELECT campaign.id, campaign.name, campaign.status 
FROM campaign 
WHERE segments.date = '{DateTime.Today.AddDays(-1):yyyy-MM-dd}'
and metrics.impressions > 0";
        }

        private void ExampleCallView(object sender, RoutedEventArgs e)
        {
            TextBoxQuery.Text = $@"SELECT call_view.call_duration_seconds, call_view.call_status, call_view.call_tracking_display_location, call_view.caller_area_code, call_view.caller_country_code, call_view.end_call_date_time, call_view.resource_name, call_view.start_call_date_time, call_view.type, campaign.id, campaign.name 
FROM call_view 
WHERE call_view.start_call_date_time > '{DateTime.Today.AddDays(-7):yyyy-MM-dd}'";
        }
    }
}
