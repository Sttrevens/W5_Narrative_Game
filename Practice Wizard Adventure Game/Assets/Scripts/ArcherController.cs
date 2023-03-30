using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherController : MonoBehaviour
{
    private Transform playerTransform;
    public float chaseRadius;
    public float attackRadius;
    public float moveSpeed;

    private Rigidbody2D rb;

    private Animator animator;

    public ParticleSystem dustEffect;

    public GameObject arrowPrefab;

    private bool isWaiting = false;
    private bool isDead = false;
    public float delayTime = 0;

    int health;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        dustEffect.Stop();
        animator = GetComponent<Animator>();
        health = 2;

        playerTransform = GameObject.Find("Wizard").transform;
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (!isDead)
        {
            if (distanceToPlayer > chaseRadius)
            {
                // 玩家在第一个检测半径范围之外，弓箭手不动
                rb.velocity = Vector2.zero;
                animator.SetBool("isMoving", false);
                dustEffect.Stop();
            }
            else if (distanceToPlayer <= chaseRadius && distanceToPlayer > attackRadius)
            {
                animator.SetBool("isShooting", false);
                // 玩家在第一个检测半径范围内，弓箭手向着玩家移动
                Vector2 direction = (playerTransform.position - transform.position).normalized;
                rb.velocity = direction * moveSpeed;
                animator.SetFloat("Direction X", direction.x);
                animator.SetBool("isMoving", true);
                dustEffect.Play();
            }
            else
            {
                // 玩家在第二个检测半径范围内，弓箭手开始射击
                // 停止移动，执行射击逻辑
                rb.velocity = Vector2.zero;
                animator.SetBool("isMoving", false);
                dustEffect.Stop();
                // 射击逻辑
                Vector2 direction = (playerTransform.position - transform.position).normalized;
                animator.SetFloat("Direction X", direction.x);
                //animator.SetBool("isShooting", true);
                if (!isWaiting)
                {
                    isWaiting = true;
                    Invoke("Shoot", delayTime);
                }
            }
        }

        if (stateInfo.IsName("ArcherHurtR") && stateInfo.normalizedTime >= 1.0f)
        {
            animator.SetBool("isHit", false);
        }

        if (stateInfo.IsName("ArcherHurtL") && stateInfo.normalizedTime >= 1.0f)
        {
            animator.SetBool("isHit", false);
        }

        if (health <= 0)
        {
            animator.SetBool("isDead1", true);
            animator.SetBool("isDead2", true);
            isDead = true;
            rb.isKinematic = true;
        }
    }

    void Shoot()
    {
        GameObject arrowObject = Instantiate(arrowPrefab, rb.position + Vector2.up * 0.3f, Quaternion.identity);

        Arrow projectile = arrowObject.GetComponent<Arrow>();
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        projectile.Shoot(direction, 300);

        animator.SetTrigger("isShooting");
        isWaiting = false;
    }

    public void Hit(int amount)
    {
        health = health + amount;
        animator.SetBool("isHit", true);
    }
}
