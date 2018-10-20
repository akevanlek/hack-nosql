using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WalletSampleApi.Models
{
    public class Coin
    {
        [BsonId]
        public string _id { get; set; }

        public string Symbol { get; set; }
        public double Buy { get; set; }
        public double Sell { get; set; }
    }
}
