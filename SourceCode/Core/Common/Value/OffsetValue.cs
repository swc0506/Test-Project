using System;
using UnityEngine;

namespace Core
{
    public class AbstractOffsetValue
    {
        /// <summary>
        /// 基础值
        /// </summary>
        public int BaseVal { get; private set; }

        /// <summary>
        /// 偏移值
        /// </summary>
        public int OffsetVal { get; private set; }

        /// <summary>
        /// 百分比值
        /// </summary>
        public int PercentVal { get; private set; }

        //百分比单位
        private int percentUnit = 100;

        private bool hasClamp;
        private int min;
        private int max;

        public void Initial(int baseVal, int offsetVal, int percentVal)
        {
            this.BaseVal = baseVal;
            this.OffsetVal = offsetVal;
            this.PercentVal = percentVal;
        }

        public void Initial(int baseVal, int offsetVal)
        {
            Initial(baseVal, offsetVal, 0);
        }

        public void Initial(int baseVal)
        {
            Initial(baseVal, 0);
        }

        public void SetPercentUnit(int percentUnit)
        {
            this.percentUnit = percentUnit;
        }

        public void SetClamp(int min, int max)
        {
            this.min = min;
            this.max = max;
            this.hasClamp = true;
        }

        public virtual void DeltaOffset(int val)
        {
            OffsetVal += val;
        }

        public virtual void DeltaPercent(int val)
        {
            PercentVal += val;
        }

        private int ClampValue(int val)
        {
            if (hasClamp)
            {
                val = Mathf.Clamp(val, min, max);
            }

            return val;
        }

        public int Value
        {
            get
            {
                int val = BaseVal + OffsetVal + Mathf.RoundToInt(PercentVal / (float)percentUnit);
                return ClampValue(val);
            }
        }

        public void Clear()
        {
            BaseVal = 0;
            OffsetVal = 0;
            PercentVal = 0;

            min = 0;
            max = 0;
            hasClamp = false;
        }
    }

    public class OffsetValue : AbstractOffsetValue
    {
        private Action<OffsetValue> changedEvent;

        /// <summary>
        /// 当值发生变化时候的事件
        /// </summary>
        public event Action<OffsetValue> ChangedEvent
        {
            add { changedEvent += value; }
            remove { changedEvent -= value; }
        }


        public override void DeltaOffset(int val)
        {
            base.DeltaOffset(val);
            changedEvent?.Invoke(this);
        }

        public override void DeltaPercent(int val)
        {
            base.DeltaPercent(val);
            changedEvent?.Invoke(this);
        }
    }

    public class OffsetValue<U> : AbstractOffsetValue
    {
        public U UserData { get; private set; }

        private Action<OffsetValue<U>> changedEvent;

        public void SetUserData(U userData)
        {
            this.UserData = userData;
        }

        /// <summary>
        /// 当值发生变化时候的事件
        /// </summary>
        public event Action<OffsetValue<U>> ChangedEvent
        {
            add { changedEvent += value; }
            remove { changedEvent -= value; }
        }

        public override void DeltaOffset(int val)
        {
            base.DeltaOffset(val);
            changedEvent?.Invoke(this);
        }

        public override void DeltaPercent(int val)
        {
            base.DeltaPercent(val);
            changedEvent?.Invoke(this);
        }
    }
}