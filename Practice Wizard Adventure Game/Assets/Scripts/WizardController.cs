using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WizardController : MonoBehaviour
{
    public float walkingspeed = 2.0f;

    public Collider2D myCollider;

    private Rigidbody2D wizardRigidbody2d;
    private Animator animator;

    Vector2 lookDirection = new Vector2(1,0);

    public ParticleSystem dustEffect;

    public int maxHealth = 50;

    public int health { get { return currentHealth; } }
    int currentHealth;

    public int maxMana = 5;

    public int Mana { get { return currentMana; } }
    int currentMana;

    public GameObject fireballPrefab;

    public float delayTime = 1;

    private bool isWaiting = false;

    private bool canLaunch = false;
    private bool canFire = false;

    public float healthRecoveryTime = 0;
    public float manaRecoveryTime = 0;

    public LayerMask enemyLayer;
    public float attackRange = 1.0f;
    public int damage = 10;

    private bool isAttacking = false;
    private float attackTimer = 0.0f;
    private bool canCombo = false;

    //private bool isRecoveringHealth = false;
    //private bool isRecoveringMana = false;

    // Start is called before the first frame update
    void Start()
    {
        wizardRigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        dustEffect.Stop();

        currentHealth = maxHealth;
        currentMana = maxMana;

        InvokeRepeating("recoverMana", 0.0f, 1.0f);
        InvokeRepeating("recoverHealth", 0.0f, 1.0f);

        Vector2 lookDirection = new Vector2(1, 0);
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
            lookDirection = move.normalized;
        }
        // 角色停止移动
        else
        {
            // 如果角色之前朝向右
            if (lookDirection.x > 0)
            {
                lookDirection.Set(1, 0);
            }
            // 如果角色之前朝向左
            else if (lookDirection.x < 0)
            {
                lookDirection.Set(-1, 0);
            }
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

        if (Input.GetKeyDown(KeyCode.L))
        {
            if (canLaunch)
            {
                if (!isWaiting)
                {
                    isWaiting = true;
                    animator.SetTrigger("Launch");
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

        if (Input.GetKeyDown(KeyCode.K))
        {
            if (canFire)
            {
                Fire();
            }
        }

        Vector3 lookDirection3D = new Vector3(lookDirection.x, lookDirection.y, 0);
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position + lookDirection3D, 1.5f);
        if (stateInfo.IsName("Fire") && stateInfo.normalizedTime >= 1.0f)
        {
            // 遍历敌人，造成伤害
            foreach (Collider2D enemy in hitEnemies)
            {
                if (enemy.CompareTag("Enemy"))
                {
                    // 调用造成伤害的函数
                    enemy.GetComponent<ArcherController>().Hit(-2);
                }
            }
            animator.SetBool("Fire", false);
        }

        if (stateInfo.IsName("FireL") && stateInfo.normalizedTime >= 1.0f)
        {
            // 遍历敌人，造成伤害
            foreach (Collider2D enemy in hitEnemies)
            {
                if (enemy.CompareTag("Enemy"))
                {
                    // 调用造成伤害的函数
                    enemy.GetComponent<ArcherController>().Hit(-2);
                }
            }
            animator.SetBool("Fire", false);
        }

        if (stateInfo.IsName("HurtR") && stateInfo.normalizedTime >= 1.0f)
        {
            animator.SetBool("isHit", false);
        }

        if (stateInfo.IsName("HurtL") && stateInfo.normalizedTime >= 1.0f)
        {
            animator.SetBool("isHit", false);
        }

        if (currentHealth <= 0)
        {
            SceneManager.LoadScene("DeathMenu");
        }

        //float regenRate = 1f;
        //if (currentMana < maxMana)
        //{
        //    Debug.Log("regenRate: " + regenRate);
        //    Debug.Log("Time.deltaTime: " + Time.deltaTime);
        //    Debug.Log("currentMana: " + currentMana);

        //Invoke("recoverMana", manaRecoveryTime);
        //}

        if (Input.GetKeyDown(KeyCode.J))
        {
            if (!isAttacking)
            {
                isAttacking = true;
                Attack(1);
            }
            else if (canCombo)
            {
                Attack(2);
            }
        }

        // 更新攻击计时器
        if (isAttacking)
        {
            attackTimer += Time.deltaTime;

            // 如果动画播放结束，重置状态
            if (attackTimer >= stateInfo.length)
            {
                isAttacking = false;
                animator.SetBool("Attack1", false);
                canCombo = false;
                animator.SetBool("Attack2", false);
                attackTimer = 0;
            }
        }
    }

    private void Attack(int attackNumber)
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (attackNumber == 1)
        {
            animator.SetTrigger("Attack1");
            canCombo = true;
        }
        else if (attackNumber == 2)
        {
            animator.SetTrigger("Attack2");
            canCombo = false;
        }

        // 检测敌人并造成伤害
        Vector3 lookDirection3D = new Vector3(lookDirection.x, lookDirection.y, 0);
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position + lookDirection3D, 0.5f);
        if (stateInfo.normalizedTime >= 1f)
        {
            foreach (Collider2D enemy in hitEnemies)
            {
                if (enemy.CompareTag("Enemy"))
                {
                    // 调用造成伤害的函数
                    enemy.GetComponent<ArcherController>().Hit(-1);
                }
            }
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

        animator.SetBool("isHit", true);
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
            ChangeMana(-1);
            GameObject fireballObject = Instantiate(fireballPrefab, wizardRigidbody2d.position, Quaternion.identity);

            Fireball projectile = fireballObject.GetComponent<Fireball>();
            projectile.Launch(lookDirection, 300);

            isWaiting = false;
        }
    }

    void Fire()
    {
        if (currentMana >= 2)
        {
            ChangeMana(-3);
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

    void recoverHealth()
    {
        currentHealth = Mathf.Clamp(currentHealth + 1, 0, maxHealth);

        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
    }

    void recoverMana()
    {
        currentMana = Mathf.Clamp(currentMana + 1, 0, maxMana);

        UIManaBar.instance.SetValue(currentMana / (float)maxMana);
    }
}