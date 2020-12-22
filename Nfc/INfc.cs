using NdefLibrary.Ndef;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Xamarinme
{
    public interface INfc : IDisposable
    {
        event EventHandler<NfcTagDetectedEventArgs> TagDetected;
        event EventHandler<EventArgs> SessionTimeout;

        Task EnableSessionAsync();
        Task DisableSessionAsync();

        Task<NdefMessage> ReadNdefAsync();
        Task WriteNdefAsync(NdefMessage ndefMessage);
        Task<NdefMessage> WriteReadNdefAsync(NdefMessage ndefMessage);
    }
}
