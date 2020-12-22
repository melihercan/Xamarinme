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

        public async Task EnableSessionAsync()
        {
            await _pcsc.EnableSessionAsync();
        }

        public Task DisableSessionAsync()
        {
            throw new NotImplementedException();
        }

        public Task<NdefMessage> ReadNdefAsync()
        {
            throw new NotImplementedException();
        }

        public Task WriteNdefAsync(NdefMessage ndefRecords)
        {
            throw new NotImplementedException();
        }

        public Task<NdefMessage> WriteReadNdefAsync(NdefMessage ndefRecords)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
