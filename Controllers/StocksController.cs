using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using dotnet.Data;
using dotnet.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace dotnet.Controllers
{
    public class StocksController : Controller
    {
        private readonly ApplicationDbContext _db;
        private IEnumerable<Stock> savedObj;

        public StocksController(ApplicationDbContext db)
        {
            this._db=db;
        }
        public IActionResult Index()
        {
            IEnumerable<Stock?> obj= _db.Stocks!.OrderByDescending(y=>y.Date.Year).ThenByDescending(m=>m.Date.Month).ThenByDescending(d=>d.Date.Day).GroupBy(g=>g.Name).Select(g=>g.FirstOrDefault());
            return View(obj);
        }
        [Authorize]
        public IActionResult Detail(string? Symbol){
            
            IEnumerable<Stock> obj= _db.Stocks!.OrderByDescending(y=>y.Date.Year).ThenByDescending(m=>m.Date.Month).ThenByDescending(d=>d.Date.Day).Where(n=> n.Symbol==Symbol).Take(10);
            savedObj=obj;
            return View(obj);
        }
        
         /*  [Authorize]
        public IActionResult Predict(string? Symbol){
            
            IEnumerable<Stock?> obj= _db.Stocks!.OrderByDescending(y=>y.Date.Year).ThenByDescending(m=>m.Date.Month).ThenByDescending(d=>d.Date.Day).GroupBy(g=>g.Name).Select(g=>g.FirstOrDefault());
            return View(obj);
        }*/

        [Authorize(Roles ="admin,superuser")]
        //[HttpPost]
        
        public async Task<IActionResult> Predict(string Symbol){
            

            var payload= new 
            {
                name=Symbol,
            };


            using (var client = new HttpClient())
            {

                var response=client.PostAsJsonAsync("http://localhost:5000/prediction",payload);
                
                
                var something= await response.Result.Content.ReadFromJsonAsync<Prediction>();
            
                float price=float.Parse(something.prediction,CultureInfo.InvariantCulture.NumberFormat);
            
                IEnumerable<Stock> obj= _db.Stocks!.OrderByDescending(y=>y.Date.Year).ThenByDescending(m=>m.Date.Month).ThenByDescending(d=>d.Date.Day).Where(n=> n.Symbol==Symbol).Take(10);

                int vol=0;

                foreach (var item in obj)
                {
                    vol+=item.Volume;
                }

                vol=(vol/10);

                Stock newObj= new Stock();
                newObj.CloseorLast=price;
                newObj.High=price;
                newObj.Low=price;
                newObj.Date=DateOnly.FromDateTime(DateTime.Now);
                newObj.Open=price;
                newObj.Symbol=Symbol;
                newObj.Name=obj.ElementAt(0).Name;
                newObj.Volume=vol;
                
                await _db.Stocks.AddAsync(newObj);
                await _db.SaveChangesAsync();


                //return Ok(something.prediction);

                return View(obj);
            }
            
        }

        [Authorize]
        public IActionResult Graph(string? Symbol,int nod)
        {
            IEnumerable<Stock> obj= _db.Stocks!.OrderByDescending(y=>y.Date.Year).ThenByDescending(m=>m.Date.Month).ThenByDescending(d=>d.Date.Day).Where(n=> n.Symbol==Symbol).Take(nod);
            
            ViewBag.TotalLength=_db.Stocks!.Where(w=>w.Symbol==Symbol).Count();

            List<DataPoint> dataPoints = new List<DataPoint>();
            
       
            

            foreach(var element in obj)
            {
                var date = element.Date;
                var day = date.Day;
                var month = date.Month;
                var year = date.Year;

                var d = new DateTime(year, month, day);
                var baseDate = new DateTime (1970, 01, 01);
                var numberOfSeconds = (Int64)d.Subtract(baseDate).TotalSeconds*1000; //*1000000

                var value =  (Int32)element.CloseorLast;
                dataPoints.Add(new DataPoint(numberOfSeconds, value));

            }
            ViewBag.Name = obj.ElementAt(0).Name;
            
            var a = JsonConvert.SerializeObject(dataPoints);
            ViewBag.DataPoints = a;
            return View(obj);

        }
    }
}
