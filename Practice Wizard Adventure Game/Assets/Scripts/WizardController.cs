using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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

    public int maxMana = 5;

    public int Mana { get { return currentMana; } }
    int currentMana;

    public GameObject fireballPrefab;

    public float delayTime = 0;

    private bool isWaiting = false;

    private bool canLaunch = false;
    private bool canFire = false;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        dustEffect.Stop();

        currentHealth = maxHealth;
        currentMana = maxMana;
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
                //animator.SetBool("Launch", true);
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

        if (Input.GetKeyDown(KeyCode.J))
        {
            if (canFire)
            {
                Fire();
            }
        }

        if (stateInfo.IsName("Fire") && stateInfo.normalizedTime >= 1.0f)
        {
            animator.SetBool("Fire", false);
        }

        if (stateInfo.IsName("FireL") && stateInfo.normalizedTime >= 1.0f)
        {
            animator.SetBool("Fire", false);
        }

        float regenRate = 1f;
        if (currentMana < maxMana)
        {
            currentMana += (int)(regenRate * Time.deltaTime);
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

    public void ChangeMana(int amount)
    {
        currentMana = Mathf.Clamp(currentMana + amount, 0, maxHealth);

        UIManaBar.instance.SetValue(currentMana / (float)maxMana);
    }

    public void Transparent() {
        myCollider.enabled = false;
    }

    void Launch()
    {
        if (currentMana >= 3)
        {
            ChangeMana(-3);
            GameObject fireballObject = Instantiate(fireballPrefab, rigidbody2d.position, Quaternion.identity);

            Fireball projectile = fireballObject.GetComponent<Fireball>();
            projectile.Launch(lookDirection, 300);

            animator.SetTrigger("Launch");

            isWaiting = false;
        }
    }

    void Fire()
    {
        if (currentMana >= 2)
        {
            ChangeMana(-2);
            animator.SetBool("Fire", true);
        }
    }

    public void getLaunch()
    {
        canLaunch = true;
    }

    public void getFire()
    {
        canFire = true;
    }
}