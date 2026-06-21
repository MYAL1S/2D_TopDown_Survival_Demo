using UnityEngine;

/// <summary>
/// 角色生成管理器
/// 负责实例化玩家
/// 并补齐运行时所需组件与物理配置
/// </summary>
[DisallowMultipleComponent]
public class CharacterSpawnManager : MonoBehaviour
{
    /// <summary>
    /// 使用默认朝向和无父对象生成角色
    /// </summary>
    public Player SpawnCharacter(string playerResourceId, Vector3 position)
    {
        return SpawnCharacter(playerResourceId, position, Quaternion.identity, null);
    }

    /// <summary>
    /// 根据资源 ID 生成玩家
    /// 并可指定位置、旋转和父对象
    /// </summary>
    public Player SpawnCharacter(string playerResourceId, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if (GameResources.Instance == null)
        {
            Debug.LogError($"{nameof(CharacterSpawnManager)} needs {nameof(GameResources)} in the scene.", this);
            return null;
        }

        PlayerConfig config = GameResources.Instance.GetPlayerConfig(playerResourceId);
        if (config == null || config.playerPrefab == null)
        {
            Debug.LogError($"{nameof(CharacterSpawnManager)} could not spawn player '{playerResourceId}'. Missing config or prefab.", this);
            return null;
        }

        GameObject playerObject = Instantiate(config.playerPrefab, position, rotation, parent);
        playerObject.tag = "Player";
        EnsurePlayerRuntimeComponents(playerObject);

        PlayerController controller = playerObject.GetComponent<PlayerController>();
        if (controller != null)
        {
            controller.Initialize(config);
        }

        Health health = playerObject.GetComponent<Health>();
        if (health != null)
        {
            health.Initialize(config.MaxHealth, config.Defense);
        }

        PlayerDamageReceiver damageReceiver = playerObject.GetComponent<PlayerDamageReceiver>();
        if (damageReceiver != null)
        {
            damageReceiver.Initialize(config.InjuredInvincibilityDuration);
        }

        PlayerWeaponSystem weaponSystem = playerObject.GetComponent<PlayerWeaponSystem>();
        if (weaponSystem != null)
        {
            weaponSystem.InitializeStartingWeapon(config.StartingWeaponConfig);
        }

        return playerObject.GetComponent<Player>();
    }

    /// <summary>
    /// 确保玩家对象具备运行时所需的基础组件
    /// </summary>
    private static void EnsurePlayerRuntimeComponents(GameObject playerObject)
    {
        Rigidbody2D rigidBody2D = EnsureComponent<Rigidbody2D>(playerObject);
        ConfigureRigidbody(rigidBody2D);
        ConfigureColliders(playerObject);
        Player player = EnsureComponent<Player>(playerObject);
        player.EnsureRuntimeComponents();
        player.CacheComponents();
    }

    /// <summary>
    /// 配置玩家的二维刚体参数
    /// 保证移动与碰撞表现符合预期
    /// </summary>
    private static void ConfigureRigidbody(Rigidbody2D rigidBody2D)
    {
        rigidBody2D.gravityScale = 0f;
        rigidBody2D.freezeRotation = true;
        rigidBody2D.interpolation = RigidbodyInterpolation2D.Interpolate;
        rigidBody2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    /// <summary>
    /// 将玩家及其子物体上的碰撞体设置为触发器
    /// </summary>
    private static void ConfigureColliders(GameObject playerObject)
    {
        Collider2D[] colliders = playerObject.GetComponentsInChildren<Collider2D>(true);
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].isTrigger = true;
        }
    }

    /// <summary>
    /// 若目标组件不存在
    /// 则自动添加
    /// </summary>
    private static T EnsureComponent<T>(GameObject gameObject) where T : Component
    {
        T component = gameObject.GetComponent<T>();
        return component != null ? component : gameObject.AddComponent<T>();
    }
}
