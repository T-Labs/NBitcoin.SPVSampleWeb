using NBitcoin;
using NBitcoin.SPV;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Stock.Models
{
    public class WalletKeyViewModel : ICommand, INotifyPropertyChanged
    {
        WalletCreationViewModel parent;
        public WalletKeyViewModel(WalletCreationViewModel parent)
        {
            this.parent = parent;
        }
        private BitcoinExtKey _PrivateKey;
        public BitcoinExtKey PrivateKey
        {
            get
            {
                return _PrivateKey;
            }
            set
            {
                if (value != _PrivateKey)
                {
                    _PrivateKey = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("PrivateKey"));
                }
            }
        }
        private BitcoinExtPubKey _PubKey;
        public BitcoinExtPubKey PubKey
        {
            get
            {
                return _PubKey;
            }
            set
            {
                if (value != _PubKey)
                {
                    _PubKey = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("PubKey"));
                    parent.Update();
                }
            }
        }

        public ICommand Generate
        {
            get
            {
                return this;
            }
        }

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            PrivateKey = new ExtKey().GetWif(Network.Main);
            PubKey = PrivateKey.ExtKey.Neuter().GetWif(Network.Main);
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
    public class WalletCreationViewModel
    {
        public WalletCreationViewModel()
        {
            SigRequired = 1;
            Keys.Add(new WalletKeyViewModel(this));
        }
        public string Name { get; set; }
        public int SigRequired { get; set; }

        public bool IsP2SH { get; set; }

        private readonly ObservableCollection<WalletKeyViewModel> _Keys = new ObservableCollection<WalletKeyViewModel>();
        public ObservableCollection<WalletKeyViewModel> Keys
        {
            get
            {
                return _Keys;
            }
        }

        internal void Update()
        {
            foreach (var key in Keys.Reverse().ToList())
            {
                if (key.PubKey == null && Keys.Count > 1)
                    Keys.Remove(key);
                else
                {
                    Keys.Add(new WalletKeyViewModel(this));
                    break;
                }
            }
        }

        internal WalletCreation CreateWalletCreation()
        {
            return new WalletCreation()
            {
                SignatureRequired = SigRequired,
                UseP2SH = IsP2SH,
                Network = Network.Main,
                RootKeys = Keys.Where(k => k.PubKey != null).Select(k => k.PubKey.ExtPubKey).ToArray()
            };
        }

        public bool IsValid
        {
            get
            {
                return RealKeys.Count() != 0 &&
                    SigRequired <= RealKeys.Count() &&
                    SigRequired >= 1;
            }
        }

        public IEnumerable<WalletKeyViewModel> RealKeys
        {
            get
            {
                return Keys.Where(k => k.PubKey != null);
            }
        }
    }
}
