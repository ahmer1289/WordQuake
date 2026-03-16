using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : WQSpecialWordEffect
{
    public override void Init()
    {
        base.Init();

        rb = GetComponent<Rigidbody2D>();
            
        if(isPlayer1){
            Debug.Log($"blaaaaaaaaaaaa");
            opponentTargetPosition = wQPlayerSpawner.GetWQPlayerSpecialWordReceiver(2).GetTarget(SpecialWordEffectType.Throw);
            throwOriginPosition = wQPlayerSpawner.GetWQPlayerSpecialWordReceiver(1).GetTarget(SpecialWordEffectType.Throw);
            forceDirection = (opponentTargetPosition - throwOriginPosition).normalized;
        }
        
        else{

            opponentTargetPosition = wQPlayerSpawner.GetWQPlayerSpecialWordReceiver(1).GetTarget(SpecialWordEffectType.Throw);
            throwOriginPosition = wQPlayerSpawner.GetWQPlayerSpecialWordReceiver(2).GetTarget(SpecialWordEffectType.Throw);
            forceDirection = (throwOriginPosition - opponentTargetPosition).normalized;
        }

        gameObject.SetActive(true);

        Throw();
    }
}