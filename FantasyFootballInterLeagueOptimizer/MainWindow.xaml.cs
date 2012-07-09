using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.IO;
using OAuth;

namespace FantasyFootballInterLeagueOptimizer {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        BobOAuth auth = new BobOAuth();

        public MainWindow() {
            InitializeComponent();
            requestAuthButton.Click += new RoutedEventHandler(connectButton_Click);
            getTokenButton.Click += new RoutedEventHandler(getTokenButton_Click);
            auth.SetConsumerProperties();
        }

        void getTokenButton_Click(object sender, RoutedEventArgs e) {
            var bobURL = auth.GenerateGetTokenURL(verifierTextBox.Text);
            var response = MakeRequest(bobURL);
            tokenTextBox.Text = response;
        }

        void connectButton_Click(object sender, RoutedEventArgs e) {
            var bobURL = auth.GenerateGetRequestTokenHMACSHA1SignedURL();
            var response = MakeRequest(bobURL);
            authTextBox.Text = auth.GenerateRequestAuthURL(response);
        }

        string MakeRequest(string url) {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            Stream resStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(resStream);
            return reader.ReadToEnd();
        }
    }
}
