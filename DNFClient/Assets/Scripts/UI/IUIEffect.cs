using UnityEngine;

namespace UnityEngine.UI
{
    public enum EUIEffType
    {
        Particle,
        Billboard,
    }

    public enum TransformUnit
    {
        Translation = 0x01,
        Orientation = 0x02,
        Scale = 0x04,
    };

    public interface IUIEffect
    {
        EUIEffType GetEffectType();

        Vector3 GetPosition();

        void SetPosition(Vector3 pos);

        Vector3 GetRotation();

        void SetRotation(Vector3 rot);

        Vector3 GetScale();

        void SetScale(Vector3 scale);

        void GetTransform(out Vector3 pos, out Vector3 rot, out Vector3 scale);

        void SetTransform(uint unMask, Vector3 pos, Vector3 rot, Vector3 scale);
    }


}
