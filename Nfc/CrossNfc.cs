using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Xamarinme
{
    public static class CrossNfc
    {
        private static Lazy<INfc> nfc = new Lazy<INfc>( () => CreateNfc() );

        public static INfc Current 
        {
            get
            {
                INfc ret = nfc.Value;
                if (ret == null)
                {
                    throw new NotImplementedException("This platform has no Nfc implementation.");
                }
                return ret;
            }
        }

        private static INfc CreateNfc()
        {
#if NETSTANDARD2_0
            return null;
#else
            return new Nfc();
#endif
        }
    }
}
