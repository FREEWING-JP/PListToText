# PListToText
macOS config.plist to Normal Text formater program  
by FREE WING  
http://www.neko.ne.jp/~freewing/

## How to Use
Default Mask Privacy Information .  
  for OpenCore, "SystemSerialNumber", "MLB", "SystemUUID", "ROM"  
  for Clover, "SerialNumber", "BoardSerialNumber", "SmUUID", "Board-ID"  
With nomask 2nd parameter is don't Mask Privacy Information .  

## Windows  
PListToText.exe config.plist  
or  
PListToText.exe config.plist nomask  

## macOS + .NET Core 5.0  
https://dotnet.microsoft.com/download/dotnet-core  
dotnet PListToText.dll config.plist  
or  
dotnet PListToText.dll config.plist nomask  

### This Program use This library
A Simple PList Parser in C#  
https://www.codeproject.com/Tips/406235/A-Simple-PList-Parser-in-Csharp  

## Sapmle
![Sapmle](https://raw.githubusercontent.com/FREEWING-JP/PListToText/main/sample_1.png "Sapmle")  
![Sapmle](https://raw.githubusercontent.com/FREEWING-JP/PListToText/main/sample_2.png "Sapmle")  
