using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    Rigidbody2D rigidbody2d;

    private Animator animator;

    void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        StartCoroutine(Countdown(1.5f));
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

    public void Launch(Vector2 direction, float force)
    {
        if (direction.x == 1)
        { animator.SetBool("isRight", true); }
        if (direction.x == -1)
        { animator.SetBool("isRight", false); }

        rigidbody2d.AddForce(direction * force);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("Projectile Collision with " + other.gameObject);
        Destroy(gameObject);
    }
}
