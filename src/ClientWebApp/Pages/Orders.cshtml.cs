using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClientWebApp.Pages
{
    [Authorize(Policy = "Orders")]
    public class Orders : PageModel
    {
        public void OnGet()
        {
            
        }
    }
}