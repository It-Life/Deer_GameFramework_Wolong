# Deer_GameFramework_Wolong
- 基于GameFramework框架衍生的一个wolong热更框架（以由之前的huatuo升级为HybridCLR(wolong)），实现除GameFramework库底层代码以及更新流程逻辑层代码，其余流程及义务层代码全部热更。

- 游戏配表Config接入Luban,采用异步加载Config，实现热更资源和Config表分离Hotfix处理，方便配表人员频繁更新Config表。

### 版本

- Unity 2020.3.33f1
- Microsoft Visual Studio Professional 2022 

### 热更框架流程图

![流程](https://github.com/It-Life/Deer_GameFramework_Wolong/blob/main/DescDocu/%E6%B5%81%E7%A8%8B.png)

### 热更程序集

目前热更程序集有四个，可在业务层扩展

- HotfixMain.dll  入口程序集

- HotfixCommon.dll 公共程序集

- HotFixFramework.Runtime.dll 框架程序集

- HotfixBusiness.dll 业务程序集

  可扩展程序集

- HotfixBusinessA.dll 业务程序集A

- HotfixBusinessB.dll 业务程序集B

程序集打AssetBundle（AB），遵循GameFramework（GF）资源管理,以及集成到GF，AB自动生成程序集，并打出ab资源。

### 使用教程

* [Deer_GameFramework_Wolong/框架使用教程.md at main · It-Life/Deer_GameFramework_Wolong (github.com)](https://github.com/It-Life/Deer_GameFramework_Wolong/blob/main/DescDocu/框架使用教程.md)

**致谢仓库**

[EllanJiang](https://github.com/EllanJiang)/**[GameFramework](https://github.com/EllanJiang/GameFramework)**

[focus-creative-games](https://github.com/focus-creative-games)/**[hybridclr](https://github.com/focus-creative-games/hybridclr)**

[focus-creative-games](https://github.com/focus-creative-games)/**[il2cpp_plus](https://github.com/pirunxi/il2cpp_plus)**

[focus-creative-games](https://github.com/focus-creative-games)/**[luban_examples](https://github.com/focus-creative-games/luban_examples)**
