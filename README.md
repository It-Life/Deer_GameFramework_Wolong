# Deer_GameFramework_Wolong
- 基于GameFramework框架衍生的一个wolong热更框架（以由之前的huatuo升级为HybridCLR(wolong)），实现除GameFramework库底层代码以及更新流程逻辑层代码，其余流程及义务层代码全部热更。

- 游戏配表Config接入Luban,采用异步加载Config，实现热更资源和Config表分离Hotfix处理，方便配表人员频繁更新Config表。

### 版本

- Unity 2021.3.1f1c1
- Microsoft Visual Studio Professional 2022 

### 程序集引用图

![流程](https://github.com/It-Life/Deer_GameFramework_Wolong/blob/2021.3.1/DescDocu/%E6%B5%81%E7%A8%8B.png?raw=true)

### 热更程序集

目前热更程序集有四个，可在业务层扩展

- HotfixMain.dll  入口程序集

- HotfixCommon.dll 公共程序集

- HotFixFramework.Runtime.dll 框架程序集

- HotfixBusiness.dll 业务程序集

  可扩展程序集

- HotfixBusinessA.dll 业务程序集A

- HotfixBusinessB.dll 业务程序集B

程序集自动打AssetBundle（AB），遵循GameFramework（GF）资源管理

### 使用教程

* [Deer_GameFramework_Wolong/框架使用教程](https://github.com/It-Life/Deer_GameFramework_Wolong/blob/2021.3.1/DescDocu/%E6%A1%86%E6%9E%B6%E4%BD%BF%E7%94%A8%E6%95%99%E7%A8%8B.md)

### 项目声明

- 项目里包含的第三方收费插件仅供学习使用，如有商业目的，请自行购买。
- [AstarPathfindingProject](https://arongranberg.com/astar/)
- [Sirenix](https://odininspector.com/?utm_source=assetstore&utm_medium=description_link&utm_campaign=default/)
- [QHierarchy](https://assetstore.unity.com/packages/tools/utilities/qhierarchy-28577?locale=zh-CN)
- [Demigiant](https://assetstore.unity.com/packages/tools/visual-scripting/dotween-pro-32416)
- [SuperScrollView](https://assetstore.unity.com/packages/tools/gui/ugui-super-scrollview-86572)
- ...

### 框架使用集合地

- [点击链接加入群聊【Gf_Wolong热更集合】](https://jq.qq.com/?_wv=1027&k=18qNRFnH)

### 致谢仓库

[EllanJiang](https://github.com/EllanJiang)/**[GameFramework](https://github.com/EllanJiang/GameFramework)**

[focus-creative-games](https://github.com/focus-creative-games)/**[hybridclr](https://github.com/focus-creative-games/hybridclr)**

[focus-creative-games](https://github.com/focus-creative-games)/**[il2cpp_plus](https://github.com/pirunxi/il2cpp_plus)**

[focus-creative-games](https://github.com/focus-creative-games)/**[luban_examples](https://github.com/focus-creative-games/luban_examples)**
