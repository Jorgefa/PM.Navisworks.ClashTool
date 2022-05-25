using Autodesk.Navisworks.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Navisworks.ClashTool.ViewModels
{
    public class MainWindowViewModel
    {
        public MainWindowViewModel(Document document)
        {
            _document = document;
        }

        private readonly Document _document;

        private int myVar;

        public int MyProperty
        {
            get { return myVar; }
            set { myVar = value; }
        }

       


    }
}
