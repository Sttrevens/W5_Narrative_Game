using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    Rigidbody2D rigidbody2d;

    private Animator animator;

    ArcherController archer;

    void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        StartCoroutine(Countdown(1.5f));

        archer = GameObject.FindObjectOfType<ArcherController>();
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
        if (direction.x > 0)
        { animator.SetBool("isRight", true); }
        if (direction.x < 0)
        { animator.SetBool("isRight", false); }

        rigidbody2d.AddForce(direction * force);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("Fireball Collision with " + other.gameObject);
        if (other.gameObject.CompareTag("Enemy"))
        {
            archer.Hit(-1);
        }
        Destroy(gameObject);
    }
}
