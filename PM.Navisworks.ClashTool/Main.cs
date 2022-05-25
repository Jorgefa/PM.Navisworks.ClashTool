using Autodesk.Navisworks.Api.Plugins;
using System.Windows;

namespace PM.Navisworks.ClashTool
{
    [Plugin("PM.Navisworks.ClashTool",
        "PMJF",
        ToolTip = "Clash management tool.",
        DisplayName = "Clash\nTool")]
    public class Main : AddInPlugin
    {
        public override int Execute(params string[] parameters)
        {
            var activeDoc = Autodesk.Navisworks.Api.Application.ActiveDocument;
            MessageBox.Show("Hello!");

            return 0;
        }
    }
}