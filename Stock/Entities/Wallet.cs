using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NBitcoin;

namespace Stock.Entities
{
    public class Wallet
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string WalletInfo { get; set; }
        public string KeysInfo { get; set; }


    }
}
