using Autodesk.Navisworks.Api;
using PM.Navisworks.ZoneTool.Commands;
using PM.Navisworks.ZoneTool.Extensions;
using PM.Navisworks.ZoneTool.Models;
using PM.Navisworks.ZoneTool.Utilities;
using System.Windows;

namespace PM.Navisworks.ZoneTool.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        public MainWindowViewModel(Document document)
        {
            _document = document;
            _configuration = new Configuration();
            _configuration.ZoneCategory = "Element";
            _configuration.ZoneProperty = "ZoneNumber";

            AddZoneDataCommand = new DelegateCommand(AddZoneData);
            SelectElementsCommand = new DelegateCommand(SelectElements);
            SelectZonesCommand = new DelegateCommand(SelectZones);
            GetElementsCommand = new DelegateCommand(GetElements);
            GetZonesCommand = new DelegateCommand(GetZones);
        }

        private readonly Document _document;

        private Configuration _configuration;

        public Configuration Configuration
        {
            get { return _configuration; }
            set { SetProperty(ref _configuration, value); }
        }

        private ModelItemCollection _zones;

        public ModelItemCollection Zones
        {
            get { return _zones; }
            set { SetProperty(ref _zones, value); }
        }

        private ModelItemCollection _elements;

        public ModelItemCollection Elements
        {
            get { return _elements; }
            set { SetProperty(ref _elements, value); }
        }

        private string _zoneParameter;

        public string ZoneParameter
        {
            get { return _zoneParameter; }
            set { SetProperty(ref _zoneParameter, value); }
        }

        public DelegateCommand AddZoneDataCommand { get; }

        private void AddZoneData()
        {
            if (_zones == null || _elements == null || Configuration.ZoneCategory == null || Configuration.ZoneProperty == null)
            {
                return;
            }
            _elements.AddZoneToElements(_zones, _configuration.ZoneCategory, _configuration.ZoneProperty);
        }

        public DelegateCommand SelectElementsCommand { get; }

        private void SelectElements()
        {
            if (_document.CurrentSelection.SelectedItems == null)
            {
                return;
            }
            if (_elements == null)
            {
                _elements = new ModelItemCollection();
            }
            _elements.Clear();
            _elements.AddRange(_document.CurrentSelection.SelectedItems);
            MessageBox.Show(_elements.Count.ToString() + " elements have been selected.");
        }

        public DelegateCommand SelectZonesCommand { get; }

        private void SelectZones()
        {
            if (_document.CurrentSelection.SelectedItems == null)
            {
                return;
            }
            if (_zones == null)
            {
                _zones = new ModelItemCollection();
            }
            _zones.Clear();
            _zones.AddRange(_document.CurrentSelection.SelectedItems);
            MessageBox.Show(_zones.Count.ToString() + " zones have been selected.");
        }

        public DelegateCommand GetElementsCommand { get; }

        private void GetElements()
        {
            //var search = new Search();
            //search.Selection.SelectAll();
            //search.PruneBelowMatch = true;

            //var conditionA = SearchCondition.HasPropertyByDisplayName("Element","Workset");
            //var conditionB = SearchCondition.HasPropertyByDisplayName("Item", "Source File").EqualValue(new VariantData("IFC-PMG-00-ZZ-MO-A-002-CLEARANCE.rvt")).Negate();

            //search.SearchConditions.Add(conditionA);
            //search.SearchConditions.Add(conditionB);

            //_elements = search.FindAll(_document, true);
            if (_elements == null)
            {
                _elements = new ModelItemCollection();
            }
            MessageBox.Show(_elements.Count.ToString() + " elements.");
            _document.CurrentSelection.Clear();
            _document.CurrentSelection.AddRange(_elements);
        }

        public DelegateCommand GetZonesCommand { get; }

        private void GetZones()
        {
            //var search = new Search();
            //search.Selection.SelectAll();
            //search.PruneBelowMatch = true;

            //var condition = SearchCondition.HasPropertyByDisplayName("Element", "ZoneNumber");
            //search.SearchConditions.Add(condition);

            //_zones = search.FindAll(_document, true);
            if (_zones == null)
            {
                _zones = new ModelItemCollection();
            }
            MessageBox.Show(_zones.Count.ToString() + " zones.");
            _document.CurrentSelection.Clear();
            _document.CurrentSelection.AddRange(_zones);
        }
    }
}