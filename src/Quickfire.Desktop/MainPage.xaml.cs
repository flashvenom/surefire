using Microsoft.Maui.Controls;
using Quickfire.Desktop.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Quickfire.Desktop
{
    public partial class MainPage : ContentPage
    {
        private readonly IQuickfireHost _quickfireHost;
        private readonly IDesktopEmberBridge _emberBridge;
        private readonly IQuickfireTrayService _trayService;

        private CancellationTokenSource? _startupCts;
        private bool _hasInitialized;

        public MainPage(IQuickfireHost quickfireHost, IDesktopEmberBridge emberBridge, IQuickfireTrayService trayService)
        {
            InitializeComponent();
            _quickfireHost = quickfireHost;
            _emberBridge = emberBridge;
            _trayService = trayService;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (_hasInitialized)
            {
                return;
            }

            _hasInitialized = true;
            _ = StartQuickfireAsync();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _startupCts?.Cancel();
        }

        private void OnRetryClicked(object sender, EventArgs e)
        {
            _ = StartQuickfireAsync();
        }

        private async Task StartQuickfireAsync()
        {
            SetLoadingState("Starting Quickfire...", showRetry: false);

            _startupCts?.Cancel();
            var cts = new CancellationTokenSource();
            _startupCts = cts;

            try
            {
                _trayService.EnsureStarted();

                var baseUri = await _quickfireHost.EnsureStartedAsync(cts.Token).ConfigureAwait(false);
                if (cts.IsCancellationRequested)
                {
                    return;
                }

                await _emberBridge.EnsureConnectedAsync(baseUri, cts.Token).ConfigureAwait(false);

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    AppWebView.Source = baseUri.ToString();
                    AppWebView.IsVisible = true;
                    LoadingOverlay.IsVisible = false;
                    RetryButton.IsVisible = false;
                });
            }
            catch (OperationCanceledException)
            {
                // ignored
            }
            catch (Exception ex)
            {
                if (cts.IsCancellationRequested)
                {
                    return;
                }

                SetLoadingState($"Failed to start Quickfire: {ex.Message}", showRetry: true);
            }
        }

        private void SetLoadingState(string message, bool showRetry)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                LoadingMessage.Text = message;
                LoadingOverlay.IsVisible = true;
                RetryButton.IsVisible = showRetry;
                AppWebView.IsVisible = false;
            });
        }
    }
}
