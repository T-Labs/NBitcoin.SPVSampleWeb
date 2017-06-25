using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Stock.Models;
using Info.Blockchain.API.BlockExplorer;
using Info.Blockchain.API.Models;
using Info.Blockchain.API.Wallet;
using System.Threading;
using Info.Blockchain.API.Client;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Stock.Controllers
{
    [Route("api/[controller]")]
    public class WalletController : Controller
    {

        private static Wallet wallet;
        private static WalletCreator walletCreator;

        BlockchainApiHelper apiHelper;

        public WalletController()
        {
            apiHelper = new BlockchainApiHelper(serviceUrl: "http://localhost:51006/");
        }

        // GET: api/wallet/GetBalance
        [HttpGet]
        [Route("balance")]
        public async Task<object> GetBalance(string walletAddress = "943b00f1-1488-4d44-91fa-f3bcc5789099")
        {
            // create a new wallet
            walletCreator = apiHelper.CreateWalletCreator();

            BlockExplorer explorer = new BlockExplorer();
            var outs = await explorer.GetUnspentOutputsAsync(new List<string> { walletAddress });

            Thread.Sleep(5000);
            return outs;
        }
        // return new ObjectResult(outs);

        [HttpPost]
        [Route("create")]
        public Wallet Create()
        {
            wallet = apiHelper.InitializeWallet("wallet-identifier", "someComplicated123Password");
           // WalletAddress newAddr = await wallet.NewAddressAsync("test label 123").Result;
            return wallet;
        }

    }
}
