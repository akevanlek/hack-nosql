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
            client = new MongoClient("mongodb://admin:A123456@ds139428.mlab.com:39428/coinwallet");
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
            if (ctmWallet != null)
            {
                var coinprices = CoinCollection.Find(x => true).ToList();

                foreach (var item in ctmWallet.Coins)
                {
                    item.USDValue = coinprices.Where(x => x.Symbol == item.Symbol).FirstOrDefault().Sell;
                    item.TotalUSDSellValue = coinprices.Where(x => x.Symbol == item.Symbol).FirstOrDefault().Sell * item.Quantity;
                }

                return ctmWallet;
            }
            else
            {
                CustomerWallet ctmMock = new CustomerWallet()
                {
                    _id = Guid.NewGuid().ToString(),
                    USD = 10000000,
                    Username = id,
                    Coins = new List<CustomerCoin>() { },
                };

                CustomerWalletCollection.InsertOne(ctmMock);
                return ctmMock;
            }

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
                if (coinprice != null)
                {
                    coinprice.Buy = item.Buy;
                    coinprice.Sell = item.Sell;

                    CoinCollection.ReplaceOne(x => x._id == coinprice._id, coinprice);
                }
                else
                {
                    Coin newcoin = new Coin()
                    {
                        _id = Guid.NewGuid().ToString(),
                        Symbol = item.Symbol,
                        Sell = item.Sell,
                        Buy = item.Buy
                    };
                    CoinCollection.InsertOne(newcoin);
                }


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

        [HttpPost]
        public CustomerCoin BuyCoin([FromBody]BuyCoinRequest request)
        {
            var ctmWallet = CustomerWalletCollection.Find(x => x.Username == request.UserName).FirstOrDefault();

            var coinprice = CoinCollection.Find(x => x.Symbol == request.Symbol).FirstOrDefault();

            var price = coinprice.Sell * request.Quantity;

            if (ctmWallet.USD >= price)
            {
                CustomerCoin newCoin = new CustomerCoin()
                {
                    _id = Guid.NewGuid().ToString(),
                    BuyingAt = DateTime.Now,
                    Quantity = request.Quantity,
                    BuyingRate = coinprice.Sell,
                    Symbol = request.Symbol,
                    USDValue = coinprice.Sell,
                    TotalUSDSellValue = coinprice.Sell * request.Quantity,
                };

                var customerCoins = ctmWallet.Coins;
                customerCoins.Add(newCoin);

                ctmWallet.Coins = customerCoins;
                ctmWallet.USD = ctmWallet.USD - price;


                CustomerWalletCollection.ReplaceOne(x => x._id == ctmWallet._id, ctmWallet);

                return newCoin;
            }
            else
            {
                return new CustomerCoin();
            }
        }

        [HttpPost]
        public CustomerCoin SellCoin([FromBody]SellCoinRequest request)
        {
            var ctmWallet = CustomerWalletCollection.Find(x => x.Username == request.UserName).FirstOrDefault();
            var customerCoin = ctmWallet.Coins.Where(x => x._id == request.CustomerCoinId).FirstOrDefault();
            var coinprice = CoinCollection.Find(x => x.Symbol == customerCoin.Symbol).FirstOrDefault();

            var price = coinprice.Sell * customerCoin.Quantity;

            var customerCoins = ctmWallet.Coins;

            customerCoins.Remove(customerCoin);
            ctmWallet.Coins = customerCoins;

            ctmWallet.USD = ctmWallet.USD + price;

            CustomerWalletCollection.ReplaceOne(x => x._id == ctmWallet._id, ctmWallet);

            customerCoin.USDValue = coinprice.Sell;
            customerCoin.TotalUSDSellValue = coinprice.Sell * customerCoin.Quantity;
            return customerCoin;

        }

        //[HttpGet]
        //public void AddCoinMock()
        //{
        //    List<Coin> coins = new List<Coin>()
        //    {
        //        new Coin(){_id = Guid.NewGuid().ToString(),Symbol ="Bitcoin",Buy =68.54, Sell =65.15 },
        //        new Coin(){_id = Guid.NewGuid().ToString(),Symbol ="EOS",Buy =54.12, Sell =53.22 },
        //        new Coin(){_id = Guid.NewGuid().ToString(),Symbol ="Stellar",Buy =79.15, Sell =77.12 },
        //        new Coin(){_id = Guid.NewGuid().ToString(),Symbol ="TRON",Buy =97.15, Sell =96.33 },
        //    };

        //    foreach (var item in coins)
        //    {
        //        CoinCollection.InsertOne(item);
        //    }

        //}

        //[HttpGet]
        //public void AddCtmMock()
        //{
        //    CustomerWallet ctmMock = new CustomerWallet()
        //    {
        //        _id = Guid.NewGuid().ToString(),
        //        USD = 10000000,
        //        Username = "ake",
        //        Coins = new List<CustomerCoin>() {
        //            new CustomerCoin(){ _id = Guid.NewGuid().ToString(), Symbol = "Bitcoin",BuyingRate = 65.15,BuyingAt = DateTime.Now,Quantity =3 , USDValue =0, TotalUSDSellValue =0 },
        //            new CustomerCoin(){ _id = Guid.NewGuid().ToString(), Symbol = "Bitcoin",BuyingRate = 63.45,BuyingAt = DateTime.Now,Quantity =2 , USDValue =0, TotalUSDSellValue =0 },
        //            new CustomerCoin(){ _id = Guid.NewGuid().ToString(), Symbol = "Stellar",BuyingRate = 78.77,BuyingAt = DateTime.Now,Quantity =5 , USDValue =0, TotalUSDSellValue =0 },
        //            new CustomerCoin(){ _id = Guid.NewGuid().ToString(), Symbol = "TRON",BuyingRate = 96.31,BuyingAt = DateTime.Now,Quantity =1 , USDValue =0, TotalUSDSellValue =0 },

        //        },
        //    };

        //    CustomerWalletCollection.InsertOne(ctmMock);
        //}
    }
}
