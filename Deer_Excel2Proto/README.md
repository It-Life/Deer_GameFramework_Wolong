# Deer_Excel2ProtoAndMessage2Proto
这是一个适用于Unity项目的打表工具 以及生成protoBuff 协议的工具,

[实际应用于在Unity框架库]: https://github.com/It-Life/Deer_Gameframework_ToLua
[基于wincc0823库 Excel2Proto]: https://github.com/wincc0823/Excel2Protobuf

### 第一步 安装环境

1. 在setup文件夹中解压 setup.zip文件

   ![image-20220427200609818](https://github.com/It-Life/Deer_Excel2Proto/blob/main/setup/tempImage/image-20220427200609818.png)

2. 打开解压出来的setup文件夹看到如下图内容

   ![image-20220427200720817](https://github.com/It-Life/Deer_Excel2Proto/blob/main/setup/tempImage/image-20220427200720817.png)

3. 运行setup.bat

   ![image-20220427201017359](https://github.com/It-Life/Deer_Excel2Proto/blob/main/setup/tempImage/image-20220427201017359.png)

4. 如上图显示，安装Python到对应目录里

5. 运行setup2.bat

6. 至此，打表环境配置完成

### 第二步 使用教程

#### 1.表转proto

1. 在表目录 ConfigFiles(excel) 配置表，参考此文件夹的其他表结构，或者参考MarkConfigAndProtobuf/excel里的表文件

   ![image-20220427202300093](https://github.com/It-Life/Deer_Excel2Proto/blob/main/setup/tempImage/image-20220427202300093.png)

2. 协议文件名根据如下图名字定义

   ![image-20220427202327599](https://github.com/It-Life/Deer_Excel2Proto/blob/main/setup/tempImage/image-20220427202327599.png)

3. 打开MarkConfigAndProtobuf目录 运行BuildConfig.bat批处理文件，

   在proto/config目录下生成如下图proto文件， config文件夹自动生成

   ![image-20220427202618916](https://github.com/It-Life/Deer_Excel2Proto/blob/main/setup/tempImage/image-20220427202618916.png)

   在client/data下生成bin文件，用于存放数据

   ![image-20220427203027113](https://github.com/It-Life/Deer_Excel2Proto/blob/main/setup/tempImage/image-20220427203027113.png)

   在client/scripts下生成cs文件，用于游戏里数据反序列，当然这里是用于C#语言，如果框架语言是Lua的实现的，这个文件加的文件对于游戏没有作用

   ![image-20220427203145252](https://github.com/It-Life/Deer_Excel2Proto/blob/main/setup/tempImage/image-20220427203145252.png)

   在server/scripts下生成用于服务器的文件，服务器语言支持多种语言 cpp|csharp|java|go|js|objc|php|python|ruby

   在server/data下生成用于服务器数据文件

#### 2. 消息协议转proto

1. 在MarkConfigAndProtobuf/proto目录新建自己的proto文件，自己手动创建，不同于MarkConfigAndProtobuf/proto/config目录是自动生成，如下图

   ![image-20220427203830254](https://github.com/It-Life/Deer_Excel2Proto/blob/main/setup/tempImage/image-20220427203830254.png)

2. 点击MarkConfigAndProtobuf/ProtocToScript.bat 批处理文件在MarkConfigAndProtobuf/client/scripts目录下生成如下图cs文件，当然如果你是Lua语言开发，继续往下看

   ![image-20220427204116313](https://github.com/It-Life/Deer_Excel2Proto/blob/main/setup/tempImage/image-20220427204116313.png)

   

####	3.proto转Lua教程

复制MarkConfigAndProtobuf/proto 下文件，到

[]: https://github.com/It-Life/Deer_Gameframework_ToLua	"Deer_Gameframework_ToLua"

框架里Assets同层目录ProtoToLua 下genproto目录里，然后运行ProtoToLua/make_proto.bat批处理，就会生成LuaProto文件

![image-20220427204613145](https://github.com/It-Life/Deer_Excel2Proto/blob/main/setup/tempImage/image-20220427204613145.png)

![image-20220427204819977](https://github.com/It-Life/Deer_Excel2Proto/blob/main/setup/tempImage/image-20220427204819977.png)

![image-20220427205044727](https://github.com/It-Life/Deer_Excel2Proto/blob/main/setup/tempImage/image-20220427205044727.png)

到此，excel表转proto 以及 proto协议转C#以及lua教程完成，proto版本3.6.1
