using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ArmaServerBrowser.Logic
{
    internal class LogicModelBase : INotifyPropertyChanged 
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event 
        protected void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (propertyName == null || propertyName == string.Empty) throw new ArgumentException("Argument 'propertyName' is empty!", "propertyName");

            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
