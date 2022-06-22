using Autodesk.Navisworks.Api;
using PM.Navisworks.ZoneTool.Utilities;

namespace PM.Navisworks.ZoneTool.Models
{
    public class ElementsOptions : BindableBase
    {
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