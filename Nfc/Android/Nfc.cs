using Android.App;
using Android.Content;
using Android.Nfc;
using Android.OS;
using NdefLibrary.Ndef;
using System;
using System.Collections.Generic;
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

        public Nfc()
        {
            _mainActivity = Platform.CurrentActivity;

            // Determine _isResumed flag initial value by checking window focus.
            _isActivityResumed = Platform.CurrentActivity.HasWindowFocus;

            Platform.ActivityStateChanged += (s, e) =>
            {
                switch (e.State)
                {
                    case (Xamarin.Essentials.ActivityState)ActivityState.Resumed:
                        _isActivityResumed = true;
                        if (_isSessionEnabled)
                            EnableNfc();
                        break;
                    case (Xamarin.Essentials.ActivityState)ActivityState.Paused:
                        _isActivityResumed = false;
                        if (_isSessionEnabled)
                            DisableNfc();
                        break;

                    default:
                        break;
                }
            }; 

            NewIntentReceived += OnNewIntentReceived;
            AdapterStateChanged += OnAdapterStateChanged;
        }

        public event EventHandler<NfcTagDetectedEventArgs> TagDetected;
        public event EventHandler<EventArgs> SessionTimeout;


        public Task EnableSessionAsync()
        {
            _isSessionEnabled = true;

            if (_isActivityResumed)
            {
                EnableNfc();
            }

            return Task.CompletedTask;
        }
        public Task DisableSessionAsync()
        {
            _isSessionEnabled = false;

            if (_isActivityResumed)
            {
                DisableNfc();
            }

            return Task.CompletedTask;
        }
        

        public Task<List<NdefLibrary.Ndef.NdefRecord>> ReadNdefAsync()
        {
            throw new NotImplementedException();
        }

        public Task WriteNdefAsync(List<NdefLibrary.Ndef.NdefRecord> wrNdefRecords)
        {
            throw new NotImplementedException();
        }

        public Task<List<NdefLibrary.Ndef.NdefRecord>> WriteReadNdefAsync(List<NdefLibrary.Ndef.NdefRecord> wrNdefRecords)
        {
            throw new NotImplementedException();
        }

        private void OnAdapterStateChanged(object sender, int e)
        {

        }

        private void OnNewIntentReceived(object sender, Intent e)
        {
            switch (e.Action)
            {
                case NfcAdapter.ActionTagDiscovered:
                    System.Diagnostics.Debug.WriteLine("========> ACTION: BridgeEventAndroidNfcActionTagDiscovered");
                    break;

////                case NfcAdapter.ActionTechDiscovered:
    ////                System.Diagnostics.Debug.WriteLine("========> ACTION: BridgeEventAndroidNfcActionTechDiscovered");
        ////            break;

                case NfcAdapter.ActionNdefDiscovered:
                    System.Diagnostics.Debug.WriteLine("========> ACTION: BridgeEventAndroidNfcActionNdefDiscovered");
                    break;

                default:
                    System.Diagnostics.Debug.WriteLine("========> ACTION: UNHANDLED");
                    return;
            }

            var tag = e.GetParcelableExtra(NfcAdapter.ExtraTag) as Tag;

            var id = tag.GetId();
            var techList = tag.GetTechList();
        }

        private void EnableNfc()
        {
            if (_isNfcEnabled)
            {
                return;
            }

            var adapter = NfcAdapter.GetDefaultAdapter(_mainActivity);
            if (adapter != null)
            {
                // Filter out NDEF or TagDiscovered intents.
                // Android prioritize the filters in the following order:
                //      - ActionNdefDiscovered
                //      - ActionTechDiscovered
                //      - ActionTagDiscovered
                var pendingIntent = PendingIntent.GetActivity(
                    _mainActivity,
                    0,
                    new Intent(
                        _mainActivity,
                        _mainActivity.GetType()).AddFlags(ActivityFlags.SingleTop),
                    0);

                var ndefFilter = new IntentFilter(NfcAdapter.ActionNdefDiscovered);
                ndefFilter.AddDataType("*/*");

                var tagFilter = new IntentFilter(NfcAdapter.ActionTagDiscovered);
                tagFilter.AddCategory(Intent.CategoryDefault);

                var filters = new IntentFilter[]
                {
                    ndefFilter,
                    tagFilter,
                };

                //var techListArray = new String[][]
                //{
                //new String[] {typeof(Android.Nfc.Tech.Ndef).Name},
                //};

                adapter.EnableForegroundDispatch(
                    _mainActivity,
                    pendingIntent,
                    filters,
                    null);// techListArray);
            }



            _isNfcEnabled = true;
        }

        private void DisableNfc()
        {
            if (!_isNfcEnabled)
            {
                return;
            }

            var adapter = NfcAdapter.GetDefaultAdapter(_mainActivity);
            if (adapter != null)
            {
                adapter.DisableForegroundDispatch(_mainActivity);
            }

            _isNfcEnabled = false;
        }


        internal enum ActivityState
        {
            Resumed,
            Paused
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

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        private static event EventHandler<int> AdapterStateChanged;

        [BroadcastReceiver]
        public class AdapterStateActionBroadcastReceiver : BroadcastReceiver
        {
            public override void OnReceive(Context context, Intent intent)
            {
                try
                {
                    var adapterState = intent.Extras.GetInt(Android.Nfc.NfcAdapter.ExtraAdapterState);
                    AdapterStateChanged?.Invoke(null, adapterState);

                    //if (adapterState == Android.Nfc.NfcAdapter.StateOn)
                    //{
                    //}
                    //else if (adapterState == Android.Nfc.NfcAdapter.StateOff)
                    //{

                    //}
                }
                catch { }
            }
        }
    }
}
