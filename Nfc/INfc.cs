using NdefLibrary.Ndef;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Xamarinme
{
    public interface INfc
    {
        event EventHandler<NfcTagDetectedEventArgs> TagDetected;
        event EventHandler<EventArgs> SessionTimeout;

        Task EnableSessionAsync();
        Task DisableSessionAsync();

        Task<List<NdefRecord>> ReadNdefAsync();
        Task WriteNdefAsync(List<NdefRecord> wrNdefRecords);
        Task<List<NdefRecord>> WriteReadNdefAsync(List<NdefRecord> wrNdefRecords);


    }
}
