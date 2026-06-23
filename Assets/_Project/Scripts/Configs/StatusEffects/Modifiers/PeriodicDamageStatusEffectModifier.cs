using UnityEngine;

/// <summary>
/// 周期性伤害状态效果修饰器
/// </summary>
[CreateAssetMenu(
    fileName = "StatusEffectModifier_PeriodicDamage_",
    menuName = "ScriptableObjects/StatusEffects/Modifiers/Periodic Damage")]
public class PeriodicDamageStatusEffectModifier : StatusEffectModifier
{
    [SerializeField]
    [Min(0.01f)]
    [Tooltip("The interval between each damage tick.")]
    private float tickInterval = 0.5f;

    [SerializeField]
    [Min(0f)]
    [Tooltip("The amount of damage dealt per tick.")]
    private float damagePerTick = 1f;

    [SerializeField]
    [Tooltip("Whether the damage scales with the number of stacks.")]
    private bool scaleWithStacks = true;

    public float TickInterval => tickInterval;
    public float DamagePerTick => damagePerTick;
    public bool ScaleWithStacks => scaleWithStacks;

    public override IStatusEffectModifierRuntime CreateRuntimeModifier()
    {
        return new RuntimeModifier(tickInterval, damagePerTick, scaleWithStacks);
    }

    private sealed class RuntimeModifier : IStatusEffectModifierRuntime
    {
        private readonly float tickInterval;
        private readonly float damagePerTick;
        private readonly bool scaleWithStacks;

        private float tickTimer;

        public RuntimeModifier(float tickInterval, float damagePerTick, bool scaleWithStacks)
        {
            this.tickInterval = Mathf.Max(0.01f, tickInterval);
            this.damagePerTick = Mathf.Max(0f, damagePerTick);
            this.scaleWithStacks = scaleWithStacks;
        }

        /// <summary>
        /// 当状态效果被应用时调用
        /// 重置计时器以确保立即开始伤害周期
        /// </summary>
        /// <param name="context">状态效果上下文</param>
        /// <param name="effect">状态效果实例</param>
        public void OnApply(StatusEffectContext context, StatusEffectInstance effect)
        {
            tickTimer = 0f;
        }

        public void OnRefresh(StatusEffectContext context, StatusEffectInstance effect)
        {
        }

        public void OnStackChanged(StatusEffectContext context, StatusEffectInstance effect)
        {
        }

        /// <summary>
        /// 每帧调用以处理周期性伤害逻辑
        /// </summary>
        /// <param name="context">状态效果上下文</param>
        /// <param name="effect">状态效果实例</param>
        /// <param name="deltaTime">帧时间</param>
        public void OnTick(StatusEffectContext context, StatusEffectInstance effect, float deltaTime)
        {
            // 如果没有有效的生命组件、伤害值为零或负数 则不执行任何操作
            if (context?.Health == null || effect == null || damagePerTick <= 0f)
            {
                return;
            }

            // 累积时间并检查是否达到伤害间隔
            tickTimer += Mathf.Max(0f, deltaTime);
            // 可能会有多个伤害周期需要处理
            // 因此使用循环来确保所有周期都被正确应用
            while (tickTimer >= tickInterval)
            {
                tickTimer -= tickInterval;
                // 计算伤害值，考虑堆叠数量和来源上下文的伤害倍率
                float stackMultiplier = scaleWithStacks ? effect.StackCount : 1f;
                float sourceMultiplier = effect.SourceContext.PowerMultiplier;
                CombatSystem.ApplyDamage(context.Health, damagePerTick * stackMultiplier * sourceMultiplier);
            }
        }

        public void OnRemove(StatusEffectContext context, StatusEffectInstance effect)
        {
        }
    }
}
