# ChatRWKV Desktop Lazy Pack
![图片alt](/English.png)

## Doc [简体中文](/README.md) || [English](/English.md)

***

## Explain

The ChatRWKV desktop Lazy Pack, originally intended for self use, is now just for people who can't deploy to get up to speed with ChatRWKV.

## Secondary development

### Environmental requirement

1.Visual Studio Community 2022/2019

2.VS Install.NET desktop Development (select from a single component). Net6.0 runtime)

### Technical requirements

You need to know a little bit about WPF, and then a little bit about the following two projects

ChatRWKV [Portal](https://github.com/BlinkDL/ChatRWKV)

HandyControl [Development documentation](https://handyorg.github.io/handycontrol/)、[GitHub](https://github.com/HandyOrg/HandyControl)

### VS Loading the project

```
git clone https://github.com/ZG1711/ChatRWKV_PC.git
```
Double-click to open the ChatRWKV_PC.sln file

### Catalog Description
```
ChatRWKV_PC
|--Properties                                #Configuration directory
   |--Settings.settings                      #Software parameter profile
|--Commands                                  #Command
   |--BtnCommand.cs                          #Button command, all button functions in the software to achieve the basis
|--Converters                                #Transformation class
   |--*.cs                                   
|--Models                                    #Model Catalog
   |--ChatInfoModel.cs                       #Chat conversation Model
|--Resources                                 #Resource location
   |--Dictionarys                            #ResourceDictionary
      |--*.xaml
   |--Images                                 #Image Resources
      |--*.jpg/*.png/*.ico
   |--Other                                  #Unclassified files
      |--*.*
   |--PyFile                                 #Integrate the Py startup file with the compiled pyd file
      |--convert_model.py                    #ChatRWKV Model transformation file
      |--Run.py                              #Core, start ChatRWKV's file
      |--*.pyd                               #Acceleration package file, currently compiled 10,20,30 series graphics card acceleration package
      |--*.py                                #Other startup files
|--Utils                                     #Tools category
   |--.cs
|--ViewModels                                #The view model directory
   |--BaseViewmodel.cs                       #Based the view model files
   |--MainViewModel.cs                       #The main window view model file, basic all operations in this
|--Views                                     ## view directory
   |--*.cs/*.xaml
|--MainWindow.xaml                           #The main window file
|--MainWindow.xaml.cs                        #individual event handling in this, such as a layer
|--*.*                                       #Other documents
```
The core file Run.Py and MainViewModel.Cs

Run. Py file is responsible for starting the socket server

MainViewModel.Cs is responsible for the functions and interactions

### What needs to be done before compilation

1. Zip your own Git.zip and put it in the Resources/Other/ directory. Change the Git.zip generation operation to resources, and the zip file path should be as follows:
   ```
    |--Git.zip              #zip
       |--Git               #Directory name
          |--*.*            #Git file directory for the installer version
   ```
   If you don't need to Git operation, just comment out the MainWindow. Xaml and Views/StartWindow xaml. Part of the code below in the cs
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
2.The non-open source version has the update prompt function, and the update prompt needs to be added by itself. Just detect the prompt at startup

### Releasing programs
1.Right click on the ChatRWKV PC project in VS and select Publish

2.Add a release configuration file

3.Folders are selected for both targets and specific targets; other options are default

4.Publish

***
## Protocol specification
Apache License 2.0

## If it feels good

ETH:[0xdcA4F31BF4D2A22AeE3CC48eD781aEB2B1B9F71E](https://etherscan.io/address/0xdca4f31bf4d2a22aee3cc48ed781aeb2b1b9f71e)
BNB:[0xdcA4F31BF4D2A22AeE3CC48eD781aEB2B1B9F71E](https://bscscan.com/address/0xdca4f31bf4d2a22aee3cc48ed781aeb2b1b9f71e)
