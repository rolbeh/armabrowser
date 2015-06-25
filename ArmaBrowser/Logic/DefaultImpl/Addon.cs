using System;
using System.Collections.Generic;
using System.Linq;
using ArmaBrowser.Data;

namespace ArmaBrowser.Logic
{
    class Addon : LogicModelBase, IAddon
    {

        private bool _isActive;
        private long _activationOrder;
        private bool _canActived;
        private IEnumerable<Uri> _downlandUris;
        private IEnumerable<AddonKey> _keyNames;


        public string Name { get; internal set; }

        public string ModName { get; internal set; }

        public string DisplayText { get; internal set; }

        public string Version { get; internal set; }

        public string Path { get; set; }

        public bool IsActive
        {
            get { return _isActive; }
            set 
            { 
                _isActive = value;
                _activationOrder = IsActive ? DateTime.Now.Ticks : 0;
                OnPropertyChanged("ActivationOrder");
                OnPropertyChanged();
            }
        }

        public bool CanActived
        {
            get { return _canActived && IsInstalled; }
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

        public IEnumerable<AddonKey> KeyNames
        {
            get { return _keyNames ?? (_keyNames = new AddonKey[0]); }
            internal set { _keyNames = value; }
        }

        public bool IsInstalled { get; internal set; }

        public IEnumerable<Uri> DownlandUris
        {
            get { return _downlandUris ?? (_downlandUris = new Uri[0]); }
            internal set
            {
                if (Equals(value, _downlandUris)) return;
                _downlandUris = value;
                OnPropertyChanged("IsInstallable");
                OnPropertyChanged();
                
            }
        }

        public bool IsInstallable
        {
            get { return _downlandUris != null && _downlandUris.Any(); }
        }

        public bool IsArmaDefaultPath { get; set; }

        public string CommandlinePath
        {
            get
            {
                return IsArmaDefaultPath ? Name : Path;
            }
        }

        public bool IsEasyInstallable { get; set; }
    }
}
