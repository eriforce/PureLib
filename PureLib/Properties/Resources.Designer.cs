﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PureLib.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("PureLib.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to , .
        /// </summary>
        internal static string Common_Comma {
            get {
                return ResourceManager.GetString("Common_Comma", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to days.
        /// </summary>
        internal static string Common_Days {
            get {
                return ResourceManager.GetString("Common_Days", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to hours.
        /// </summary>
        internal static string Common_Hours {
            get {
                return ResourceManager.GetString("Common_Hours", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ms.
        /// </summary>
        internal static string Common_Milliseconds {
            get {
                return ResourceManager.GetString("Common_Milliseconds", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to minutes.
        /// </summary>
        internal static string Common_Minutes {
            get {
                return ResourceManager.GetString("Common_Minutes", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to seconds.
        /// </summary>
        internal static string Common_Seconds {
            get {
                return ResourceManager.GetString("Common_Seconds", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to -: Inner Exception :-.
        /// </summary>
        internal static string ExceptionHandling_InnerException {
            get {
                return ResourceManager.GetString("ExceptionHandling_InnerException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The member access expression does not access a property..
        /// </summary>
        internal static string NotifyObject_ExpressionNotProperty_Exception {
            get {
                return ResourceManager.GetString("NotifyObject_ExpressionNotProperty_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The expression is not a member access expression..
        /// </summary>
        internal static string NotifyObject_NotMemberAccessExpression_Exception {
            get {
                return ResourceManager.GetString("NotifyObject_NotMemberAccessExpression_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The referenced property is a static property..
        /// </summary>
        internal static string NotifyObject_StaticExpression_Exception {
            get {
                return ResourceManager.GetString("NotifyObject_StaticExpression_Exception", resourceCulture);
            }
        }
    }
}
