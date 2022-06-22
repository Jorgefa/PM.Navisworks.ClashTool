using Autodesk.Navisworks.Api;
using PM.Navisworks.ZoneTool.Utilities;

namespace PM.Navisworks.ZoneTool.Models
{
    public class Configuration : BindableBase
    {
        public Configuration()
        {
            ZonesOptions = new ZonesOptions();
            ElementsOptions = new ElementsOptions();
        }

        private string _zoneCategory;

        public string ZoneCategory
        {
            get { return _zoneCategory; }
            set { SetProperty(ref _zoneCategory, value); }
        }

        private string _zoneProperty;

        public string ZoneProperty
        {
            get { return _zoneProperty; }
            set { SetProperty(ref _zoneProperty, value); }
        }

        private bool _updatePrevValues = false;

        public bool UpdatePrevValues
        {
            get { return _updatePrevValues; }
            set { SetProperty(ref _updatePrevValues, value); }
        }

        private bool _searchBelowSelection = true;

        public bool SearchBelowSelection
        {
            get { return _searchBelowSelection; }
            set { SetProperty(ref _searchBelowSelection, value); }
        }

        private string _folderName = "PMG-Zones";

        public string FolderName
        {
            get { return _folderName; }
            set { SetProperty(ref _folderName, value); }
        }

        private bool _onlyNotEmpty = true;

        public bool OnlyNotEmpty
        {
            get { return _onlyNotEmpty; }
            set { SetProperty(ref _onlyNotEmpty, value); }
        }

        private ZonesOptions _zonesOptions;

        public ZonesOptions ZonesOptions
        {
            get { return _zonesOptions; }
            set { SetProperty(ref _zonesOptions, value); }
        }

        private ElementsOptions _elementsOptions;

        public ElementsOptions ElementsOptions
        {
            get { return _elementsOptions; }
            set { SetProperty(ref _elementsOptions, value); }
        }
    }
}