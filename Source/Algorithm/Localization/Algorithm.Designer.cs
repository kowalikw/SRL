﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SRL.Algorithm.Localization {
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
    internal class Algorithm {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Algorithm() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("SRL.Algorithm.Localization.Algorithm", typeof(Algorithm).Assembly);
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
        ///   Looks up a localized string similar to Angle density.
        /// </summary>
        internal static string angleDensity {
            get {
                return ResourceManager.GetString("angleDensity", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Divides the 360 degree spectrum into this many possible vehicle orientations. The bigger the value the better the precision..
        /// </summary>
        internal static string angleDensityTooltip {
            get {
                return ResourceManager.GetString("angleDensityTooltip", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Bidirectional movement.
        /// </summary>
        internal static string bidirectional {
            get {
                return ResourceManager.GetString("bidirectional", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Allows the vehicle to move backwards (with the same cost)..
        /// </summary>
        internal static string bidirectionalTooltip {
            get {
                return ResourceManager.GetString("bidirectionalTooltip", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Move weight.
        /// </summary>
        internal static string moveWeight {
            get {
                return ResourceManager.GetString("moveWeight", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cost of the vehicle move by the distance of [{0}]..
        /// </summary>
        internal static string moveWeightTooltip {
            get {
                return ResourceManager.GetString("moveWeightTooltip", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Multidirectional movement.
        /// </summary>
        internal static string multidirectional {
            get {
                return ResourceManager.GetString("multidirectional", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Allows the vehicle to move in any direction without having to point its front towards it. Overrides [{0}] setting..
        /// </summary>
        internal static string multidirectionalTooltip {
            get {
                return ResourceManager.GetString("multidirectionalTooltip", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Net density.
        /// </summary>
        internal static string netDensity {
            get {
                return ResourceManager.GetString("netDensity", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Divides the intermediate point net into ({0} x {0}) equidistant points..
        /// </summary>
        internal static string netDensityTooltip {
            get {
                return ResourceManager.GetString("netDensityTooltip", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Point precision.
        /// </summary>
        internal static string pointPrecision {
            get {
                return ResourceManager.GetString("pointPrecision", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Maximum distance between two vertices that makes then indistinguishable by the algorithm. The smaller the value the better the precision..
        /// </summary>
        internal static string pointPrecisionTooltip {
            get {
                return ResourceManager.GetString("pointPrecisionTooltip", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Turn weight.
        /// </summary>
        internal static string turnWeight {
            get {
                return ResourceManager.GetString("turnWeight", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cost of the minimal turn the vehicle can take (which is defined by [{0}] option)..
        /// </summary>
        internal static string turnWeightTooltip {
            get {
                return ResourceManager.GetString("turnWeightTooltip", resourceCulture);
            }
        }
    }
}
