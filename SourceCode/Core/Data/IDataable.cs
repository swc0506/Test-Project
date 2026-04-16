using System;

namespace Core.Data
{
    public interface IDataable
    {
        void Initial(IDataable data);
        void Clear();
        void Dispose();
    }
}