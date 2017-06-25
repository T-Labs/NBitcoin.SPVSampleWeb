using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NBitcoin;
using QBitNinja.Client;
using Stock.Models.Transactions;
using QBitNinja.Client.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Stock.Controllers
{
    [Route("api/[controller]")]
    public class TransactionController : BaseController
    {
        [HttpPost]
        [Route("create")]
        public void create(TransactionTemporaryViewModel transactionVM)
        {
            var network = Network.TestNet;

            var secret = new ExtKey().GetWif(network);
            var source = BitcoinAddress.Create(transactionVM.SourceAddress);
            var client = new QBitNinjaClient(network);

            Transaction tx = new Transaction();
            var input = new TxIn();

            var transactionId = uint256.Parse(transactionVM.PrevTransactionId);

            input.PrevOut = new OutPoint(new uint256(transactionVM.Hex), 1); //Transaction ID
            input.ScriptSig = source.ScriptPubKey;
            tx.AddInput(input);

            TxOut output = new TxOut();
            var desctination = BitcoinAddress.Create(transactionVM.DestinationAddress);
            Money fee = Money.Satoshis(40000);
            output.Value = Money.Coins(0.1m) - fee;
            output.ScriptPubKey = desctination.ScriptPubKey;
            tx.AddOutput(output);

            tx.Sign(secret, false);

            BroadcastResponse broadcastResponse = client.Broadcast(tx).Result;
        }
    }
}
