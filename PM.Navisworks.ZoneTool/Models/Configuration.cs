using PM.Navisworks.ZoneTool.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;

namespace PM.Navisworks.ZoneTool.Models
{
    public class Configuration : BindableBase
    {
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

        private bool _searchBelowSelection = false;

        public bool SearchBelowSelection
        {
            get { return _searchBelowSelection; }
            set { SetProperty(ref _searchBelowSelection, value); }
        }

        private string _searchSetsFolderName = "PMG-Zones";

        public string SelectionSetsFolderName
        {
            get { return _searchSetsFolderName; }
            set { SetProperty(ref _searchSetsFolderName, value); }
        }
        private string _viewsFolderName = "PMG-Zones";

        public string ViewsFolderName
        {
            get { return _viewsFolderName; }
            set { SetProperty(ref _viewsFolderName, value); }
        }
    }
}