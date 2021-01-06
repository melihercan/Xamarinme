using NdefLibrary.Ndef;
using XamarinmeNfc.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Xamarinme
{
    public class Nfc : INfc
    {
        private readonly INfc _pcsc;

        public event EventHandler<NfcTagDetectedEventArgs> TagDetected;
        public event EventHandler<EventArgs> SessionTimeout;

        public Nfc()
        {
            _pcsc = new Pcsc();
        }

        public Task EnableSessionAsync()
        {
            return _pcsc.EnableSessionAsync();
        }

        public Task DisableSessionAsync()
        {
            return _pcsc.DisableSessionAsync();
        }

        public Task<NdefMessage> ReadNdefAsync()
        {
            return _pcsc.ReadNdefAsync();
        }

        public Task WriteNdefAsync(NdefMessage ndefMessage)
        {
            return _pcsc.WriteNdefAsync(ndefMessage);
        }

        public Task<NdefMessage> WriteReadNdefAsync(NdefMessage ndefMessage)
        {
            return _pcsc.WriteReadNdefAsync(ndefMessage);
        }

        public void Dispose()
        {
            _pcsc.Dispose();
        }
    }
}
