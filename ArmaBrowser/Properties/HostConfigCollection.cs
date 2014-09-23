using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmaBrowser.Properties
{
    [Serializable]
    public class HostConfig
    {
        public string EndPoint {get;set;}
        
        public string  PossibleAddons { get; set; }

        internal string Key
        {
            get { return string.Format("{0}", EndPoint); }
        }
    }

    [Serializable] 
    public class HostConfigCollection : IEnumerable<HostConfig>
    {
        private ArrayList _employees = new ArrayList();
          
        public void Add(HostConfig element)
        {
            _employees.Add(element);
        }

        public void Clear()
        {
            _employees.Clear();
        }

        public int IndexOf(HostConfig element)
        {
            return _employees.IndexOf(element);
        }

        public void Remove(HostConfig element)
        {
            _employees.Remove(element);
        }

        public void RemoveAt(int index)
        {
            _employees.RemoveAt(index);
        }

        public HostConfig this[int index]
        {
            get { return (HostConfig)_employees[index]; }
            set
            {
                _employees[index] =  value;
            }
        }

        public IEnumerator<HostConfig> GetEnumerator()
        {
            return _employees.Cast<HostConfig>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _employees.GetEnumerator();
        }
    }

    
}
