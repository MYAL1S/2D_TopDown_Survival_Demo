# 2D 俯视角生存原型 - 技术分析文档 v0.1

## 1. 文档说明

本文档为项目早期技术分析文档，版本为 v0.1。

当前阶段目标是明确项目方向、核心玩法闭环、Unity 技术路线、基础架构拆分和后续扩展预留。文档不会在 v0.1 阶段覆盖所有实现细节，具体类设计、接口细节、性能数据和最终实现方案会在开发过程中逐步补充。

## 2. 项目背景

本项目是一个用于 U3D 实习生面试的 Unity 2D 俯视角生存原型 Demo。

项目重点不是最终美术品质，而是展示以下能力：

1. Unity 2D 基础开发能力。
2. 玩法系统拆解能力。
3. 代码结构和工程组织能力。
4. ScriptableObject 数据驱动设计意识。
5. 对 Buff、Debuff、AOE、对象池、热更新接口、网络模块等扩展方向的理解。

## 3. 项目目标

实现一个基础但可扩展的 2D 俯视角生存玩法闭环：

```text
玩家移动
  -> 摄像机跟随
  -> 敌人从屏幕外生成
  -> 敌人追踪玩家
  -> 玩家自动攻击
  -> 敌人死亡
  -> 掉落物品
  -> 玩家拾取
  -> 获得成长或临时效果
```

## 4. 需求分析

### 4.1 基础需求

| 功能 | 描述 | 优先级 |
| --- | --- | --- |
| 玩家移动 | 支持 WASD 移动，预留虚拟摇杆输入 | P0 |
| 摄像机跟随 | 摄像机平滑跟随玩家 | P0 |
| 敌人生成 | 丧尸从当前屏幕外随机生成 | P0 |
| 敌人追踪 | 敌人持续向玩家移动 | P0 |
| 自动攻击 | 玩家自动攻击范围内敌人 | P0 |
| 敌人死亡 | 敌人 HP 归零后死亡 | P0 |
| 物品掉落 | 敌人死亡后生成掉落物 | P0 |
| 物品拾取 | 玩家靠近掉落物后自动拾取 | P0 |

### 4.2 进阶需求

| 功能 | 描述 | 优先级 |
| --- | --- | --- |
| Buff | 例如玩家加速、攻击提升 | P1 |
| Debuff | 例如敌人减速、中毒 | P1 |
| AOE | 范围伤害或范围状态效果 | P1 |
| 对象池 | 优化敌人、子弹、掉落物、AOE 创建销毁 | P1 |
| UI | 显示生命值、击杀数、生存时间、当前 Buff | P1 |
| 调试信息 | 显示敌人数量、FPS、对象池状态 | P2 |

### 4.3 扩展预留需求

| 功能 | 描述 | 优先级 |
| --- | --- | --- |
| 配置热更新接口 | 后续支持远程更新敌人、武器、掉落、状态数值 | P2 |
| 网络模块接口 | 后续支持排行榜、远程配置、账号数据等 | P2 |
| 资源热更新 | 后续可通过 Addressables 扩展 | P3 |
| 代码热修复 | 商业化方向预留，原型阶段不实现 | P3 |
| 实时多人网络 | 超出当前 Demo 范围，仅保留概念预留 | P3 |

## 5. 技术选型

### 5.1 引擎选择

本项目选择 **Unity 2D** 作为开发引擎。

选择原因：

1. Unity 是 U3D 岗位最常见的商业项目引擎。
2. Unity 2D 能较好支持俯视角移动、碰撞、特效、UI 和摄像机控制。
3. Prefab 和 ScriptableObject 适合构建可配置、可扩展的玩法系统。
4. Unity Input System 可以同时支持键盘、手柄和移动端输入。
5. Cinemachine 可以快速实现平滑摄像机跟随。
6. 后续可扩展 Android、iOS 或 PC 平台。

### 5.2 技术栈选型

| 技术 | 用途 |
| --- | --- |
| Unity 2022.3.62f3 LTS  | 主开发引擎 |
| C# | 主要开发语言 |
| Unity Input System | 输入管理 |
| Cinemachine | 摄像机跟随 |
| ScriptableObject | 玩法配置 |
| 2D Physics | 碰撞、命中、拾取检测 |
| Object Pool | 对象复用和性能优化 |
| Unity Test Framework | 后续单元测试和 PlayMode 测试 |

