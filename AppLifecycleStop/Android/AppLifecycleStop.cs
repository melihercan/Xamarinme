using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamarinme
{
    class AppLifecycleStop : IAppLifecycleStop
    {
        public void Register(Action onStop)
        {
            throw new NotImplementedException();
        }
    }
}
