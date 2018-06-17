using System;
using Xamarin.Forms;
using ZXing.Net.Mobile.Forms;

namespace Phoneword
{
    public partial class MainPage : ContentPage
    {
        string translatedNumber;

        public MainPage()
        {
            InitializeComponent();
        }

        void OnTranslate(object sender, EventArgs e)
        {
            translatedNumber = Core.PhonewordTranslator.ToNumber(phoneNumberText.Text);
            if (!string.IsNullOrWhiteSpace(translatedNumber))
            {
                callButton.IsEnabled = true;
                callButton.Text = "Call " + translatedNumber;
            }
            else
            {
                callButton.IsEnabled = false;
                callButton.Text = "Call";
            }
        }

        async void OnCall(object sender, EventArgs e)
        {
            if (await this.DisplayAlert(
                    "Dial a Number",
                    "Would you like to call " + translatedNumber + "?",
                    "Yes",
                    "No"))
            {
                var dialer = DependencyService.Get<IDialer>();
                if (dialer != null) {
                    App.PhoneNumbers.Add(translatedNumber);
                    callHistoryButton.IsEnabled = true;
                    dialer.Dial(translatedNumber);
                }
            }
        }

        async void OnCallHistory(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CallHistoryPage());
        }

        async void ScanButtonClicked(object sender, EventArgs e)
        {
            // スキャナページの設定
            var scanPage = new ZXingScannerPage()
            {
                DefaultOverlayTopText = "バーコードの読み取ります。",
                DefaultOverlayBottomText = "",
            };
            // スキャナページの表示
            await Navigation.PushAsync(scanPage);

            // データが取れると発火
            scanPage.OnScanResult += (result) =>
            {
                // スキャン停止
                scanPage.IsScanning = false;

                // PosAsyncで元のページに戻り、結果をダイアログで表示
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Navigation.PopAsync();
                    await DisplayAlert("すきゃ完了", result.Text, "OK");
                });
            };

        }
    }
}