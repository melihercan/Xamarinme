using NdefLibrary.Ndef;
using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarinme
{
    public class NfcTagDetectedEventArgs : EventArgs
    {
        public string TagId { get; }

        public NdefMessage NdefMessage { get; }

        public NfcTagDetectedEventArgs(string tagId, NdefMessage ndefMessage)
        {
            TagId = tagId;
            NdefMessage = ndefMessage;
        }
    }
}
