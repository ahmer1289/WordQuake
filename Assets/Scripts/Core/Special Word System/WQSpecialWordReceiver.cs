using UnityEngine;

public enum SpecialWordEffectType
{
    Throw,
    Drop
}

public class WQSpecialWordReceiver : MonoBehaviour
{
    public Transform throwTargetLR;
    public Transform dropTarget;

    public Vector2 GetTarget(SpecialWordEffectType effectType)
    {

        Transform target = throwTargetLR;

        switch (effectType)
        {
            case SpecialWordEffectType.Throw:
                target = throwTargetLR;
            break;

            case SpecialWordEffectType.Drop:
                target = dropTarget;
            break;
        }
        
        return target.position;
    }
}
