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

        public event EventHandler<NfcTagDetectedEventArgs> TagDetected
        {
            add => _pcsc.TagDetected += value;
            remove => _pcsc.TagDetected -= value;
        }
        
        public event EventHandler<EventArgs> SessionTimeout
        {
            add => _pcsc.SessionTimeout += value;
            remove => _pcsc.SessionTimeout -= value;
        }

        public Nfc()
        {
            //// TODO: FUTURE: Add ProximityDevice support...
            /// For now PCSC only.
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
