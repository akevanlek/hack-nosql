using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WalletSampleApi.Models
{
    public class BuyCoinRequest
    {
        public string UserName { get; set; }
        public string Symbol { get; set; }
        public double Quantity { get; set; }
    }
}
