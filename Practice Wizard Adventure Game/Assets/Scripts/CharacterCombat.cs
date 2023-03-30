using UnityEngine;

public class CharacterCombat : MonoBehaviour
{
    Animator animator;
    public LayerMask enemyLayer;
    public float attackRange = 1.0f;
    public int damage = 10;

    private bool isAttacking = false;
    private float attackTimer = 0.0f;
    private bool canCombo = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // 检查玩家输入
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
            if (attackTimer >= animator.GetCurrentAnimatorStateInfo(0).length)
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
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);
        foreach (Collider2D enemy in hitEnemies)
        {
            // 对敌人造成伤害的逻辑
        }
    }

    private void OnDrawGizmosSelected()
    {
        // 在Scene视图中显示攻击范围
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}