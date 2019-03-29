using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SitecoreLogicConnector.Feature.SLC.AzureFunctions.Authentication
{
    public static class SitecoreAuth
    {
        public static async Task<Cookie> Login(string username, string password)
        {
            var cookies = new CookieContainer();
            var handler = new HttpClientHandler { CookieContainer = cookies };
            var client = new HttpClient(handler);
            var loginRoute = Environment.GetEnvironmentVariable("SITECORE_BASE_URL", EnvironmentVariableTarget.Process) + "/api/ssc/auth/login";

            var credentials = new SitecoreCredentials
            {
                Username = username,
                Password = password,
                Domain = Environment.GetEnvironmentVariable("SITECORE_LOGIN_DOMAIN",
                    EnvironmentVariableTarget.Process)
            };

            var jsonString = JsonConvert.SerializeObject(credentials);
            var json = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(loginRoute, json);

            var responseCookies = cookies.GetCookies(new Uri(loginRoute)).Cast<Cookie>();

            return responseCookies.FirstOrDefault();
        }
    }

    internal class SitecoreCredentials
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Domain { get; set; }
    }
}
