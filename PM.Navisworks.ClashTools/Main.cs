using Autodesk.Navisworks.Api.Plugins;
using PM.Navisworks.ClashTools.Views;
using System;
using System.Windows.Forms.Integration;

namespace PM.Navisworks.ClashTools
{
    [Plugin("PM.Navisworks.ClashTool",
        "PMJF",
        ToolTip = "Clash management tool.",
        DisplayName = "Clash\nTool")]
    public class Main : AddInPlugin
    {
        private static MainWindow _window;

        public override int Execute(params string[] parameters)
        {
            var activeDoc = Autodesk.Navisworks.Api.Application.ActiveDocument;

            if (_window != null)
            {
                _window.Activate();
            }
            else
            {
                _window = new MainWindow(activeDoc);
                ElementHost.EnableModelessKeyboardInterop(_window);
                _window.Closed += WindowClosed;
                _window.Show();
            }


            return 0;
        }

        private void WindowClosed(object sender, EventArgs e)
        {
            _window = null;
        }
    }
}