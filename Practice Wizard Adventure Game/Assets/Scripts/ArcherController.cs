using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherController : MonoBehaviour
{
    public Transform playerTransform;
    public float chaseRadius;
    public float attackRadius;
    public float moveSpeed;

    private Rigidbody2D rb;

    public ParticleSystem dustEffect;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        dustEffect.Stop();
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer > chaseRadius)
        {
            // 玩家在第一个检测半径范围之外，弓箭手不动
            rb.velocity = Vector2.zero;
            dustEffect.Stop();
        }
        else if (distanceToPlayer <= chaseRadius && distanceToPlayer > attackRadius)
        {
            // 玩家在第一个检测半径范围内，弓箭手向着玩家移动
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            rb.velocity = direction * moveSpeed;
            dustEffect.Play();
        }
        else
        {
            // 玩家在第二个检测半径范围内，弓箭手开始射击
            // 停止移动，执行射击逻辑
            rb.velocity = Vector2.zero;
            dustEffect.Stop();
            // 射击逻辑
            // ...
        }
    }
}
