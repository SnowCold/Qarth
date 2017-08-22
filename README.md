# Framework For Game Develop with Unity3d.

#==============================

# Base 基础工具等

## --ByteBuffer 应用配置

* `ByteBuffer` 字节缓存对象

## --CommandNode 命令节点

* `CommandGroup` 命令组,并行执行。
* `CommandNode` 命令节点
* `CommandSequence` 顺序命令节点，有点类似行为树的顺序执行节点

## --ConstDefine

* `DelegateDefine` 委托形式的定义
* `LayerDefine` 层级定义

## --DataStruct 数据结构

* `Main` 测试代码
* `ITestUnit` 测试代码
* `IData` 数据接口（一个int的value）

### ---BinaryHeap 二叉树
### ---BinarySearchHeap 二叉搜索树
### ---List 链表结构（链表，栈）


## --DisposableObject

* `DisposableObject` 可释放对象基类，ResLoader实现此类

## --ERunner

* `ERunner` 函数执行封装，配合全局委托的Run使用。

## --ExecuteNode

* `ExecuteNode` 执行节点（包含进度）
* `ExecuteNodeContainer` 执行节点容器，线性执行

## --FSM 完全状态机

* `FSMState` 状态机状态
* `FSMStateFactory` 状态工厂类
* `FSMStateMachine` 状态机类
* `FSMStateTransition` 状态过渡

## --Helper 工具类

* `DateFormatHelper` 日期时间格式化输出工具类
* `GameObjectHelper` GameObject扩展类，查找GameObject，支持没有则创建
* `Helper` 杂七杂八工具类
* `LogHelper` 日志工具类

## --Pool 池

游戏中大量东西使用对象池。

* `GameObjectPool` GameObject对象池
* `GameObjectPoolGroup` GameObject对象池组
* `GameObjectPoolMgr` 带一个默认poolgroup
* `IGameObjectPoolStrategy` 对象池策略（UI和Default两种策略）
* `ListPool` 链表池，用来装链表
* `ObjectPool` 对象池 对象缓存要实现ICacheAble的，如果实现ICacheType可在Recycle2Cache中实现回收操作 
* `ObjectPoolObserver` 对象池观察者，观察对象池的技术情况暂没卵用！！！
* `PoolObjectComponent` 用来响应当对象返回池中的时候（OnReset2Cache）

## --ProjectConfig 工程配置

* `ProjectPathConfig` 序列化保存工程的一些配置，提供访问器。

## --RefCounter 引用计数

* `RefCounter` 引用计数基类，资源系统大量基于引用计数。

## --Safty 客户端防作弊数据类型

* `EInt` 安全整型
* `EFloat` 安全浮点型

## --Singleton 单例

* `ISingleton` 单例接口，所有单例实现OnSingletonInit
* `MonoSingleton` Mono单例
* `TMonoSingleton` Mono单例模版类
* `TMonoSingletonAttribute` 单例挂载路径
* `TSingleton` 单例模版类

#==============================

# Engine 引擎层

## --App 应用配置

* `AppConfig` 应用配置解析，包括服务端地址端口，热更新引导的启用与否

## --Audio 声音相关

* `AudioMgr` 管理单例型音效播放，提供播放接口等
* `AudioUnit` 音效单元管理（可对象池缓存）

## --Component 提供组件封装

基本不需要MonoBehavior，可提高大量组件存在时的性能,一个GameObject对应一个AbstractActor，一个AbstractActor中挂载多个ICom

* `AbstractActor(MonoBehavior)` 行为组件基类
* `AbstractCom(ICom)` 挂载在Actor上，模拟Component
* `AbstractMonoCom(ICom)` 挂载在Actor上，模拟MonoBehavior
* `ComOrderDefine` 组件排序的定义，根据排序调整调用顺序
* `ICom` 挂载在Actor上的组件的接口


## --DataRecord 数据保存

* `DataRecord(singleton)` 对PlayerPerfs的调用封装，保存数据用


## --Debuger 调试工具集

* `DebugLogger` 输出日志到屏幕或者文件
* `Log`	日志打印器，LogLevel控制输出类型，实际在dll中
* `TimeDebugger` 打印时间间隔用的

## --Event 全局事件系统

* `EngineEventID` 引擎事件ID定义
* `EventRegisterHelper` 事件注册反注册辅助工具
* `EventSystem（singleton）` 事件系统实现


