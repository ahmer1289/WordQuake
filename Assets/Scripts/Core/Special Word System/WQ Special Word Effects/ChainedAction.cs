using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainedAction : WQSpecialWordEffect
{
    Animator animator;
    Rigidbody2D rigidBody;
    [SerializeField] GameObject m_PhysicsObject;

    public override void Init()
    {
        base.Init();
        
        animator = GetComponent<Animator>();
        rigidBody = m_PhysicsObject.GetComponent<Rigidbody2D>();

        gameObject.SetActive(true);
        animator.Play("Hit Puck");
    }

    public override void TriggerFromAnimation()
    {
        base.TriggerFromAnimation();
        ThrowFromCustomPosition(rigidBody, m_PhysicsObject.transform.position);
    }
}
