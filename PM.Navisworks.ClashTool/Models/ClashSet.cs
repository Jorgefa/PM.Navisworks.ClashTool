using PM.Navisworks.DataExtraction.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Navisworks.ClashTool.Models
{
    public class ClashSet : BindableBase
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

    }
}
