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
using PCSC.Reactive.Events;
using System.Reactive.Linq;
using System.Threading;

namespace XamarinmeNfc.Shared
{
    internal class Pcsc : INfc
    {
        private bool _isSessionEnabled;
        private SemaphoreSlim _lockSemaphore = new SemaphoreSlim(1);


        private bool _isDisposed = false;
        private IDisposable _deviceMonitorFactorySubscription;
        private IDisposable _monitorFactorySubscription;
        private string _reader; // One active reader at a time. 

        const byte CustomCla = 0xFF;
        const byte NdefTlv = 0x03;

        private byte _ndefTlvPage;    // NDEF page on NFC memory (page 4 to 0x84)


        public Pcsc()
        {
            StartMonitoring();
        }

        private void StartMonitoring()
        {
            // Start readers monitoring.
            _deviceMonitorFactorySubscription = DeviceMonitorFactory.Instance
                .CreateObservable(SCardScope.System)
                .Subscribe(
                    onNext: deviceMonitorEvent =>
                    {
                        var readers = deviceMonitorEvent.Readers;
                        Debug.WriteLine($"---> {deviceMonitorEvent.GetType().Name} " +
                            $"- readers: {string.Join(", ", readers)}");

                        switch (deviceMonitorEvent)
                        {
                            case DeviceMonitorInitialized deviceMonitorInitialized:
                                break;

                            case ReadersAttached readersAttached:
                                _monitorFactorySubscription?.Dispose();
                                _monitorFactorySubscription = null;
                                break;

                            case ReadersDetached readersDetached:
                                if (_reader != null && !readers.Contains(_reader))
                                {
                                    // Active reader has been removed. Abort anything ongoing.
                                    _reader = null;
                                }
                                _monitorFactorySubscription?.Dispose();
                                _monitorFactorySubscription = null;
                                break;

                            default:
                                return;
                        }

                        // Start card monitoring if there are readers.
                        if (readers.Count() != 0)
                        {
                            _monitorFactorySubscription = MonitorFactory.Instance
                                .CreateObservable(SCardScope.System, readers)
                                .Subscribe(
                                    onNext: monitorEvent =>
                                    {
                                        var reader = monitorEvent.ReaderName;
                                        Debug.WriteLine($"---> {monitorEvent.GetType().Name} " +
                                            $"- reader: {reader}");

                                        switch (monitorEvent)
                                        {
                                            case CardInserted inserted:
                                                if (_isSessionEnabled)
                                                {
                                                    _reader = reader;
                                                    TagDetected?.Invoke(this, new NfcTagDetectedEventArgs(
                                                        tagId: GetTagId(),
                                                        ndefMessage: ReadNdef()));
                                                }

                                                break;

                                            case CardRemoved removed:
                                                _reader = null;
                                                break;

                                            case MonitorInitialized initialized:
                                            case MonitorCardInfoEvent infoEvent:
                                            case CardStatusChanged statusChanged:
                                            default:
                                                return;
                                        }

                                    },
                                    onError: ex =>
                                    {
                                        Debug.WriteLine($"Card Error: {ex.Message} _reader: {_reader}");
                                        _reader = null;
                                    });
                        }
                    },
                    onError: ex =>
                    {
                        Debug.WriteLine($"Reader Error: {ex.Message} _reader: {_reader}");
                        _reader = null;
                        Task.Run(() => StartMonitoring());
                    });
        }

        private void StopMonitoring()
        {
            _deviceMonitorFactorySubscription?.Dispose();
            _monitorFactorySubscription?.Dispose();
        }

        public event EventHandler<NfcTagDetectedEventArgs> TagDetected;
        public event EventHandler<EventArgs> SessionTimeout;

        public async Task EnableSessionAsync()
        {
            try
            {
                await _lockSemaphore.WaitAsync();

                if (_isSessionEnabled)
                    return;



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



                _isSessionEnabled = false;
                System.Diagnostics.Debug.WriteLine("========> Session disabled");
            }
            finally
            {
                _lockSemaphore.Release();
            }
        }

        public Task<NdefMessage> ReadNdefAsync()
        {
            return Task.FromResult(ReadNdef());
        }

