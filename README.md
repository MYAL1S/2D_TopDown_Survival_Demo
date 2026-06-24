# 2D TopDown Survival Demo

一个基于 Unity 的 2D 俯视角生存原型 Demo，目标是实现类似割草生存玩法的基础闭环，并展示可配置、可扩展、可维护的 Unity 工程组织方式。

当前项目已完成 M1-M7：工程初始化、玩家移动、摄像机跟随、敌人生成与追踪、自动战斗、掉落拾取、Buff/Debuff/AOE、对象池、主菜单、角色选择、关卡流程、局内 UI、暂停设置、音频管理、玩家数据持久化和结算流程。M8 文档整理与演示准备仍在推进中。

## 项目定位

本项目用于展示 Unity 2D 原型开发能力，重点不是最终美术品质，而是完整玩法闭环、系统拆分和后续扩展能力。

核心目标：

- 通过 WASD 或 UI 摇杆控制玩家移动。
- 摄像机跟随玩家，背景支持循环复用。
- 敌人从屏幕外生成，并持续追踪玩家。
- 玩家自动索敌并使用武器攻击。
- 敌人受伤、死亡、掉落物品。
- 玩家吸附并拾取掉落物，获得金币、经验、治疗或分数。
- 武器、敌人、掉落、状态、AOE、关卡等内容通过 ScriptableObject 配置。
- 高频对象通过对象池复用，降低运行时 Instantiate/Destroy 压力。
- UI 使用 MVP 思路拆分 View 与 Presenter。
- 玩家数据和音频设置保存到本地。

## 技术栈

| 项目 | 方案 |
| --- | --- |
| 引擎 | Unity 2022.3.62f3 LTS |
| 语言 | C# |
| 渲染 | Universal Render Pipeline 14.0.12 + 2D Renderer |
| 输入 | Unity Input System 1.14.2 |
| 摄像机 | Cinemachine 2.10.7 |
| UI | UGUI + TextMeshPro |
| 配置 | ScriptableObject |
| 持久化 | 本地二进制序列化 |
| 架构 | 组件化玩法系统 + 事件通信 + MVP UI |
| 性能 | ComponentPool 对象池 |

## 当前功能

### 玩家、输入与摄像机

- 支持 WASD 与 UI 摇杆输入。
- `InputSystemAdapter` 统一输出移动方向，摇杆输入优先于键盘输入。
- 玩家移动由 Rigidbody2D velocity 驱动，斜向输入会归一化。
- `PlayerConfig` 配置角色基础属性、速度、资源 ID 和预制体。
- `CameraTargetBinder` 在角色生成后重新绑定 Cinemachine Follow 目标。
- `InfiniteBackground` 通过固定背景瓦片复用实现连续背景。

### 敌人与刷怪

- 敌人从摄像机视野外生成。
- `SpawnConfig` 通过曲线配置刷怪频率、最大存活数量和批量生成数量。
- 支持按关卡使用不同刷怪配置。
- 支持按时间解锁不同敌人资源。
- 敌人使用 `EnemyAISystem` 追踪玩家，并加入近距离分离推力降低重叠。
- 敌人死亡后从活跃列表移除并回收到对象池。

### 战斗与武器

- `PlayerWeaponSystem` 支持多武器同时持有和独立冷却。
- `TargetingSystem` 负责范围内目标收集和最近目标查询。
- 武器攻击、伤害形状、投射物命中行为使用策略模式扩展。
- 已接入直接命中、投射物、范围伤害和 AOE 武器。
- `CombatSystem` 和 `WeaponDamageApplier` 提供统一伤害入口，并负责附加命中状态。
- 敌人可通过接触玩家造成伤害。

### 掉落与拾取

- 敌人死亡时由 `EnemyDropHandler` 触发掉落。
- `DropTableConfig` 配置掉落表和权重。
- `ItemConfig` 配置经验、治疗、分数、金币等拾取效果。
- 掉落物进入玩家吸附范围后靠近玩家，并在拾取后执行效果。
- 掉落物和拾取特效均可复用对象池。

### Buff、Debuff 与 AOE

- `StatusEffectConfig` 定义状态 ID、类型、目标类型、持续时间、叠加规则和修饰器列表。
- 支持 Refresh、Stack、Independent 三类叠加方式。
- `StatusEffectManager` 挂在实体上管理状态实例。
- `StatusEffectTickSystem` 统一驱动状态 Tick，避免每个状态单独 Update。
- 当前包含 Haste、Slow、Poison 示例状态。
- 状态修饰器支持移动速度倍率和周期伤害。
- AOE 支持范围伤害、持续 Tick、状态施加、视觉生命周期和对象池复用。

