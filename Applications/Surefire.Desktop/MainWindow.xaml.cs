using System;
using System.Runtime.InteropServices;
using Microsoft.Web.WebView2.Core;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace Surefire.Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        private Process _blazorProcess;
        //private const int WM_NCHITTEST = 0x0084;
        //private const int HTCLIENT = 1;
        //private const int HTCAPTION = 2;
        //private const int HTLEFT = 10;
        //private const int HTRIGHT = 11;
        //private const int HTTOP = 12;
        //private const int HTTOPLEFT = 13;
        //private const int HTTOPRIGHT = 14;
        //private const int HTBOTTOM = 15;
        //private const int HTBOTTOMLEFT = 16;
        //private const int HTBOTTOMRIGHT = 17;

        public MainWindow()
        {
            InitializeComponent();

            // Start the Blazor Server app
            StartBlazorServerApp();

            // Initialize WebView2 after ensuring the environment is ready
            WebView.NavigationCompleted += OnWebViewNavigationCompleted;
            InitializeWebView();
        }
        private async void InitializeWebView()
        {
            try
            {
                // Ensure WebView2 environment is initialized
                await WebView.EnsureCoreWebView2Async();
                WebView.Source = new Uri("http://localhost:5000");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing WebView2: {ex.Message}");
            }
        }
        //private void MainWindow_SourceInitialized(object sender, EventArgs e)
        //{
        //    var hwndSource = (HwndSource)PresentationSource.FromVisual(this);
        //    hwndSource.AddHook(WndProc);
        //}
        //private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        //{
        //    if (msg == WM_NCHITTEST)
        //    {
        //        handled = true;
        //        var mousePoint = PointFromLParam(lParam);
        //        var result = GetResizeDirection(mousePoint);
        //        return new IntPtr(result);
        //    }

        //    return IntPtr.Zero;
        //}
        //private static Point PointFromLParam(IntPtr lParam)
        //{
        //    int x = lParam.ToInt32() & 0xFFFF;
        //    int y = (lParam.ToInt32() >> 16) & 0xFFFF;
        //    return new Point(x, y);
        //}

        //private int GetResizeDirection(Point mousePoint)
        //{
        //    const int resizeBorderThickness = 8; // Resize border thickness in pixels.

        //    var windowRect = new Rect(0, 0, ActualWidth, ActualHeight);
        //    if (mousePoint.X < resizeBorderThickness)
        //    {
        //        if (mousePoint.Y < resizeBorderThickness)
        //            return HTTOPLEFT;
        //        if (mousePoint.Y > windowRect.Height - resizeBorderThickness)
        //            return HTBOTTOMLEFT;
        //        return HTLEFT;
        //    }
        //    if (mousePoint.X > windowRect.Width - resizeBorderThickness)
        //    {
        //        if (mousePoint.Y < resizeBorderThickness)
        //            return HTTOPRIGHT;
        //        if (mousePoint.Y > windowRect.Height - resizeBorderThickness)
        //            return HTBOTTOMRIGHT;
        //        return HTRIGHT;
        //    }
        //    if (mousePoint.Y < resizeBorderThickness)
        //        return HTTOP;
        //    if (mousePoint.Y > windowRect.Height - resizeBorderThickness)
        //        return HTBOTTOM;

        //    return HTCLIENT;
        //}


        private void StartBlazorServerApp2()
        {
            // Base directory includes the configuration folder (e.g., Debug or Release)
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // Adjust the relative path to go up two levels (to Build) and then into Web
            //var relativePath = @"..\..\..\Web\Surefire.exe";
            //var blazorAppPath = Path.GetFullPath(Path.Combine(baseDirectory, relativePath));
            var blazorAppPath = @"C:\Users\flash\source\devops\Surefire\Build\Web\Surefire.exe";

        

            if (!File.Exists(blazorAppPath))
            {
                throw new FileNotFoundException($"The Blazor Server app executable was not found at {blazorAppPath}");
            }

            _blazorProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = blazorAppPath,
                    UseShellExecute = true,
                    RedirectStandardOutput = false,
                    RedirectStandardError = false
                }
            };

            _blazorProcess.Start();
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
                    UseShellExecute = false, // Required for RedirectStandardOutput/Hidden Window
                    RedirectStandardOutput = false, // Not redirecting output
                    RedirectStandardError = false, // Not redirecting errors
                    CreateNoWindow = true, // Suppress the command window
                    WindowStyle = ProcessWindowStyle.Hidden // Ensures no visible command window
                }
            };

            _blazorProcess.Start();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Ensure the Blazor app is terminated
            if (_blazorProcess != null && !_blazorProcess.HasExited)
            {
                _blazorProcess.Kill();
            }
        }

        private void OnWebViewNavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            // Attach event handler for JavaScript messages
            WebView.CoreWebView2.WebMessageReceived += OnWebMessageReceived;
        }

        private void OnWebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            Console.WriteLine("message recieved web");
            try
            {
                // Parse the message from JavaScript
                var message = e.WebMessageAsJson;
                if (!string.IsNullOrEmpty(message))
                {
                    var data = JsonSerializer.Deserialize<Dictionary<string, string>>(message);

                    if (data != null && data.TryGetValue("action", out var action))
                    {
                        switch (action)
                        {
                            case "drag_start":
                                DragMoveWindow();
                                break;
                            case "drag_stop":
                                // Optional: handle if needed
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in WebView message handling: {ex.Message}");
            }
        }
        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("draggers");
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        private void DragMoveWindow()
        {
            Console.WriteLine("dragmoveg");
           
                this.DragMove();
            
        }
    }
}