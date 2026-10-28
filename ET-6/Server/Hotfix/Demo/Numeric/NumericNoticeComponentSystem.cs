using ET.EventType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    [FriendClass(typeof(NumericNoticeComponent))]
    public static class NumericNoticeComponentSystem
    {
        public static void NoticeImmediately(this NumericNoticeComponent self, NumbericChange args)
        {
            Unit unit = self.GetParent<Unit>();
            self.m2C_NoticeUnitNumericMessage.UnitId = unit.Id;
            self.m2C_NoticeUnitNumericMessage.NumericType = args.NumericType;
            self.m2C_NoticeUnitNumericMessage.NewValue = args.New;
            MessageHelper.SendToClient(unit, self.m2C_NoticeUnitNumericMessage);
        }
    }
}
