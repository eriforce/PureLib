# PureLib

[![Build status](https://ci.appveyor.com/api/projects/status/aogji08cvj7g1rq6?svg=true)](https://ci.appveyor.com/project/eriforce/purelib)

A lightweight C# utility library.


## Download

The library is available on nuget.org via package name `PureLib`.

To install PureLib, run the following command in the Package Manager Console
```
PM> Install-Package PureLib
```
More information about NuGet package avaliable at https://nuget.org/packages/PureLib


## Features

- WPF
  - [NotifyObject](#notify-object)
  - [ViewModelBase](#view-model-base)
  - [RelayCommand](#relay-command)
  - [SingleInstanceApp](#single-instance-app)
  - [Converters](#converters)
- Web
  - [AdvancedWebClient](#advanced-web-client)
  - [ResumableWebClient](#resumable-web-client)
  - [WebRequester](#web-requester)
  - [WebDownloader](#web-downloader)
- [Utility](#utility)

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

`RelayCommand` implements `ICommand`, which could be bind to UI controls.
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

### Single Instance App

`SingleInstanceApp` inherits `Application`. The application inherits `SingleInstanceApp` will not be able to run multiple instances.
```csharp
    public partial class App : SingleInstanceApp { 
    }
```

### Converters

PureLib provides commonly used converters for UI bindings.
- `BooleanToVisibilityConverter`
- `InverseBooleanConverter`

### Advanced Web Client

`AdvancedWebClient` inherits `WebClient`, which added Basic Authentication and cookies support.
```csharp
    using (AdvancedWebClient client = new AdvancedWebClient(referer, userName, password, cookies)) {
    }
```

### Resumable Web Client

`ResumableWebClient` uses `AdvancedWebClient` internally. It could be used to resume imcompleted downloads.
```csharp
    using (ResumableWebClient client = new ResumableWebClient(referer, userName, password, cookies)) {
    }
```

### Web Requester

`WebRequester` is a wrapper for easily using `HttpWebRequest`. It features auto retrying with specified times.
```csharp
    _requester = new WebRequester(_cookiePersister.CookieContainer) {
        UserAgent = agent,
        Referer = referer,
        Encoding = Encoding.GetEncoding("gbk"),
        RetryInterval = 2000,
        RetryLimit = 5
    };
    _requester.SetRequest += (s, e) => {
        e.Data.Proxy = null;
    };
    _requester.GotResponse += (s, e) => {
        if (!_ignoreCookie)
            _cookiePersister.Update(e.Data);
    };
```

### Web Downloader

`WebDownloader` features essential functions of a download manager. It has a download queue and dispatches web clients to download tasks in pararell.
```csharp
    WebDownloader downloader = new WebDownloader(Global.Config.ThreadCount, null, false);
    downloader.DownloadCompleting += OnDownloadCompleting;
    downloader.AddItems(_itemPostMaps.Keys.ToList());
```

### Utility

Get running time of specfic code segment:
```csharp
    TimeSpan ts = Utility.GetExecutingDuration(() => { 
        for (int i = 0; i < 1000000; i++) {
        }
    });
```

Convert a wildcard to a regular expression:
```csharp
    string regex = "*.txt".WildcardToRegex();
```

Convert a byte array to a string in hex format:
```csharp
    string hex = Encoding.UTF8.GetBytes("hello").ToHexString();
```

Convert a string of enum list to an enum array:
```csharp
    DayOfWeek[] days = "Sunday,Saturday".ToEnum<DayOfWeek>();
```

Get human friendly text of an exception:
```csharp
    try {
    } catch (Exception ex) {
        string text = ex.GetTraceText();
    }
```

Shortcuts for reading/writing files on disk:
```csharp
    "D:\\data.bin".WriteBinary(new byte[] { 0x00, 0x01 });
    byte[] data = "D:\\data.bin".ReadBinary();
    
    "D:\\text.txt".WriteText("hello", Encoding.Default);
    string text = "D:\\text.txt".ReadText();
```



## License

MIT