/// <summary>
/// 可池化接口
/// 继承该接口的对象可以被对象池管理
/// </summary>
public interface IPoolable
{
    /// <summary>
    /// 当对象从池中获取时调用
    /// </summary>
    void OnSpawnedFromPool();

    /// <summary>
    /// 当对象返回池中时调用
    /// </summary>
    void OnReturnedToPool();
}
