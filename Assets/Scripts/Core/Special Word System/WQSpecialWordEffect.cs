using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WQSpecialWordEffect : MonoBehaviour
{
    public float m_Force = 5f;
    public bool isPlayer1 = true;
    public float m_LifeTime = 2f;
    public Vector2 opponentTargetPosition;
    public Vector2 throwOriginPosition;
    public Vector2 forceDirection;
    public Rigidbody2D rb;
    protected WQPlayerSpawner wQPlayerSpawner = WQPlayerSpawner.Instance;

    public virtual void Init(){
        
        Debug.Log($"Initialized WQSpecialWordEffect!");

        //SetDefaultPosition();
        gameObject.SetActive(false);

        DestroyEffect();
    }
    
    public virtual void TriggerFromAnimation(){

        Debug.Log($"Triggered WQSpecialWordEffect Animation Event!");
    }

    public bool CanDamagePlayer(int m_PlayerIndex){

        int playerIndexToDamage = isPlayer1 == true ? 2 : 1;
        if(playerIndexToDamage == m_PlayerIndex) return true;
        else return false;
    }

    public void Throw()
    {
        if (rb != null)
        {
            Debug.Log($"throwinggggg");

            float torque = m_Force * 0.75f;

            if(isPlayer1){

                rb.AddForce(forceDirection * m_Force, ForceMode2D.Impulse);
                //rb.AddTorque(torque, ForceMode2D.Impulse);
            }

            else {

                rb.AddForce(- forceDirection * m_Force, ForceMode2D.Impulse);
                //rb.AddTorque(- torque, ForceMode2D.Impulse);
            } 
        }
    }

    public void ThrowFromCustomPosition(Rigidbody2D rigidbody2D, Vector2 launchPosition)
    {
        rb = rigidbody2D;

        if(isPlayer1){

            opponentTargetPosition = wQPlayerSpawner.GetWQPlayerSpecialWordReceiver(2).GetTarget(SpecialWordEffectType.Throw);
            forceDirection = (opponentTargetPosition - launchPosition).normalized;
        }
        
        else{

            opponentTargetPosition = wQPlayerSpawner.GetWQPlayerSpecialWordReceiver(1).GetTarget(SpecialWordEffectType.Throw);
            forceDirection = (launchPosition - opponentTargetPosition).normalized;
        }

        Throw();
    }

    private void SetDefaultPosition(){

        if(isPlayer1){

            transform.SetParent(wQPlayerSpawner.GetWQPlayerSpecialWordReceiver(1).throwTargetLR);
        }
        
        else{

            transform.SetParent(wQPlayerSpawner.GetWQPlayerSpecialWordReceiver(2).throwTargetLR);
        }

        transform.position = Vector3.zero;
    }

    //todo: handle this properly. The game scene is freezing after each word.
    void DestroyEffect(){

        Destroy(gameObject, m_LifeTime + 0.001f);
    }
}
