using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Stock.Models;
using System.Threading;
using NBitcoin.SPV;
using System.Collections.ObjectModel;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Stock.Controllers
{
    [Route("api/[controller]")]
    public class WalletController : BaseController
    {
        private readonly ObservableCollection<WalletViewModel> _Wallets = new ObservableCollection<WalletViewModel>();
        public ObservableCollection<WalletViewModel> Wallets
        {
            get
            {
                return _Wallets;
            }
        }
        

        // GET: api/wallet/GetBalance
        [HttpGet]
        [Route("balance")]
        public object GetBalance(string walletAddress = "943b00f1-1488-4d44-91fa-f3bcc5789099")
        {
            return null;
        }
        // return new ObjectResult(outs);

        [HttpPost]
        [Route("create")]
        public async Task<Wallet> CreateWallet()
        {
            WalletCreationViewModel walletCreationViewModel = new WalletCreationViewModel();
            walletCreationViewModel.Name = new Random().Next(1, 100).ToString();
            WalletCreation creation = walletCreationViewModel.CreateWalletCreation();
            Wallet wallet = await CreateWallet(creation);
            var walletVm = new WalletViewModel(wallet, walletCreationViewModel);
            walletVm.Save();
            if (_ConnectionParameters != null)
            {
                wallet.Configure(_ConnectionParameters);
                wallet.Connect();
            }

            return wallet;
        }

        private Task<Wallet> CreateWallet(WalletCreation creation)
        {
            return Task.Factory.StartNew(() => new Wallet(creation));
        }

    }
}
