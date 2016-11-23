using System.ComponentModel;

namespace BDMultiTool.Macros {
    public class MacroItemModel : INotifyPropertyChanged
    {

        private string _macroName;
        public string MacroName
        {
            get { return _macroName; }
            set
            {
                _macroName = value;
                OnPropertyChanged("macroName");
            }
        }

        private bool _paused;
        public bool Paused
        {
            get { return _paused; }
            set
            {
                _paused = value;
                OnPropertyChanged("Paused");
            }
        }

        private bool _addMode;
        public bool AddMode
        {
            get { return _addMode; }
            set
            {
                _addMode = value;
                OnPropertyChanged("AddMode");
            }
        }

        private bool _notPaused;
        public bool NotPaused
        {
            get { return _notPaused; }
            set
            {
                _notPaused = value;
                OnPropertyChanged("NotPaused");
            }
        }

        private string _coolDownTime;
        public string CoolDownTime
        {
            get { return _coolDownTime; }
            set
            {
                _coolDownTime = value;
                OnPropertyChanged("coolDownTime");
            }
        }

        private string _keyString;
        public string KeyString
        {
            get { return _keyString; }
            set
            {
                _keyString = value;
                OnPropertyChanged("keyString");
            }
        }

        private string _lifeTime;
        public string LifeTime
        {
            get { return _lifeTime; }
            set
            {
                _lifeTime = value;
                OnPropertyChanged("lifeTime");
            }
        }

        private float _coolDownPercentage;
        public float CoolDownPercentage
        {
            get { return _coolDownPercentage; }
            set
            {
                _coolDownPercentage = value;
                OnPropertyChanged("coolDownPercentage");
            }
        }

        private float _lifeTimePercentage;
        public float LifeTimePercentage
        {
            get { return _lifeTimePercentage; }
            set
            {
                _lifeTimePercentage = value;
                OnPropertyChanged("lifeTimePercentage");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name) {
            var handler = PropertyChanged;

            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
