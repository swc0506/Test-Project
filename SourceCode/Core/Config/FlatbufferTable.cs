using System;
using System.Collections;
#if FLATBUFFERS
using System.Collections.Generic;
using Google.FlatBuffers;

#endif

namespace Core.Config
{
#if FLATBUFFERS
    public class FlatbufferTable : IEnumerable<KeyValuePair<int, IFlatbufferObject>>, IClearable
    {
        private Type unitType;
        public IFlatbufferObject cfg { get; private set; }

        private Dictionary<int, IFlatbufferObject> unitMap = new Dictionary<int, IFlatbufferObject>();


        public void Initial(Type type, Type unitType, byte[] bytes)
        {
            ByteBuffer bb = new ByteBuffer(bytes);
            Initial(type, unitType, bb);
        }

        public void Initial(Type type, Type unitType, ByteBuffer bb)
        {
            if (bb.Length == 0)
            {
                return;
            }

            this.unitType = unitType;
            cfg = (IFlatbufferObject)Activator.CreateInstance(type);
            cfg.__init(bb.GetInt(bb.Position) + bb.Position, bb);
            InitialUnits();
        }

        private void InitialUnits()
        {
            if (null == unitType)
            {
                return;
            }

            ByteBuffer bb = ((IFlatbufferObject)cfg).ByteBuffer;
            Table p = new Table(bb.GetInt(bb.Position) + bb.Position, bb);

            int length = p.__offset(4);
            length = length == 0 ? 0 : p.__vector_len(length);
            //valueslength
            if (length > 0)
            {
                for (int i = 0; i < length; i++)
                {
                    //unit
                    int offset = p.__offset(4);
                    if (offset == 0)
                    {
                        continue;
                    }

                    Table up = new Table(p.__indirect(p.__vector(offset) + i * 4), p.bb);
                    //id
                    int num = up.__offset(4);
                    num = num == 0 ? 0 : up.bb.GetInt(num + up.bb_pos);

                    IFlatbufferObject unit = (IFlatbufferObject)Activator.CreateInstance(unitType);
                    unit.__init(up.bb_pos, up.bb);
                    unitMap[num] = unit;
                }
            }
        }

        public bool TryGetConfig<T>(out T config) where T : IFlatbufferObject
        {
            if (null != cfg)
            {
                config = (T)cfg;
                return true;
            }

            config = default(T);
            return false;
        }

        public T? GetConfig<T>() where T : struct, IFlatbufferObject
        {
            if (TryGetConfig<T>(out T cfg))
            {
                return cfg;
            }

            return null;
        }

        public bool TryGetUnit<T>(int id, out T unit) where T : IFlatbufferObject
        {
            if (unitMap.TryGetValue(id, out IFlatbufferObject unitObj))
            {
                unit = (T)unitObj;
                return true;
            }

            unit = default(T);
            return false;
        }

        public T? GetUnit<T>(int id) where T : struct, IFlatbufferObject
        {
            if (TryGetUnit<T>(id, out T unit))
            {
                return unit;
            }

            return null;
        }


        public IEnumerator<KeyValuePair<int, IFlatbufferObject>> GetEnumerator()
        {
            return unitMap.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Clear()
        {
            unitMap.Clear();
        }
    }

#endif
}