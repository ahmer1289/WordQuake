using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedBackground : MonoBehaviour
{
    private Animator animator; 

    void Start()
    {   
        animator = gameObject.GetComponent<Animator>();
        animator.Play("doorOpening"); 
    }
}
