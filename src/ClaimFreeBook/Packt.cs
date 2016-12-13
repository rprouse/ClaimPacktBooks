using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ClaimFreeBook
{
    public class Packt
    {
        const string Domain = "https://www.packtpub.com";
        HttpClient _client;
        CookieContainer _cookies;
        HttpClientHandler _handler;

        public async Task ClaimFreeBook(string email, string password)
        {
            Init();
            if (!await Login(email, password))
                return;

            var token = await FindClaimToken();
            await MakeClaim(token);
        }

        void Init()
        {
            _cookies = new CookieContainer();
            _handler = new HttpClientHandler();
            _handler.CookieContainer = _cookies;
            _client = new HttpClient(_handler);
        }

        async Task<bool> Login(string email, string password)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Logging in ");

            string html = await _client.GetStringAsync(Domain + "/packt/offers/free-learning");

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var token = doc.DocumentNode
                           .Descendants("input")
                           .Where(i => i.Attributes.Contains("name") && i.Attributes["name"].Value == "form_build_id")
                           .Select(i => i.Attributes["value"].Value)
                           .FirstOrDefault();

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("email", email),
                new KeyValuePair<string, string>("password", password),
                new KeyValuePair<string, string>("op", "Login"),
                new KeyValuePair<string, string>("form_id", "packt_user_login_form"),
                new KeyValuePair<string, string>("form_build_id", token)
            });

            var response = await _client.PostAsync(Domain, content);
            if (response.StatusCode == HttpStatusCode.Found)
            {
                var location = response.Headers.Where(h => h.Key == "Location").Select(h => h.Value).FirstOrDefault()?.FirstOrDefault();
                if (location != null)
                {
                    response = await _client.GetAsync(location);
                }
            }
            return ReportResults(response);
        }

        async Task<string> FindClaimToken()
        {
            string html = await _client.GetStringAsync(Domain + "/packt/offers/free-learning");

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var title = doc.DocumentNode
                           .Descendants("div")
                           .Where(d => d.Attributes.Contains("class") && d.Attributes["class"].Value == "dotd-title")
                           .Select(d => d.Descendants("h2")?.FirstOrDefault()?.InnerText.Trim())
                           .FirstOrDefault();

            var claim = doc.DocumentNode
                           .Descendants("a")
                           .Where(a => a.Attributes.Contains("class") && a.Attributes["class"].Value == "twelve-days-claim")
                           .Select(a => a.Attributes["href"].Value)
                           .FirstOrDefault();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Claiming: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(title + " ");
            return claim;
        }

        async Task MakeClaim(string token)
        {
            var response = await _client.GetAsync(Domain + token);
            ReportResults(response);
        }

        private static bool ReportResults(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[Success]");
                return true;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[Failed]");
                return false;
            }
        }
    }
}
