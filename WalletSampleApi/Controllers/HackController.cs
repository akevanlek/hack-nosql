using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using WalletSampleApi.Models;

namespace WalletSampleApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HackController : ControllerBase
    {
        private readonly IMongoClient client;
        private readonly IMongoDatabase database;
        private readonly IMongoCollection<Coin> CoinCollection;
        private readonly IMongoCollection<CustomerWallet> CustomerWalletCollection;


        public HackController()
        {
            client = new MongoClient("mongodb://admin:abcd1234@ds237373.mlab.com:37373/coinwallet");
            database = client.GetDatabase("coinwallet");

            CoinCollection = database.GetCollection<Coin>("coin");
            CustomerWalletCollection = database.GetCollection<CustomerWallet>("ctmwallet");

        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "jdoe", "ptparker" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<CustomerWallet> Get(string id)
        {
            //try
            //{
            var ctmWallet = CustomerWalletCollection.Find(x => x.Username == id).FirstOrDefault();
            var coinprices = CoinCollection.Find(x => true).ToList();

            foreach (var item in ctmWallet.Coins)
            {
                item.USDValue = coinprices.Where(x => x.Symbol == item.Symbol).FirstOrDefault().Sell;
                item.TotalUSDSellValue = coinprices.Where(x => x.Symbol == item.Symbol).FirstOrDefault().Sell * item.Quantity;
            }

            return ctmWallet;
            //}
            //catch (Exception)
            //{

            //    return new CustomerWallet
            //    {


            //    };
            //}


            //return new CustomerWallet
            //{
            //    Username = "jdoe",
            //    Coins = new List<CustomerCoin>
            //    {
            //        new CustomerCoin
            //        {
            //            Symbol = "BTC",
            //            BuyingRate = 6565.25,
            //            BuyingAt = new DateTime(2018, 10, 9, 9, 32, 23),
            //            USDValue = 6500
            //        },
            //        new CustomerCoin
            //        {
            //            Symbol = "ETH",
            //            BuyingRate = 203.47,
            //            BuyingAt = new DateTime(2018, 9, 7, 12, 38, 33),
            //            USDValue = 200.23
            //        },
            //    },
            //};
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] CoinPriceUpdate updateCoin)
        {
            var coinprices = CoinCollection.Find(x => true).ToList();

            foreach (var item in updateCoin.PriceList)
            {
                var coinprice = coinprices.Where(c => c.Symbol == item.Symbol).FirstOrDefault();

                coinprice.Buy = item.Buy;
                coinprice.Sell = item.Sell;

                CoinCollection.ReplaceOne(x => x._id == coinprice._id, coinprice);
            }
        }

        // POST api/values
        [HttpGet]
        public List<Coin> GetCoinPrice()
        {
            return CoinCollection.Find(x => true).ToList();
        }

        // POST api/values
        [HttpPost]
        public void AddCoin([FromBody]Coin newcoin)
        {
            newcoin._id = Guid.NewGuid().ToString();
            CoinCollection.InsertOne(newcoin);
        }

    }
}
