using dnlib.DotNet;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace dnSpy.Loader
{
    public partial class Splash : Form
    {
        private string myFilePath;
        private Timer t1 = new Timer();
        private Process process = new Process();

        public Splash(string filePath) {
            myFilePath = (filePath);
            InitializeComponent();
        }

        private void Splash_Load(object sender, EventArgs e) {
            Opacity = 0;
            t1.Interval = 10;
            t1.Tick += new EventHandler(FadeIn);
            t1.Start();
            backgroundWorker1.RunWorkerAsync();
        }

        private void FadeIn(object sender, EventArgs e) {
            if (Opacity >= 1)
                t1.Stop();
            else
                Opacity += 0.05;
        }

        private void BackgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e) {
            ModuleDefMD mod = ModuleDefMD.Load(myFilePath);
            Assembly asm = Assembly.LoadFile(mod.Location);

            asm.ManifestModule.GetPEKind(out PortableExecutableKinds peKind, out ImageFileMachine machine);

            string filePath;
            string currentDirectory = System.IO.Path.GetDirectoryName(typeof(Splash).Assembly.Location);

            //Any CPU
            if (peKind == PortableExecutableKinds.ILOnly && machine == ImageFileMachine.I386) {
                if (Environment.Is64BitOperatingSystem) {
                    filePath = $"{currentDirectory}\\dnSpy.exe";
                } else {
                    filePath = $"{currentDirectory}\\dnSpy-x86.exe";
                }
            }
            //Any CPU - Prefer 32-bit
            else if (peKind == (PortableExecutableKinds.ILOnly | PortableExecutableKinds.Preferred32Bit) && machine == ImageFileMachine.I386) {
                filePath = $"{currentDirectory}\\dnSpy-x86.exe";
            }
            //x86
            else if (peKind == (PortableExecutableKinds.ILOnly | PortableExecutableKinds.Required32Bit) && machine == ImageFileMachine.I386) {
                filePath = $"{currentDirectory}\\dnSpy-x86.exe";
            }
            //x64
            else if (peKind == PortableExecutableKinds.PE32Plus && machine == ImageFileMachine.AMD64) {
                filePath = $"{currentDirectory}\\dnSpy.exe";
            } else {
                throw new Exception("Unknown PE value !");
            }
            process.StartInfo.FileName = filePath;
            process.StartInfo.Arguments = $"\"{myFilePath}\"";
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start();
        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e) {
            System.Threading.Thread.Sleep(2000);
            process.Dispose();
            Environment.Exit(0);
        }
    }
}
