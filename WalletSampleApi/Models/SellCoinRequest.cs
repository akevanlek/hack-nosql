using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WalletSampleApi.Models
{
    public class SellCoinRequest
    {
        public string UserName { get; set; }
        public string CustomerCoinId { get; set; }
    }
}
