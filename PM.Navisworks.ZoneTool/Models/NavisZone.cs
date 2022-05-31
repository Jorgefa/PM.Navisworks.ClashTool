using Autodesk.Navisworks.Api;
using PM.Navisworks.DataExtraction.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Navisworks.ZoneTool.Models
{
    public class NavisZone : BindableBase
    {
        private string _name;

        public string Code
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private ModelItem _element;

        public ModelItem Element
        {
            get { return _element; }
            set { SetProperty(ref _element, value); }
        }

    }
}
