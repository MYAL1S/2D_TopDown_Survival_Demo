# 2D 俯视角生存原型 - 实现文档 v0.1

## 1. 文档说明

本文档为项目早期实现文档，版本为 v0.1。

当前版本用于记录项目的初始实现计划、工程结构、开发流程、里程碑拆分和各模块的初步落地方案。具体类实现、接口细节、代码结构调整和最终实现结果会随着开发推进持续更新。

实现文档与技术分析文档的关系：

```text
技术分析文档：说明为什么这么做、整体方案是什么、有哪些风险和扩展方向。
实现文档：说明具体怎么做、当前做到哪里、实际实现和计划是否一致。
```

## 2. 当前实现状态

| 里程碑 | 状态 | 说明 |
| --- | --- | --- |
| M1 工程初始化 | 已完成 | 创建 Unity 工程、目录结构、基础场景 |
| M2 玩家移动和摄像机 | 已完成 | WASD 移动、摄像机跟随 |
| M3 敌人生成和追踪 | 已完成 | 屏幕外刷怪、敌人追踪玩家 |
| M4 自动攻击和战斗 | 已完成 | 自动索敌、攻击、伤害、死亡 |
| M5 掉落和拾取 | 已完成 | 敌人死亡掉落、玩家吸附拾取、经验和回血效果 |
| M6 Buff/Debuff/AOE | 已完成 | 状态效果、武器命中 Debuff、AOE 范围效果和对象池 |
| M7 UI、调试和优化 | 已完成 | UI 管理、MVP 面板、玩家数据/设置持久化、关卡结算、音频管理、对象池和玩家血条 |
| M8 文档整理和演示准备 | 未开始 | README、截图、演示说明 |

## 3. 开发环境

计划使用：

| 项目 | 方案 |
| --- | --- |
| 引擎 | 2022.3.62f3 LTS |
| 开发语言 | C# |
| 版本管理 | Git + GitHub |
| Git 客户端 | SourceTree |
| 输入方案 | Unity Input System |
| 摄像机方案 | Cinemachine |
| 配置方式 | ScriptableObject |
| 物理检测 | 2D Physics |
| 文档格式 | Markdown |

实际使用版本将在工程初始化完成后补充。

## 4. Git 分支管理

本项目使用简化 Git Flow：

```text
main      稳定展示分支
develop   日常开发集成分支
feature/* 具体功能开发分支
```

推荐流程：

```text
develop
  -> feature/player-movement
  -> 合并回 develop
  -> develop 达到阶段稳定后合并到 main
```

分支命名建议：

```text
feature/project-setup
feature/player-movement
feature/enemy-spawn
feature/auto-combat
feature/drop-pickup
feature/status-effect-aoe
feature/ui-debug
```

提交信息规范：

```text
docs: add initial technical analysis
docs: add initial implementation document
chore: initialize unity project structure
feat: add player movement and camera follow
feat: add enemy spawn and chase behavior
feat: add auto attack combat loop
feat: add drop and pickup system
feat: add status effect and aoe systems
refactor: split combat and targeting systems
fix: correct enemy spawn outside camera view
```

## 5. 项目目录规划

Unity 工程根目录建议结构：

```text
UnitySurvivalDemo/
  Assets/
  Packages/
  ProjectSettings/
  Docs/
    technical-analysis.md
    implementation-document.md
  README.md
  .gitignore
```

`Assets/_Project` 内部建议结构：

```text
Assets/
  _Project/
    Art/
      Sprites/
      VFX/
    Audio/
    Prefabs/
      Player/
      Enemies/
      Weapons/
      Projectiles/
      Pickups/
      AOE/
      UI/
    Scenes/
      Game.unity
    Scripts/
      Core/
      Configs/
      Entities/
      Systems/
      Runtime/
      UI/
      Utils/
    ScriptableObjects/
      Players/
      Enemies/
      Weapons/
      Items/
      StatusEffects/
      AOE/
      Spawn/
      DropTables/
```

说明：

1. `Docs` 放在 Unity 工程根目录，不放入 `Assets`。
2. 项目代码和资源统一放在 `Assets/_Project`。
3. 第三方插件应与 `_Project` 分离，避免项目资源和插件资源混杂。
4. 初期可先使用占位 Sprite，不优先寻找正式美术资源。

## 6. M1 工程初始化实现计划

目标：创建一个干净、可运行、可提交到 GitHub 的 Unity 工程基础。

任务：

1. 创建 Unity 2D 工程。
2. 添加 Unity `.gitignore`。
3. 创建 `Docs` 文件夹。
4. 添加 `technical-analysis.md` 和 `implementation-document.md`。
5. 创建 `Assets/_Project` 目录结构。
6. 创建 `Game.unity` 场景。
7. 创建 `GameRoot` 空对象，用于挂载系统管理脚本。
8. 创建基础 Player 占位对象。
9. 创建基础 Camera。
10. 确认项目运行无报错。

验收标准：

1. Unity 工程可以打开并运行。
2. GitHub 仓库结构清晰。
3. 文档位于 `Docs` 目录。
4. `main` 和 `develop` 分支创建完成。
5. Console 无持续报错。

实际实现记录：

```text
实际实现记录：

M1 工程初始化已完成，当前已完成内容如下：

1. 创建 Unity 2D 工程。
2. 初始化 Git 仓库并关联 GitHub。
3. 创建 `main` 和 `develop` 分支。
4. 添加 Unity `.gitignore`。
5. 创建 `Docs` 文件夹。
6. 添加技术分析文档和实现文档。
7. 创建基础项目目录结构。
8. 创建 `Assets/_Project` 目录。
9. 创建基础 `Game.unity` 场景。
10. 确认项目可以正常打开，Unity Console 无持续报错。

当前项目已具备继续开发 M2 玩家移动与摄像机跟随功能的基础。
```

## 7. M2 玩家移动和摄像机实现计划

目标：实现玩家 WASD 移动和摄像机跟随。

计划脚本：

| 脚本 | 职责 |
| --- | --- |
| PlayerController.cs | 玩家移动、基础属性、引用管理 |
| InputSystemAdapter.cs | 输入读取，将 WASD/摇杆转为移动向量 |
| PlayerConfig.cs | 玩家基础配置 |

计划 Prefab：

| Prefab | 说明 |
| --- | --- |
| Player.prefab | 玩家预制体，包含 Sprite、Rigidbody2D、Collider2D、PlayerController |

移动方案：

1. 输入系统输出 `Vector2 moveInput`。
2. 当输入长度大于 1 时归一化。
3. 使用 `Rigidbody2D.velocity` 或 `MovePosition` 移动玩家。
4. 玩家速度从 `PlayerConfig` 读取。

摄像机方案：

1. 优先使用 Cinemachine Virtual Camera。
2. Follow 目标设置为 Player。
3. 调整 Dead Zone、Damping 或 Orthographic Size。

验收标准：

1. WASD 可以控制玩家移动。
2. 斜向移动速度正常。
3. 摄像机平滑跟随玩家。
4. 移动速度可通过配置调整。

实际实现记录：

状态：已完成

### 实际创建/使用的脚本

| 脚本 | 实际职责 |
| --- | --- |
| PlayerController.cs | 读取输入，触发移动/待机事件，读取 PlayerConfig 中的移动速度 |
| InputSystemAdapter.cs | 统一读取键盘 WASD 和 UI 摇杆输入，输出 Vector2 MoveInput |
| VirtualJoystick.cs | 将 UI 摇杆拖拽转换为移动向量 |
| PlayerConfig.cs | 使用 ScriptableObject 配置玩家基础速度和角色资源引用 |
| MovementByVelocity.cs | 根据移动事件设置 Rigidbody2D.velocity |
| Idle.cs | 根据待机事件停止 Rigidbody2D |
| PlayerAnimator.cs | 根据移动/待机事件设置 Animator 的 Speed 参数 |

### 实际使用的 Prefab / 场景对象

| 对象 | 说明 |
| --- | --- |
| Mage.prefab | 当前玩家角色预制体，包含 Animator、Rigidbody2D、Collider2D、PlayerController 等组件 |
| Joystick.prefab | UI 摇杆，放在 Canvas 下，用于移动端/鼠标拖拽输入 |
| Virtual Camera | Cinemachine 虚拟摄像机，Follow 目标设置为玩家 |
| Canvas | 使用 Screen Space - Overlay 渲染 UI |
| EventSystem | 处理 UI 摇杆输入事件 |

### 实际实现方式

