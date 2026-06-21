using UnityEngine;

/// <summary>
/// 投射物命中策略基类
/// </summary>
public abstract class ProjectileHitStrategy : ScriptableObject
{
    // 是否需要一个活着的目标 来决定是否处理命中
    public virtual bool RequiresAliveTarget => false;
    // 是否使用追踪目标 来决定是否每次命中都更新目标位置
    public virtual bool UsesHomingTarget => false;
    // 是否只接受初始目标 来决定是否只处理第一次命中的目标
    public virtual bool AcceptsOnlyInitialTarget => false;

    /// <summary>
    /// 处理投射物命中敌人的逻辑
    /// </summary>
    /// <param name="projectile">投射物控制器</param>
    /// <param name="enemy">目标敌人</param>
    public abstract void HandleHit(ProjectileController projectile, Enemy enemy);
}
