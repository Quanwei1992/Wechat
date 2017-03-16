using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wechat
{
    public class Group : Contact
    {

        public override string NickName
        {
            get {
                if (string.IsNullOrWhiteSpace(base.NickName)) {
                    if (Members != null && Members.Length > 0) {
                        int max = Members.Length > 4 ? 4 : Members.Length;
                        string newName = "";
                        for (int i = 0; i < max; i++) {
                            newName = newName + Members[i].NickName;
                            if (i != max - 1) newName += ",";
                        }
                        base.NickName = newName;
                    }
                }
                return base.NickName;
            }

            set
            {
                base.NickName = value;
            }
        }

        public Contact[] Members { get; set; }

        public Contact GetMember(string ID)
        {
            if (Members == null) return null;
            for (int i = 0; i < Members.Length; i++)
            {
                if (Members[i].ID == ID) return Members[i];
            }
            return null;
        }
    }
}
