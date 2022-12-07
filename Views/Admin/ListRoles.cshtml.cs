using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace dotnet.Views.Admin
{
    public class ListRoles : PageModel
    {
        private readonly ILogger<ListRoles> _logger;

        public ListRoles(ILogger<ListRoles> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}