## 6. 总体架构设计

项目采用“表现层 + 玩法层 + 配置层 + 基础服务层”的结构。

```text
表现层
  Scene、Prefab、Animator、Sprite、VFX、UI、Camera

玩法层
  PlayerController、EnemyController、WeaponRuntime
  SpawnSystem、CombatSystem、DropSystem、StatusEffectSystem、AOESystem

配置层
  PlayerConfig、EnemyConfig、WeaponConfig、ItemConfig
  StatusEffectConfig、AOEConfig、DropTableConfig、SpawnConfig

基础服务层
  EventBus、ObjectPool、ConfigRepository、NetworkService、HotUpdateService
```

设计原则：

1. 不把所有逻辑写在一个 MonoBehaviour 中。
2. 配置数据尽量使用 ScriptableObject 管理。
3. 玩法系统之间通过事件或明确接口通信。
4. 高频对象使用对象池复用。
5. 热更新和网络模块先预留接口，不影响单机玩法开发。

## 7. 工程目录规划

目录结构：

```text
Assets/
  _Project/
    Art/
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
Docs/
  technical-analysis.md
  implementation-document.md
README.md
.gitignore
```

说明：

1. `Assets/_Project` 存放项目资源和脚本。
2. `Docs` 放项目文档，不放入 `Assets`，避免 Unity 导入非运行资源。
3. `README.md` 用于 GitHub 展示项目目标、功能和运行方式。

## 8. 核心系统分析

### 8.1 输入与移动系统

玩家移动支持 WASD，后续预留虚拟摇杆。

设计思路：

1. 输入读取和移动执行分离。
2. 输入系统输出统一的 `Vector2` 移动方向。
3. 移动时对斜向输入进行归一化，避免斜向速度过快。

### 8.2 摄像机系统

摄像机使用 Cinemachine Virtual Camera 跟随玩家。

设计目标：

1. 跟随平滑。
2. 不影响玩家移动逻辑。
3. 后续可以扩展震屏、边界限制和镜头缩放。

### 8.3 敌人生成系统

敌人应从当前屏幕外生成，而不是固定从地图边缘生成。

生成逻辑：

1. 获取摄像机当前可视区域。
2. 在可视区域外扩展一圈生成范围。
3. 随机选择上、下、左、右其中一侧。
4. 在该侧生成敌人。
5. 使用最大敌人数量限制，防止无限生成。

### 8.4 敌人 AI 系统

v0.1 阶段敌人 AI 使用简单追踪逻辑：

```text
方向 = 玩家位置 - 敌人位置
敌人速度 = 方向归一化 * 敌人移动速度
```

后续可扩展：

1. 敌人之间的分离力。
2. 不同敌人移动行为。
3. 简单寻路或障碍规避。

### 8.5 自动攻击系统

玩家自动攻击范围内最近敌人。

攻击流程：

```text
武器冷却完成
  -> 查询范围内敌人
  -> 选择最近目标
  -> 发射子弹或创建 AOE
  -> 命中敌人
  -> 计算伤害
  -> 敌人死亡时触发事件
```

### 8.6 掉落和拾取系统

敌人死亡后触发死亡事件，掉落系统根据掉落表生成物品。

拾取逻辑：

1. 玩家拥有拾取半径。
2. 掉落物进入拾取半径后被吸附或直接拾取。
3. 拾取后执行物品效果。
4. 掉落物回收到对象池。

### 8.7 Buff/Debuff 系统

Buff/Debuff 使用统一状态效果系统处理。

状态效果包含：

1. 持续时间。
2. 叠加规则。
3. 属性修正。
4. 周期伤害或治疗。

初期计划实现：

| 状态 | 类型 | 效果 |
| --- | --- | --- |
| Haste | Buff | 玩家移动速度提升 |
| Slow | Debuff | 敌人移动速度降低 |
| Poison | Debuff | 敌人持续受到伤害 |

### 8.8 AOE 系统

AOE 作为独立运行时对象存在。

AOE 可支持：

