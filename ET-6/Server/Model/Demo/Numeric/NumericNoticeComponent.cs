using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    [ComponentOf(typeof(Unit))]
    public class NumericNoticeComponent : Entity, IAwake
    {
        public M2C_NoticeUnitNumeric m2C_NoticeUnitNumericMessage = new M2C_NoticeUnitNumeric();
    }
}
