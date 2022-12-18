using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace dotnet.Views.Stocks
{
    public class Predict : PageModel
    {
        private readonly ILogger<Predict> _logger;

        public Predict(ILogger<Predict> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}