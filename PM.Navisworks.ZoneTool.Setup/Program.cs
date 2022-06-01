using System;
using System.Collections.Generic;
using WixSharp;
using WixSharp.CommonTasks;
using File = WixSharp.File;

namespace PM.Navisworks.ZoneTool.Setup
{
    internal class Program
    {
        private static readonly DateTime ProjectStartedDate = new DateTime(year: 2022, month: 5, day: 31);

        const string Guid = "2FEFC31C-74E2-4FA8-A178-92969AE92A2E";
        const string ProjectName = "Navisworks Zone Tool";
        const string PackageName = "PM.Navisworks.ZoneTool";
        const string ProjectDescription = "Navisworks Zone Tool";

        private const string ProjectLocation = @"..\PM.Navisworks.ZoneTool";

        public static void Main(string[] args)
        {
            var folders = new Dictionary<string, string>
            {
                { "2020", $@"{ProjectLocation}\bin\x64\Release_2020\net47" },
                { "2021", $@"{ProjectLocation}\bin\x64\Release_2021\net47" },
                { "2022", $@"{ProjectLocation}\bin\x64\Release_2022\net47" }
            };

            AutoElements.DisableAutoKeyPath = true;
            var feature = new Feature(ProjectName, true, false);
            var directories = CreateDirectories(feature, folders);
            var dir = new Dir(feature, $@"%AppData%/Autodesk/ApplicationPlugins/{PackageName}.bundle",
                new File(feature, "./PackageContents.xml"),
                new Dir(feature, "Contents")
                {
                    Dirs = directories
                });

            var project = new Project(ProjectName, dir)
            {
                Name = ProjectName,
                Description = ProjectDescription,
                OutFileName = ProjectName,
                OutDir = "output",
                Platform = Platform.x64,
                UI = WUI.WixUI_Minimal,
                Version = GetVersion(),
                InstallScope = InstallScope.perUser,
                MajorUpgrade = MajorUpgrade.Default,
                GUID = new Guid(Guid),
                LicenceFile = "./Resources/EULA.rtf",
                ControlPanelInfo =
                {
                    ProductIcon = "./Resources/PM.ico",
                },
                BannerImage = "./Resources/Banner.bmp",
                BackgroundImage = "./Resources/Main.bmp",
            };

            project.AddRegValues(new RegValue(RegistryHive.CurrentUser, $"Software\\PM Group\\{ProjectName}", "Version",
                GetVersion().ToString()));
            project.AddRegValues(new RegValue(RegistryHive.CurrentUser, $"Software\\PM Group\\{ProjectName}", "Guid",
                Guid));
            project.BuildMsi();
        }

        private static Dir[] CreateDirectories(Feature feature, Dictionary<string, string> folders)
        {
            var dirs = new List<Dir>();
            foreach (var folder in folders)
            {
                var dir = new Dir(folder.Key,
                    new Files(feature,
                        $@"{folder.Value}\*.*"));
                dirs.Add(dir);
            }

            return dirs.ToArray();
        }

        private static Version GetVersion()
        {
            const int majorVersion = 0;
            const int minorVersion = 3;
            var daysSinceProjectStarted = (int)((DateTime.UtcNow - ProjectStartedDate).TotalDays);
            var minutesSinceMidnight = (int)DateTime.UtcNow.TimeOfDay.TotalMinutes;
            var version = $"{majorVersion}.{minorVersion}.{daysSinceProjectStarted}.{minutesSinceMidnight}";
            return new Version(version);
        }
    }
}