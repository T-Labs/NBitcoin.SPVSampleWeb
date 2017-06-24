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

            //mysLDbXCoie81EysydNHfSTva5sy4rRzKB

            var secret = new ExtKey().GetWif(network); //new BitcoinSecret("943b00f1-1488-4d44-91fa-f3bcc5789099");
            var key = secret.PrivateKey;

            Transaction tx = new Transaction();
            var input = new TxIn();
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

            var node = Node.ConnectToLocal(network);
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
