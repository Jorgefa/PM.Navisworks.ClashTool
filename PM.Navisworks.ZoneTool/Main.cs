using Autodesk.Navisworks.Api.Plugins;
using PM.Navisworks.ZoneTool.Views;
using System;
using System.Windows;
using System.Windows.Forms.Integration;

namespace PM.Navisworks.ZoneTool
{
    [Plugin("PM.Navisworks.ZoneTool",
        "PMJF",
        ToolTip = "Clash management tool.",
        DisplayName = "Zone\nTool")]
    public class Main : AddInPlugin
    {
        private static MainWindow _window;

        public override int Execute(params string[] parameters)
        {
            try
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
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "\n" + e.StackTrace);
            }

            return 0;
        }

        private void WindowClosed(object sender, EventArgs e)
        {
            _window = null;
        }
    }
}