### UI、流程与数据

- `UIManager` 统一创建 Canvas/EventSystem，并缓存 UI 面板。
- UI 面板由预制体创建，不依赖场景中手动预放。
- UI 采用 MVP 拆分，View 只保存控件引用，Presenter 负责事件监听、数据显示和交互逻辑。
- 主菜单可显示并切换关卡信息。
- 角色面板支持已解锁、未解锁、购买、选择和价格状态。
- 点击 Play 后通过 `GameLaunchContext` 传递关卡和角色选择。
- `StageRuntimeManager` 管理关卡倒计时和通关判定。
- `LevelManager` 管理局内金币、击杀数、暂停、恢复、退出、胜利、失败和结算。
- 游戏结束后显示结算宝箱面板，将本局金币写入玩家数据。
- `PlayerDataManager` 保存金币、已解锁角色、已解锁关卡和当前选择角色。
- `SettingDataManager` 保存音乐/音效开关与音量。
- `AudioManager` 管理背景音乐、音效播放和音量应用，音效对象使用池化。

## 目录结构

```text
Assets/
  _Project/
    Art/                  美术、动画、材质、特效资源
    Audio/                音频资源
    Prefabs/              角色、敌人、掉落、UI、系统预制体
    Scenes/               MainMenu 和 Game 场景
    ScriptableObjects/    角色、敌人、武器、关卡、掉落、状态、AOE、音频配置
    Scripts/
      AStar/              A* 相关预留或实验代码
      Camera/             摄像机目标绑定
      Configs/            ScriptableObject 配置类
      Entities/           玩家、敌人、武器、投射物、拾取物、AOE、通用生命/移动/状态组件
      Map/                背景循环复用
      System/             生成、战斗、AOE、音频、事件、输入、数据、对象池、索敌等系统
      Tools/              二进制数据工具
      UI/                 UIManager、Model、Presenter、View
Docs/
  technical-analysis.md
  implementation-document.md
Packages/
ProjectSettings/
README.md
```

## 关键入口

| 脚本 | 职责 |
| --- | --- |
| `Main.cs` | 主菜单场景入口，初始化数据、UI 和背景音乐 |
| `UIManager.cs` | 创建 Canvas/EventSystem，管理 UI 面板实例与显隐 |
| `GameResources.cs` | 集中缓存玩家、敌人、关卡、武器、状态、AOE、音频等配置 |
| `GameLaunchContext.cs` | 从主菜单进入 Game 场景时传递当前关卡和角色选择 |
| `Generator.cs` | 聚合玩家生成、敌人生成和掉落生成系统 |
| `CharacterSpawnManager.cs` | 根据当前角色配置生成玩家，并绑定摄像机目标 |
| `SpawnSystem.cs` | 根据关卡刷怪配置控制敌人生成、暂停、停止和清理 |
| `EnemySpawnManager.cs` | 按敌人配置创建或复用敌人对象池 |
| `PlayerWeaponSystem.cs` | 管理玩家武器运行时实例、冷却和自动攻击 |
| `WeaponDamageApplier.cs` | 统一处理武器伤害和命中状态施加 |
| `AOESystem.cs` | 生成、推进、回收 AOE 区域对象 |
| `StatusEffectManager.cs` | 管理实体身上的 Buff/Debuff 实例 |
| `LevelManager.cs` | 管理局内统计、暂停、结束和结算 |
| `StageRuntimeManager.cs` | 管理关卡剩余时间与通关判定 |
| `PlayerDataManager.cs` | 加载、保存和广播玩家数据变化 |
| `SettingDataManager.cs` | 加载、保存音频设置 |
| `AudioManager.cs` | 播放背景音乐和音效，应用音频设置 |

## UI 架构

项目 UI 使用 MVP 思路：

```text
View
  只保存 Text、Button、Image、Slider 等控件引用。
  不读取业务数据，不格式化显示文本，不直接处理跨系统逻辑。

Presenter
  监听按钮和系统事件。
  从 Manager 或 Model 读取数据。
  负责刷新 View 显示、处理购买/选择/暂停/结算等交互。

Model / Manager
  保存业务状态或持久化数据。
  例如 PlayerDataManager、SettingDataManager、LevelManager、StageRuntimeManager。
```

示例：

