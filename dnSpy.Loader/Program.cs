using Microsoft.Win32;
using System;
using System.Reflection;
using System.Windows.Forms;

namespace dnSpy.Loader
{
    class Program
    {
        [STAThread]
        static void Main(string[] args) {
            if (args != null && args.Length != 0) {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Splash(args[0]));
            } else {
                string currentDirectory = typeof(Program).Assembly.Location;
                string[] listExtension = new string[] { "exefile", "dllfile" };
                foreach (var extension in listExtension) {
                    Registry.SetValue($"HKEY_CURRENT_USER\\Software\\Classes\\{extension}\\shell\\dnSpyLoader", "", "Open with dnSpy", RegistryValueKind.String);
                    Registry.SetValue($"HKEY_CURRENT_USER\\Software\\Classes\\{extension}\\shell\\dnSpyLoader\\command", "", $"\"{currentDirectory}\" \" %1\"", RegistryValueKind.String);
                }
                MessageBox.Show("Set explorer menu registry key!");
            }
        }
    }
}
