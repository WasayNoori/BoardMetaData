using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BoardDataView.Pages
{
    public class PureDataViewModel : PageModel
    {
        public void OnGet()
        {
            // This is a pure frontend page, so we don't need any server-side logic here
            // The page will load and the JavaScript will handle everything via API calls
        }
    }
}
