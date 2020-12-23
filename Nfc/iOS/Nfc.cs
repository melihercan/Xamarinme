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

//https://forums.xamarin.com/discussion/170773/how-do-i-use-the-writendef-method-in-ios-13-to-write-to-an-nfc-tag

namespace Xamarinme
{
    public class Nfc : /*NFCTagReaderSessionDelegate*/ NFCNdefReaderSessionDelegate, INfc
    {
        private bool _isSessionEnabled;
        //private NFCTagReaderSession _session;
        private NFCNdefReaderSession _session;

        public Nfc()
        {
        }

        public event EventHandler<NfcTagDetectedEventArgs> TagDetected;
        public event EventHandler<EventArgs> SessionTimeout;

        public Task EnableSessionAsync()
        {
            if (_isSessionEnabled)
                return Task.CompletedTask;

            ///            _session = new NFCTagReaderSession(
            ///             NFCPollingOption.Iso14443, //// | NFCPollingOption.Iso15693 | NFCPollingOption.Iso18092, 
            ////          this,
            ///      DispatchQueue.CurrentQueue)
            //{
            //  AlertMessage = "Tap a tag to start..."
            //};

            _session = new NFCNdefReaderSession(this, DispatchQueue.CurrentQueue, false);
            _session.BeginSession();
            _isSessionEnabled = true;
            return Task.CompletedTask;
        }

        public Task DisableSessionAsync()
        {
            if (!_isSessionEnabled)
                return Task.CompletedTask;

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

        ////[Foundation.Export("readerSession:didDetectTags:")]
        public override void DidDetectTags(NFCNdefReaderSession session, INFCNdefTag[] tags)
        {
            base.DidDetectTags(session, tags);
        }

        public override void DidDetect(NFCNdefReaderSession session, NFCNdefMessage[] messages)
        {
            throw new NotImplementedException();
        }

        public override void DidInvalidate(NFCNdefReaderSession session, NSError error)
        {
            throw new NotImplementedException();
        }

#if false
        public override void DidDetectTags(NFCTagReaderSession session, INFCTag[] tags)
        {
            var tag = tags.First();

            Debug.WriteLine($"===========> TAG TAPPED: tag count= {tags.Length}");
            Debug.WriteLine($"TAG TYPE: {tag.Type}");
        }

        public override void DidInvalidate(NFCTagReaderSession session, NSError error)
        {
        }
#endif
    }
}
