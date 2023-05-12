# ChatRWKV桌面版懒人包
![图片alt](/20230425182102.png)

## Doc [简体中文](/README.md) || [English](/English.md)

***

## 说明

ChatRWKV桌面版懒人包,最初的目的是给自己使用,现在只是为了提供给不会部署的人,快速上手ChatRWKV。

## 二次开发

### 环境要求

1.Visual Studio Community 2022/2019

2.VS安装.Net桌面开发(单个组件中选择.Net6.0运行时)

### 技术要求

需要会一点WPF,然后再稍微了解一下下面两个项目

ChatRWKV [传送门](https://github.com/BlinkDL/ChatRWKV)

HandyControl [开发文档地址](https://handyorg.github.io/handycontrol/)、[GitHub地址](https://github.com/HandyOrg/HandyControl)

### VS加载项目

```
git clone https://github.com/ZG1711/ChatRWKV_PC.git
```
双击打开ChatRWKV_PC.sln文件

### 目录说明
```
ChatRWKV_PC
|--Properties                                #配置目录
   |--Settings.settings                      #软件参数配置文件
|--Commands                                  #命令
   |--BtnCommand.cs                          #按钮命令,软件中所有的按钮功能实现基础
|--Converters                                #转换类
   |--*.cs                                   
|--Models                                    #模型目录
   |--ChatInfoModel.cs                       #聊天对话Model
|--Resources                                 #资源存放位置
   |--Dictionarys                            #资源字典
      |--*.xaml
   |--Images                                 #图片资源
      |--*.jpg/*.png/*.ico
   |--Other                                  #不分类的文件
      |--*.*
   |--PyFile                                 #集成Py启动文件和已编译的pyd文件
      |--convert_model.py                    #ChatRWKV模型转换文件
      |--Run.py                              #核心,启动ChatRWKV的文件
      |--*.pyd                               #加速包文件,目前编译了10,20,30系的显卡加速包
      |--*.py                                #其它启动文件
|--Utils                                     #工具类目录
   |--.cs
|--ViewModels                                #视图模型目录
   |--BaseViewmodel.cs                       #基础视图模型文件
   |--MainViewModel.cs                       #主窗口视图模型文件,基本所有操作都在这
|--Views                                     #视图目录
   |--*.cs/*.xaml
|--MainWindow.xaml                           #主窗口文件
|--MainWindow.xaml.cs                        #个别事件处理在这,比如拖入文件
|--*.*                                       #其它文件
```
核心文件Run.py和MainViewModel.cs

Run.py文件负责启动socket服务端

MainViewModel.cs负责功能实现与交互

### 编译之前需要做的事情

1. 自行压缩一个Git.zip放入Resources/Other/目录下,将Git.zip的生成操作改为资源,压缩包路径应如下:
   ```
    |--Git.zip              #压缩包
       |--Git               #目录名
          |--*.*            #绿色版的Git文件目录
   ```
   如果不需要Git操作,就注释掉MainWindow.xaml和Views/StartWindow.xaml.cs中下面部分的代码
   ```
   //MainWindow.xaml
   <Button Grid.Row="3" Grid.Column="0" Command="{Binding GitHubRWKDownloadCommand,Mode=OneWay}" CommandParameter="{Binding Mode=OneWay,RelativeSource={RelativeSource Mode=Self}}" Content="{DynamicResource Lang_DownloadGitHub}" Height="32" Margin="5,0" />
   <Button Grid.Row="3" Grid.Column="1" Command="{Binding GitHubRWKVUpdateCommand,Mode=OneWay}"  CommandParameter="{Binding Mode=OneWay,RelativeSource={RelativeSource Mode=Self}}" Content="{DynamicResource Lang_UpdateGitHub}" Height="32" Margin="5,5,5,0"/>
   <Button Grid.Row="3" Grid.Column="2" Command="{Binding EditBtnCommand,Mode=OneWay}" CommandParameter="{Binding Mode=OneWay,RelativeSource={RelativeSource Mode=Self}}" x:Name="EditBtn" Content="{DynamicResource Lang_EditV2Chat}" Height="32" Margin="5,5,5,0" />
   <Button Grid.Row="3" Grid.Column="3" Command="{Binding StartGitHubChatPyCommand,Mode=OneWay}" CommandParameter="{Binding Mode=OneWay,RelativeSource={RelativeSource Mode=Self}}" x:Name="StartChatBtn" Content="{DynamicResource Lang_StartV2Chat}" Height="32" Margin="5,5,5,0" />
   
   //Views/StartWindow.xaml.cs
   Task.Run(() =>
    {
        if (!Directory.Exists(current + "Git"))
        {
            Uri uri = new Uri("/Resources/Other/Git.zip", UriKind.Relative);
            StreamResourceInfo info = Application.GetResourceStream(uri);
            using (ZipArchive archive = new ZipArchive(info.Stream))
            {
                archive.ExtractToDirectory(current);
            }
        }
    }),                   
   ```
2.未开源版本的有更新提示功能,需要更新提示的自行增加,只需在启动时检测提示即可

### 发布程序
1.VS中右键ChatRWKV_PC项目,选择发布

2.添加发布配置文件

3.目标和特定目标都选择文件夹,其它选项默认

4.发布

***
## 协议说明
Apache License 2.0

## 觉得不错的话

![图片alt](/20230425224023.png)
![图片alt](/20230425225459.png)