1. 一次性范围伤害。
2. 持续范围伤害。
3. 范围施加 Buff/Debuff。

初期计划实现：

| AOE | 效果 |
| --- | --- |
| Shock Ring | 玩家周围范围伤害 |
| Slow Field | 区域内敌人减速 |

## 9. 热更新接口预留

当前版本以单机玩法闭环为主，热更新接口不作为 P0 必做功能。

预留目标：

1. 后续可以远程更新敌人属性、武器数值、掉落表和状态效果参数。
2. 玩法系统不直接依赖具体配置来源。
3. 通过 `ConfigRepository` 统一读取配置。

规划方向：

```text
本地 ScriptableObject
  -> ConfigRepository
  -> Gameplay Systems

后续可扩展为：

远程 JSON / Addressables
  -> ConfigRepository
  -> Gameplay Systems
```

原型阶段可先实现本地配置，后续再扩展远程配置。

## 10. 网络模块预留

当前版本不实现完整网络玩法，但预留网络模块接口。

网络模块后续可能用于：

1. 拉取远程配置。
2. 上传分数。
3. 获取排行榜。
4. 登录或账号数据。
5. 活动数据。

设计原则：

1. UI 和玩法系统不直接调用 `UnityWebRequest`。
2. 通过 `NetworkService` 统一管理请求。
3. 没有后端时，可以使用 `MockNetworkService` 返回模拟数据。
4. 网络失败时应能降级使用本地默认配置。

## 11. 性能考虑

初期性能风险主要来自：

1. 大量敌人生成和销毁。
2. 大量子弹生成和销毁。
3. 每帧搜索目标。
4. Buff/Debuff 频繁刷新属性。

优化方向：

| 问题 | 方案 |
| --- | --- |
| 频繁 Instantiate/Destroy | 使用对象池 |
| 大量目标搜索 | 初期遍历，后期可用空间网格或 Physics2D.OverlapCircleNonAlloc |
| 频繁 GetComponent | 初始化时缓存引用 |
| GC 分配 | 高频逻辑避免 LINQ 和临时 List |
| 状态效果过多 | 状态变化时重算属性，而不是每帧完整重算 |

## 12. 开发里程碑

| 里程碑 | 目标 | 状态 |
| --- | --- | --- |
| M1 | 工程初始化、目录结构、基础场景 | 未开始 |
| M2 | 玩家移动和摄像机跟随 | 未开始 |
| M3 | 敌人生成和追踪 | 未开始 |
| M4 | 自动攻击和战斗 | 未开始 |
| M5 | 敌人死亡、掉落、拾取 | 未开始 |
| M6 | Buff、Debuff、AOE | 未开始 |
| M7 | UI、调试信息、对象池优化 | 未开始 |
| M8 | 文档整理、README、演示准备 | 未开始 |

## 13. 验收标准

### 13.1 基础验收

1. 玩家可以使用 WASD 移动。
2. 摄像机可以平滑跟随玩家。
3. 敌人从屏幕外生成。
4. 敌人会追踪玩家。
5. 玩家会自动攻击敌人。
6. 敌人受击后扣血，HP 归零后死亡。
7. 敌人死亡后掉落物品。
8. 玩家可以拾取掉落物。

### 13.2 进阶验收

1. 至少实现一个 Buff。
2. 至少实现一个 Debuff。
3. 至少实现一个 AOE。
4. 使用对象池管理高频生成对象。
5. 核心配置使用 ScriptableObject。
6. 项目运行 3 分钟无明显卡顿或持续报错。

### 13.3 工程验收

1. GitHub 仓库结构清晰。
2. README 说明项目目标、功能和运行方式。
3. Docs 中包含技术分析和实现文档。
4. Git 提交记录能体现阶段性开发过程。
5. `main` 分支保持可运行版本，`develop` 分支用于开发集成。

## 14. 当前版本结论

v0.1 阶段确定使用 Unity 2D 作为项目主技术路线，优先完成单机玩法闭环。

热更新接口和网络模块在当前阶段只做架构预留，不作为核心开发目标。项目后续会优先完成玩家移动、敌人生成、自动攻击、掉落拾取等 P0 功能，再逐步扩展 Buff、Debuff、AOE、对象池、UI 和调试能力。