        public async Task WriteNdefAsync(NdefMessage ndefMessage)
        {
            try
            {
                await _lockSemaphore.WaitAsync();

                if (!_isSessionEnabled)
                    throw new Exception("NFC is not enabled");


                // Start writing NDEF TLV starting from _ndefTlvPage.
                // First reset TLV length.
                byte[] tlvPage = { NdefTlv, 0, 0, 0 };
                UpdateBinary(0, _ndefTlvPage, tlvPage);

                // Write NDEF data (TLV payload).
                int ndefTlvLen = ndefMessage.ToByteArray().Length;
                int numTotalPages = 0;
                int payloadOffset = 0;

                if (ndefTlvLen >= 0xFF)
                {
                    // Calculate number of total pages to write.
                    numTotalPages = ndefTlvLen / 4 + (ndefTlvLen % 4 == 0 ? 0 : 1) + 1;

                    // Payload offset.
                    payloadOffset = 4;
                }
                else
                {
                    // Calculate number of total pages to write.
                    numTotalPages = ndefTlvLen / 4 + (ndefTlvLen % 4 < 3 ? 0 : 1) + 1;

                    // Payload offset.
                    payloadOffset = 2;
                }

                if (numTotalPages > 0x84)
                    throw new Exception();

                byte[] command = new byte[numTotalPages * 4];
                Buffer.BlockCopy(tlvPage, 0, command, 0, 4);
                Buffer.BlockCopy(ndefMessage.ToByteArray(), 0, command, payloadOffset, ndefTlvLen);

                byte[] page = new byte[4];
                int idx = 0;
                for (byte i = _ndefTlvPage; i < _ndefTlvPage + numTotalPages; i++)
                {
                    Buffer.BlockCopy(command, idx, page, 0, 4);
                    idx += 4;
                    UpdateBinary(0, i, page);
                }

                // Calculate TLV total length.
                int ndefTlvTotalLen = 0;
                if (payloadOffset == 4)
                    ndefTlvTotalLen = 4 + ndefTlvLen;
                else
                    ndefTlvTotalLen = 2 + ndefTlvLen;

                Debug.WriteLine($"{DateTime.Now.TimeOfDay}" + $"####WRITE:" +
                    BitConverter.ToString(ndefMessage.ToByteArray()).Replace("-", string.Empty));

                // Add TLV terminator.
                byte tlvTerminatePage = 0;
                byte[] tlvTerminate = new byte[4];
                if (ndefTlvTotalLen % 4 == 0)
                {
                    tlvTerminatePage = (byte)(_ndefTlvPage + numTotalPages);
                    tlvTerminate[0] = 0xFE;
                }
                else
                {
                    tlvTerminatePage = (byte)(_ndefTlvPage + numTotalPages - 1);
                    Buffer.BlockCopy(command, (numTotalPages - 1) * 4, tlvTerminate, 0, 4);
                    tlvTerminate[ndefTlvTotalLen % 4] = 0xFE;
                }
                UpdateBinary(0, tlvTerminatePage, tlvTerminate);

                // Update TLV length.
                // Should be the last write.
                Buffer.BlockCopy(command, 0, tlvPage, 0, 4);
                if (payloadOffset == 4)
                {
                    tlvPage[1] = 0xFF;
                    tlvPage[2] = (byte)(ndefTlvLen >> 8);
                    tlvPage[3] = (byte)(ndefTlvLen);
                }
                else
                {
                    tlvPage[1] = (byte)(ndefTlvLen);
                }
                UpdateBinary(0, _ndefTlvPage, tlvPage);


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
                await Task.Delay(60);
                var rdNdefMessage = await ReadNdefAsync();
                if (!rdNdefMessage.ToByteArray().SequenceEqual(ndefMessage.ToByteArray()))
                    return rdNdefMessage;
            }

            throw new Exception("WriteReadNdefAsync failed!!!");
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
                    StopMonitoring();
////                    _readersMonitor.Cancel();
    /////                _readersMonitor.StatusChanged -= OnReadersStatusChanged;
         /////           _readersMonitor.Dispose();
                }

