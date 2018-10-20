using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WalletSampleApi.Models
{
    public class CustomerWallet
    {
        [BsonId]
        public string _id { get; set; }
        public string Username { get; set; }
        public double USD { get; set; }
        public List<CustomerCoin> Coins { get; set; }
    }

    public class CustomerCoin
    {
        public string _id { get; set; }

        public string Symbol { get; set; }
        /// <summary>
        /// ราคาตอนซื้อ
        /// </summary>
        public double BuyingRate { get; set; }
        /// <summary>
        /// ซื้อเมื่อไหร่
        /// </summary>
        public DateTime BuyingAt { get; set; }
        /// <summary>
        /// มูลค่าหากแลกเป็น USD ตอนนี้
        /// </summary>
        public double USDValue { get; set; }
        /// <summary>
        /// จำนวน coin
        /// </summary>
        public double Quantity { get; set; }
        /// <summary>
        /// มูลค่าหากแลกเป็น USD ตอนนี้ * Quantity
        /// </summary>
        public double TotalUSDSellValue { get; set; }

    }
}
