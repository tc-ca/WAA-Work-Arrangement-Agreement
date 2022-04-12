using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Resources;
using Microsoft.Extensions.Options;

namespace Web.api
{
    [Route("api/[controller]")]
    public class AddressController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly CanadaPostApiSetting _addressApiOption;
        public AddressController(IHttpClientFactory clientFactory, IOptions<CanadaPostApiSetting> options)
        {
            _clientFactory = clientFactory;
            _addressApiOption = options.Value;
        }
       
        [HttpPost("lookup")]
        public async Task<ActionResult> Lookup(string addr)
        {
            string urlStr = "{0}?Key={1}&SearchTerm={2}&LastId=&Country=CAN&LanguagePreference=EN&MaxSuggestions=7";
            string query = string.Format(urlStr, _addressApiOption.FindAPIUrl, _addressApiOption.APIKey,addr);
            string resp = await CanadaPostAPI(query);

            var suggestions =  JsonConvert.DeserializeObject<List<AddrSuggestion>>(resp);
            var result = suggestions.Where(x=>x.Next!="Find")
                .Select(o => new { label = o.Text + " " + o.Description, value = o.Id });
            return new JsonResult(result);
        }
        [HttpPost("retrieve")]
        public async Task<ActionResult> Retrieve(string addrId)
        {
            string urlStr = "{0}?Key={1}&Id={2}";
            string apiUrl = string.Format(urlStr, _addressApiOption.RetrieveAPIUrl, _addressApiOption.APIKey, addrId);
            string resp = await CanadaPostAPI(apiUrl);
            var addrs = JsonConvert.DeserializeObject<List<MailingAddr>>(resp);
            if (addrs != null && addrs.Count > 0)
                return new JsonResult(addrs[0]);
            else 
                return NotFound();
        }
        private async Task<string> CanadaPostAPI(string url)
        {
            var _httpClient = _clientFactory.CreateClient();
            _httpClient.DefaultRequestHeaders.Add("Referer", _addressApiOption.ReferrerUrl);

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            var responseMsg = await _httpClient.SendAsync(request).ConfigureAwait(false);
            responseMsg.EnsureSuccessStatusCode();
            string resp = await responseMsg.Content.ReadAsStringAsync();

            return resp;
        }
    }
    public class AddrSuggestion {
        public string Id { get; set; }
        public string Text { get; set; }
        public string Description { get; set; }
        public string Next { get; set; }
    }
    public class MailingAddr
    {
        public string Language { get; set; } //ENG/FRE
        public string Street { get; set; }
        public string City { get; set; }
        public string Line1 { get; set; }
        public string ProvinceCode { get; set; }
        public string PostalCode { get; set; }
    }
    public class CanadaPostApiSetting
    {
        public string ReferrerUrl { get; set; }
        public string FindAPIUrl { get; set; }
        public string RetrieveAPIUrl { get; set; }
        public string APIKey { get; set; }
    }
}
