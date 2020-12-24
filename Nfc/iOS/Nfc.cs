using CoreFoundation;
using CoreNFC;
using Foundation;
using NdefLibrary.Ndef;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

//https://forums.xamarin.com/discussion/170773/how-do-i-use-the-writendef-method-in-ios-13-to-write-to-an-nfc-tag

namespace Xamarinme
{
    public class Nfc : NFCNdefReaderSessionDelegate, INfc
    {
        private bool _isSessionEnabled;
        private NFCNdefReaderSession _session;
        private INFCNdefTag _tag;
        private SemaphoreSlim _lockSemaphore = new SemaphoreSlim(1);

        public event EventHandler<NfcTagDetectedEventArgs> TagDetected;
        public event EventHandler<EventArgs> SessionTimeout;

        public async Task EnableSessionAsync()
        {
            try
            {
                await _lockSemaphore.WaitAsync();                

                if (_isSessionEnabled)
                    return;

                _session = new NFCNdefReaderSession(this, DispatchQueue.CurrentQueue, false)
                {
                    AlertMessage = "Tap a tag to start..."
                };

                _session.BeginSession();
                _isSessionEnabled = true;
                System.Diagnostics.Debug.WriteLine("========> Session enabled");
            }
            finally
            {
                _lockSemaphore.Release();
            }
        }

        public async Task DisableSessionAsync()
        {
            try
            {
                await _lockSemaphore.WaitAsync();

                if (!_isSessionEnabled)
                    return;

                _session.InvalidateSession();
                _isSessionEnabled = false;
                System.Diagnostics.Debug.WriteLine("========> Session disabled");
            }
            finally
            {
                _lockSemaphore.Release();
            }
        }


        public async Task<NdefMessage> ReadNdefAsync()
        {
            try
            {
                await _lockSemaphore.WaitAsync();

                if (!_isSessionEnabled)
                    throw new Exception("NFC is not enabled");

                var tcs = new TaskCompletionSource<NdefMessage>();

                _tag.ReadNdef((iosNdefMessage, error) =>
                {
                    if (error != null)
                        throw new Exception(error.Description);

                    var ndefMessage = new NdefMessage();
                    ndefMessage.AddRange(iosNdefMessage.Records.Select(iosRecord => new NdefRecord 
                    { 
                        Id = iosRecord.Identifier.ToArray(),
                        Type = iosRecord.Type.ToArray(),
                        TypeNameFormat = (NdefRecord.TypeNameFormatType)iosRecord.TypeNameFormat,
                        Payload = iosRecord.Payload.ToArray()
                    }));

                    tcs.SetResult(ndefMessage);
                });

                return await tcs.Task;
            }
            finally
            {
                _lockSemaphore.Release();
            }
        }

        public async Task WriteNdefAsync(NdefMessage ndefMessage)
        {
            try
            {
                await _lockSemaphore.WaitAsync();

                if (!_isSessionEnabled)
                    throw new Exception("NFC is not enabled");

                var tcs = new TaskCompletionSource<object>();

                var iosNdefMessage = new NFCNdefMessage(ndefMessage.ToList().Select(record => new NFCNdefPayload 
                (
                    format: (NFCTypeNameFormat)record.TypeNameFormat,
                    type: NSData.FromArray(record.Type),
                    identifier: NSData.FromArray(record.Id),
                    payload: NSData.FromArray(record.Payload)
                )).ToArray());
                _tag.WriteNdef(iosNdefMessage, (error) => 
                {
                    if (error != null)
                        throw new Exception(error.Description);

                    tcs.SetResult(null);
                });

                await tcs.Task;
            }
            finally
            {
                _lockSemaphore.Release();
            }
        }

        public async Task<NdefMessage> WriteReadNdefAsync(NdefMessage ndefMessage)
        {
            await WriteNdefAsync(ndefMessage);

            // Compare the write content to read content to make sure that device responded.
            // Try this few times with timeout before generating error.
            for (int i = 0; i < 5; i++)
            {
                await Task.Delay(40);
                var rdNdefMessage = await ReadNdefAsync();
                if (!rdNdefMessage.ToByteArray().SequenceEqual(ndefMessage.ToByteArray()))
                    return rdNdefMessage;
            }

            throw new Exception("WriteReadNdefAsync failed!!!");
        }

        public override async void DidDetectTags(NFCNdefReaderSession session, INFCNdefTag[] tags)
        {
            System.Diagnostics.Debug.WriteLine("========> DidDetectTags");

            _tag = tags[0];
            await session.ConnectToTagAsync(_tag);

            TagDetected?.Invoke(this, new NfcTagDetectedEventArgs(
                tagId: string.Empty,    //// TODO: CHECK HOW TO GET ID
                ndefMessage: await ReadNdefAsync()));
        }

        public override void DidDetect(NFCNdefReaderSession session, NFCNdefMessage[] messages)
        {
            System.Diagnostics.Debug.WriteLine("========> DidDetect");
            throw new Exception("DidDetectTags should be used to access the tag");
        }

        public override async void DidInvalidate(NFCNdefReaderSession session, NSError error)
        {
            System.Diagnostics.Debug.WriteLine($"========> DidInvalidate {error}");
            await DisableSessionAsync();

            if ((NFCReaderError)(long)error.Code == NFCReaderError.ReaderSessionInvalidationErrorSessionTimeout)
                SessionTimeout?.Invoke(this, EventArgs.Empty);
        }
    }
}
