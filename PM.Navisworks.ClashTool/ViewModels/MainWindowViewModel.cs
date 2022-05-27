using Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.Clash;
using Autodesk.Navisworks.Api.DocumentParts;
using PM.Navisworks.ClashTool.Commands;
using PM.Navisworks.ClashTool.Models;
using PM.Navisworks.DataExtraction.Utilities;
using System.Windows.Forms;

namespace PM.Navisworks.ClashTool.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        public MainWindowViewModel(Document document)
        {
            _document = document;
            _configuration = new Configuration();
            TestButton();

            TestButtonCommand = new DelegateCommand(TestButton);
        }

        private readonly Document _document;

        private Configuration _configuration;

        public Configuration Configuration
        {
            get { return _configuration; }
            set { SetProperty(ref _configuration, value); }
        }

        public DelegateCommand TestButtonCommand { get; }

        private void TestButton()
        {
            _configuration.UpdateClashTestsMatrix();

        }
    }
}