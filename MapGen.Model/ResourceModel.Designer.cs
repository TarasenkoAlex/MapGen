﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MapGen.Model {
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
    internal class ResourceModel {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ResourceModel() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("MapGen.Model.ResourceModel", typeof(ResourceModel).Assembly);
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
        ///   Looks up a localized string similar to C:\MapGen\Database\.
        /// </summary>
        internal static string DATABASE_DIR_PATH {
            get {
                return ResourceManager.GetString("DATABASE_DIR_PATH", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to MapGen.db.
        /// </summary>
        internal static string DATABASE_FILE_NAME {
            get {
                return ResourceManager.GetString("DATABASE_FILE_NAME", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to C:\MapGen\Runtime\.
        /// </summary>
        internal static string DIR_RUNTIME {
            get {
                return ResourceManager.GetString("DIR_RUNTIME", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to C:\MapGen\Tests\.
        /// </summary>
        internal static string DIR_TESTS {
            get {
                return ResourceManager.GetString("DIR_TESTS", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Byte[].
        /// </summary>
        internal static byte[] MapGen {
            get {
                object obj = ResourceManager.GetObject("MapGen", resourceCulture);
                return ((byte[])(obj));
            }
        }
    }
}
