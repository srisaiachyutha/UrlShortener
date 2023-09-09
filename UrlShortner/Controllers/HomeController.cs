using Microsoft.AspNetCore.Mvc;
using UrlShortner.Clients.Interfaces;
using UrlShortner.Models;

namespace UrlShortner.Controllers
{
    public class HomeController: Controller
    {
        private readonly IUrlClient _urlClient;
        private readonly ILogger<HomeController> _logger;
        
        public HomeController(
            ILogger<HomeController> logger,
            IUrlClient urlClient)
        {
            _logger = logger;
            _urlClient = urlClient;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(
           string url)
        {
            string code = string.Empty;
            // validating the request
            if(string.IsNullOrEmpty(url))
            {
                _logger.LogDebug("Bad request url is empty to save");
                return BadRequest("Url is empty");
            }

            try
            {
                code = _urlClient.CreateSmallUrl(url);
                _logger.LogInformation("Successfully create the small url");
            }
            catch(Exception) 
            {
                _logger.LogError("An exception occured while create a new url");
            }
            
            
            string host = Request.HttpContext.Request.Host.ToString();
            
            if (string.IsNullOrEmpty(code))
                return NotFound();
            else
                return View("Response", new EncodeUrlResponse {
                    OriginalUrl = url,
                    EncodedCode = $"https://{host}/{code}"
                });
        }

        public IActionResult RedirectUrl(string code)
        {
            
            string url = String.Empty;
            try
            {
                url = _urlClient.GetOriginalUrl(code);
                if (url == string.Empty)
                {
                    return NotFound();
                }

                
            }
            catch (Exception ex)
            {
                _logger.LogError("An exception occured while fetching original url");
            }

            return Redirect(url);
        }
    }
}
