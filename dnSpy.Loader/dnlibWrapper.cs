using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace dnSpy.Loader
{
	class dnlibWrapper
	{
		internal static string GetModuleLocation(string assemblyLocation)
		{
			var dnlib = Assembly.Load("dnlib");
			var ModuleDefMD = dnlib.GetType("dnlib.DotNet.ModuleDefMD");
			var ModuleCreationOptions = dnlib.GetType("dnlib.DotNet.ModuleCreationOptions");
			var Load = ModuleDefMD.GetMethod("Load", new Type[] { typeof(string), ModuleCreationOptions });
			var Location = ModuleDefMD.GetProperty("Location");

			var mod = Load.Invoke(null, new object[] { assemblyLocation, null });
			return (string)Location.GetValue(mod);
		}
	}
}
