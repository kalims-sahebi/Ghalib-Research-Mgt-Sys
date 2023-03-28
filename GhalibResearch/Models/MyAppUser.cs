using Microsoft.AspNetCore.Identity;


namespace GhalibResearch.Data
{
    public class MyAppUser: IdentityUser
    {
        public string FullName { get; set; }
    }
}
