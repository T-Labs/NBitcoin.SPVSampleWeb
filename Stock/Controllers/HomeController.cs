using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Stock.Models;
using NBitcoin;
using NBitcoin.Protocol;
using System.Threading;

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

            var network = Network.TestNet;

            var secret = new ExtKey().GetWif(network); //new BitcoinSecret("943b00f1-1488-4d44-91fa-f3bcc5789099");
            var key = secret.PrivateKey;

            Transaction tx = new Transaction();
            var input = new TxIn();
            input.PrevOut = new OutPoint(new uint256("943b00f1-1488-4d44-91fa-f3bcc5789099"), 1);
            input.ScriptSig = key.GetBitcoinSecret(network).GetAddress().ScriptPubKey;
            tx.AddInput(input);

            TxOut output = new TxOut();
            var desctination = BitcoinAddress.Create("943b00f1-1488-4d44-91fa-f3bcc5789099");
            Money fee = Money.Satoshis(40000);
            output.Value = Money.Coins(0.1m) - fee;
            output.ScriptPubKey = desctination.ScriptPubKey;
            tx.AddOutput(output);

            tx.Sign(secret, false);

            var node = Node.Connect(network, "76.6.72.220:8333");
            node.VersionHandshake();
            node.SendMessage(new InvPayload());
            node.SendMessage(new TxPayload());

            Thread.Sleep(4000);

            node.Disconnect();

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
