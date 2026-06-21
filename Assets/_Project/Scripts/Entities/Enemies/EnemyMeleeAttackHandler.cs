using UnityEngine;

[RequireComponent(typeof(AttackEvent))]
[DisallowMultipleComponent]
public class EnemyMeleeAttackHandler : MonoBehaviour
{
    private Enemy enemy;

    private void Awake()
    {
        CacheEnemy();
    }

    private void OnEnable()
    {
        CacheEnemy();
        if (enemy != null && enemy.AttackEvent != null)
        {
            enemy.AttackEvent.OnAttack += AttackEvent_OnAttack;
        }
    }

    private void OnDisable()
    {
        if (enemy != null && enemy.AttackEvent != null)
        {
            enemy.AttackEvent.OnAttack -= AttackEvent_OnAttack;
        }
    }

    private void AttackEvent_OnAttack(AttackEvent eventSource, AttackContext context)
    {
        if (context.AttackerObject != gameObject)
        {
            return;
        }

        Player target = context.GetTargetComponent<Player>();
        if (target == null)
        {
            return;
        }

        PlayerDamageReceiver damageReceiver = target.GetComponent<PlayerDamageReceiver>();
        if (damageReceiver != null)
        {
            damageReceiver.TryTakeDamage(context.Damage);
        }
    }

    private void CacheEnemy()
    {
        if (enemy == null)
        {
            enemy = GetComponent<Enemy>();
        }

        if (enemy != null)
        {
            enemy.CacheComponents();
        }
    }
}
