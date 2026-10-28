using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    public class C2M_TestUnitNumericHandler : AMActorLocationRpcHandler<Unit, C2M_TestUnitNumeric, M2C_TestUnitNumeric>
    {
        protected override async ETTask Run(Unit unit, C2M_TestUnitNumeric request, M2C_TestUnitNumeric response, Action reply)
        {
            NumericComponent numericComponent = unit.GetComponent<NumericComponent>();

            int newGold = numericComponent.GetAsInt(NumericType.Gold) + 100;
            long newExp = numericComponent.GetAsLong(NumericType.Exp) + 50;
            long level = numericComponent.GetAsLong(NumericType.Level) + 1;
            numericComponent.Set(NumericType.Gold, newGold);
            numericComponent.Set(NumericType.Level, level);
            numericComponent.Set(NumericType.Exp, newExp);

            reply();
            await ETTask.CompletedTask;
        }
    }
}
