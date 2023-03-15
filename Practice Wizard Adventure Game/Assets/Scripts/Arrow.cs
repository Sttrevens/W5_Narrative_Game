using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    Rigidbody2D rigidbody2d;

    private Animator animator;

    WizardController wizard;
    void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        StartCoroutine(Countdown(1f));

        wizard = GameObject.FindObjectOfType<WizardController>();
    }

    IEnumerator Countdown(float seconds)
    {
        float timer = seconds;

        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

    public void Shoot(Vector2 direction, float force)
    {
        if (direction.x > 0)
        { animator.SetBool("isRight", true); }
        if (direction.x < 0)
        { animator.SetBool("isRight", false); }

        rigidbody2d.AddForce(direction * force);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Arrow Collision with " + other.gameObject);
        if (other.gameObject.name == "Wizard")
        {
            wizard.ChangeHealth(-10);
        }
        Destroy(gameObject);
    }
}
