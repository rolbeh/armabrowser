using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmaServerBrowser.Logic.DefaultImpl
{
    class ServerItem : LogicModelBase, IServerItem
    {
        const string imageUrl = "http://arma3.swec.se/images/flags/png/{0}.png";
        #region Fields
        private string _country = "--";
        private string _countryUrl = string.Format(imageUrl, "--");

        private int _maxNumPlayers;
        private string _mission;
        private string _version;

        #endregion Field

        public string Gamename { get; internal set; }

        public string Mode { get; internal set; }

        public string Name { get; internal set; }

        public string Mission
        {
            get { return _mission; }
            set
            {
                _mission = value;
                OnPropertyChanged();
            }
        }

        public string Country
        {
            get { return _country; }
            set
            {
                _country = value;
                if (_country != null) _country = _country.ToLower();
                _countryUrl = null;
            }
        }

        public string CountryUrl
        {
            get { return _countryUrl ?? (_countryUrl = string.Format(imageUrl, _country ?? "--")); }
        }
        
        public string Host { get; internal set; }

        public int Port { get; internal set; }

        public string Version
        {
            get { return _version; }
            set
            {
                if (_version == value) return;
                _version = value;
                OnPropertyChanged();
            }
        }

        //public string Version { get; internal set; }

        public string ModsText
        {
            get { return _modsText; }
            internal set
            {
                _modsText = value ?? string.Empty;
                Mods = _modsText.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        public string[] Mods { get; internal set; }

        public string Modhashs { get; internal set; }

        public int Players
        {
            get { return _maxNumPlayers; }
            set
            {
                _maxNumPlayers = value;
                OnPropertyChanged();
            }
        }

        public string Island { get; internal set; }

        public string Signatures { get; internal set; }

        public string FullText
        {
            get { return string.Format("{0} {1}", Name, Mission); }
        }

        public string _modsText { get; set; }

        public bool Passworded { get; set; }

        public bool IsVersionOk { get; set; }
    }
}
