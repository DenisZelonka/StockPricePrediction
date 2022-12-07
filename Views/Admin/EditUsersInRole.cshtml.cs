using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace dotnet.Views.Admin
{
    public class EditUsersInRole : PageModel
    {
        private readonly ILogger<EditUsersInRole> _logger;

        public EditUsersInRole(ILogger<EditUsersInRole> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}