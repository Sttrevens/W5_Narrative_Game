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

    public int maxHealth = 5;

    public int health { get { return currentHealth; } }
    int currentHealth;

    public GameObject fireballPrefab;

    public float delayTime = 0;

    private bool isWaiting = false;

    private bool canLaunch = false;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        dustEffect.Stop();

        currentHealth = maxHealth;
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
        //Debug.Log(lookDirection + " this is look direction.");

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

        if (Input.GetKeyDown(KeyCode.K))
        {
            if (canLaunch)
            {
                if (!isWaiting)
                {
                    isWaiting = true;
                    Invoke("Launch", delayTime);
                }
                animator.SetBool("Launch", true);
            }
        }

        if (stateInfo.IsName("Launch") && stateInfo.normalizedTime >= 1.0f)
        {
            animator.SetBool("Launch", false);
        }

        if (stateInfo.IsName("LaunchL") && stateInfo.normalizedTime >= 1.0f)
        {
            animator.SetBool("Launch", false);
        }
    }

    public void ChangeHealth(int amount)
    {
        //if (amount < 0)
        //{
        //    if (isInvincible)
        //        return;

        //    isInvincible = true;
        //    invincibleTimer = timeInvincible;
        //}

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);

        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
    }

    public void Transparent() {
        myCollider.enabled = false;
    }

    void Launch()
    {
        GameObject fireballObject = Instantiate(fireballPrefab, rigidbody2d.position, Quaternion.identity);

        Fireball projectile = fireballObject.GetComponent<Fireball>();
        projectile.Launch(lookDirection, 300);

        animator.SetTrigger("Launch");

        isWaiting = false;
    }

    public void getLaunch()
    {
        canLaunch = true;
    }
}