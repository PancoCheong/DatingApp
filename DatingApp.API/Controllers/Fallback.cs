using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    public class Fallback : Controller //with View support
    {
        public IActionResult Index()
        {
            return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(),
                "wwwroot", "index.html"), "text/HTML");
        }
    }
}
