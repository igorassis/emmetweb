using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Emmet.Models;

namespace Emmet.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        //LOGIN
        public async Task<ActionResult> DoLogin(string email, string password)
        {
            HttpClient client = new HttpClient();
            string clientId = "3MVG9zlTNB8o8BA3LidfAjKPb3d.6ywqS3nK6GbRN2_VEJIta1PmkYwTIP92J8IVZaQGyF9yiMZsals10zRun";
            string clientSecret = "2008777746554522938";
            string userName = "paesvitor@gw.com";
            string passWord = "sued123suedehcHpyhWSxTsRW8M3kJdForT";

            HttpContent content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    {"grant_type","password"},
                    {"client_id",clientId },
                    {"client_secret",clientSecret},
                    {"username",userName},
                    {"password",passWord}
                }
            );

            HttpResponseMessage message = await client.PostAsync("https://login.salesforce.com/services/oauth2/token", content);

            string responseString = await message.Content.ReadAsStringAsync();

            JObject obj = JObject.Parse(responseString);
            string oauthToken = (string)obj["access_token"];
            string serviceUrl = (string)obj["instance_url"];

            String url = serviceUrl + "/services/data/v43.0/query?q="
               + "SELECT+Id,sexo__c,email__c,idade__c"
               + "+FROM+Usuario__c"
               + "+WHERE+email__c='" + email + "'"
               + "+AND+senha__c='" + password + "'";            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);            request.Headers.Add("Authorization", "Bearer " + oauthToken);            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client.SendAsync(request);            string result = await response.Content.ReadAsStringAsync();            var status = response.StatusCode.ToString();            if (status.Equals("OK"))
            {

                await Events();
                return View("Events");
            }
            else
            {
                ViewBag.LoginError = "Log in Error";
                return View("Login");
            }
        }

        //REGISTER
        public async Task<ActionResult> DoRegister(string email, string name, string pwd, string confirmPwd)
        {

            if(pwd != confirmPwd)
            {
                ViewBag.ErrorMsg = "Password doesnt match!";
                return View("Register");
            }

            HttpClient client = new HttpClient();
            HttpClient queryClient = new HttpClient();
            string clientId = "3MVG9zlTNB8o8BA3LidfAjKPb3d.6ywqS3nK6GbRN2_VEJIta1PmkYwTIP92J8IVZaQGyF9yiMZsals10zRun";
            string clientSecret = "2008777746554522938";
            string userName = "paesvitor@gw.com";
            string passWord = "sued123suedehcHpyhWSxTsRW8M3kJdForT";

            HttpContent content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    {"grant_type","password"},
                    {"client_id",clientId },
                    {"client_secret",clientSecret},
                    {"username",userName},
                    {"password",passWord}
                }
            );

            HttpResponseMessage message = await client.PostAsync("https://login.salesforce.com/services/oauth2/token", content);

            string responseString = await message.Content.ReadAsStringAsync();

            JObject obj = JObject.Parse(responseString);
            string oauthToken = (string)obj["access_token"];
            string serviceUrl = (string)obj["instance_url"];            string registerQuery = serviceUrl + "/services/data/v43.0/sobjects/usuario__c/";            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, registerQuery);            request.Headers.Add("Authorization", "Bearer " + oauthToken);            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));            var newUser = new
            {
                Name = name,
                email__c = email,
                senha__c = pwd
            };

            var body = JsonConvert.SerializeObject(newUser, Formatting.Indented);            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await queryClient.SendAsync(request);            string result = await response.Content.ReadAsStringAsync();

            var status = response.StatusCode.ToString();            if (status.Equals("OK"))
            {
                await Events();
                return View("Events");
            }
            else
            {
                ViewBag.LoginError = "Log in Error";
                return View("Register");
            }
        }

        //EVENTS
        public async Task<ActionResult> Events()
        {
            HttpClient client = new HttpClient();
            string clientId = "3MVG9zlTNB8o8BA3LidfAjKPb3d.6ywqS3nK6GbRN2_VEJIta1PmkYwTIP92J8IVZaQGyF9yiMZsals10zRun";
            string clientSecret = "2008777746554522938";
            string userName = "paesvitor@gw.com";
            string passWord = "sued123suedehcHpyhWSxTsRW8M3kJdForT";

            HttpContent content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    {"grant_type","password"},
                    {"client_id",clientId },
                    {"client_secret",clientSecret},
                    {"username",userName},
                    {"password",passWord}
                }
            );

            HttpResponseMessage message = await client.PostAsync("https://login.salesforce.com/services/oauth2/token", content);

            string responseString = await message.Content.ReadAsStringAsync();

            JObject obj = JObject.Parse(responseString);
            string oauthToken = (string)obj["access_token"];
            string serviceUrl = (string)obj["instance_url"];

            String url = serviceUrl + "/services/data/v43.0/query?q="
                + "SELECT+id,Name,description__c,date__c,price__c,game__r.Name"
                + "+FROM+Evento__c";

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);            request.Headers.Add("Authorization", "Bearer " + oauthToken);            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client.SendAsync(request);
            string result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                //ViewBag.result = JsonConvert.DeserializeObject<EventsMD>(result);
                ViewBag.result = JsonConvert.DeserializeObject(result);
                //ViewBag.result = response.Content.ReadAsAsync<IEnumerable<EventsMD>>().Result;

                //SessionStore ServiceInfo = JsonConvert.DeserializeObject<SessionStore>(responseBody);
            }

            return View("Events");
        }

    }
}