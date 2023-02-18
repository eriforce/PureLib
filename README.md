# PureLib

[![Build status](https://ci.appveyor.com/api/projects/status/aogji08cvj7g1rq6?svg=true)](https://ci.appveyor.com/project/eriforce/purelib)
![License MIT](https://img.shields.io/badge/license-MIT-blue.svg)

A C# utility library.


## Download

The library is available on nuget.org.

To install PureLib, run the following command in the Package Manager Console
```
PM> Install-Package PureLib
PM> Install-Package PureLib.WPF
PM> Install-Package PureLib.MediaInfo
PM> Install-Package PureLib.Generators.DicToInstGenerator
```
More information about NuGet package avaliable at:

|Package|Version|
|-------|-------|
|PureLib|[![NuGet Version](https://img.shields.io/nuget/v/PureLib.svg?style=flat-square)](https://www.nuget.org/packages/PureLib/)|
|PureLib.WPF|[![NuGet Version](https://img.shields.io/nuget/v/PureLib.WPF.svg?style=flat-square)](https://www.nuget.org/packages/PureLib.WPF/)|
|PureLib.MediaInfo|[![NuGet Version](https://img.shields.io/nuget/v/PureLib.MediaInfo.svg?style=flat-square)](https://www.nuget.org/packages/PureLib.MediaInfo/)|
|PureLib.Generators.DicToInstGenerator|[![NuGet Version](https://img.shields.io/nuget/v/PureLib.Generators.DicToInstGenerator.svg?style=flat-square)](https://www.nuget.org/packages/PureLib.Generators.DicToInstGenerator/)|


## Features

- [Utility](#utility)
- Source Generators
  - [DictionaryToInstanceGenerator](#dictionary-to-instance-generator)
- WPF
  - [NotifyObject](#notify-object)
  - [ViewModelBase](#view-model-base)
  - [RelayCommand](#relay-command)
  - [SingletonApp](#singleton-app)
  - [Converters](#converters)
- Web
  - [WebDownloader](#web-downloader)


### Utility

Convert a wildcard to a regular expression:
```csharp
string regex = "*.txt".WildcardToRegex();
```

Convert a string of enum list to an enum array:
```csharp
DayOfWeek[] days = "Sunday,Saturday".ToEnum<DayOfWeek>();
```

Conversions between binary data and base64url string:
```csharp
byte[] data = Encoding.UTF8.GetBytes("test");
string result = Base64Url.Encode(data);
byte[] bin = Base64Url.Decode(result);
```

### Dictionary to Instance Generator

```csharp
[FromDictionary]
public class Payload {
    public int Id { get; set; }
    public string Name { get; init; }
    [Ignore]
    public string Value { get; set; }
}
```
will generate
```csharp
public static class DictionaryToPayloadExtensions {
    public static Payload ToPayload(this Dictionary<string, object> dic) {
        ref var refOfId = ref CollectionsMarshal.GetValueRefOrNullRef(dic, "Id");
        ref var refOfName = ref CollectionsMarshal.GetValueRefOrNullRef(dic, "Name");

        return new Payload {
            Id = Unsafe.IsNullRef(ref refOfId) ? default : (Int32)refOfId,
            Name = Unsafe.IsNullRef(ref refOfName) ? default : (String)refOfName,
        };
    }
}
```

### Notify Object

`NotifyObject` implements `INotifyPropertyChanged`, which enables you to raise changes of properties.
```csharp
public string StatusBarText {
    get { return _statusBarText; }
    set {
        _statusBarText = value;
        RaiseChange(() => StatusBarText); // or RaiseChange("StatusBarText");
    }
}
```

### View Model Base

`ViewModelBase` inherits `NotifyObject`, which is designed to be the base class of ViewModels in MVVM pattern.
```csharp
    public class MainWindowViewModel : ViewModelBase {
        public ObservableCollection<string> Files { get; set; }
        
        private void OnTaskStarted(object sender, TaskStartedEventArgs e) {
            RunOnUIThread(() => {
                if (!Files.Contains(e.File))
                    Files.Add(e.File);
            });
        }
    }
```

### Relay Command

`RelayCommand` implements `ICommand`, which could be bound to UI controls.
```csharp
private ICommand _openDescriptionCommand;
public ICommand OpenDescriptionCommand {
    get {
        if (_openDescriptionCommand == null)
            _openDescriptionCommand = new RelayCommand(p => {
                OpeningDescription(this, new EventArgs<string>(((WatFile)p).Description));
            }, p => !((WatFile)p).Description.IsNullOrEmpty());
        return _openDescriptionCommand;
    }
}
```

### Singleton App

`SingletonApp` inherits `Application`. The application inherits `SingletonApp` will not be able to run multiple instances.
```csharp
public partial class App : SingletonApp { 
}
```

### Converters

PureLib provides commonly used converters for UI bindings.
- `BooleanToVisibilityConverter`
- `InverseBooleanConverter`

### Web Downloader

`WebDownloader` contains essential functions of a download manager. It can dispatch any number of threads to download concurrently.
```csharp
WebDownloader downloader = new WebDownloader(Global.Config.ThreadCount, null, false);
downloader.DownloadCompleting += OnDownloadCompleting;
downloader.AddItems(_itemPostMaps.Keys.ToList());
```


## License

MIT