1. `InputSystemAdapter` 统一输出 `MoveInput`。
2. 键盘输入使用 `Horizontal / Vertical`。
3. UI 摇杆输入通过 `VirtualJoystick` 输出。
4. 当摇杆有输入时优先使用摇杆，否则使用键盘输入。
5. 玩家速度从 `PlayerConfig.MoveSpeed` 读取。
6. `PlayerController.Update()` 只读取输入。
7. `PlayerController.FixedUpdate()` 触发移动或待机事件。
8. `MovementByVelocity` 使用 `Rigidbody2D.velocity` 移动玩家。
9. 斜向输入会归一化，避免斜向速度过快。
10. `IdleEvent` 只在从移动切换到停止时触发一次，避免每帧重复广播。
11. Animator 使用 `Speed` 参数切换 Idle / Run 动画。
12. 摄像机使用 Cinemachine Virtual Camera 跟随玩家。

### 和原计划不一致的地方

1. 原计划只列出 `PlayerController.cs / InputSystemAdapter.cs / PlayerConfig.cs`，实际为了事件解耦，增加并使用了 `MovementByVelocity`、`Idle`、`PlayerAnimator` 等组件。
2. 除 WASD 外，额外接入了 UI 摇杆输入。
3. UI 当前采用 `Screen Space - Overlay`，暂不使用独立 UI Camera。

### 验收结果

1. WASD 可以控制玩家移动。
2. UI 摇杆可以控制玩家移动。
3. 斜向移动速度正常。
4. 玩家停止时可以切换回待机状态。
5. 玩家移动时可以播放 Run 动画。
6. 摄像机可以跟随玩家。
7. 移动速度可以通过 `PlayerConfig` 调整。
8. 项目编译通过，Console 无持续脚本编译错误。

## 8. M3 敌人生成和追踪实现计划

目标：实现敌人从屏幕外随机生成，并追踪玩家。

计划脚本：

| 脚本 | 职责 |
| --- | --- |
| EnemyController.cs | 敌人属性、受击、死亡、回收 |
| SpawnSystem.cs | 敌人生成控制 |
| EnemyAISystem.cs | 敌人追踪玩家 |
| EnemyConfig.cs | 敌人配置 |
| SpawnConfig.cs | 生成配置 |

生成规则：

```text
获取摄像机视野
  -> 扩展生成边距
  -> 随机选择视野外一侧
  -> 生成敌人
  -> 检查最大敌人数量
```

追踪规则：

```text
方向 = 玩家位置 - 敌人位置
速度 = 方向归一化 * 敌人移动速度
```

验收标准：

1. 敌人不会在屏幕内直接生成。
2. 敌人会朝玩家移动。
3. 敌人数量不会超过配置上限。

实际实现记录：

状态：已完成

### 实际创建/使用的脚本

| 脚本 | 实际职责 |
| --- | --- |
| GameResources.cs | 缓存 PlayerConfig 和 EnemyConfig 资源引用，提供按 ResourceId 查询和随机获取敌人配置的入口 |
| Generator.cs | 生成系统上下文，集中引用 EnemySpawnManager、SpawnSystem、ItemDropGenerator、CharacterSelectionSystem |
| EnemyConfig.cs | 配置敌人 resourceId、预制体、生命、速度、攻击、防御和动画控制器 |
| SpawnConfig.cs | 配置基础敌人列表、按时间解锁敌人、刷怪速率曲线、最大存活数曲线、批量生成曲线、每帧生成上限和屏幕外边距 |
| SpawnSystem.cs | 控制刷怪开始、暂停、结束，计算屏幕外生成点，维护活跃敌人字典和死亡清理列表 |
| EnemySpawnManager.cs | 根据 EnemyConfig 从对象池获取或创建敌人，补齐运行时组件，初始化配置和追踪目标 |
| ComponentPool.cs | 通用组件对象池，约束对象实现 IPoolable，出池/入池时自动调用对象自身回调 |
| IPoolable.cs | 对象池生命周期接口，定义 OnSpawnedFromPool 和 OnReturnedToPool |
| EnemyController.cs | 缓存敌人配置、属性、Health 和追踪目标，负责运行时组件兜底补齐和对象池生命周期重置 |
| EnemyAISystem.cs | 根据目标位置驱动 Rigidbody2D 追踪玩家，并在重叠过近时加入分离推力 |
| Health.cs | 通用生命组件，负责受伤、扣血、防御计算、死亡判定，并触发 InjuredEvent / DeathEvent |
| InjuredEvent.cs | 通用受伤事件组件 |
| DeathEvent.cs | 通用死亡事件组件 |
| EnemyInjuredHandler.cs | 监听敌人受伤事件，执行受击闪白表现 |
| EnemyDeathHandler.cs | 监听敌人死亡事件，停止移动、关闭碰撞、播放死亡效果、通知 SpawnSystem 清理记录并回收到对象池 |
| EnemyDropHandler.cs | 监听敌人死亡事件，调用 ItemDropGenerator 生成掉落，作为 M5 掉落系统接入点 |
| NearestEnemyDamageTest.cs | 调试脚本，按 J 键让距离玩家最近的敌人扣血，用于测试受伤和死亡事件 |

### 实际使用的配置和场景对象

| 对象 | 说明 |
| --- | --- |
| GameResources | 持有玩家和敌人配置资源引用，并在运行时构建查询缓存 |
| Generator | 作为生成系统根对象，持有敌人生成、刷怪、掉落和角色选择相关管理器 |
| SpawnConfig | 使用 ScriptableObject 配置刷怪规则和随时间变化的生成压力 |
| EnemyConfig | 使用 ScriptableObject 配置每类敌人的属性和预制体 |
| Enemy Pool | EnemySpawnManager 创建的池容器，用于存放已失活的敌人实例 |

### 实际实现方式

1. `GameResources` 只负责保存和查询资源配置，不承载生成、选择、掉落等业务逻辑。
2. `SpawnSystem` 对外提供 `StartSpawning`、`PauseSpawning`、`StopSpawning` 三个接口。
3. `SpawnSystem` 通过 `Camera.main` 获取主相机，通过 `Player` 标签查找玩家目标，不强依赖场景中手动拖入相机或玩家引用。
4. `SpawnSystem` 根据 `elapsedTime` 查询 `SpawnConfig`，通过 `spawnRateByTime`、`maxAliveByTime`、`batchCountByTime` 曲线控制生成压力。
5. `SpawnConfig` 支持通过 `timedEnemyResources` 按游戏时间加入不同敌人资源。
6. `SpawnSystem` 每帧累积 `spawnBudget`，再结合当前最大存活数、批量生成数和每帧生成上限决定本帧生成数量。
7. `EnemySpawnManager` 按 `EnemyConfig.ResourceId` 维护独立对象池，并通过 `ComponentPool<EnemyController>` 复用敌人实例。
8. `EnemySpawnManager` 只负责实例化敌人、补齐运行时组件、初始化配置和追踪目标，不再统一注册或转发敌人死亡事件。
9. `EnemyController` 实现 `IPoolable`，在出池和入池时重置刚体速度、角速度和追踪目标。
10. `EnemyAISystem` 在 `FixedUpdate` 中根据目标位置计算移动方向，并通过 `Rigidbody2D.velocity` 驱动敌人追踪玩家。
11. `EnemyAISystem` 加入极近距离分离推力，降低敌人完全重叠时的显示异常。
12. 敌人使用 `Health` 统一处理生命、受伤和死亡，玩家后续也可以复用同一套 `Health / InjuredEvent / DeathEvent`。
13. `EnemyInjuredHandler`、`EnemyDeathHandler`、`EnemyDropHandler` 分别监听事件并处理敌人的受伤表现、死亡回收和掉落入口。
14. `EnemyDeathHandler` 死亡时调用 `SpawnSystem.AddToDeadEnemies`，`SpawnSystem` 在 `Update` 开头统一 `CleanupDeadEnemies`。
15. `ComponentPool` 使用 `Queue` 保存未激活实例，`Get` 时激活对象并调用 `OnSpawnedFromPool`，`Release` 时调用 `OnReturnedToPool`、失活对象并放回队列。
16. `NearestEnemyDamageTest` 用于调试，按 `J` 键让距离玩家最近的敌人扣血，验证 `InjuredEvent` 和 `DeathEvent` 链路。

### 和原计划不一致的地方

