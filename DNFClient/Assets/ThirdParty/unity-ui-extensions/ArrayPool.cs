using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZM.AssetFrameWork;

public class ArrayPool<T> : Singleton<ArrayPool<T>>
{
    Dictionary<int, Stack<T[]>> m_ArrayPoolTbl = new Dictionary<int, Stack<T[]>>();

    public T[] AllocateArray(int len)
    {
        T[] resArray = null;
        Stack<T[]> arrayStack = null;
        if (m_ArrayPoolTbl.TryGetValue(len, out arrayStack))
            resArray = arrayStack.Pop();

        if (null == resArray)
            resArray = new T[len];

        return resArray;
    }

    public void ReleaseArray(T[] array)
    {
        if(null != array)
        {
            Stack<T[]> arrayStack = null;
            if (!m_ArrayPoolTbl.TryGetValue(array.Length, out arrayStack))
            {
                arrayStack = new Stack<T[]>();
                m_ArrayPoolTbl.Add(array.Length, arrayStack);
            }

            arrayStack.Push(array);
        }
    }
}
