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
    public class NumericWatcher_RefreshMainUI : INumericWatcher
    {
        public void Run(NumbericChange args)
        {
            args.Parent.ZoneScene().GetComponent<UIComponent>().GetDlgLogic<DlgMain>()?.Refresh();
        }
    }
}