- `MainMenu_View` 保存关卡名称、等级、图片和按钮引用。
- `MainMenu_Presenter` 负责关卡切换、刷新关卡显示和进入游戏。
- `CharacterItem_View` 保存单个角色条目的显示控件。
- `CharacterItem_Presenter` 负责角色条目的状态显示和点击回调。
- `Game_View` 保存金币、击杀数、倒计时和暂停按钮引用。
- `Game_Presenter` 监听局内事件并刷新对应文本。

## 如何运行

1. 使用 Unity Hub 打开项目根目录。
2. 确认 Unity 版本为 `2022.3.62f3 LTS`。
3. 打开 `Assets/_Project/Scenes/MainMenu.unity`。
4. 点击 Play。
5. 在主菜单选择关卡和角色后进入游戏。

也可以直接打开 `Assets/_Project/Scenes/Game.unity` 做局内调试，但完整流程建议从 `MainMenu.unity` 进入。

## 操作方式

| 操作 | 说明 |
| --- | --- |
| WASD | 控制玩家移动 |
| UI 摇杆 | 控制玩家移动 |
| 自动攻击 | 玩家会自动攻击范围内敌人 |
| Pause 按钮 | 暂停游戏并打开暂停面板 |
| Setting 面板 | 调整音乐、音效开关和音量 |
| Take 按钮 | 结算本局金币并返回主菜单 |

## 配置扩展

常见扩展入口：

- 新增角色：创建 `PlayerConfig`，绑定角色预制体，并加入 `GameResources`。
- 新增敌人：创建 `EnemyConfig`，绑定敌人预制体，并加入刷怪配置。
- 新增关卡：创建 `StageConfig` 和对应 `SpawnConfig`，加入 `GameResources.StageConfigs`。
- 新增武器：创建 `WeaponConfig`，选择攻击策略、伤害策略、投射物或 AOE 配置。
- 新增状态：创建 `StatusEffectConfig` 和对应 `StatusEffectModifier`。
- 新增 AOE：创建 `AOEConfig`，配置半径、持续时间、Tick 间隔、伤害、状态和视觉预制体。
- 新增掉落：创建 `ItemConfig`，并加入 `DropTableConfig`。
- 新增音频：创建 `AudioConfig`，并加入 `GameResources` 音频配置列表。

## 里程碑状态

| 里程碑 | 状态 | 内容 |
| --- | --- | --- |
| M1 工程初始化 | 已完成 | Unity 工程、目录结构、基础场景、Git 忽略规则 |
| M2 玩家移动和摄像机 | 已完成 | WASD/摇杆移动、Cinemachine 跟随、动画切换 |
| M3 敌人生成和追踪 | 已完成 | 屏幕外刷怪、敌人追踪、对象池、死亡清理、循环背景 |
| M4 自动攻击和战斗 | 已完成 | 自动索敌、多武器、伤害策略、投射物、敌人接触伤害 |
| M5 掉落和拾取 | 已完成 | 掉落表、吸附拾取、经验/治疗/分数/金币效果 |
| M6 Buff/Debuff/AOE | 已完成 | 状态系统、状态修饰器、命中 Debuff、AOE 区域和池化 |
| M7 UI、调试和优化 | 已完成 | MVP UI、主菜单、角色选择、关卡倒计时、暂停设置、结算、音频、数据持久化 |
| M8 文档整理和演示准备 | 进行中 | README 整理、后续可补充截图、演示 GIF 和操作说明 |

## 构建与验证

可使用以下命令进行脚本编译验证：

```bash
dotnet build "2D_TopDown_Survival_Demo.sln" --no-restore
```

最近一次文档记录中的验证结果为 `0` 错误、`0` 警告。若 Unity 重新生成 `.csproj` 后依赖发生变化，建议先在 Unity 中打开项目完成资源导入，再执行命令行构建。

## 当前边界与后续方向

当前已完成核心单机玩法闭环，但仍有一些内容适合作为后续演示增强：

- 补充 README 截图或演示 GIF。
- 增加正式 Buff/Debuff 图标 UI。
- 增加 FPS、敌人数量、对象池状态等调试面板。
- 增加更多角色、关卡、武器、敌人和状态配置。
- 补充 Unity Test Framework 的 EditMode 或 PlayMode 测试。
- 为远程配置、Addressables 或热更新方案继续预留统一配置读取入口。

## 文档

- [技术分析文档](Docs/technical-analysis.md)
- [实现文档](Docs/implementation-document.md)

