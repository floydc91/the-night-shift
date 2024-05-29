using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PenguinProductions
{
    public class AnimationManager : MonoBehaviour
    {

        Animator animator;

        void Start()
        {
            animator = GetComponent<Animator>();
        }

        
        void Update()
        {
            animator.SetBool("isMoving", true);
            animator.SetBool("isAttacking", true);
            animator.SetBool("isAttacking", false);
        }
    }
}
