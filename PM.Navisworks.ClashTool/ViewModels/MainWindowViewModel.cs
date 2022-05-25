using Autodesk.Navisworks.Api;
using PM.Navisworks.ClashTool.Commands;
using System.Windows.Forms;

namespace PM.Navisworks.ClashTool.ViewModels
{
    public class MainWindowViewModel
    {
        public MainWindowViewModel(Document document)
        {
            _document = document;

            TestButtonCommand = new DelegateCommand(TestButton);
        }

        private readonly Document _document;

        private int myVar;

        public int MyProperty
        {
            get { return myVar; }
            set { myVar = value; }
        }

        public DelegateCommand TestButtonCommand { get; }

        private void TestButton()
        {
            MessageBox.Show("Hello");
        }
    }
}