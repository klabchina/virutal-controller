# virutal-controller
Simulator UI for Unity Event System (UGUI)

# Support
```
NewInputScene  		NewInputSystem をサポーターする
StandaloneInput		OldInputSystem をサポーターする
InputSystemが何方が使うのは ProjectSettingの中でセード出来ます
```

# Example
```
SampleScene 
考え方
1. EventSystem.RaycastAll 使って　UIなとObjectを探し出して 
2. ExecuteEvents.Execute 使って いろいろなエッベントを通知する。

``` 

![実装した考え方](https://raw.githubusercontent.com/klabchina/virutal-controller/main/imgs/unity-input.png)


# Jigboxについて
```
workspace: https://github.jp.klab.com/oshiro-n/sub-pointer
这个仓库实现的内容实现的不错 对inputModule源码有一定的解读。
但是我认为重写InputModule并非是最佳办法。
1. 首先重写InputModule 需要对源码有很深入了解
2. 其次最重要的是InputModule是针对设备的输入， 目前StandAlone仅包含 键盘 鼠标 触摸
3. 其次BaseInputModule是基于input 进行操作映射的， 并不支持new input system 对项目会有要求。

结论：
所以我认为重写InputModule并非首选。

```
