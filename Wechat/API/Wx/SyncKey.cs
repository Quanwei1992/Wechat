using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wechat.API
{
    public class SyncItem {
        public int Key;
        public int Val;
    }
    public class SyncKey
    {
        public int Count;
        public SyncItem[] List;
    }
}
