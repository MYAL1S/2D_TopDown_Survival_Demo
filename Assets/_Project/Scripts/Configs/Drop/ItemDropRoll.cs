/// <summary>
/// 物品掉落结果
/// </summary>
public readonly struct ItemDropRoll
{
    public ItemDropRoll(ItemConfig itemConfig, int amount)
    {
        ItemConfig = itemConfig;
        Amount = amount;
    }

    // 物品配置
    public ItemConfig ItemConfig { get; }
    // 掉落数量
    public int Amount { get; }
}
