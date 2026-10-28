using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    public partial class PlayerNumericConfigCategory : ProtoObject, IMerge
    {
        public int GetShowConfigCount()
        {
            int count = 0;
            foreach (PlayerNumericConfig config in this.dict.Values)
            {
                if(config.isNeedShow == 1)
                {
                    count++;
                }
            }
            return count;
        }

        public PlayerNumericConfig GetConfigByIndex(int index)
        {
            int id = index + 1011;

            return Get(id);
        }
    }
}
