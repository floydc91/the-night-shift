using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PenguinProductions
{
    public class FakeGargoyle : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            Animator animator = gameObject.GetComponent<Animator>();
            animator.SetBool("Statue1", true);
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
