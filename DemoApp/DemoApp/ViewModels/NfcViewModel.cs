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
        });
    }
}