1. 原计划只有 `EnemyController / SpawnSystem / EnemyAISystem / EnemyConfig / SpawnConfig`，实际为了彻底解耦，增加了 `GameResources`、`Generator`、`EnemySpawnManager`、对象池和事件监听脚本。
2. 原计划中 `EnemyController` 包含受击、死亡和回收职责，实际改为 `Health` 负责生命判定，`EnemyInjuredHandler` 负责受击表现，`EnemyDeathHandler` 负责死亡表现和回收，`EnemyDropHandler` 负责掉落入口。
3. 原计划中的生成规则偏固定间隔和固定上限，实际改为基于游戏时间曲线和 `spawnBudget` 的持续增压刷怪。
4. 原计划只描述固定敌人列表，实际支持按时间段加入不同敌人资源。
5. 原计划未明确对象池，实际 M3 已提前接入通用对象池，为后续子弹、特效、伤害数字和掉落物复用。
6. 原计划未处理敌人死亡后字典清理问题，实际使用 `deadEnemyIds` 延迟清理，避免遍历字典时修改字典。
7. 原计划未处理背景和角色显示层级问题，实际通过 Sorting Layer / Order 保证背景始终在最底层。
8. 原计划未处理敌人重叠显示问题，实际在 `EnemyAISystem` 中加入分离推力，减少敌人完全贴合。
9. 原计划未包含无限地图/背景生成，实际增加了固定池版本 `InfiniteBackground`，保证玩家和相机持续移动时画面内始终有连续背景。

### 验收结果

1. 敌人可以在屏幕外生成。
2. 敌人会追踪带 `Player` 标签的玩家。
3. 生成压力会随游戏时间通过 `SpawnConfig` 曲线变化。
4. 可以按时间段加入不同敌人配置。
5. 当前活跃敌人数量不会超过当前时间对应的最大存活数。
6. 敌人死亡后会从 `activeEnemies` 字典移除。
7. 敌人死亡后会禁用碰撞并回收到对象池；无法回池时至少会失活。
8. 敌人受伤和死亡通过 `Health` 下的 `InjuredEvent / DeathEvent` 触发，具体表现由监听脚本负责。
9. 敌人重叠过近时会通过分离推力散开。
10. 可以通过 `NearestEnemyDamageTest` 按 `J` 键测试最近敌人的受伤和死亡事件。
11. 背景通过 Sorting Layer / Order 保持在最底层，不会遮挡玩家和敌人。
12. 相机移动时背景瓦片会循环重定位，画面内保持连续背景。
13. 无限背景使用固定数量瓦片复用，不会在移动过程中持续创建或销毁背景对象。

实现过程中遇到的问题，以及解决方案:
1. 通过 Cinemachine 实现跟随角色时，画面会有抖动。
```text
通过设置 Player 和 Enemy 对象上的 Rigidbody2D.interpolation 为 RigidbodyInterpolation2D.Interpolate，通过插值避免渲染与物理帧不同步。
移动逻辑集中在 FixedUpdate 内写入 Rigidbody2D.velocity，减少 Update 与物理帧混用导致的抖动。
降低 Cinemachine 阻尼让相机跟随更平滑。
调整死区避免小范围移动跟随
```
2. 角色通过角色生成系统生成时，场景开始时 Cinemachine 可能没有 Follow 目标。
```text
为 Cinemachine 添加 CameraTargetBinder 脚本，对外提供绑定和解绑 target 的方法。
CharacterSpawnManager 创建角色后将角色 transform 传入绑定方法。
后续如果角色频繁销毁和重建，不重复创建 Cinemachine，只更新已有虚拟相机的 Follow 目标。
```
3. 怪物重叠时显示异常。
```text
为背景、敌人、玩家的 SpriteRenderer 配置层级，通过 Sorting Layer / Order 保证背景始终位于最底层。
敌人和玩家的 Sprite Pivot 调整为 Bottom Center，并将 SpriteRenderer 的 Sprite Sort Point 设置为 Pivot。
敌人之间不只依赖层级排序解决完全重叠问题，还在移动逻辑中处理。
EnemyAISystem 在移动向量上叠加极近距离分离推力。
当周围敌人距离小于 separationDistance 时，当前敌人获得反向推力，表现上会像水流一样散开。
```
4. 敌人死亡事件触发后没有失活。
```text
确认 DeathEvent 只负责广播事件，不负责具体死亡逻辑。
EnemyDeathHandler 负责监听 DeathEvent 并执行死亡协程。
修正运行时补组件顺序，先确保 EnemyController，再添加 EnemyDeathHandler / EnemyDropHandler 等监听脚本。
EnemyDeathHandler 在回池前重新获取 EnemyController，避免 Awake 时缓存为空导致 ReleaseEnemy(null) 后直接返回。
如果找不到 EnemySpawnManager 或 EnemyController，则兜底执行 gameObject.SetActive(false)。
```
5. 管理器职责过重。
```text
EnemySpawnManager 不再订阅所有敌人死亡事件，也不再统一处理敌人死亡表现。
死亡、受伤、掉落逻辑分散到每个敌人对象上的监听脚本。
SpawnSystem 只维护 activeEnemies 字典和 deadEnemyIds 列表。
敌人死亡时由 EnemyDeathHandler 调用 AddToDeadEnemies，SpawnSystem 每帧统一 CleanupDeadEnemies，避免遍历字典时修改字典。
```


## 9. M4 自动攻击和战斗实现计划

目标：实现玩家自动索敌、攻击、伤害和敌人死亡流程。

计划脚本：

| 脚本 | 职责 |
| --- | --- |
| WeaponRuntime.cs | 武器运行时冷却和攻击触发 |
| WeaponConfig.cs | 武器配置 |
| TargetingSystem.cs | 目标选择 |
| CombatSystem.cs | 伤害计算和死亡判定 |
| ProjectileController.cs | 子弹移动和命中 |
| ProjectileConfig.cs | 子弹配置 |

初始武器：

| 武器 | 行为 |
| --- | --- |
| Magic Bolt | 每隔一段时间攻击范围内最近敌人 |

攻击流程：

```text
武器冷却完成
  -> 查找范围内最近敌人
  -> 创建子弹
  -> 子弹命中敌人
  -> CombatSystem 计算伤害
  -> 敌人 HP 归零
  -> 触发死亡事件
```

验收标准：

1. 玩家无需按键即可自动攻击。
2. 自动攻击优先选择范围内最近敌人。
3. 敌人可以受到伤害并死亡。

实际实现记录：

状态：已完成

### 实际创建/使用的脚本

| 脚本 | 实际职责 |
| --- | --- |
| PlayerWeaponSystem.cs | 玩家武器系统，维护当前持有的多把武器，逐把处理冷却、自动索敌、触发攻击事件，并管理发射物对象池 |
| WeaponRuntimeInstance.cs | 单把武器的运行时状态，保存武器配置、冷却计时、等级和启用状态 |
| IWeaponInventory.cs | 武器库存接口，提供 ActiveWeapons、AddWeapon、RemoveWeapon、HasWeapon 等外部入口 |
| WeaponConfig.cs | 武器 ScriptableObject 配置，保存基础数值、发射物引用以及攻击/伤害/命中策略引用 |
| WeaponAttackStrategy.cs | 武器攻击策略基类，定义武器如何发起攻击 |
| DirectWeaponAttackStrategy.cs | 直接攻击策略，不创建发射物，直接按伤害策略结算伤害 |
| ProjectileWeaponAttackStrategy.cs | 发射物攻击策略，生成发射物；当未配置发射物预制体时可退化为直接伤害 |
| WeaponDamageStrategy.cs | 武器伤害策略基类，定义单体、范围等伤害形状 |
| SingleTargetDamageStrategy.cs | 单体伤害策略，对命中目标造成伤害 |
| AreaDamageStrategy.cs | 范围伤害策略，通过 TargetingSystem 收集范围内敌人并造成伤害 |
| ProjectileHitStrategy.cs | 发射物命中策略基类，定义命中后是否追踪、是否只接受初始目标、是否要求目标存活 |
| DestroyOnHitProjectileHitStrategy.cs | 命中后造成伤害并回收发射物 |
| PierceUntilRangeProjectileHitStrategy.cs | 发射物在射程内持续飞行，命中敌人后按间隔造成伤害，不立即回收 |
| ProjectileController.cs | 单个发射物运行时组件，负责移动、触发检测、射程回收和将命中交给 ProjectileHitStrategy |
| WeaponAttackServices.cs | 提供给攻击策略使用的运行时服务，包括发射物生成和伤害应用入口 |
| WeaponDamageApplier.cs | 通用伤害应用工具，供直接攻击、发射物和范围伤害复用 |
| TargetingSystem.cs | 敌人注册、空间网格索引、最近敌人查询和范围敌人收集 |
| AttackEvent.cs | 通用攻击事件组件，玩家和敌人攻击都通过该事件解耦 |
| AttackContext.cs | 攻击上下文，携带攻击者、目标、起点、方向、伤害、范围、发射物和武器配置 |
| CombatSystem.cs | 通用伤害入口，调用 Health 执行扣血 |
| Health.cs | 通用生命组件，处理生命值、防御、受伤事件和死亡事件 |
| PlayerDamageReceiver.cs | 玩家受伤入口，处理受击扣血和短暂无敌时间 |
| PlayerInjuredHandler.cs | 玩家受伤表现处理 |
| PlayerDeathHandler.cs | 玩家死亡表现和组件禁用处理 |
| EnemyAttackSystem.cs | 敌人攻击系统，基于碰撞中的玩家、攻击范围和攻击冷却触发攻击事件 |
| EnemyMeleeAttackHandler.cs | 敌人近战攻击处理器，监听敌人 AttackEvent 并对玩家造成伤害 |
| EnemyAISystem.cs | 敌人追踪和分离移动，分离逻辑使用 TargetingSystem 空间网格并错峰低频计算 |

