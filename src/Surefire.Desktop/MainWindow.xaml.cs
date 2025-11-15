using System.Diagnostics;
using System.IO;
using System.Windows;
using Microsoft.Web.WebView2.Core;

namespace Surefire.Desktop
{
    public partial class MainWindow : Window
    {
        private Process _blazorProcess;

        public MainWindow()
        {
            InitializeComponent();
            StartBlazorServerApp();
            InitializeWebView();
        }

        private async void InitializeWebView()
        {
            try
            {
                // Ensure WebView2 environment is initialized
                await WebView.EnsureCoreWebView2Async();

                // Subscribe to WebView2 events
                WebView.NavigationStarting += OnNavigationStarting;
                WebView.NavigationCompleted += OnNavigationCompleted;

                // Navigate to the Blazor app
                WebView.Source = new Uri("http://localhost:5000");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing WebView2: {ex.Message}");
            }
        }

        private void OnNavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            // Show the loading overlay when navigation starts
            LoadingOverlay.Visibility = Visibility.Visible;
        }

        private void OnNavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            // Hide the loading overlay when navigation is complete
            LoadingOverlay.Visibility = Visibility.Collapsed;
        }

        private void StartBlazorServerApp()
        {
            // Get the base directory of the current application
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // Path to the Blazor Server executable (assumes it's in the same folder as this app)
            var blazorAppPath = Path.Combine(baseDirectory, "Surefire.exe");

            // Check if the Blazor Server app exists
            if (!File.Exists(blazorAppPath))
            {
                throw new FileNotFoundException($"The Blazor Server app executable was not found at {blazorAppPath}");
            }

            // Start the Blazor Server app with a hidden window
            _blazorProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = blazorAppPath,
                    UseShellExecute = false,
                    RedirectStandardOutput = false,
                    RedirectStandardError = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                }
            };

            _blazorProcess.Start();
        }
    }
}
