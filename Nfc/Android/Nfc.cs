using Android.App;
using Android.Content;
using Android.OS;
using NdefLibrary.Ndef;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Xamarinme
{
    public class Nfc : INfc
    {
        private readonly Activity _mainActivity;
        private bool _isSessionEnabled;
        private bool _isActivityResumed;
        private bool _isNfcEnabled;
        private Android.Nfc.Tag _tag;

        public Nfc()
        {
            _mainActivity = Platform.CurrentActivity;

            // Determine _isResumed flag initial value by checking window focus.
            _isActivityResumed = Platform.CurrentActivity.HasWindowFocus;

            Platform.ActivityStateChanged += (s, e) =>
            {
                switch (e.State)
                {
                    case ActivityState.Resumed:
                        _isActivityResumed = true;
                        if (_isSessionEnabled)
                            EnableNfc();
                        break;
                    case ActivityState.Paused:
                        _isActivityResumed = false;
                        if (_isSessionEnabled)
                            DisableNfc();
                        break;

                    default:
                        break;
                }
            }; 

            NewIntentReceived += OnNewIntentReceived;
        }

        public event EventHandler<NfcTagDetectedEventArgs> TagDetected;
        public event EventHandler<EventArgs> SessionTimeout;

        public Task EnableSessionAsync()
        {
            _isSessionEnabled = true;

            if (_isActivityResumed)
                EnableNfc();

            return Task.CompletedTask;
        }

        public Task DisableSessionAsync()
        {
            _isSessionEnabled = false;

            if (_isActivityResumed)
                DisableNfc();

            return Task.CompletedTask;
        }

        public Task<NdefMessage> ReadNdefAsync()
        {
            var ndef = Android.Nfc.Tech.Ndef.Get(_tag);

            try
            {
                ndef.Connect();
                var ndefMessage = NdefMessage.FromByteArray(ndef.NdefMessage.ToByteArray());
                ndef.Close();

                return Task.FromResult(ndefMessage);
            }
            catch (Exception ex)
            {
                if (ndef.IsConnected)
                    ndef.Close();
                throw ex;
            }
        }

        public async Task WriteNdefAsync(NdefMessage ndefMessage)
        {
            var ndef = Android.Nfc.Tech.Ndef.Get(_tag);

            try
            {
                ndef.Connect();
                await ndef.WriteNdefMessageAsync(new Android.Nfc.NdefMessage(ndefMessage.ToByteArray()));
                ndef.Close();
            }
            catch (Exception ex)
            {
                if (ndef.IsConnected)
                    ndef.Close();
                throw ex;
            }
        }

        public async Task<NdefMessage> WriteReadNdefAsync(NdefMessage ndefMessage)
        {
            await WriteNdefAsync(ndefMessage);

            // Compare the write content to read content to make sure that device responded.
            // Try this few times with timeout before generating error.
            for (int i=0; i<5; i++)
            {
                await Task.Delay(70);
                var rdNdefMessage = await ReadNdefAsync();
                if (!rdNdefMessage.ToByteArray().SequenceEqual(ndefMessage.ToByteArray()))
                    return rdNdefMessage;
            }

            throw new Exception("WriteReadNdefAsync failed!!!");
        }

        private void EnableNfc()
        {
            if (_isNfcEnabled)
                return;

            var adapter = Android.Nfc.NfcAdapter.GetDefaultAdapter(_mainActivity);
            if (adapter != null)
            {
                // Filter out NDEF intents. In this library we are not interested in Tech or Tag low level intents.
                // Android prioritize the filters in the following order:
                //      - ActionNdefDiscovered
                //      - ActionTechDiscovered
                //      - ActionTagDiscovered
                var pendingIntent = PendingIntent.GetActivity(
                    _mainActivity,
                    0,
                    new Intent(_mainActivity, _mainActivity.GetType()).AddFlags(ActivityFlags.SingleTop),
                    0);

                var ndefFilter = new IntentFilter(Android.Nfc.NfcAdapter.ActionNdefDiscovered);
                ndefFilter.AddDataType("*/*");

                var filters = new IntentFilter[]
                {
                    ndefFilter,
                };

                adapter.EnableForegroundDispatch(
                    _mainActivity,
                    pendingIntent,
                    filters,
                    null);
            }

            _isNfcEnabled = true;
            System.Diagnostics.Debug.WriteLine("========> NFC Enabled");
        }

        private void DisableNfc()
        {
            if (!_isNfcEnabled)
                return;

            var adapter = Android.Nfc.NfcAdapter.GetDefaultAdapter(_mainActivity);
            if (adapter != null)
                adapter.DisableForegroundDispatch(_mainActivity);

            _isNfcEnabled = false;
            System.Diagnostics.Debug.WriteLine("========> NFC DISABLED");
        }

        public void Dispose()
        {
        }

        private void OnNewIntentReceived(object sender, Intent e)
        {
            if (e.Action == Android.Nfc.NfcAdapter.ActionNdefDiscovered)
            {
                _tag = e.GetParcelableExtra(Android.Nfc.NfcAdapter.ExtraTag) as Android.Nfc.Tag;
                var ndefMessage = NdefMessage.FromByteArray(Android.Nfc.Tech.Ndef.Get(_tag).CachedNdefMessage
                    .ToByteArray());
                TagDetected?.Invoke(this, new NfcTagDetectedEventArgs(
                    BitConverter.ToString(_tag.GetId()).Replace("-", ":"),
                    ndefMessage)); 
            }
        }

        private static event EventHandler<Intent> NewIntentReceived;
        public static void OnNewIntent(Intent intent)
        {
            try
            {
                NewIntentReceived?.Invoke(null, intent);
            }
            catch { }
        }

    }
}