### 实际使用的配置和资源

| 资源 | 说明 |
| --- | --- |
| WeaponConfig_MagicBolt.asset | Magic Bolt 武器配置，使用发射物攻击、单体伤害、命中销毁策略 |
| WeaponConfig_Star.asset | Star 武器配置，使用发射物攻击、单体伤害、穿透到射程结束策略 |
| WeaponAttackStrategy_Projectile.asset | 发射物攻击策略资源 |
| WeaponAttackStrategy_Direct.asset | 直接攻击策略资源 |
| WeaponDamageStrategy_SingleTarget.asset | 单体伤害策略资源 |
| WeaponDamageStrategy_Area.asset | 范围伤害策略资源 |
| ProjectileHitStrategy_DestroyOnHit.asset | 命中销毁策略资源 |
| ProjectileHitStrategy_PierceUntilRange.asset | 穿透到射程结束策略资源 |
| Mage.prefab / Wizard.prefab | 挂载 PlayerWeaponSystem，通过 startingWeaponConfigs 配置默认武器 |
| Player Layer | 玩家层级，供敌人攻击触发过滤 |
| Enemy Layer | 敌人层级，供子弹命中和敌人分离过滤/索引使用 |

### 实际实现方式

1. `PlayerWeaponSystem` 作为玩家武器系统入口，不再只维护单把武器，而是维护 `List<WeaponRuntimeInstance>`。
2. 每把武器都有独立 `CooldownTimer`，`PlayerWeaponSystem.Update()` 遍历所有启用武器，冷却结束后尝试攻击。
3. 武器系统通过 `TargetingSystem.FindNearestAliveEnemy` 查找范围内最近敌人。
4. 找到目标后构造 `AttackContext`，并通过玩家身上的 `AttackEvent` 广播攻击事件。
5. `PlayerWeaponSystem` 监听自身 `AttackEvent`，从 `AttackContext.WeaponConfig` 取出 `WeaponAttackStrategy` 并执行。
6. 武器行为完全由策略组合决定：攻击方式由 `WeaponAttackStrategy` 决定，伤害形状由 `WeaponDamageStrategy` 决定，发射物命中行为由 `ProjectileHitStrategy` 决定。
7. `ProjectileWeaponAttackStrategy` 负责请求 `PlayerWeaponSystem` 生成发射物。
8. `PlayerWeaponSystem` 按发射物 prefab 维护多个 `ComponentPool<ProjectileController>`，避免不同武器发射物共用同一个池。
9. `ProjectileController` 只负责移动、触发检测、射程回收和把命中交给 `ProjectileHitStrategy`，不再通过枚举判断命中行为。
10. 子弹命中前先过滤 `Enemy Layer`，再执行 `GetComponentInParent<Enemy>`，减少无效触发处理。
11. 敌人攻击前先过滤 `Player Layer`，再执行 `GetComponentInParent<Player>`，减少无效触发处理。
12. 敌人攻击通过 `EnemyAttackSystem` 判断冷却、碰撞玩家和攻击距离，满足条件时触发敌人 `AttackEvent`。
13. `EnemyMeleeAttackHandler` 监听敌人攻击事件，对玩家调用 `PlayerDamageReceiver.TryTakeDamage`。
14. 玩家受伤由 `PlayerDamageReceiver` 统一处理，并在受伤后进入短暂无敌时间。
15. 玩家和敌人都复用 `Health / InjuredEvent / DeathEvent`，受伤和死亡表现由各自 handler 监听处理。
16. `TargetingSystem` 维护已注册敌人的列表和空间网格索引，同一帧最多重建一次。
17. `TargetingSystem` 提供最近敌人查询和范围敌人收集，武器索敌、范围伤害和敌人分离都复用该入口。
18. 敌人分离不再使用每敌人每 FixedUpdate 的 `Physics2D.OverlapCircleNonAlloc`。
19. `EnemyAISystem` 以 `separationUpdateInterval` 低频计算分离方向，并按 instance id 错峰，避免所有敌人同帧查询。
20. 敌人分离候选来自 `TargetingSystem.CollectAliveEnemiesInRange` 的空间网格结果，并通过 `maxSeparationChecks` 限制最大检查数量。

### 和原计划不一致的地方

1. 原计划中的 `WeaponRuntime.cs` 已被 `PlayerWeaponSystem.cs` 取代，实际实现支持多武器同时持有和独立冷却，更符合割草游戏的武器叠加模式。
2. 原计划中的 `ProjectileConfig.cs` 没有单独实现，发射物相关数值当前集中在 `WeaponConfig` 中，发射物行为通过策略资源扩展。
3. 原计划只描述 Magic Bolt 单武器，实际实现已支持多武器库存、添加/移除武器、不同发射物和不同命中行为。
4. 原计划未包含策略模式，实际将攻击方式、伤害形状和发射物命中行为拆成 ScriptableObject 策略。
5. 原计划未包含玩家健康系统，实际已补全玩家受伤、无敌时间、受伤事件和死亡事件。
6. 原计划未包含敌人攻击，实际已实现敌人攻击冷却、攻击范围判断、碰撞玩家后触发攻击事件和近战扣血。
7. 原计划未包含大规模敌人索敌性能优化，实际通过 `TargetingSystem` 空间网格降低最近敌人查询、范围伤害和分离查询的开销。
8. 原计划未包含 Layer 过滤，实际给敌人攻击和子弹命中加入 Player/Enemy Layer 过滤，减少无效 trigger 回调处理。

### 验收结果

1. 玩家无需按键即可自动攻击范围内最近敌人。
2. 玩家可以同时持有多把武器，每把武器独立冷却并自动攻击。
3. 外部系统可以通过 `IWeaponInventory` / `PlayerWeaponSystem` 添加、移除和查询玩家武器。
4. 武器攻击方式、伤害形状和发射物命中行为均可通过 ScriptableObject 策略组合配置。
5. Magic Bolt 可以发射命中后销毁的单体子弹。
6. Star 可以发射穿透型子弹，在超出射程前持续存在并对路径敌人造成伤害。
7. 子弹命中敌人后可以通过 `CombatSystem / Health` 正确造成伤害。
8. 敌人血量归零后会触发死亡流程，并由 M3 的死亡回收链路处理。
9. 敌人碰撞玩家且冷却结束、距离满足攻击范围时可以攻击玩家。
10. 玩家受伤后会进入短暂无敌时间，避免同一段碰撞中被连续高频扣血。
11. 敌人分离逻辑不再使用每敌人每物理帧物理查询，敌人数量较多时性能风险降低。

### 实现过程中遇到的问题，以及解决方案

1. 单武器结构不适合割草游戏的武器叠加。
```text
将 WeaponRuntime / PlayerProjectileAttackHandler 收拢为 PlayerWeaponSystem。
使用 WeaponRuntimeInstance 保存每把武器的运行时状态。
PlayerWeaponSystem 维护 activeWeapons 列表，每把武器独立冷却和攻击。
```

2. 武器逻辑最初仍残留枚举分支，扩展性不足。
```text
移除 WeaponAttackType、WeaponDamageShape、ProjectileHitBehavior 等行为枚举。
将攻击方式、伤害形状、发射物命中行为分别拆成策略类。
WeaponConfig 只保存基础数值和策略引用，不再由调用层硬编码行为分支。
```

3. 发射物对象池最初只适合单一 projectile prefab。
```text
PlayerWeaponSystem 改为按 projectile prefab 维护多个 ComponentPool。
activeProjectilePools 记录每个已发射 ProjectileController 对应的池。
回收时按实际池归还，避免多武器发射物串池。
```

4. 大量敌人时每个敌人每 FixedUpdate 分离查询开销过高。
```text
移除 EnemyAISystem 中的 Physics2D.OverlapCircleNonAlloc 分离查询。
复用 TargetingSystem 的敌人注册和空间网格结果。
分离方向低频缓存，并按 instance id 错峰计算。
使用 maxSeparationChecks 限制每个敌人的分离候选数量。
```

