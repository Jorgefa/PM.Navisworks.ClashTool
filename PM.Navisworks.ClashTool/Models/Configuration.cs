using PM.Navisworks.DataExtraction.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;

namespace PM.Navisworks.ClashTool.Models
{
    public class Configuration : BindableBase
    {
        private ObservableCollection<ClashSet> _primaryClashSets;

        public ObservableCollection<ClashSet> PrimaryClashSets
        {
            get { return _primaryClashSets; }
            set { SetProperty(ref _primaryClashSets, value); }
        }

        private ObservableCollection<ClashSet> _secondaryClashSets;

        public ObservableCollection<ClashSet> SecondaryClashSets
        {
            get { return _secondaryClashSets; }
            set { SetProperty(ref _secondaryClashSets, value); }
        }

        private Boolean _primarySetsColisionOnly;

        public Boolean PrimarySetsColisionOnly
        {
            get { return _primarySetsColisionOnly; }
            set { SetProperty(ref _primarySetsColisionOnly, value); }
        }

        private double _defaultTolerance;

        public double DefaultTolerance
        {
            get { return _defaultTolerance; }
            set { SetProperty(ref _defaultTolerance, value); }
        }

        private ObservableCollection<ClashSetPair> _clashSetsPairs;

        public ObservableCollection<ClashSetPair> ClashSetsPairs
        {
            get { return _clashSetsPairs; }
            set { SetProperty(ref _clashSetsPairs, value); }
        }

        private DataTable _clashTestsMatrix;

        public DataTable ClashTestsMatrix
        {
            get { return _clashTestsMatrix; }
            set { SetProperty(ref _clashTestsMatrix, value); }
        }

        public void UpdateClashsSetsPairs()
        {
            ClashSetsPairs = new ObservableCollection<ClashSetPair>();

            if (PrimaryClashSets==null)
            {
                PrimaryClashSets = new ObservableCollection<ClashSet>();
                PrimaryClashSets.Add(new ClashSet { Name = "Set01" });
                PrimaryClashSets.Add(new ClashSet { Name = "Set02" });
                PrimaryClashSets.Add(new ClashSet { Name = "Set03" });

                SecondaryClashSets = new ObservableCollection<ClashSet>();
                SecondaryClashSets.Add(new ClashSet { Name = "SetAA" });
                SecondaryClashSets.Add(new ClashSet { Name = "SetAB" });
                SecondaryClashSets.Add(new ClashSet { Name = "SetAC" });


            }

            if (PrimaryClashSets==null)
            {
                return;
            }
            if (SecondaryClashSets==null && PrimarySetsColisionOnly==false)
            {
                return;
            }

            var ClashsetsPairsList = new List<ClashSetPair>();
            if (_primarySetsColisionOnly)
            {
                foreach (var primarySet in _primaryClashSets)
                {
                    foreach (var secondarySet in _primaryClashSets)
                    {
                        var newClashSetPair = new ClashSetPair();
                        newClashSetPair.PrimarySet = primarySet;
                        newClashSetPair.SecondarySet = secondarySet;
                        newClashSetPair.Tolerance = _defaultTolerance;
                        var duplicated = ClashsetsPairsList.Where(x => x.PrimarySet == secondarySet && x.SecondarySet == primarySet).Count() > 0;
                        if (!duplicated)
                        {
                            newClashSetPair.Active = true;
                        }
                        else
                        {
                            newClashSetPair.Active = false;
                        }
                        ClashsetsPairsList.Add(newClashSetPair);
                    }
                }
            }
            else
            {
                foreach (var primarySet in _primaryClashSets)
                {
                    foreach (var secondarySet in _secondaryClashSets)
                    {
                        var newClashSetPair = new ClashSetPair();
                        newClashSetPair.PrimarySet = primarySet;
                        newClashSetPair.SecondarySet = secondarySet;
                        newClashSetPair.Tolerance = _defaultTolerance;
                        var duplicated = ClashsetsPairsList.Where(x => x.PrimarySet == secondarySet && x.SecondarySet == primarySet).Count() > 0;
                        if (!duplicated)
                        {
                            newClashSetPair.Active = true;
                        }
                        else
                        {
                            newClashSetPair.Active = false;
                        }
                        ClashsetsPairsList.Add(newClashSetPair);
                    }
                }
            }
            ClashSetsPairs = new ObservableCollection<ClashSetPair>(ClashsetsPairsList);
        }

        public void UpdateClashTestsMatrix()
        {
            UpdateClashsSetsPairs();

            var inTable = new DataTable();
            inTable.Columns.Add("P");
            inTable.Columns.Add("S");
            inTable.Columns.Add("T");
            foreach (var pair in ClashSetsPairs)
            {
                var newRow = inTable.NewRow();
                newRow["P"] = pair.PrimarySet.Name;
                newRow["S"] = pair.SecondarySet.Name;
                newRow["T"] = pair.Tolerance;
                inTable.Rows.Add(newRow);
            }

            var outTable = new DataTable();

            var dfq = new Dictionary<String, DataRow>();

            outTable.Columns.Add("Primary/Secondary");

            for (int i = 0; i < inTable.Rows.Count; i++)
            {
                var primary = (string)inTable.Rows[i][0];
                var secondary = (string)inTable.Rows[i][1];
                var tolerance = inTable.Rows[i][2];

                if (!outTable.Columns.Contains(secondary))
                {
                    outTable.Columns.Add(secondary);
                }
                if (dfq.ContainsKey(primary))
                {
                    dfq[primary][secondary] = tolerance;
                }
                else
                {
                    var newRow = outTable.NewRow();
                    newRow[0] = primary;
                    newRow[secondary] = tolerance;
                    dfq.Add(primary, newRow);
                    outTable.Rows.Add(newRow);
                }
            }
            _clashTestsMatrix = outTable;
        }
    }
}