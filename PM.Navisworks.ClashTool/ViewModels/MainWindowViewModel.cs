using Autodesk.Navisworks.Api;
using PM.Navisworks.ZoneTool.Commands;
using PM.Navisworks.ZoneTool.Models;
using PM.Navisworks.DataExtraction.Utilities;

namespace PM.Navisworks.ZoneTool.ViewModels
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