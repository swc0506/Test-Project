using System;

namespace Core
{
    public interface IStageBehaviour
    {
        Stage Context { get; }

        void Initial(Stage context, IStageBehaviour inst);

        void Start();

        void Stop();

        void Dispose();
    }
}