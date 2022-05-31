using PM.Navisworks.DataExtraction.Utilities;

namespace PM.Navisworks.ZoneTool.Models
{
    public class ClashSetPair : BindableBase
    {
        private ClashSet _primarySet;

        public ClashSet PrimarySet
        {
            get { return _primarySet; }
            set { SetProperty(ref _primarySet, value); }
        }

        private ClashSet _secondarySet;

        public ClashSet SecondarySet
        {
            get { return _secondarySet; }
            set { SetProperty(ref _secondarySet, value); }
        }

        private double _tolerance;

        public double Tolerance
        {
            get { return _tolerance; }
            set { SetProperty(ref _tolerance, value); }
        }

        private bool _active;

        public bool Active
        {
            get { return _active; }
            set { SetProperty(ref _active, value); }
        }
    }
}