5. Trigger 回调中存在无效组件查找。
```text
敌人攻击只处理 Player Layer。
子弹命中只处理 Enemy Layer。
先做 LayerMask 过滤，再调用 GetComponentInParent，减少无效查找。
```

## 10. M5 掉落和拾取实现计划

目标：实现敌人死亡掉落和玩家拾取。

计划脚本：

| 脚本 | 职责 |
| --- | --- |
| DropSystem.cs | 根据死亡事件生成掉落 |
| DropTableConfig.cs | 掉落表配置 |
| ItemConfig.cs | 物品配置 |
| PickupController.cs | 掉落物表现和拾取检测 |
| PickupSystem.cs | 处理拾取效果 |

初始物品：

| 物品 | 效果 |
| --- | --- |
| XP Orb | 增加经验或分数 |
| Heal Orb | 回复生命值 |

流程：

```text
EnemyKilledEvent
  -> DropSystem 读取 DropTable
  -> 生成 Pickup
  -> 玩家进入拾取范围
  -> 执行 Item 效果
  -> 回收 Pickup
```

验收标准：

1. 敌人死亡后可以生成掉落物。
2. 玩家靠近后可以拾取掉落物。
3. 拾取后产生对应效果。

实际实现记录：

状态：已完成

### 10.1 最终实现范围

M5 已实现敌人死亡掉落、玩家范围检测、掉落物吸附、拾取效果执行、掉落物对象池回收和拾取特效对象池回收。

当前实现包含两类初始物品：

| 物品 | 配置资源 | 掉落预制体 | 拾取效果 |
| --- | --- | --- | --- |
| XP Orb | `ItemConfig_XPOrb` | `Small Gem.prefab` | 增加经验 |
| Score Orb | `ItemConfig_Score Orb` | `Big Gem.prefab` | 增加分数 |
| Heal Orb | `ItemConfig_HealOrb` | `Food Drop.prefab` | 回复玩家生命值 |

### 10.2 新增和调整脚本

| 脚本 | 类型 | 职责 |
| --- | --- | --- |
| `ItemEffectType.cs` | 枚举 | 定义物品效果类型，目前支持 `Experience` 和 `Heal` |
| `ItemConfig.cs` | ScriptableObject | 定义物品 ID、名称、效果类型、效果数值、掉落预制体和拾取特效预制体 |
| `DropTableEntry.cs` | 配置数据 | 定义单个掉落项的物品、掉落概率、最小数量和最大数量 |
| `ItemDropRoll.cs` | 运行时结果 | 表示一次掉落表随机后的物品和数量 |
| `DropTableConfig.cs` | ScriptableObject | 保存掉落项列表，并根据概率生成掉落结果 |
| `ItemDropGenerator.cs` | 生成系统 | 根据敌人配置或默认掉落表生成拾取物，并管理拾取物对象池 |
| `PickupController.cs` | 运行时实体 | 表示单个掉落物，处理初始化、吸附移动、触发拾取和回收 |
| `PickupSystem.cs` | 静态系统 | 维护活跃掉落物列表，执行物品效果，管理拾取特效对象池 |
| `PickupEffectController.cs` | 特效实体 | 控制拾取特效播放，并在播放结束后归还对象池 |
| `PlayerPickupSystem.cs` | 玩家系统 | 根据玩家拾取半径扫描活跃掉落物，触发吸附或直接拾取 |
| `PlayerProgress.cs` | 玩家数据 | 保存经验和分数，并暴露变化事件供后续 UI 接入 |

既有脚本调整：

1. `EnemyConfig` 增加 `DropTableConfig` 引用，用于配置敌人专属掉落表。
2. `PlayerConfig` 增加拾取半径、收集距离和吸附速度。
3. `Player` 增加 `PlayerPickupSystem` 和 `PlayerProgress` 组件依赖及缓存。
4. `CharacterSpawnManager` 在生成玩家后初始化 `PlayerPickupSystem`。
5. `Health` 增加 `Heal` 方法，用于治疗类物品恢复生命值。

### 10.3 资源配置

新增资源目录：

```text
Assets/_Project/ScriptableObjects/Items
Assets/_Project/ScriptableObjects/DropTables
```

已创建默认资源：

| 资源 | 说明 |
| --- | --- |
| `ItemConfig_XPOrb.asset` | XP 掉落物配置，效果数值为经验增加量 |
| `ItemConfig_HealOrb.asset` | 治疗掉落物配置，效果数值为回血量 |
|  `ItemConfig_Score Orb` | Score 掉落物配置 增加分数 |
| `DropTableConfig_Default.asset` | 默认掉落表，包含 XP Orb 和低概率 Heal Orb，必定掉落Score Orb |

`Generator.prefab` 上的 `ItemDropGenerator` 已配置 `DropTableConfig_Default` 作为默认掉落表。
单个敌人可以通过 `EnemyConfig.DropTableConfig` 覆盖默认掉落表；若敌人未配置专属掉落表，则自动使用默认掉落表。

### 10.4 运行流程

最终运行流程如下：

```text
敌人 Health 归零
  -> DeathEvent.CallDeathEvent
  -> EnemyDropHandler 监听死亡事件
  -> ItemDropGenerator.GenerateDrop(position, enemyConfig)
  -> 读取 enemyConfig.DropTableConfig 或默认掉落表
  -> DropTableConfig.RollDrops 计算掉落结果
  -> ItemDropGenerator 从拾取物对象池获取 PickupController
  -> PickupController 初始化物品配置、数量和回收 owner
  -> PickupController 注册到 PickupSystem.ActivePickups
  -> PlayerPickupSystem 每帧扫描 PickupSystem.ActivePickups
  -> 进入拾取半径后 PickupController.BeginAttract
  -> 进入收集距离后 PickupController.TryCollect
  -> PickupSystem.CompletePickup
  -> PickupSystem.ApplyPickup 执行经验或回血
  -> PickupSystem 从拾取特效对象池获取 PickupEffectController
  -> PickupEffectController 在玩家当前位置播放粒子
  -> ItemDropGenerator 回收 PickupController
  -> PickupEffectController 播放结束后归还特效对象池
```

### 10.5 对象池策略

M5 中有两类高频对象使用对象池：

1. 掉落物对象池：由 `ItemDropGenerator` 按 `ItemConfig.ResourceId` 维护，每种物品一个 `ComponentPool<PickupController>`。
2. 拾取特效对象池：由 `PickupSystem` 按拾取特效 prefab 维护，每种特效一个 `ComponentPool<PickupEffectController>`。

对象池接入原因：

1. 敌人死亡和玩家拾取在生存类玩法中属于高频行为。
2. 掉落物和拾取粒子如果频繁 `Instantiate` / `Destroy`，容易造成 GC 和帧波动。
3. 当前项目 M3 已提前引入通用 `ComponentPool<T>`，M5 延续该实现方式。

### 10.6 职责边界

最终职责拆分如下：

1. `PickupController` 只负责单个掉落物的运行时状态，不保存全局掉落物列表。
2. `PickupSystem` 维护全局活跃掉落物列表，统一提供注册、注销和查询入口。
3. `PlayerPickupSystem` 只负责玩家侧检测逻辑，不直接拥有掉落物生命周期。
4. `ItemDropGenerator` 只负责掉落生成和掉落物对象池回收。
5. `PickupSystem` 负责物品效果和拾取特效播放，因为二者都发生在“拾取完成”这一统一时刻。

这样可以避免单个实体组件承担全局管理职责，也方便后续 M7 UI 或调试工具读取当前活跃掉落物。

### 10.7 拾取效果

当前 `PickupSystem.ApplyPickup` 支持：

| 效果类型 | 行为 |
| --- | --- |
| `Experience` | 调用 `PlayerProgress.AddExperience`，同时增加经验和分数 |
| `Heal` | 调用 `Health.Heal`，在最大生命值内回复玩家生命 |

拾取特效播放规则：

1. 拾取完成后在玩家当前位置播放，而不是在掉落物原始位置播放。
2. 特效对象从拾取特效对象池获取。
3. `PickupEffectController` 会主动 `Clear` 并 `Play` 粒子系统。
4. 粒子播放结束后，特效对象归还对象池。

### 10.8 验收结果

| 验收项 | 结果 |
| --- | --- |
| 敌人死亡后生成掉落物 | 通过 |
| 玩家进入拾取半径后掉落物吸附 | 通过 |
| 玩家进入收集距离后完成拾取 | 通过 |
| XP Orb 增加经验和分数 | 通过 |
| Heal Orb 回复玩家生命值 | 通过 |
| 掉落物拾取后回收到对象池 | 通过 |
| 拾取特效在玩家位置播放 | 通过 |
| 拾取特效播放结束后回收到对象池 | 通过 |

