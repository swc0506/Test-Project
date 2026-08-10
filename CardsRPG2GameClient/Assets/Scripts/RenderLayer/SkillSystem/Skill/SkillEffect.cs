using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillEffect : MonoBehaviour
{
    public void SetEffectPos(VInt3 logicPosition, float destroyTime = 3000)
    {
        transform.position = logicPosition.vec3;
        Destroy(gameObject, destroyTime / 1000f);
    }

    public void SetScale(Vector3 scale)
    {
        transform.localScale = scale;
    }
}
