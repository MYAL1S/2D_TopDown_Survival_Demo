using UnityEngine;

/// <summary>
/// 移动速度状态效果修饰器
/// </summary>
[CreateAssetMenu(
    fileName = "StatusEffectModifier_MoveSpeed_",
    menuName = "ScriptableObjects/StatusEffects/Modifiers/Move Speed")]
public class MoveSpeedStatusEffectModifier : StatusEffectModifier
{
    [SerializeField]
    [Tooltip("Additive speed multiplier per stack. 0.3 means +30%, -0.3 means -30%.")]
    private float additiveMultiplierPerStack;

    /// <summary>
    /// 每层叠加的移动速度加成百分比 0.3 表示增加30% -0.3 表示减少30%
    /// </summary>
    public float AdditiveMultiplierPerStack => additiveMultiplierPerStack;

    public override IStatusEffectModifierRuntime CreateRuntimeModifier()
    {
        return new RuntimeModifier(additiveMultiplierPerStack);
    }

    /// <summary>
    /// 运行时修饰器 
    /// 负责根据状态效果实例的当前层数计算并应用移动速度加成
    /// </summary>
    private sealed class RuntimeModifier : IStatusEffectModifierRuntime
    {
        // 每层叠加的移动速度加成百分比 0.3 表示增加30% -0.3 表示减少30%
        private readonly float additiveMultiplierPerStack;

        public RuntimeModifier(float additiveMultiplierPerStack)
        {
            this.additiveMultiplierPerStack = additiveMultiplierPerStack;
        }

        /// <summary>
        /// 应用状态效果时 根据当前层数计算并设置移动速度加成
        /// </summary>
        /// <param name="context">状态效果上下文</param>
        /// <param name="effect">状态效果实例</param>
        public void OnApply(StatusEffectContext context, StatusEffectInstance effect)
        {
            Apply(context, effect);
        }

        public void OnRefresh(StatusEffectContext context, StatusEffectInstance effect)
        {
        }

        /// <summary>
        /// 状态层数变化时调用
        /// </summary>
        /// <param name="context">状态效果上下文</param>
        /// <param name="effect">状态效果实例</param>
        public void OnStackChanged(StatusEffectContext context, StatusEffectInstance effect)
        {
            Apply(context, effect);
        }

        public void OnTick(StatusEffectContext context, StatusEffectInstance effect, float deltaTime)
        {
        }

        /// <summary>
        /// 移除状态效果时 移除对应的移动速度加成
        /// </summary>
        /// <param name="context">状态效果上下文</param>
        /// <param name="effect">状态效果实例</param>
        public void OnRemove(StatusEffectContext context, StatusEffectInstance effect)
        {
            if (context?.RuntimeStats != null)
            {
                context.RuntimeStats.RemoveMoveSpeedMultiplier(this);
            }
        }

        /// <summary>
        /// 根据状态效果实例的当前层数计算并应用移动速度加成
        /// </summary>
        /// <param name="context">状态效果上下文</param>
        /// <param name="effect">状态效果实例</param>
        private void Apply(StatusEffectContext context, StatusEffectInstance effect)
        {
            // 如果上下文或状态效果实例无效 则不进行任何操作
            if (context?.RuntimeStats == null || effect == null)
            {
                return;
            }

            // 计算移动速度加成百分比
            float multiplier = additiveMultiplierPerStack * effect.StackCount;
            // 如果加成百分比接近于0 则移除对应的加成
            if (Mathf.Approximately(multiplier, 0f))
            {
                context.RuntimeStats.RemoveMoveSpeedMultiplier(this);
                return;
            }

            // 设置新的移动速度加成
            context.RuntimeStats.SetMoveSpeedMultiplier(this, multiplier);
        }
    }
}
