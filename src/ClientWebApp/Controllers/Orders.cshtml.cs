using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClientWebApp.Controllers
{
    [Authorize(Policy = "Orders")]
    public class Orders : PageModel
    {
        public void OnGet()
        {
            
        }
    }
}