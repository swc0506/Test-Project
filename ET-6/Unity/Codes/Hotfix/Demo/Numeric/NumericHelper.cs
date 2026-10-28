using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    public static class NumericHelper
    {
        public static async ETTask<int> TestUpdateNumeric(Scene zoneScene)
        {
            M2C_TestUnitNumeric m2C_TestUnitNumeric = null;
            try
            {
                m2C_TestUnitNumeric = (M2C_TestUnitNumeric)await zoneScene.GetComponent<SessionComponent>().Session.Call(new C2M_TestUnitNumeric() { });
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return ErrorCode.ERR_NetworkError;
            }

            if (m2C_TestUnitNumeric.Error != ErrorCode.ERR_Success)
            {
                Log.Error(m2C_TestUnitNumeric.Error.ToString());
                return m2C_TestUnitNumeric.Error;
            }


            return ErrorCode.ERR_Success;
        }

        public static async ETTask<int> ReqeustAddAttributePoint(Scene zoneScene, int numericType)
        {
            M2C_AddAttributePoint m2C_AddAttributePoint = null;
            try
            {
                m2C_AddAttributePoint = (M2C_AddAttributePoint)await zoneScene.GetComponent<SessionComponent>().Session.Call(new C2M_AddAttributePoint() { NumericType = numericType });
                if (m2C_AddAttributePoint.Error == ErrorCode.ERR_Success)
                {
                    NumericComponent numericComponent = UnitHelper.GetMyUnitFromCurrentScene(zoneScene.CurrentScene()).GetComponent<NumericComponent>();

                    int AttributePointCount = numericComponent.GetAsInt(NumericType.AttributePoint);
                    --AttributePointCount;
                    numericComponent.Set(NumericType.AttributePoint, AttributePointCount);

                    int targetAttributeCount = numericComponent.GetAsInt(numericType) + 1;
                    numericComponent.Set(numericType, targetAttributeCount);
                }
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                return ErrorCode.ERR_NetworkError;
            }

            if (m2C_AddAttributePoint.Error != ErrorCode.ERR_Success)
            {
                Log.Error(m2C_AddAttributePoint.Error.ToString());
                return m2C_AddAttributePoint.Error;
            }
            return ErrorCode.ERR_Success;
        }
    }
}
