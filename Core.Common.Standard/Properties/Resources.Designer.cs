﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KY.Core.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("KY.Core.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to Can not resolve dependency {0}.
        /// </summary>
        internal static string CanNotResolveDependency {
            get {
                return ResourceManager.GetString("CanNotResolveDependency", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0:dd.MM.yyyy HH:mm:ss.fff}: {1} occurred. {2}.
        /// </summary>
        internal static string ConsoleErrorFormat {
            get {
                return ResourceManager.GetString("ConsoleErrorFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0:dd.MM.yyyy HH:mm:ss.fff}: {1}.
        /// </summary>
        internal static string ConsoleTraceFormat {
            get {
                return ResourceManager.GetString("ConsoleTraceFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Can not log empty exception.
        /// </summary>
        internal static string EmptyException {
            get {
                return ResourceManager.GetString("EmptyException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Can not be empty.
        /// </summary>
        internal static string ErrorCanNotBeEmpty {
            get {
                return ResourceManager.GetString("ErrorCanNotBeEmpty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}.{1:MM.dd}.log.
        /// </summary>
        internal static string FileCustomFileName {
            get {
                return ResourceManager.GetString("FileCustomFileName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to error.{0:MM.dd}.log.
        /// </summary>
        internal static string FileErrorFileName {
            get {
                return ResourceManager.GetString("FileErrorFileName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ================================================
        ///{0:dd.MM.yyyy HH:mm:ss.fff}: {1} occured
        ///{2}
        ///.
        /// </summary>
        internal static string FileErrorFormat {
            get {
                return ResourceManager.GetString("FileErrorFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to trace.{0:MM.dd}.log.
        /// </summary>
        internal static string FileTraceFileName {
            get {
                return ResourceManager.GetString("FileTraceFileName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0:dd.MM.yyyy HH:mm:ss.fff}: {2}
        ///.
        /// </summary>
        internal static string FileTraceFormat {
            get {
                return ResourceManager.GetString("FileTraceFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}-Exception. Logging failed.\n\n{1}.
        /// </summary>
        internal static string LoggingFailedMessage {
            get {
                return ResourceManager.GetString("LoggingFailedMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ungültige Domain. War {0} gemeint?.
        /// </summary>
        internal static string MailAdressInvalidDomain {
            get {
                return ResourceManager.GetString("MailAdressInvalidDomain", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to nicht vertrauenswürdige Top-Level-Domain.
        /// </summary>
        internal static string MailAdressUntrustedTopLevelDomain {
            get {
                return ResourceManager.GetString("MailAdressUntrustedTopLevelDomain", resourceCulture);
            }
        }
    }
}
