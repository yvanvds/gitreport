using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitReport
{
    class File
    {
        public static bool Acceptable(GitHubCommitFile file)
        {
            if (IsHidden(file)) return false;
            if (IsLogFile(file)) return false;

            // unity generated files
            if (IsPackageFolder(file)) return false;
            if (IsSettingsFolder(file)) return false;
            if (IsInImportedAssetFolder(file)) return false;

            // binary files
            if (IsBinaryFolder(file)) return false;

            // flutter exceptions
            if (IsInFlutterFolder(file)) return false;

            // code extensions
            if (HasCodeExtension(file)) return true;

            return false;
        }

        private static bool IsHidden(GitHubCommitFile file)
        {
            var path = file.Filename.Split('/');
            if (path.Last().StartsWith('.'))
            {
                return true;
            }
            return false;
        }

        private static bool IsLogFile(GitHubCommitFile file)
        {
            var path = file.Filename.Split('/');
            if (path.Last().EndsWith(".log"))
            {
                return true;
            }
            return false;
        }

        private static bool IsPackageFolder(GitHubCommitFile file)
        {
            var path = file.Filename.Split('/');
            foreach(var part in path)
            {
                if (part.Equals("Packages", StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsSettingsFolder(GitHubCommitFile file)
        {
            var path = file.Filename.Split('/');
            foreach (var part in path)
            {
                if (part.Equals("ProjectSettings", StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }

                if (part.Equals("UserSettings", StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsInFlutterFolder(GitHubCommitFile file)
        {
            var path = file.Filename.Split('/');
            if (path.First().Equals("android", StringComparison.CurrentCultureIgnoreCase)) return true;
            if (path.First().Equals("ios", StringComparison.CurrentCultureIgnoreCase)) return true;
            if (path.First().Equals("web", StringComparison.CurrentCultureIgnoreCase)) return true;
            //if (path.First().Equals("android", StringComparison.CurrentCultureIgnoreCase)) return true;

            return false;
        }

        private static bool IsBinaryFolder(GitHubCommitFile file)
        {
            var path = file.Filename.Split('/');
            foreach (var part in path)
            {
                if (part.Equals("buildnrun", StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsInImportedAssetFolder(GitHubCommitFile file)
        {
            var path = file.Filename.Split('/');
            for(int i = 0; i < path.Length - 2; i++)
            {
                if (path[i].Equals("Assets", StringComparison.CurrentCultureIgnoreCase))
                {
                    if (path[i+1].EndsWith(".cs"))
                    {
                        return false;
                    } else if (path[i+1].Equals("Scripts", StringComparison.CurrentCultureIgnoreCase))
                    {
                        return false;
                    } else if (path[i+1].Equals("My Assets", StringComparison.CurrentCultureIgnoreCase))
                    {
                        return false;
                    } else
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool HasCodeExtension(GitHubCommitFile file)
        {
            var parts = file.Filename.Split('.');
            if (parts.Last().Equals("cs", StringComparison.CurrentCultureIgnoreCase)) return true;
            if (parts.Last().Equals("dart", StringComparison.CurrentCultureIgnoreCase)) return true;
            
            if (parts.Last().Equals("ts", StringComparison.CurrentCultureIgnoreCase))
            {
                if (parts.Length < 3) return true;
                if (parts[parts.Length - 2].Equals("spec")) return false;
                return true;
            }

            if (parts.Last().Equals("py", StringComparison.CurrentCultureIgnoreCase)) return true;
            if (parts.Last().Equals("js", StringComparison.CurrentCultureIgnoreCase)) return true;
            if (parts.Last().Equals("css", StringComparison.CurrentCultureIgnoreCase)) return true;
            if (parts.Last().Equals("html", StringComparison.CurrentCultureIgnoreCase)) return true;
            return false; 
        }
    }
}
