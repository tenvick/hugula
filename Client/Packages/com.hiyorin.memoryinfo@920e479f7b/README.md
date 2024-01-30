# upm.MemoryInfoPlugin-for-Unity
unity package manager  
A set of tools for Unity to allow handling memory info for Android and iOS.


# Install
Specify repository URL git://github.com/hiyorin/upm.MemoryInfoPlugin-for-Unity.git with key com.hiyorin.memoryinfo into Packages/manifest.json like below.
```javascript
{
  "dependencies": {
    // ...
    "com.hiyorin.memoryinfo": "git://github.com/hiyorin/upm.MemoryInfoPlugin-for-Unity.git",
    // ...
  }
}
```


# Usage
```cs
using MemoryInfo;
```

#### Example
```cs
public void Example()
{
  var plugin = new MemoryInfoPlugin();
  var info = plugin.GetMemoryInfo();
  var text = string.Format("{0}/{1} KB ({2}%)", info.UsedSize, info.TotalSize, (int)(100f * info.UsedSize / info.TotalSize));
  Debug.Log(text);
}
```
