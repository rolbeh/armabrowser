using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArmaBrowser.Logic;

namespace ArmaBrowser.ViewModel
{
    class ProgressState : ObjectNotify, IProgressState
    {
        private int _maximum;
        private int _minimum;
        private int _value;

        public int Maximum
        {
            get { return _maximum; }
            set
            {
                if (value == _maximum) return;
                _maximum = value;
                OnPropertyChanged();
            }
        }

        public int Minimum
        {
            get { return _minimum; }
            set
            {
                if (value == _minimum) return;
                _minimum = value;
                OnPropertyChanged();
            }
        }

        public int Value
        {
            get { return _value; }
            set
            {
                if (value == _value) return;
                _value = value;
                OnPropertyChanged();
            }
        }
    }
}
