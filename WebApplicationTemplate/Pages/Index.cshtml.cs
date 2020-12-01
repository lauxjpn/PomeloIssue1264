using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace WebApplicationTemplate.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly Context _context;
        
        public IReadOnlyList<IceCream> IceCreams { get; private set; }
        public IReadOnlyList<Cookie> Cookies { get; private set; }

        public IndexModel(ILogger<IndexModel> logger, Context context)
        {
            _logger = logger;
            _context = context;
        }

        public void OnGet()
        {
            IceCreams = _context.IceCreams.ToList().AsReadOnly();
            Cookies = _context.Cookies.ToList().AsReadOnly();
        }
    }
}