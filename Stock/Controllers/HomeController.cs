using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Stock.Models;
using NBitcoin;
using NBitcoin.Protocol;
using System.Threading;
using QBitNinja.Client.Models;
using QBitNinja.Client;
using Stock.Models.Transactions;

namespace Stock.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var vm = new MainWindowViewModel();
            return View(vm);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            var network = Network.Main;

            //mysLDbXCoie81EysydNHfSTva5sy4rRzKB

            var secret = new ExtKey().GetWif(network); //new BitcoinSecret("943b00f1-1488-4d44-91fa-f3bcc5789099");
            var key = secret.PrivateKey;
            var client = new QBitNinjaClient(network);

            Transaction tx = new Transaction();
            var input = new TxIn();

            var transactionId = uint256.Parse("dc7268d85689fe3a4dade2a5886794237221fb0161e59f12d8128c46ca7fab90");
            var transactionResponse = client.GetTransaction(transactionId).Result;

            input.PrevOut = new OutPoint(new uint256("dc7268d85689fe3a4dade2a5886794237221fb0161e59f12d8128c46ca7fab90"), 1); //Transaction ID
            input.ScriptSig = key.GetBitcoinSecret(network).GetAddress().ScriptPubKey;
            tx.AddInput(input);

            TxOut output = new TxOut();
            var desctination = BitcoinAddress.Create("mwCwTceJvYV27KXBc3NJZys6CjsgsoeHmf");
            Money fee = Money.Satoshis(40000);
            output.Value = Money.Coins(0.1m) - fee;
            output.ScriptPubKey = desctination.ScriptPubKey;
            tx.AddOutput(output);

            tx.Sign(secret, false);

            BroadcastResponse broadcastResponse = client.Broadcast(tx).Result;

            return View();
        }

        public IActionResult Contact(TransactionTemporaryViewModel transactionVM)
        {
            ViewData["Message"] = "Your contact page.";

            var network = Network.Main;

            // generate private key

            var privateKey = new Key();
            var bitcoinPrivateKey = privateKey.GetWif(network);
            var address = bitcoinPrivateKey.GetAddress();

            // get transaction info

            var client = new QBitNinjaClient(network);
            var transactionId = uint256.Parse(transactionVM.Hex);
            var transactionResponse = client.GetTransaction(transactionId).Result;

            // from where

            var receivedCoins = transactionResponse.ReceivedCoins;
            OutPoint outPointToSpend = null;
            foreach (var coin in receivedCoins)
            {
                if (coin.TxOut.ScriptPubKey == bitcoinPrivateKey.ScriptPubKey)
                {
                    outPointToSpend = coin.Outpoint;
                }
            }

            // create transaction

            var transaction = new Transaction();
            transaction.Inputs.Add(new TxIn()
            {
                PrevOut = outPointToSpend
            });

            // to where

            var destinationAddress = BitcoinAddress.Create("mzp4No5cmCXjZUpf112B1XWsvWBfws5bbB");

            // how match

            TxOut spentTxOut = new TxOut()
            {
                Value = new Money((decimal)transactionVM.Quantity, MoneyUnit.BTC),
                ScriptPubKey = destinationAddress.ScriptPubKey
            };

            TxOut changeBackTxOut = new TxOut()
            {
                Value = new Money((decimal)(transactionVM.Balance - transactionVM.Quantity - transactionVM.Fee), MoneyUnit.BTC),
                ScriptPubKey = bitcoinPrivateKey.ScriptPubKey
            };

            transaction.Outputs.Add(spentTxOut);
            transaction.Outputs.Add(changeBackTxOut);

            // sign transaction

            transaction.Sign(bitcoinPrivateKey, false);

            BroadcastResponse broadcastResponse = client.Broadcast(transaction).Result;

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
