using System;
using System.Diagnostics;

namespace Bridge.Translator
{
    public partial class Translator
    {
        protected static readonly char ps = System.IO.Path.DirectorySeparatorChar;

        protected virtual string GetBuilderPath()
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                    return Environment.GetEnvironmentVariable("windir") + ps + "Microsoft.NET" + ps + "Framework" + ps +
                        "v" + this.MSBuildVersion + ps + "msbuild";

                case PlatformID.Unix:
                case PlatformID.MacOSX:
                    return "xbuild";

                default:
                    throw (Exception)Bridge.Translator.Exception.Create("Unsupported platform - {0}", Environment.OSVersion.Platform);
            }
        }

        protected virtual string GetBuilderArguments()
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                case PlatformID.Unix:
                case PlatformID.MacOSX:
                    return String.Format(" \"{0}\" /t:Rebuild /p:Configuration={1} {2}", Location, this.Configuration, this.BuildArguments);

                default:
                    throw (Exception)Bridge.Translator.Exception.Create("Unsupported platform - {0}", Environment.OSVersion.Platform);
            }
        }

        public virtual void BuildAssembly()
        {
            var info = new ProcessStartInfo()
            {
                FileName = this.GetBuilderPath(),
                Arguments = this.GetBuilderArguments(),
                UseShellExecute = true
            };
            info.WindowStyle = ProcessWindowStyle.Hidden;
            using (var p = Process.Start(info))
            {
                p.WaitForExit();

                if (p.ExitCode != 0)
                {
                    Bridge.Translator.Exception.Throw("Compilation was not successful, exit code - {0}; FileName - {1}; Arguments - {2}.", p.ExitCode, info.FileName, info.Arguments);
                }
            }
        }
    }
}
