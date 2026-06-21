using System.Collections.Generic;

public interface IWeaponInventory
{
    /// <summary>
    /// 当前激活的武器列表 包含玩家当前装备的所有武器实例 
    /// 通过这个列表可以访问每个武器的配置数据 冷却状态等信息
    /// </summary>
    IReadOnlyList<WeaponRuntimeInstance> ActiveWeapons { get; }
    /// <summary>
    /// 添加武器到玩家的武器库中 
    /// 如果武器已经存在 则返回false 否则创建一个新的武器实例并添加到ActiveWeapons列表中 返回true
    /// </summary>
    /// <param name="config">添加的武器配置</param>
    /// <returns></returns>
    bool AddWeapon(WeaponConfig config);
    /// <summary>
    /// 添加武器到玩家的武器库中
    /// </summary>
    /// <param name="config">添加的武器配置</param>
    /// <param name="resetCooldown">是否重置冷却时间</param>
    /// <returns></returns>
    bool AddWeapon(WeaponConfig config, bool resetCooldown);
    /// <summary>
    /// 从玩家的武器库中移除一个武器实例
    /// </summary>
    /// <param name="config">要移除的武器配置</param>
    /// <returns></returns>
    bool RemoveWeapon(WeaponConfig config);
    /// <summary>
    /// 从玩家的武器库中移除一个武器实例
    /// </summary>
    /// <param name="resourceId">要移除的武器资源ID</param>
    /// <returns></returns>
    bool RemoveWeapon(string resourceId);
    /// <summary>
    /// 检查玩家的武器库中是否已经拥有某个武器实例
    /// </summary>
    /// <param name="config">要检查的武器配置</param>
    /// <returns></returns>
    bool HasWeapon(WeaponConfig config);
    /// <summary>
    /// 检查玩家的武器库中是否已经拥有某个武器实例
    /// </summary>
    /// <param name="resourceId">要检查的武器资源ID</param>
    /// <returns></returns>
    bool HasWeapon(string resourceId);
}
