using ET.EventType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    [NumericWatcher(NumericType.Exp)]
    [NumericWatcher(NumericType.Gold)]
    [NumericWatcher(NumericType.Level)]
    public class NumericWatcher_MainUI_ShowUI : INumericWatcher
    {
        public void Run(NumbericChange args)
        {
            if (!(args.Parent is Unit unit))
            {
                return;
            }
            unit.GetComponent<NumericNoticeComponent>()?.NoticeImmediately(args);
            //MessageHelper.SendToClient(unit, new M2C_NoticeUnitNumeric()
            //{
            //    UnitId = unit.Id,
            //    NumericType = args.NumericType,
            //    NewValue = args.New,
            //});
        }
    }
}
