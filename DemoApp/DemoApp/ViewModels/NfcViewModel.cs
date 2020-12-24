using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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
            
            _nfc.TagDetected += async (s, e) => 
            {
                System.Diagnostics.Debug.WriteLine($"======== TAG DETECTED ========");
                System.Diagnostics.Debug.WriteLine($"Id: {e.TagId}");
                System.Diagnostics.Debug.WriteLine(
                    $"Data: {BitConverter.ToString(e.NdefMessage.ToByteArray()) .Replace("-", string.Empty)}");

                var nm = await _nfc.ReadNdefAsync();
                System.Diagnostics.Debug.WriteLine($"======== READ ========");
                System.Diagnostics.Debug.WriteLine(
                    $"Data: {BitConverter.ToString(nm.ToByteArray()).Replace("-", string.Empty)}");

            };

            _nfc.SessionTimeout += (s, e) =>
            {
                System.Diagnostics.Debug.WriteLine($"======== SESSION TIMEOUT ========");
            };



            //await Task.Delay(2000);
            //await _nfc.DisableSessionAsync();
        });
    }
}
