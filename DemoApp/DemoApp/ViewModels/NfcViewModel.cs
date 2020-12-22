using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarinme;

namespace DemoApp.ViewModels
{
    public class NfcViewModel
    {
        private readonly INfc _nfc;

        public NfcViewModel()
        {
            _nfc = CrossNfc.Current;
        }

        public ICommand StartCommand => new Command(async () => 
        {
            await _nfc.EnableSessionAsync();
            _nfc.TagDetected += (s, e) => 
            {
                System.Diagnostics.Debug.WriteLine($"======== TAG DETECTED ========");
                System.Diagnostics.Debug.WriteLine($"Id: {e.TagId}");
                System.Diagnostics.Debug.WriteLine(
                    $"Data: {BitConverter.ToString(e.NdefMessage.ToByteArray()) .Replace("-", string.Empty)}");
            };
        });
    }
}