### 10.9 和原计划的差异

原计划中的 `DropSystem.cs` 没有单独创建，实际由 `ItemDropGenerator` 承担掉落生成职责。
这样做的原因是项目中已经存在 `Generator` 聚合入口和 `ItemDropGenerator` 预留脚本，直接补完该脚本可以保持当前生成器结构一致。

原计划中的 `PickupSystem.cs` 保留为静态系统，负责活跃掉落物列表、物品效果和拾取特效池。
玩家侧检测逻辑没有写进 `PickupSystem`，而是拆到 `PlayerPickupSystem`，便于后续不同角色拥有不同拾取半径和吸附速度。

### 10.10 后续扩展点

1. M7 UI 可监听 `PlayerProgress.OnExperienceChanged` 和 `OnScoreChanged` 显示经验、分数或击杀收益。
2. 后续可在 `ItemEffectType` 中扩展金币、磁铁、临时 Buff、清屏炸弹等效果。
3. Boss 或精英敌人可通过专属 `DropTableConfig` 配置更高价值掉落。
4. 若掉落数量大幅上升，可将 `PlayerPickupSystem` 的线性扫描替换为空间分区查询。
5. 可将拾取特效池的根节点挂到场景内统一运行时容器，方便调试层级。

## 11. M6 Buff/Debuff/AOE 实现计划

目标：实现基础状态效果系统和范围效果系统。

计划脚本：

| 脚本 | 职责 |
| --- | --- |
| StatusEffectConfig.cs | 状态效果配置 |
| StatusEffectInstance.cs | 状态效果运行时实例 |
| StatusEffectSystem.cs | 状态添加、刷新、叠加、tick、移除 |
| AOEConfig.cs | AOE 配置 |
| AOEZoneController.cs | AOE 范围对象 |
| AOESystem.cs | AOE 伤害和状态施加 |

初始状态：

| 状态 | 类型 | 效果 |
| --- | --- | --- |
| Haste | Buff | 玩家移动速度提升 |
| Slow | Debuff | 敌人移动速度降低 |
| Poison | Debuff | 敌人持续受到伤害 |

初始 AOE：

| AOE | 效果 |
| --- | --- |
| Shock Ring | 玩家周围范围伤害 |
| Slow Field | 区域内敌人减速 |

验收标准：

1. Buff 可以临时修改玩家属性。
2. Debuff 可以临时修改敌人属性或造成周期伤害。
3. AOE 可以影响范围内多个目标。
4. 状态效果可通过 ScriptableObject 配置。

实际实现记录：

状态：已完成

### 11.1 最终实现范围

M6 已实现可配置状态效果系统、Buff/Debuff 运行时管理、修饰器模式、全局状态 Tick、武器命中状态附加、AOE 范围效果、AOE 对象池和 AOE 特效重播/视觉生命周期处理。

当前实现支持：

| 能力 | 说明 |
| --- | --- |
| Buff | 通过状态效果修改玩家或敌人的运行时属性，例如移动速度提升 |
| Debuff | 通过状态效果修改敌人属性或造成周期伤害，例如减速和中毒 |
| 状态叠加 | 支持 Refresh、Stack、Independent 三种叠加策略 |
| 修饰器模式 | 状态实例只管理生命周期，具体行为由 `StatusEffectModifier` 运行时对象执行 |
| 全局 Tick | 状态系统不为每个 Buff 创建 `Update`，由 `StatusEffectTickSystem` 统一驱动 |
| 状态来源 | 每个状态实例保存 `StatusEffectSourceContext`，记录来源对象、施加者、伤害归属、武器和 AOE 配置 |
| 武器命中状态 | `WeaponConfig.onHitStatusEffects` 统一定义武器命中后附加的 Debuff/Buff |
| AOE | 支持持续区域、周期伤害、范围命中、多目标影响和对象池复用 |
| AOE 视觉 | 支持池化后重播粒子/动画，并通过 `visualLifetime` 处理瞬发 AOE 的视觉保留 |

### 11.2 新增和调整脚本

| 脚本 | 类型 | 职责 |
| --- | --- | --- |
| `StatusEffectConfig.cs` | ScriptableObject | 定义状态 ID、类型、目标类型、叠加规则、持续时间、最大层数和修饰器列表 |
| `StatusEffectType.cs` | 枚举 | 区分 Buff、Debuff 等状态类型 |
| `StatusEffectTargetType.cs` | 枚举 | 限定状态可施加到玩家、敌人或任意实体 |
| `StatusEffectStackingBehavior.cs` | 枚举 | 定义 Refresh、Stack、Independent 三种叠加行为 |
| `IStatusEffect.cs` | 接口 | 定义状态实例生命周期：Apply、Tick、Remove、Refresh、Stack |
| `StatusEffectInstance.cs` | 运行时实例 | 保存状态剩余时间、层数和运行时修饰器，并转发生命周期回调 |
| `StatusEffectManager.cs` | 实体组件 | 挂在玩家或敌人身上，负责接收、刷新、叠加、移除和清理状态 |
| `StatusEffectContext.cs` | 运行时上下文 | 缓存目标实体、Health、RuntimeStats 等状态行为所需信息 |
| `StatusEffectSourceContext.cs` | 施加来源上下文 | 保存状态来源对象、施加者、伤害归属对象、武器配置、AOE 配置和强度倍率 |
| `StatusEffectTickSystem.cs` | 全局系统 | 按固定间隔广播状态 Tick，避免每个状态实例独立 `Update` |
| `StatusEffectModifier.cs` | ScriptableObject 基类 | 定义状态具体行为的配置入口 |
| `IStatusEffectModifierRuntime.cs` | 运行时接口 | 定义修饰器运行时对象的 Apply、Tick、Remove、Refresh、Stack 回调 |
| `MoveSpeedStatusEffectModifier.cs` | 状态修饰器 | 根据层数修改目标 `RuntimeStats.MoveSpeedMultiplier` |
| `PeriodicDamageStatusEffectModifier.cs` | 状态修饰器 | 按内部间隔对目标造成周期伤害 |
| `RuntimeStats.cs` | 实体属性 | 保存运行时属性倍率，当前用于移动速度倍率 |
| `AOEConfig.cs` | ScriptableObject | 定义 AOE ID、半径、持续时间、视觉生命周期、Tick 间隔、伤害、目标上限、区域状态和 prefab |
| `AOESpawnPositionMode.cs` | 枚举 | 定义 AOE 生成在目标、攻击者或攻击原点 |
| `AOEZoneController.cs` | AOE 实体 | 管理单个 AOE 区域的生命周期、范围查询、周期伤害、状态施加和特效重播 |
| `AOESystem.cs` | 全局系统 | 生成和回收 AOE，维护 AOE 对象池，并用单一 Runner 推进活动 AOE |
| `AOEDebugSpawner.cs` | 调试组件 | 通过按键在玩家或当前物体位置生成指定 AOE |
| `AOEWeaponAttackStrategy.cs` | 武器攻击策略 | 将武器攻击桥接到 `AOESystem.SpawnAOE` |
| `WeaponDamageApplier.cs` | 战斗系统 | 在统一伤害入口中附加 `WeaponConfig.onHitStatusEffects` |

既有脚本调整：

1. `Player`、`Enemy` 增加 `StatusEffectManager`、`RuntimeStats` 相关缓存或运行时补齐。
2. `PlayerController`、`EnemyController` 的移动速度读取接入 `RuntimeStats`，使 Haste/Slow 能临时影响移动速度。
3. `WeaponConfig` 增加 AOE 配置、AOE 生成位置、命中状态列表和状态施加概率。
4. `WeaponAttackServices` 增加 `SpawnAOE` 桥接方法，把来源 `WeaponConfig` 和施加者信息传给 AOE。
5. `SingleTargetDamageStrategy` 和 `AreaDamageStrategy` 改为通过 `WeaponDamageApplier` 统一处理武器命中状态。
6. `GameResources` 增加 StatusEffectConfig 和 AOEConfig 的资源列表与按 ID 查询缓存。

### 11.3 资源配置

新增资源目录：

```text
Assets/_Project/ScriptableObjects/Status
Assets/_Project/ScriptableObjects/Status/Modifiers
Assets/_Project/ScriptableObjects/AOE
```

状态资源：

| 资源 | 类型 | 行为 |
| --- | --- | --- |
| `StatusEffectConfig_Haste.asset` | Buff | 通过移动速度修饰器提升移动速度 |
| `StatusEffectConfig_Slow.asset` | Debuff | 通过移动速度修饰器降低移动速度 |
| `StatusEffectConfig_Poison.asset` | Debuff | 通过周期伤害修饰器造成持续伤害，并支持叠层 |
| `StatusEffectModifier_HasteMoveSpeed.asset` | Modifier | 配置加速倍率 |
| `StatusEffectModifier_SlowMoveSpeed.asset` | Modifier | 配置减速倍率 |
| `StatusEffectModifier_PoisonDamage.asset` | Modifier | 配置毒伤 Tick 间隔和每层伤害 |

