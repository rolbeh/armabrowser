using System;
using System.Net;
using ArmaBrowser.Data;
using Magic.Steam;

namespace ArmaBrowser.Logic
{
    internal class ServerItem : ObjectNotify, IServerItem, IServerQueryAddress
    {
        private string _modsText;

        internal ServerItem()
        {
            Signatures = string.Empty;
        }

        public ServerItemGroup GroupName
        {
            get
            {
                if (_isFavorite) return ServerItemGroup.Favorite;
                if (LastPlayed.HasValue) return ServerItemGroup.Recently;
                if (Port > 0) return ServerItemGroup.Found;
                return ServerItemGroup.NoResponse;
            }
            // ReSharper disable once ValueParameterNotUsed
            private set => OnPropertyChanged();
        }

        // ReSharper disable once UnusedMember.Global
        public string Endpoint => string.Format("{0}:{1}", Host, Port);

        public string PlayersState => string.Format("{0}/{1}", _playersNum, _maxPlayers);

        public bool IsFavorite
        {
            get => _isFavorite;
            set
            {
                _isFavorite = value;
                OnPropertyChanged();
                GroupName = ServerItemGroup.Favorite;
            }
        }

        public DateTime? LastPlayed
        {
            get => _lastPlayed;
            set
            {
                _lastPlayed = value;
                GroupName = ServerItemGroup.Recently;
                OnPropertyChanged();
            }
        }

        public string Gamename { get; internal set; }

        public string Mode
        {
            get => _mode;
            set
            {
                _mode = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public string Mission
        {
            get => _mission;
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

        public IPAddress Host { get; internal set; }

        public int Port { get; internal set; }

        public int QueryPort { get; internal set; }

        public string Version
        {
            get => _version;
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
            get => _modsText;
            internal set
            {
                _modsText = value ?? string.Empty;
                Mods = _modsText.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        public string[] Mods { get; private set; }

        public string Modhashs { get; internal set; }

        public string Signatures
        {
            get => _signatures;
            internal set => _signatures = value ?? string.Empty;
        }

        public int CurrentPlayerCount
        {
            get => _playersNum;
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
            get => _maxPlayers;
            set
            {
                _maxPlayers = value;
                OnPropertyChanged("IsPlayerSlotsFull");
                OnPropertyChanged("PlayersState");
                OnPropertyChanged();
            }
        }

        public ISteamGameServerPlayer[] CurrentPlayers
        {
            get => _currentPlayers;
            internal set
            {
                if (_currentPlayers == value) return;
                _currentPlayers = value;
                OnPropertyChanged();
            }
        }

        public bool IsPlayerSlotsFull => _maxPlayers == _playersNum;

        public string CurrentPlayersText
        {
            get => _currentPlayersText;
            internal set
            {
                if (_currentPlayersText == value) return;
                _currentPlayersText = value;
                OnPropertyChanged();
            }
        }

        public string Island { get; internal set; }

        public string FullText => string.Format("{0} {1} {2}", Name, Mission, Island);

        public bool Passworded { get; set; }

        public int Ping
        {
            get => _ping;
            internal set
            {
                if (_ping == value) return;
                _ping = value;
                OnPropertyChanged();
            }
        }


        public bool VerifySignatures { get; set; }
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
        private ISteamGameServerPlayer[] _currentPlayers;
        private int _ping;
        private DateTime? _lastPlayed;
        private bool _isFavorite;

        #endregion Field
    }
}