                _isDisposed = true;
            }
        }


        byte[] ReadBinary(byte msb, byte lsb, int size)
        {
            var isoReader = new IsoReader(
                    ContextFactory.Instance.Establish(SCardScope.System),
                    _reader,
                    SCardShareMode.Shared,
                    SCardProtocol.Any,
                    false);

            var readBinaryCmd = new CommandApdu(IsoCase.Case2Short, SCardProtocol.Any)
            {
                CLA = CustomCla,
                Instruction = InstructionCode.ReadBinary,
                P1 = msb,
                P2 = lsb,
                Le = size
            };

            //Debug.WriteLine($"Read Binary: {BitConverter.ToString(readBinaryCmd.ToArray())}");
            var response = isoReader.Transmit(readBinaryCmd);
            //Debug.WriteLine($"SW1 SW2 = {response.SW1:X2} {response.SW2:X2} Data: {BitConverter.ToString(response.GetData())}");

            if ((response.SW1 == (byte)SW1Code.Normal) && (response.SW2 == 0x00))
                return response.GetData();
            else
                 throw new InvalidOperationException("Read binary failed");
        }

        public void UpdateBinary(byte msb, byte lsb, byte[] data)
        {
            var isoReader = new IsoReader(
                    ContextFactory.Instance.Establish(SCardScope.System),
                    _reader,
                    SCardShareMode.Shared,
                    SCardProtocol.Any,
                    false);


            var updateBinaryCmd = new CommandApdu(IsoCase.Case3Short, SCardProtocol.Any)
            {
                CLA = CustomCla,
                Instruction = InstructionCode.UpdateBinary,
                P1 = msb,
                P2 = lsb,
                Data = data
            };

            //Debug.WriteLine($"Update Binary: {BitConverter.ToString(updateBinaryCmd.ToArray())}");
            var response = isoReader.Transmit(updateBinaryCmd);
            //Debug.WriteLine($"SW1 SW2 = {response.SW1:X2} {response.SW2:X2}");

            if ((response.SW1 == (byte)SW1Code.Normal) && (response.SW2 == 0x00))
                return;
            else
                throw new InvalidOperationException("Update binary failed");
        }


        private NdefMessage ReadNdef()
        {
            try
            {
                byte[] response = new byte[] { };

                _lockSemaphore.Wait();

                if (!_isSessionEnabled)
                    throw new Exception("NFC is not enabled");


                // Type2 tags provide TLV data from page 0x04 to 0x84 (512 bytes). 
                // TLVs start on page boundaries always, hence length field is in the same page
                // regardless of short or long length.
                // Page 4 on there are TLVs. Get the NDEF one.
                // When last page of NDEF message read, it will create interrupt on device
                // that we read the NFC message.
                byte ndefTlvPage = 0;
                for (byte i = 4; i < 0x84; i++)
                {
                    byte[] page = ReadBinary(0, i, 4);
                    if (page[0] == NdefTlv)
                    {
                        ndefTlvPage = i;
                        response = page;
                        break;
                    }
                }
                // Check if we have good NDEF msg.
                if (ndefTlvPage == 0)
                    throw new Exception();

                _ndefTlvPage = ndefTlvPage;

                // Get length.
                int ndefTlvLen = response[1];
                int numPayloadPages = 0;
                int payloadOffset = 0;

                if (ndefTlvLen == 0)
                {
                    // Len 0 means the device is in the middle of updating the memory.
                    throw new Exception("Invalid NDEF data");
                }
                else if (ndefTlvLen == 0xFF)
                {
                    // Get length.
                    ndefTlvLen = response[2] << 8 | response[3];

                    // Calculate number of payload page still to be read.
                    numPayloadPages = ndefTlvLen / 4 + (ndefTlvLen % 4 == 0 ? 0 : 1);

                    // Payload offset.
                    payloadOffset = 4;
                }
                else
                {
                    // Calculate number of payload page still to be read.
                    numPayloadPages = ndefTlvLen / 4 + (ndefTlvLen % 4 < 3 ? 0 : 1);

                    // Payload offset.
                    payloadOffset = 2;
                }

                // Make sure we have enough room for payload.
                if (ndefTlvPage + numPayloadPages > 0x84)
                    throw new Exception();

                // Read payload pages.
                for (byte i = (byte)(ndefTlvPage + 1); i < ndefTlvPage + numPayloadPages + 1; i++)
                {
                    byte[] page = ReadBinary(0, i, 4);
                    response = Combine(response, page);
                }

                var ndef = new byte[ndefTlvLen];
                Buffer.BlockCopy(response, payloadOffset, ndef, 0, ndefTlvLen);
                Debug.WriteLine($"{DateTime.Now.TimeOfDay}" + $"#### READ:" + BitConverter.ToString(ndef).
                    Replace("-", string.Empty));

                return NdefMessage.FromByteArray(ndef);
            }
            finally
            {
                _lockSemaphore.Release();
            }

            byte[] Combine(byte[] a, byte[] b)
            {
                byte[] c = new byte[a.Length + b.Length];
                Buffer.BlockCopy(a, 0, c, 0, a.Length);
                Buffer.BlockCopy(b, 0, c, a.Length, b.Length);
                return c;
            }
        }

        private string GetTagId()
        {
            var uid0 = ReadBinary(0, 0, 4);
            var uid1 = ReadBinary(0, 1, 4);
            var tagId = new byte[]
            {
                uid0[0], uid0[1], uid0[2], uid1[0], uid1[1], uid1[2], uid1[3]
            };
            return BitConverter.ToString(tagId).Replace("-", string.Empty);
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
