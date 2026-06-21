using UnityEngine;

/// <summary>
/// 武器攻击策略 抽象基类 定义了武器攻击的执行逻辑
/// </summary>
public abstract class WeaponAttackStrategy : ScriptableObject
{
    public abstract bool TryExecute(AttackContext context, WeaponAttackServices services);
}
