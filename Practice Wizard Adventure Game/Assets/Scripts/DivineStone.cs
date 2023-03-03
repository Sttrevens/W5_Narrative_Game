using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DivineStone : MonoBehaviour
{
    Animator animator;
    int i = 0;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        WizardController controller = other.gameObject.GetComponent<WizardController>();

        if (i == 1)
            return;

        if (controller != null)
        {
            animator.SetBool("ShineBool", true);
            controller.Transparent();
            i++;
        }
    }
}

