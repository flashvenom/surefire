using Quickfire.Desktop.Services;

namespace Quickfire.Desktop
{
    public partial class App : Application
    {
        private readonly MainPage _mainPage;
        private readonly IQuickfireHost _quickfireHost;
        private readonly IDesktopEmberBridge _emberBridge;

        public App(MainPage mainPage, IQuickfireHost quickfireHost, IDesktopEmberBridge emberBridge)
        {
            InitializeComponent();
            _mainPage = mainPage;
            _quickfireHost = quickfireHost;
            _emberBridge = emberBridge;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = new Window(_mainPage) { Title = "Quickfire" };

            // Handle window closing to soft-shutdown Quickfire.Blazor.exe
            window.Destroying += async (s, e) =>
            {
                try
                {
                    await _emberBridge.DisposeAsync().ConfigureAwait(false);
                    await _quickfireHost.DisposeAsync().ConfigureAwait(false);
                }
                catch
                {
                    // Ignore errors during shutdown
                }
            };
            
            return window;
        }
    }
}
