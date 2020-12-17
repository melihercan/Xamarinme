using Android.App;
using Android.Content;
using Android.Nfc;
using Android.OS;
using NdefLibrary.Ndef;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Xamarinme
{
    public class Nfc : INfc
    {
        private Activity _mainActivity;
        private bool _isSessionEnabled;
        private bool _isResumed;
        private bool _isEnabled;


        public Nfc()
        {
            ActivityStateChanged += Nfc_ActivityStateChanged;
            NewIntentReceived += Nfc_NewIntentReceived;
            AdapterStateChanged += Nfc_AdapterStateChanged;
        }

        public event EventHandler<NfcTagDetectedEventArgs> TagDetected;
        public event EventHandler<EventArgs> SessionTimeout;




        //public async Task<NfcServiceStatus> InitAsync()
        //{
        //    ///var activity =  Application. .Context as Activity;


        //    await Task.Delay(100);

        //    return NfcServiceStatus.Available;
         
        //}
        //public async Task DeInitAsync()
        //{
        //    throw new NotImplementedException();
        //}

        public Task EnableSessionAsync()
        {
            _isSessionEnabled = true;

            if (_isResumed)
            {
                EnableNfc();
            }

            return Task.CompletedTask;
        }
        public Task DisableSessionAsync()
        {
            _isSessionEnabled = false;

            if (_isResumed)
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

        private void Nfc_AdapterStateChanged(object sender, int e)
        {

        }

        private void Nfc_ActivityStateChanged(object sender, ActivityState e)
        {
            //            _mainActivity ??= (Activity)sender;
            _mainActivity = (Activity)sender;

            switch(e)
            {
                case ActivityState.Resumed:
                    _isResumed = true;
                    if (_isSessionEnabled)
                    {
                        EnableNfc();
                    }
                    break;
                case ActivityState.Paused:
                    _isResumed = false;
                    if (_isSessionEnabled)
                    {
                        DisableNfc();
                    }
                    break;
            }
        }

        private void Nfc_NewIntentReceived(object sender, Intent e)
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
            if (_isEnabled)
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



            _isEnabled = true;
        }

        private void DisableNfc()
        {
            if (!_isEnabled)
            {
                return;
            }

            var adapter = NfcAdapter.GetDefaultAdapter(_mainActivity);
            if (adapter != null)
            {
                adapter.DisableForegroundDispatch(_mainActivity);
            }

            _isEnabled = false;
        }


        internal enum ActivityState
        {
            Resumed,
            Paused
        }

        public static void Init(Activity mainActivity) => 
            mainActivity.Application.RegisterActivityLifecycleCallbacks(new MainActivityLifecycleListener());

        private static event EventHandler<ActivityState> ActivityStateChanged;
        private class MainActivityLifecycleListener : Java.Lang.Object, Application.IActivityLifecycleCallbacks
        {
            public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
            {
            }

            public void OnActivityDestroyed(Activity activity)
            {
            }

            public void OnActivityPaused(Activity activity)
            {
                ActivityStateChanged?.Invoke(activity, ActivityState.Paused);
            }

            public void OnActivityResumed(Activity activity)
            {
                ActivityStateChanged?.Invoke(activity, ActivityState.Resumed);
            }

            public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
            {
            }

            public void OnActivityStarted(Activity activity)
            {
            }

            public void OnActivityStopped(Activity activity)
            {
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
