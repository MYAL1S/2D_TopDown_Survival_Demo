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
| M2 玩家移动和摄像机 | 未开始 | WASD 移动、摄像机跟随 |
| M3 敌人生成和追踪 | 未开始 | 屏幕外刷怪、敌人追踪玩家 |
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

```text
待 M2 完成后补充。
```

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

```text
待 M3 完成后补充。
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