using Autodesk.Navisworks.Api;
using PM.Navisworks.ZoneTool.Commands;
using PM.Navisworks.ZoneTool.Extensions;
using PM.Navisworks.ZoneTool.Models;
using PM.Navisworks.ZoneTool.Utilities;
using PM.Navisworks.ZoneTool.Utilities.ProgressBar;
using System;
using System.Collections.Generic;
using System.Windows;

namespace PM.Navisworks.ZoneTool.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        public MainWindowViewModel(Document document)
        {
            _document = document;
            Configuration = new Configuration();
            Configuration.ZonesOptions.CodeCategory = "Element";
            Configuration.ZonesOptions.CodeProperty = "ZoneNumber";

            TestCommand = new DelegateCommand(Test);

            AddZoneDataCommand = new DelegateCommand(AddZoneData);
            SelectElementsCommand = new DelegateCommand(SelectElements);
            SelectZonesCommand = new DelegateCommand(SelectZones);
            GetElementsCommand = new DelegateCommand(GetElements);
            GetZonesCommand = new DelegateCommand(GetZones);
            CreateSelectionSetsCommand = new DelegateCommand(CreateSelectionSets);
            CreateSelectionSetsAndViewsCommand = new DelegateCommand(CreateSelectionSetsAndViews);
        }

        //Properties

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

        //Commands

        public DelegateCommand AddZoneDataCommand { get; }

        private void AddZoneData()
        {
            if (Zones == null || _elements == null || Configuration.ZoneCategory == null || Configuration.ZoneProperty == null)
            {
                return;
            }
            _elements.AddZoneToElements(_zones, _configuration);
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

            var newElements = _document.GetElementsFromSelection(_document.CurrentSelection.SelectedItems, Configuration);

            _elements.Clear();
            _elements.AddRange(newElements);
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

            var newZones = _document.GetZonesFromSelection(_document.CurrentSelection.SelectedItems, Configuration);

            _zones.Clear();
            _zones.AddRange(newZones);
        }

        public DelegateCommand GetElementsCommand { get; }

        private void GetElements()
        {
            try
            {
                if (_elements == null)
                {
                    _elements = new ModelItemCollection();
                }
                _document.CurrentSelection.Clear();
                _document.CurrentSelection.AddRange(_elements);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "\n" + e.StackTrace);
            }
            finally
            {
                MessageBox.Show(_elements.Count.ToString() + " elements.");
            }
        }

        public DelegateCommand GetZonesCommand { get; }

        private void GetZones()
        {
            try
            {
                if (_zones == null)
                {
                    _zones = new ModelItemCollection();
                }
                _document.CurrentSelection.Clear();
                _document.CurrentSelection.AddRange(_zones);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "\n" + e.StackTrace);
            }
            finally
            {
                MessageBox.Show(_zones.Count.ToString() + " zones.");
            }
        }

        public DelegateCommand CreateSelectionSetsCommand { get; }

        private void CreateSelectionSets()
        {
            if (_zones == null)
            {
                return;
            }
            if (_elements == null)
            {
                return;
            }

            _document.CreateZoneSelectionSets(_elements, _zones, _configuration);
        }

        public DelegateCommand CreateSelectionSetsAndViewsCommand { get; }

        private void CreateSelectionSetsAndViews()
        {
            if (_zones == null)
            {
                return;
            }
            if (_elements == null)
            {
                return;
            }

            _document.CreateZoneSelectionSetsAndViews(_elements, _zones, _configuration);
        }

        public DelegateCommand TestCommand { get; }

        private void Test()
        {
            if (_zones == null)
            {
                return;
            }
            if (_elements == null)
            {
                return;
            }
            if (_elements.Count == 0)
            {
                _elements = _document.CurrentSelection.SelectedItems;
            }

            _document.Test(_elements);
        }
    }
}