## --GamePlay

游戏逻辑实现相关，目前都是废的。没卵用！！！

## --Inputer 输入管理

* `IInputter` 输入处理器接口
* `KeyboardInputter` 键盘输入和处理的绑定处理
* `KeyCodeEventInfo` 按钮案件状态
* `KeyCodeTracker` 按键状态追踪者

## --SerializeHelper 序列化工具

* `SerializeHelper` 序列化与反序列化并写入文件工具

## --Math 数学工具

* `MathHelper` 数学工具类
* `RandomHelper` 随机工具，更随机
* `Rect2D` Rect数据结构

## --Path文件和文件路径管理

* `FileMgr(singleton)` 文件管理（支持安卓读取，ab读取，热更新读取路径先后顺序）
* `FilePath` 文件路径封装，管理，快速访问接口
* `PathHelper` 文件名后缀之类的工具


## --ResSystem 资源系统

基于引用计数自动管理资源的加载与释放，上层只需考虑ResLoader即可。不需要考虑ab以及依赖的概念，一概由ResLoader处理。前提要求必须资源名称不同，每个包中的资源必须都是独特的命名。

### ---AssetDataTable 资源数据结构

* `ABUnit` AssetBundle信息的封装，其实类似Manifest（依赖,尺寸,md5）
* `AssetData` Asset的数据封装（资源名，ab在package中的索引,资源类型）
* `AssetDataPackage` 资源包数据(包中哪些ab之类的)
* `AssetDataTable` 资源总表，所有包的都在这里。


### ---Core 资源系统核心

* `IEnumeratorTask` 携程任务接口，用于异步加载
* `IEnumeratorTaskMgr` 携程任务管理接口
* `ResFactory` 资源工厂
* `ResMgr(singleton)` 资源管理器，处理资源加载任务

#### ----Res 资源类

资源对象卸载后会进入对象池缓存，下次直接使用次对象池，ResFactory中有对象池的最大数量。其实不止ab之类的素材理解为资源，这个资源是宏观概念，包含比如用10个面板，10个角色都可理解为资源。

* `AbstractRes` 所有资源基类，引用计数，依赖引用计数的处理
* `AssetBundleRes` AssetBundle资源（依赖Ab）
* `AssetRes` Asset资源（依赖Ab）
* `BaseRes` 基础资源类（InternalRes和AssetRes）主要是`Resources.UnloadAsset`
* `HotUpdateRes` 内部资源加载的实现为下载，用ResLoader来热更
* `InternalRes` 内部Resource的资源
* `IRes` Res接口
* `NetImageRes` 网络图片资源，内部实现为下载
* `SceneRes` 场景资源

#### ----ResLoader 资源加载器

* `DefaultLoaderStrategy` 默认加载策略
* `IResLoader` 资源加载器接口（目前只有一个ResLoader）
* `IResLoaderStrategy` 资源加载策略接口
* `ResLoader` 资源加载器
* `UILoaderStrategy(singleton)` UI加载策略



### ---ResUpdate 资源更新

* `ResPackage` 资源包
* `ResPackageHandler` 资源包管理（生成更新列表，实际管理下载等）
* `ResUpdateConfig` 资源更新配置
* `ResUpdateMgr(singleton)` 多个包Hander下载的管理
* `ResUpdateRecord` 资源包数据（下载状态，大小记录等）


### ---Tools 资源系统工具

* `ABUnitHelper` 资源更新列表计算器

#### ----ResDownloader 资源下载器

* `HttpDownloaderMgr` Http下载管理器(HttpWebRequest)
* `IHttpDownloader` 实际网络下载器接口
* `ResDownloader` 资源下载器（管理WWWDownloader）
* `WWWDownloader` WWW实现下载器

#### ----ResHolder 资源保持器

* `ResHolder` 资源保持器，保持常用资源，Shader，通用音效等
* `ShaderFinder` 加载ab中的Shader的方法。专门用于访问shader的工具


## --SceneMgr 场景管理

* `SceneMgr` 

## --TableMgr 表管理

暂缺

## --Text 字符串正则处理

* `RegexHelper` 正则处理工具类


## --Timer 定时器

* `TimeItem` 定时器实例子
* `Timer(singleton)` 所有定时器管理(用了最小堆)
* `TimerHelper` 没卵用！！！

