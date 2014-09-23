
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmaBrowser.Logic.DefaultImpl
{
    class Addon : LogicModelBase, IAddon
    {

        private bool _isActive;
        private long _activationOrder;
        private bool _canActived;


        public string Name { get; internal set; }

        public string ModName { get; internal set; }

        public string DisplayText { get; internal set; }

        public string Version { get; internal set; }

        public bool IsActive
        {
            get { return _isActive; }
            set 
            { 
                _isActive = value;
                if (IsActive == true)
                    _activationOrder = DateTime.Now.Ticks;
                else
                    _activationOrder = 0;
                OnPropertyChanged("ActivationOrder");
                OnPropertyChanged();
            }
        }

        public bool CanActived
        {
            get { return _canActived; }
            set
            {
                if (_canActived == value) return;
                _canActived = value;
                   OnPropertyChanged();
            }
        }
        
        public long ActivationOrder
        {
            get
            {
                return _activationOrder;
            }
        }

        public IEnumerable<string> KeyNames { get; internal set; }
    }
}
