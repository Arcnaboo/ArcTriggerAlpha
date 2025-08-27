
namespace ArcTrigger.UI
{
    public partial class MainPage : ContentPage
    {
        
        int count = 0;
        private readonly ArcTrigger.Domain.IServices.IMarketDataService _marketDataService;
        private readonly CancellationTokenSource _cts = new();

        public MainPage(ArcTrigger.Domain.IServices.IMarketDataService marketDataService)
        {
            InitializeComponent();
            _marketDataService = marketDataService;
            _marketDataService.MessageReceived += OnMarketMessage;
        }

        private void OnCounterClicked(object? sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }

        private void OnMarketMessage(object? sender, string e)
        {
            System.Diagnostics.Debug.WriteLine($"WS: {e}");
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (LogBox != null)
                {
                    LogBox.Text += (LogBox.Text?.Length > 0 ? "\n" : string.Empty) + e;
                }
            });
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
        }

        protected override async void OnDisappearing()
        {
            base.OnDisappearing();
            try { _cts.Cancel(); } catch { }
            await _marketDataService.DisposeAsync();
        }

        private async void OnStartClicked(object? sender, EventArgs e)
        {
            try
            {
                var conids = new[] { "265598" }; // örnek conid
                var fields = new[] { "31", "84", "85" ,"86","88","82"}; // örnek alanlar
                await _marketDataService.WarmupAndSnapshotAsync(conids, fields, _cts.Token);
                await _marketDataService.StartStreamingAsync(conids, fields, _cts.Token);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", ex.Message, "Tamam");
            }
        }

        private async void OnStopClicked(object? sender, EventArgs e)
        {
            try { _cts.Cancel(); } catch { }
            await _marketDataService.DisposeAsync();
        }
    }
}
