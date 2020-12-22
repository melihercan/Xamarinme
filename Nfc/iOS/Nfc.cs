using CoreFoundation;
using CoreNFC;
using Foundation;
using NdefLibrary.Ndef;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamarinme
{
    public class Nfc : NFCTagReaderSessionDelegate, INfc
    {
        private bool _isSessionEnabled;
        private NFCTagReaderSession _session;

        public Nfc()
        {
            _session = new NFCTagReaderSession(
                NFCPollingOption.Iso14443 | NFCPollingOption.Iso15693 | NFCPollingOption.Iso18092, 
                this, 
                DispatchQueue.CurrentQueue)
            {
                AlertMessage = "Tap a tag to start..."
            };
        }

        public event EventHandler<NfcTagDetectedEventArgs> TagDetected;
        public event EventHandler<EventArgs> SessionTimeout;


        //public Task<NfcServiceStatus> InitAsync()
        //{
        //    throw new NotImplementedException();
        //}
        //public Task DeInitAsync()
        //{
        //    throw new NotImplementedException();
        //}

        public Task EnableSessionAsync()
        {
            if (_isSessionEnabled)
            {
                return Task.CompletedTask;
            }

            _session.BeginSession();


            _isSessionEnabled = true;

            return Task.CompletedTask;

        }
        public Task DisableSessionAsync()
        {
            if (!_isSessionEnabled)
            {
                return Task.CompletedTask;
            }

            _session.InvalidateSession();

            _isSessionEnabled = false;

            return Task.CompletedTask;
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

        public override void DidDetectTags(NFCTagReaderSession session, INFCTag[] tags)
        {
            var tag = tags.First();

            Debug.WriteLine($"===========> TAG TAPPED: tag count= {tags.Length}");
            Debug.WriteLine($"TAG TYPE: {tag.Type}");
        }

        public override void DidInvalidate(NFCTagReaderSession session, NSError error)
        {
        }
    }
}
