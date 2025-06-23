using BoardDataView.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BoardDataView.Pages
{
    public class SafeViewModel : PageModel
    {
        public string? PublicIpAddress { get; set; }

        private readonly SPAutomationDbContext _context;

        public SafeViewModel(SPAutomationDbContext context)
        {
            _context = context;
        }
        public async Task OnGetAsync()
        {
            var remoteIp = HttpContext.Connection.RemoteIpAddress;
            //if (remoteIp != null && (remoteIp.ToString() == "127.0.0.1" || remoteIp.ToString() == "::1"))
            //{
            //    PublicIpAddress = "LOCAL";
            //}
            //else
            //{
                using var httpClient = new HttpClient();
                try
                {
                    var test = await _context.Users.FirstOrDefaultAsync();
                    //PublicIpAddress = await httpClient.GetStringAsync("https://api.ipify.org");
                    PublicIpAddress = "DB OK: " + (test?.FullName ?? "No data");

                }
                catch(Exception ex )
                {
                    PublicIpAddress =$"Error in either getting the IP or SQL {ex.Message}";
                }
            
        }




       
    }
}
