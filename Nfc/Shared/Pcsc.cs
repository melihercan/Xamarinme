using NdefLibrary.Ndef;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PCSC;
using PCSC.Monitoring;
using System.Diagnostics;
using System.Linq;
using PCSC.Iso7816;
using PCSC.Reactive;
using Xamarinme;

namespace XamarinmeNfc.Shared
{
    internal class Pcsc : INfc
    {
        private bool _isDisposed = false;

        public Pcsc()
        {
            using (var context = ContextFactory.Instance.Establish(SCardScope.System))
            {
                var readers = context.GetReaders();
                Debug.WriteLine($"NUM READERS: {readers.Count()}");
                foreach (var reader in readers)
                {
                    Debug.WriteLine($"READER: {reader}");
                }
            }
        }


        public event EventHandler<NfcTagDetectedEventArgs> TagDetected;
        public event EventHandler<EventArgs> SessionTimeout;

        public Task EnableSessionAsync()
        {
            return Task.CompletedTask;
        }

        public Task DisableSessionAsync()
        {
            throw new NotImplementedException();
        }

        public Task<NdefMessage> ReadNdefAsync()
        {
            throw new NotImplementedException();
        }

        public Task WriteNdefAsync(NdefMessage ndefMessage)
        {
            throw new NotImplementedException();
        }

        public Task<NdefMessage> WriteReadNdefAsync(NdefMessage ndefMessage)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
////                    _readersMonitor.Cancel();
    /////                _readersMonitor.StatusChanged -= OnReadersStatusChanged;
         /////           _readersMonitor.Dispose();
                }

                _isDisposed = true;
            }
        }









#if false

        private readonly IDeviceMonitor _readersMonitor;
        private class ReaderContext
        {
            internal ISCardMonitor CardMonitor { get; set; }
            internal ISCardContext CardContext { get; set; }
            internal IIsoReader IsoReader { get; set; }
        }
        private IDictionary<string, ReaderContext> Readers { get; set; } = new Dictionary<string, ReaderContext>();

        // Only one reader will be active.
        private string _activeReaderName = string.Empty;
        private object _lock = new object();
        private const byte CustomCla = 0xFF;

        public Pcsc()
        {
            using(var context = ContextFactory.Instance.Establish(SCardScope.System))
            {
                var readers = context.GetReaders();
                Debug.WriteLine($"NUM READERS: {readers.Count()}");
                foreach (var reader in readers)
                {
                    Debug.WriteLine($"READER: {reader}");
                }

                UpdateReaders(readers.ToList());
            }

            _readersMonitor = DeviceMonitorFactory.Instance.Create(SCardScope.System);
            _readersMonitor.StatusChanged += OnReadersStatusChanged;
            _readersMonitor.Start();
        }



        private void OnReadersStatusChanged(object sender, DeviceChangeEventArgs e)
        {
            Debug.WriteLine($"EVENT - NUM READERS: {e.AllReaders.Count()}");
            foreach (var reader in e.AllReaders)
            {
                Debug.WriteLine($"EVENT - READER: {reader}");
            }

            UpdateReaders(e.AllReaders.ToList());
        }


        private void OnCardInserted(object sender, CardStatusEventArgs e)
        {
            lock (_lock)
            {
                // One reader can be active at a time.
                if (_activeReaderName == string.Empty)
                {
                    _activeReaderName = e.ReaderName;
                }
                else
                {
                    return;
                }
            }

            var readerContext = Readers[e.ReaderName];
            readerContext.CardContext = ContextFactory.Instance.Establish(SCardScope.System);
            readerContext.IsoReader = new IsoReader(
                readerContext.CardContext,
                e.ReaderName,
                SCardShareMode.Shared,
                SCardProtocol.Any,
                false);

////            var uid0 = ReadBinary(0, 0, 4);
    ////        var uid1 = ReadBinary(0, 1, 4);
        ///    if (uid0.Length != 0)
           /// {
              ///  var tagId = new byte[]
                ///{
                ////uid0[0], uid0[1], uid0[2], uid1[0], uid1[1], uid1[2], uid1[3]
                ///};
                ////var tagIdStr = BitConverter.ToString(tagId);
                ////Debug.WriteLine($"TAG ID: {tagIdStr}");
           /// }
            var tagAtrStr = BitConverter.ToString(e.Atr).Replace("-", " ");
            Debug.WriteLine($"TAG ATR: {tagAtrStr}");
            Debug.WriteLine($"TAG STATE: {e.State}");
        }

        private void OnCardRemoved(object sender, CardStatusEventArgs e)
        {
            var readerContext = Readers[e.ReaderName];

            if (readerContext.CardContext != null)
            {
                readerContext.IsoReader.Dispose();
                readerContext.IsoReader = null;

                readerContext.CardContext.Cancel();
                readerContext.CardContext.Dispose();
                readerContext.CardContext = null;
            }

            lock (_lock)
            {
                _activeReaderName = string.Empty;
            }
        }

        private void UpdateReaders(List<string> newReaderNames)
        {
            var currentReaderNames = Readers.Select(reader => reader.Key);
            var deleteReaderNames = currentReaderNames.Except(newReaderNames).ToList();
            foreach (var deleteReaderName in deleteReaderNames)
            {
                var readerContext = Readers[deleteReaderName];

                if (readerContext.CardContext != null)
                {
                    readerContext.IsoReader.Dispose();
                    readerContext.CardContext.Cancel();
                    readerContext.CardContext.Dispose();
                }

                if (readerContext.CardMonitor != null)
                {
                    readerContext.CardMonitor.Cancel();
                    readerContext.CardMonitor.CardInserted -= OnCardInserted;
                    readerContext.CardMonitor.CardRemoved -= OnCardRemoved;
                    readerContext.CardMonitor.Dispose();
                }

                Readers.Remove(deleteReaderName);
            }

            var addReaderNames = newReaderNames.Except(Readers.Select(reader => reader.Key)).ToList();
            foreach (var addReaderName in addReaderNames)
            {
                var readerContext = new ReaderContext 
                {
                    CardMonitor = MonitorFactory.Instance.Create(SCardScope.System)
                };
                readerContext.CardMonitor.CardInserted += OnCardInserted;
                readerContext.CardMonitor.CardRemoved += OnCardRemoved;
                readerContext.CardMonitor.Start(addReaderName);
                Readers.Add(addReaderName, readerContext);
            }
        }

        byte[] ReadBinary(byte msb, byte lsb, int size)
        {
            var readerContext = Readers[_activeReaderName];

////            unchecked
            {
                var readBinaryCmd = new CommandApdu(IsoCase.Case2Short, SCardProtocol.Any)
                {
                    CLA = CustomCla,
                    Instruction = InstructionCode.ReadBinary,
                    P1 = msb,
                    P2 = lsb,
                    Le = size
                };

                //Debug.WriteLine($"Read Binary: {BitConverter.ToString(readBinaryCmd.ToArray())}");
                var response = readerContext.IsoReader.Transmit(readBinaryCmd);
                var data = response.GetData();
                Debug.WriteLine($"SW1 SW2 = {response.SW1:X2} {response.SW2:X2}");
                if (data != null)
                {
                    Debug.WriteLine($"Data: {BitConverter.ToString(data)}");
                }

                if ((response.SW1 == (byte)SW1Code.Normal) && (response.SW2 == 0x00))
                    return response.GetData();
                else
                    ////       throw new Exception("Read binary failed");
                    return new byte[] { };
            }
        }
#endif

    }
}
