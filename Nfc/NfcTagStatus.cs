using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarinme
{
    public class NfcTagStatus
    {
        public byte[] TagId { get; set; }
        public byte[] TagVersion { get; set; }
        public bool IsWriteSupported { get; set; }
    }
}