AOE 资源：

| 资源 | 行为 |
| --- | --- |
| `AOEConfig_Daggers.asset` | 持续区域伤害，使用 AOE prefab 和对象池生成 |
| `AOEConfig_SlowPoisonField.asset` | 持续区域伤害，状态效果由来源武器的命中状态配置提供 |

武器示例：

| 资源 | 行为 |
| --- | --- |
| `WeaponAttackStrategy_AOE.asset` | 通用 AOE 武器攻击策略 |
| `WeaponConfig_PoisonField.asset` | 使用 AOE 攻击策略生成 `aoe_slow_poison_field`，并在命中时附加 Slow 和 Poison |

### 11.4 运行流程

状态效果流程：

```text
外部系统调用 StatusEffectManager.ApplyEffect(config)
  -> StatusEffectConfig.CanApplyTo 校验目标类型
  -> 创建 StatusEffectInstance
  -> StatusEffectManager 根据 stackingBehavior 处理新增、刷新、叠层或独立实例
  -> StatusEffectInstance.OnApply 调用所有运行时修饰器
  -> StatusEffectTickSystem 按固定间隔广播 Tick
  -> StatusEffectManager.TickEffects 更新剩余时间并触发修饰器 OnTick
  -> 持续时间归零或实体死亡时调用 OnRemove 并清理状态
```

武器命中 Debuff 流程：

```text
PlayerWeaponSystem 自动攻击
  -> WeaponAttackStrategy 执行攻击
  -> Direct / Projectile / AOE 产生命中
  -> WeaponDamageApplier.ApplySingleTargetDamage 或 ApplyAreaDamage
  -> CombatSystem.ApplyDamage
  -> 目标仍存活时读取 WeaponConfig.onHitStatusEffects
  -> 构建 StatusEffectSourceContext
  -> 依据 onHitStatusApplyChance 对目标 StatusEffectManager.ApplyEffect(config, sourceContext)
```

AOE 流程：

```text
AOEWeaponAttackStrategy.TryExecute
  -> 根据 AOESpawnPositionMode 计算生成位置
  -> WeaponAttackServices.SpawnAOE
  -> AOESystem.SpawnAOE
  -> 从 ComponentPool<AOEZoneController> 获取区域对象或运行时创建
  -> AOEZoneController.Initialize
  -> 重播 playOnAwake 粒子和 Animator
  -> Tick 时通过 TargetingSystem.CollectAliveEnemiesInRange 收集范围内敌人
  -> 对每个目标执行伤害和来源武器命中状态，并传递 AOE 来源上下文
  -> 生命周期结束后回收到 AOE 对象池
```

### 11.5 设计调整

1. 原计划中的 `StatusEffectSystem.cs` 没有作为单一脚本实现，实际拆分为 `StatusEffectManager` 和 `StatusEffectTickSystem`：前者挂在实体上管理状态列表，后者提供全局 Tick。
2. 状态具体行为没有写死在 `StatusEffectInstance` 内，而是通过 `StatusEffectModifier` 和 `IStatusEffectModifierRuntime` 实现修饰器模式，便于继续扩展护甲、暴击、攻速、治疗、燃烧等效果。
3. Debuff 没有作为 AOE 独有能力处理，而是上移到 `WeaponConfig.onHitStatusEffects`。这样单体武器、投射物武器和 AOE 武器都能复用同一套命中状态逻辑。
4. `AOEConfig.statusEffects` 被保留，但语义调整为“区域自身天然状态”，例如地图毒池或怪物技能；武器造成的 Debuff 由 `WeaponConfig` 管理。
5. AOE 没有直接写入传统武器伤害策略，而是通过 `AOEWeaponAttackStrategy` 桥接武器系统和 AOE 系统，保持 AOE 可被武器、怪物技能、场景机关等复用。
6. AOE 对象使用 `ComponentPool<AOEZoneController>` 复用，并由 `AOESystemRunner` 单一 Update 推进所有活动区域，避免每个系统散落运行时入口。
7. 对于 `duration = 0` 的瞬发 AOE，新增 `visualLifetime`，保证玩法效果瞬时结算但特效不会同帧消失。

### 11.6 验收结果

| 验收项 | 结果 |
| --- | --- |
| Buff 可以临时修改玩家属性 | 通过，Haste 可通过移动速度修饰器影响 `RuntimeStats` |
| Debuff 可以临时修改敌人属性 | 通过，Slow 可降低敌人移动速度 |
| Debuff 可以造成周期伤害 | 通过，Poison 可通过周期伤害修饰器造成持续伤害 |
| 状态效果可通过 ScriptableObject 配置 | 通过，状态配置和修饰器均为 ScriptableObject |
| 状态效果支持叠加/刷新 | 通过，支持 Refresh、Stack、Independent |
| 状态 Tick 不依赖每个 Buff 独立 Update | 通过，由 `StatusEffectTickSystem` 统一 Tick |
| 状态效果保存来源、施加者和伤害归属 | 通过，`StatusEffectSourceContext` 随每个状态实例保存 |
| 单体/范围/投射物武器可复用命中 Debuff | 通过，统一接入 `WeaponDamageApplier` |
| AOE 可以影响范围内多个目标 | 通过，AOE 通过 `TargetingSystem` 收集范围敌人并逐个结算 |
| AOE 可由武器系统触发 | 通过，`AOEWeaponAttackStrategy` 已接入 |
| AOE 高频对象可复用 | 通过，AOE prefab 使用 `ComponentPool<AOEZoneController>` |
| AOE 池化特效可重播 | 通过，生成时重播粒子和 Animator，回池时清理粒子状态 |
| 项目编译 | 通过，`dotnet build .\Assembly-CSharp.csproj` 为 0 错误 0 警告 |

### 11.7 后续扩展点

1. M7 可基于 `StatusEffectManager.OnEffectApplied` 和 `OnEffectRemoved` 显示当前 Buff/Debuff 图标。
2. 可继续增加护甲、攻速、暴击、吸血、灼烧、冰冻、眩晕等 `StatusEffectModifier`。
3. 可基于 `StatusEffectSourceContext.DamageOwnerObject` 扩展击杀归属、伤害统计、吸血和成就统计。
4. 可增加 AOE 目标层级、阵营过滤、命中次数限制和不同形状区域。
5. 后续如果 AOE 数量继续上升，可在 TargetingSystem 内部继续优化空间分区、查询批处理或缓存策略，而不是修改 AOE 调用层。

## 12. M7 UI、调试和优化实现计划

目标：完善展示效果和基础性能优化。

计划功能：

1. 显示玩家 HP。
2. 显示击杀数。
3. 显示生存时间。
4. 显示当前 Buff。
5. 显示敌人数量和 FPS。
6. 为敌人、子弹、掉落物、AOE 接入对象池。

验收标准：

1. UI 信息清晰。
2. 高频对象不再频繁 Instantiate/Destroy。
3. 连续运行 3 分钟无明显卡顿。
4. Console 无持续报错。

实际实现记录：