## --UI UI管理及工具

* `UIMgr(singleton)` UI管理器
* `EngineUI` 引擎UI定义
* `PanelChain` 面板关联工具类
* `PanelInfo` 面板状态，UIMgr直接管理PanelInfo，PanelInfo在管理面板，Panel的handle控制者
* `UIDataTable` UI的资源，特性的注册映射表

### ---Helper

* `EffectMask` UI粒子特效的这招
* `ResolutionHelper` 分辨率适配工具
* `SortingOrderObserver` 排序观察者（OnSortingOrderUpdate）
* `TweenHelper` 缓动工具
* `UIEffectHelper` UI特效工具
* `UIFinder` UI查找工具(节点查找)
* `UIHelper` UI工具（灰显，修改父节点）
* `UITools` UI工具(设置层级，点击状态，画布状态)

### ---UGUI

* `AbstractPage` UI中的子界面，子模版(subview)
* `AbstractPanel` UI窗口面板（window,也可以当page用）
* `AbstractUIElement` UI元素基类
* `IView` 没卵用的接口，面板内部事件ViewEvent
* `LuaPanel` 没什么卵用
* `UIRoot` 处理排序，访问跟上的相机等


## --WorldMgr 世界管理（没卵用）

没卵用没卵用没卵用没卵用

## --ZipMgr 压缩管理器

* `ZipMgr(singleton)` 解压数据




#==============================
# Framework 框架层


## --Client 客户端

* `AbstractClient` 客户端基类,模拟用，目前没卵用！！！

## --Gameplay 游戏玩法

* `IGameplay` 游戏玩法接口，目前没卵用！！！

## --Guide 引导系统 

***Command***负责执行行为

***Trigger*** 负责处理触发，需要被触发的继承 ***GuideTriggerHandler***

***Guide*** 为步骤链，由Trigger触发，不可执行命令，只能控制步骤

***GuideStep*** 为引导的每个步骤，由Trigger触发，触发后执行command

GuideStep有一个KeyFrame用于控制是否在本步骤保存进度




* `Guide(GuideTriggerHandler)` 引导链
* `GuideCommandFactory` 引导命令工厂
* `GuideMgr` 引导管理器
* `GuideStep(GuideTriggerHandler)` 引导步骤类
* `GuideTriggerFactory` 引导触发器工厂


### ---Command 命令行为

* `AbstractGuideCommand` 命令类基类
* `ButtonHackCommand` Button操作屏蔽
* `EventPauseCommand` UI操作关闭
* `GuideHandCommand` 指引手指命令
* `HighlightUICommand` 高亮UI控件
* `PlayAudioCommand` 播放音效
* `TimerCommand` 延迟命令

### ---Help 工具类

* `GuideConfigParamProcess` 解析参数
* `IUINodeFinder(interface)` UI查找器接口
* `MonoFuncCall(command)` 执行代码
* `UINodeFinder(UI查找器实现)` 查找UI控件

### ---RuntimeParam 运行时参数

* `IRuntimeParam(interface)` 运行时参数接口
* `RuntimeParamFactory(singleton)` 运行时参数工厂类

### ---Trigger 触发器

* `GuideTriggerHandler`		引导出发器控制器（管理trigger）
* `ITrigger`					触发器接口
* `TopPanelTrigger`			顶层面板发生改变时触发
* `UINodeVisibleTrigger	`	UI节点显示状态改变时触发

### ---UI 引导UI相关

* `GuideHandPanel` 引导手指动画面板
* `GuideHighlightMask` 高亮遮罩形状控制脚本
* `HighlightMaskPanel` 高亮遮罩面板
* `RectTransformHelper` rectTransform工具类



## --Memory 内存调试

* `MemoryMgr(singleton)` 左上角内存显示

## --Module 模块

* `AbstractModuleMgr` 模块管理器基类

### ---Modules 模块相关

* `IModule` 模块接口，重命名接口
* `AbstractModule` 模块基类
* `AbstractMonoModule` 没卵用！！！
* `AbstractStartProcess` 启动器基类

## --Servers 服务器

* `AbstractServer` 服务器基类


## --Tables 表格管理

提供三个默认表 Const表，引导，语言

## --UI UI封装

提供组件，通用控件，MessageBox，UI过渡等功能

