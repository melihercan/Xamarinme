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
        private bool _isSessionEnabled;

        public event EventHandler<NfcTagDetectedEventArgs> TagDetected;
        public event EventHandler<EventArgs> SessionTimeout;

        public Task EnableSessionAsync()
        {
            if(_isSessionEnabled)
            {
                return Task.CompletedTask;
            }


            _isSessionEnabled = true;

            return Task.CompletedTask;


        }

        public Task DisableSessionAsync()
        {
            if(!_isSessionEnabled)
            {
                return Task.CompletedTask;
            }

            _isSessionEnabled = false;

            return Task.CompletedTask;

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
