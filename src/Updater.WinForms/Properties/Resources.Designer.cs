﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.269
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ZeroInstall.Updater.WinForms.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("ZeroInstall.Updater.WinForms.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to Copying files....
        /// </summary>
        internal static string CopyFiles {
            get {
                return ResourceManager.GetString("CopyFiles", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Deleting obsolete files....
        /// </summary>
        internal static string DeleteFiles {
            get {
                return ResourceManager.GetString("DeleteFiles", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Done..
        /// </summary>
        internal static string Done {
            get {
                return ResourceManager.GetString("Done", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Fixing NTFS security permissions....
        /// </summary>
        internal static string FixPermissions {
            get {
                return ResourceManager.GetString("FixPermissions", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Waiting for all Zero Install instances to end....
        /// </summary>
        internal static string MutexWait {
            get {
                return ResourceManager.GetString("MutexWait", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Running as administrator....
        /// </summary>
        internal static string RerunElevated {
            get {
                return ResourceManager.GetString("RerunElevated", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Pre-compiling .NET assemblies for faster application startup....
        /// </summary>
        internal static string RunNgen {
            get {
                return ResourceManager.GetString("RunNgen", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Starting Zero Install Store Service....
        /// </summary>
        internal static string StartService {
            get {
                return ResourceManager.GetString("StartService", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Stopping Zero Install Store Service....
        /// </summary>
        internal static string StopService {
            get {
                return ResourceManager.GetString("StopService", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Updating registry....
        /// </summary>
        internal static string UpdateRegistry {
            get {
                return ResourceManager.GetString("UpdateRegistry", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Wrong number of arguments.
        ///Usage: {0}.
        /// </summary>
        internal static string WrongNoArguments {
            get {
                return ResourceManager.GetString("WrongNoArguments", resourceCulture);
            }
        }
    }
}
