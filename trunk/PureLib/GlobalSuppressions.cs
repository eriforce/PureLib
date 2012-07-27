using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Scope = "type", Target = "PureLib.WPF.SingleInstanceApp")]

[assembly: SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Scope = "member", Target = "PureLib.Web.EmailSender.#SendMail(System.String,System.Int32,System.Boolean,System.String,System.String,System.String,System.String,System.String,System.String,System.String,System.Boolean,System.Boolean,System.String,System.String)")]

[assembly: SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Scope = "member", Target = "PureLib.Common.PathWrapper.#CombineWithAppName(System.String)")]
[assembly: SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Scope = "member", Target = "PureLib.Native.Messaging.#SendMessage(System.String,System.Diagnostics.Process,System.Boolean)")]
[assembly: SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Scope = "member", Target = "PureLib.Native.TaskbarManager.#_ownerHandle")]
[assembly: SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Scope = "member", Target = "PureLib.WPF.SingleInstanceApp.#OnNextStartup(System.Windows.StartupEventArgs,System.Diagnostics.Process)")]
[assembly: SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Scope = "member", Target = "PureLib.WPF.SingleInstanceApp.#OnStartup(System.Windows.StartupEventArgs)")]
