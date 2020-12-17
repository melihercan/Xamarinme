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

        public Task<List<NdefRecord>> ReadNdefAsync()
        {
            throw new NotImplementedException();
        }

        public Task WriteNdefAsync(List<NdefRecord> wrNdefRecords)
        {
            throw new NotImplementedException();
        }

        public Task<List<NdefRecord>> WriteReadNdefAsync(List<NdefRecord> wrNdefRecords)
        {
            throw new NotImplementedException();
        }
    }
}
