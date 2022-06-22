using Autodesk.Navisworks.Api;
using PM.Navisworks.ZoneTool.Utilities;

namespace PM.Navisworks.ZoneTool.Models
{
    public class ZonesOptions : BindableBase
    {
        private string _codeCategory;

        public string CodeCategory
        {
            get { return _codeCategory; }
            set { SetProperty(ref _codeCategory, value); }
        }

        private string _codeProperty;

        public string CodeProperty
        {
            get { return _codeProperty; }
            set { SetProperty(ref _codeProperty, value); }
        }

        private bool _pruneBelowMatch = true;

        public bool PruneBelowMatch
        {
            get { return _pruneBelowMatch; }
            set { SetProperty(ref _pruneBelowMatch, value); }
        }

        private SearchLocations _searchLocations = SearchLocations.Self;

        public SearchLocations SearchLocations
        {
            get { return _searchLocations; }
            set { SetProperty(ref _searchLocations, value); }
        }
    }
}