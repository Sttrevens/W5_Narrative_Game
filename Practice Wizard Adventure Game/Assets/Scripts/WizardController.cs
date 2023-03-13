using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardController : MonoBehaviour
{
    public float walkingspeed = 2.0f;

    public Collider2D myCollider;

    private Rigidbody2D rigidbody2d;
    private Animator animator;

    Vector2 lookDirection = new Vector2(1,0);

    public ParticleSystem dustEffect;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        dustEffect.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector2 position = transform.position;
        position.x = position.x + walkingspeed * horizontal * Time.deltaTime;
        position.y = position.y + walkingspeed * vertical * Time.deltaTime;
        transform.position = position;

        Vector2 move = new Vector2(horizontal, vertical);

        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        animator.SetFloat("Direction X", horizontal);
        if (horizontal != 0 || vertical != 0)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }

        //if (horizontal == 0 || vertical != 0)
        //{
        //    animator.SetBool("Moving Y", true);
        //}
        //else {
        //    animator.SetBool("Moving Y", false);
        //}

        if (Input.GetKey(KeyCode.LeftShift))
        {
            animator.SetBool("isRunning", true);
            if (stateInfo.IsName("RunL") || stateInfo.IsName("RunR"))
            {
                walkingspeed = 5.0f;
                dustEffect.Play();
            }
        }
        else {
            animator.SetBool("isRunning", false);
            if (!stateInfo.IsName("RunL") || !stateInfo.IsName("RunR"))
            {
                walkingspeed = 2.0f;
                dustEffect.Stop();
            }
        }
    }

    public void Transparent() {
        myCollider.enabled = false;
    }
}