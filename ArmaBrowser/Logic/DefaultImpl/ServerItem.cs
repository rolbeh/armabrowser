using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmaBrowser.Logic.DefaultImpl
{
    class ServerItem : LogicModelBase, IServerItem, ArmaBrowser.Data.IServerQueryAddress
    {
        //const string imageUrl = "http://arma3.swec.se/images/flags/png/{0}.png";
        #region Fields
        //private string _country = "--";
        //private string _countryUrl = string.Format(imageUrl, "--");

        private int _playersNum;
        private string _mission;
        private string _version;
        private string _currentPlayersText;
        private string _signatures;
        private int _maxPlayers;
        private string _name;
        private string _mode;
        private Data.ISteamGameServerPlayer[] _currentPlayers;
        private long _ping;
        private DateTime? _lastPlayed;
        private bool _isFavorite;

        #endregion Field

        public bool IsFavorite
        {
            get { return _isFavorite; }
            set
            {
                _isFavorite = value;
                OnPropertyChanged();
                GroupName = ServerItemGroup.Recently;
            }
        }

        public DateTime? LastPlayed  
        {
            get { return _lastPlayed; }
            set
            {
                _lastPlayed = value;
                GroupName = ServerItemGroup.Recently;
                OnPropertyChanged();
            }
        }

        public ServerItemGroup GroupName
        {
            get 
            {
                if (_isFavorite) return  ServerItemGroup.Favorite;
                if (LastPlayed.HasValue) return ServerItemGroup.Recently;
                if (Port > 0) return ServerItemGroup.Found;
                return ServerItemGroup.NoResponse;
            }
            private set { OnPropertyChanged(); }
        }

        internal ServerItem()
        {
            Signatures = string.Empty;
        }

        public string Gamename { get; internal set; }

        public string Mode
        {
            get { return _mode; }
            set
            {
                _mode = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public string Mission
        {
            get { return _mission; }
            set
            {
                _mission = value;
                OnPropertyChanged();
            }
        }

        //public string Country
        //{
        //    get { return _country; }
        //    set
        //    {
        //        _country = value;
        //        if (_country != null) _country = _country.ToLower();
        //        _countryUrl = null;
        //    }
        //}

        //public string CountryUrl
        //{
        //    get { return _countryUrl ?? (_countryUrl = string.Format(imageUrl, _country ?? "--")); }
        //}

        public System.Net.IPAddress Host { get; internal set; }

        public int Port { get; internal set; }

        public string Endpoint
        {
            get { return string.Format("{0}:{1}", Host, Port); }
        }

        public int QueryPort { get; internal set; }

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

        public string[] Mods { get; private set; }

        public string Modhashs { get; internal set; }

        public int CurrentPlayerCount
        {
            get { return _playersNum; }
            set
            {
                _playersNum = value;
                OnPropertyChanged("IsPlayerSlotsFull");
                OnPropertyChanged("PlayersState");
                OnPropertyChanged();
            }
        }

        public int MaxPlayers
        {
            get { return _maxPlayers; }
            set
            {
                _maxPlayers = value;
                OnPropertyChanged("IsPlayerSlotsFull");
                OnPropertyChanged("PlayersState");
                OnPropertyChanged();
            }
        }

        public string PlayersState
        {
            get { return string.Format("{0}/{1}", _playersNum, _maxPlayers); }
        }

        public Data.ISteamGameServerPlayer[] CurrentPlayers
        {
            get { return _currentPlayers; }
            internal set
            {
                if (_currentPlayers == value) return;
                    _currentPlayers = value;
                OnPropertyChanged();
            }
        }

        public bool IsPlayerSlotsFull
        {
            get { return _maxPlayers == _playersNum; }
        }

        public string CurrentPlayersText
        {
            get { return _currentPlayersText; }
            internal set
            {
                if (_currentPlayersText == value) return;
                _currentPlayersText = value;
                OnPropertyChanged();
            }
        }

        public string Island { get; internal set; }


        public string Signatures
        {
            get { return _signatures; }
            internal set
            {
                _signatures = (value ?? string.Empty);
            }
        }

        public string FullText
        {
            get { return string.Format("{0} {1} {2}", Name, Mission, Island); }
        }

        public string _modsText { get; set; }

        public bool Passworded { get; set; }

        public bool IsVersionOk { get; set; }

        public long Ping
        {
            get { return _ping; }
            internal set
            {
                if (_ping == value) return;
                _ping = value;
                OnPropertyChanged();
            }
        }

    }

}