```text
状态：已完成

M7 阶段实际完成内容如下：

1. UI 架构整理
   - 新增 `UIManager` 统一管理 UI 面板显隐。
   - `UIManager` 首次调用时会创建并缓存 Canvas 与 EventSystem。
   - 面板不再依赖场景上预放对象，而是通过预设体创建并挂载到 Canvas 根节点。
   - UI 面板采用 MVP 思路拆分为 View / Presenter。
   - View 调整为严格 Passive View，只保留手动绑定的控件引用，不包含刷新、格式化、监听和业务逻辑。
   - `CharacterItem` 拆分为 `CharacterItem_View` 和 `CharacterItem_Presenter`，单个角色条目的渲染与点击回调由条目 Presenter 负责，`Player_Presenter` 只维护角色列表和购买/选择业务。

2. 主菜单与关卡选择
   - 新增 `StageConfig` ScriptableObject，用于保存关卡 id、关卡名、关卡 level、关卡图片、刷怪配置和关卡持续时间。
   - `MainMenu_View` 只保留关卡名称、关卡 level、关卡图片、播放按钮等控件引用。
   - `MainMenu_Presenter` 根据 `GameResources.StageConfigs` 切换并显示关卡信息。
   - 点击 Play 后通过 `GameLaunchContext` 保存待进入关卡和当前选择角色，并在 Game 场景加载完成后配置 `Generator`。
   - `Generator.SpawnSystem` 会根据当前关卡替换刷怪配置。
   - `CharacterSelectionSystem` 会根据玩家当前选择角色生成对应玩家对象，并重新绑定摄像机跟随目标。

3. 玩家数据与角色选择
   - 新增 `PlayerData` 保存玩家金币、已解锁角色、已解锁关卡和当前选中角色。
   - 新增 `PlayerDataManager` 负责加载、保存、修复和通知玩家数据变化。
   - 玩家数据通过项目中的二进制序列化工具保存到本地。
   - 角色选择面板会读取玩家本地数据，判断角色是否已解锁、是否已选中、金币是否足够购买。
   - 未解锁角色显示价格，已解锁角色显示 Available 或 Selected。
   - 解锁角色会扣除金币、写入本地数据并触发玩家数据变化事件。
   - 选择角色会保存 `selectedCharacterId`，后续进入关卡时按该角色生成玩家。

4. 关卡运行与结算
   - 新增 `StageRuntimeManager` 管理当前关卡运行时间。
   - 关卡时间随游戏时间推进，剩余时间变化时通过事件通知 UI。
   - 时间到达配置时长后触发通关。
   - 新增 `LevelManager` 管理关卡内金币、击杀数、暂停、恢复、退出、胜利和失败结算。
   - 玩家拾取 `ItemEffectType.Gold` 类型掉落物时，`PickupSystem` 触发金币拾取事件，`LevelManager` 累加本局金币并通知 `Game_Presenter` 刷新金币文本。
   - 敌人死亡时触发 `EnemyKilled` 事件，`LevelManager` 累加击杀数并通知 `Game_Presenter` 刷新击杀数文本。
   - 游戏结束时无论胜利或失败都会显示 `ChestPanel`。
   - `Chest_Presenter` 显示本局获得金币，点击 Take 后将奖励写入 `PlayerDataManager` 并返回 MainMenu 场景。

5. 暂停、设置和音频
   - `GamePanel` 暂停按钮会调用 `LevelManager.PauseGame()` 并显示 `PausePanel`。
   - `PausePanel` 负责恢复游戏、退出当前关卡，以及显示音效/音乐设置。
   - `SettingPanel` 只管理音频设置，不再承担不同场景下的暂停或退出职责。
   - 新增 `SettingData` 与 `SettingDataManager`，用于保存音效开关、音乐开关、音效音量和音乐音量。
   - 设置数据通过二进制序列化工具持久化到本地。
   - 新增 `AudioConfig` ScriptableObject，将音乐和音效资源通过 id 缓存到 `GameResources`。
   - 新增 `AudioManager` 懒汉单例，运行时自动创建并 `DontDestroyOnLoad`。
   - `AudioManager` 统一负责背景音乐播放、音效播放、音量设置应用和场景加载后的默认音乐播放。
   - 音效播放使用 `SoundEffect` 对象池复用，避免频繁创建和销毁音效 GameObject。

6. 玩家血条与局内显示
   - 新增 `PlayerHealthBar`，玩家第一次受伤后显示血条，并在生命变化时持续刷新。
   - 血条不通过缩放实现，而是根据最大血量、当前血量和 Mask 的 X 轴长度计算偏移量，移动 `Healthbar Mask` 的本地 X 坐标。
   - `Health` 新增生命变化事件，供血条等显示组件监听。
   - `Game_Presenter` 负责局内金币、击杀数和计时器文本刷新。

7. 资源与配置集中管理
   - `GameResources` 增加玩家、关卡、音频等配置缓存。
   - 外部系统通过配置 id 获取玩家、关卡和音频资源，降低场景硬引用。
   - `StageConfig`、`PlayerConfig`、`AudioConfig` 均通过 ScriptableObject 管理，便于后续扩展更多关卡、角色和音频资源。

8. 对象池与性能优化
   - 敌人生成继续使用 `EnemySpawnManager` 和 `ComponentPool` 复用敌人实例。
   - 发射物、掉落物、拾取特效、AOE 和音效均采用对象池或已有池化结构减少高频 Instantiate/Destroy。
   - 音效对象池默认预热，播放结束后释放回池。
   - UI 面板由 `UIManager` 缓存，避免重复查找和散落管理。

9. 和原计划不一致的地方
   - 原计划只写“显示玩家 HP”，实际实现为玩家受伤后显示头顶血条，并随生命变化自动更新。
   - 原计划中的“显示生存时间”实际改为显示关卡剩余时间，并由 `StageRuntimeManager` 统一驱动。
   - 原计划中的“显示当前 Buff”“显示敌人数量和 FPS”本阶段未做成正式 UI；当前重点落在主菜单、角色、关卡、暂停、设置、结算和核心局内信息。
   - 原计划中的“调试信息”没有新增独立调试面板，主要通过系统事件、面板显示和编译验证确认链路。
   - M7 实际范围扩展到了玩家数据持久化、关卡配置、音频配置、UI 管理器、结算流程和本地设置存档，为后续 M8 演示准备打基础。

10. 验收结果
   - 主菜单面板可通过 `UIManager` 创建和显示。
   - Canvas 与 EventSystem 可由 `UIManager` 自动创建并缓存。
   - 主菜单可切换关卡并显示关卡名、关卡 level 和关卡图片。
   - 点击 Play 后可进入 Game 场景，并按当前选择关卡配置刷怪。
   - 当前选择角色会在 Game 场景生成。
   - 角色选择面板可显示已解锁、未解锁、Available、Selected 和价格状态。
   - 未解锁角色可消耗金币解锁，已解锁角色可选择。
   - 本局拾取金币后 GamePanel 金币文本会更新。
   - 击杀敌人后 GamePanel 击杀数文本会更新。
   - 关卡倒计时会随游戏时间更新，时间结束触发通关。
   - 暂停按钮可打开 PausePanel，Back 可恢复游戏，Exit 可退出当前关卡。
   - 游戏胜利或失败都会打开 ChestPanel。
   - ChestPanel 可显示本局获得金币，Take 后金币写入玩家本地数据并返回 MainMenu。
   - 设置面板和暂停面板可调整音乐/音效开关与音量，并持久化到本地。
   - 背景音乐由 `AudioManager` 播放，音效通过对象池播放。
   - 玩家受伤后血条显示，并随当前血量更新。
   - View 层已调整为严格 Passive View，显示逻辑由 Presenter 负责。
```

## 13. 热更新接口实现计划

当前阶段不实现完整热更新，只做接口和架构预留。

计划设计：

| 模块 | 职责 |
| --- | --- |
| ConfigRepository | 统一配置读取入口 |
| HotUpdateService | 后续协调版本检查和更新流程 |
| RemoteConfigService | 后续拉取远程 JSON 配置 |

v0.1 原则：

1. 玩法系统不直接依赖远程数据。
2. 配置先使用 ScriptableObject。
3. 后续如接入远程配置，应通过 `ConfigRepository` 扩展。

当前实现记录：

```text
仅规划，不实现。
```

## 14. 网络模块实现计划

当前阶段不实现完整网络模块，只做接口预留。

计划设计：

| 模块 | 职责 |
| --- | --- |
| NetworkService | 网络请求统一入口 |
| MockNetworkService | 无服务器时提供模拟数据 |
| HttpClientAdapter | 后续封装 UnityWebRequest |

后续可能支持：

1. 拉取远程配置。
2. 上传分数。
3. 获取排行榜。
4. 登录和账号数据。

v0.1 原则：

1. UI 和玩法系统不直接调用 `UnityWebRequest`。
2. 网络失败时可以使用本地默认配置。
3. 原型阶段以单机玩法闭环为主。

当前实现记录：

```text
仅规划，不实现。
```

## 15. 文档更新规则

每完成一个里程碑后更新本文档：

1. 将里程碑状态从“未开始”改为“已完成”或“进行中”。
2. 补充实际创建的脚本和 Prefab。
3. 记录实际实现方式。
4. 记录和原计划不一致的地方。
5. 记录遇到的问题和解决方案。

推荐提交信息：

```text
docs: add initial implementation document
docs: update implementation document for player movement
docs: update implementation document for enemy spawn
docs: update implementation document after combat milestone
```

## 16. v0.1 结论

当前 v0.1 实现文档的目标是为项目开发建立基础流程和落地计划。

后续开发优先级为：

```text
工程初始化
  -> 玩家移动和摄像机
  -> 敌人生成和追踪
  -> 自动攻击和战斗
  -> 掉落和拾取
  -> Buff/Debuff/AOE
  -> UI、对象池和调试优化
```

热更新接口和网络模块仅作为扩展预留，不影响当前 Demo 的核心玩法开发。
