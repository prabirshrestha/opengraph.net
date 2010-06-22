using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("OpenGraph.NET")]
[assembly: AssemblyDescription("Graph API client for .NET")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Robert Paveza")]
[assembly: AssemblyProduct("OpenGraph.NET")]
[assembly: AssemblyCopyright("Copyright © Robert Paveza, 2010.  Licensed under the New BSD license; see http://robpaveza.net/opengraph.net/docs/license.txt for details.")]
[assembly: AssemblyTrademark("Facebook and Open Graph are trademarks or registered trademarks")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("1f8b1cd8-3186-4c0d-b245-79de1015f67b")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("0.9.0.0")]
[assembly: AssemblyFileVersion("0.9.0.0")]

#if DEBUG
[assembly: InternalsVisibleTo("Facebook.TestOpenGraphParser")]
#endif

#if !SILVERLIGHT
[assembly: InternalsVisibleTo("Facebook.OpenGraph.Web")]
[assembly: InternalsVisibleTo("Facebook.OpenGraph.Web.Mvc")]
#endif