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
| M4 自动攻击和战斗 | 未开始 | 自动索敌、攻击、伤害、死亡 |
| M5 掉落和拾取 | 未开始 | 敌人死亡掉落、玩家拾取 |
| M6 Buff/Debuff/AOE | 未开始 | 状态效果和范围效果 |
| M7 UI、调试和优化 | 未开始 | UI、对象池、调试信息、性能优化 |
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

```text
待 M4 完成后补充。
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

```text
待 M5 完成后补充。
```

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

```text
待 M6 完成后补充。
```

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
待 M7 完成后补充。
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
