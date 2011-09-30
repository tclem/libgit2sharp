using System.IO;
using System.Reflection;
using System.Text;

namespace LibGit2Sharp.Core
{
    public static class AssemblyExtensions
    {
        public static string ExtractResourceToFile(this Assembly assembly, string resourceName, string outPath)
        {
            assembly = assembly ?? Assembly.GetExecutingAssembly();

            var fi = new FileInfo(outPath);
            if (fi.Exists)
            {
                return fi.FullName;
            }

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var output = File.Create(outPath))
            {
                int bytesRead = 0;
                byte[] buf = new byte[4096];
                while ((bytesRead = stream.Read(buf, 0, buf.Length)) > 0)
                {
                    output.Write(buf, 0, bytesRead);
                }
            }

            return outPath;
        }
    
        public static string ExtractResourceToString(this Assembly assembly, string resourceName, Encoding encoding = null)
        {
            assembly = assembly ?? Assembly.GetExecutingAssembly();
            encoding = encoding ?? Encoding.Default;

            using (var sr = new StreamReader(assembly.GetManifestResourceStream(resourceName), encoding))
            {
                return sr.ReadToEnd();
            }
        }
    }
}