using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PhonePePaymentintegration.Models;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Razor.Tokenizer;

namespace PhonePePaymentintegration.Controllers
{
    public class HomeController : Controller
    {
        string merchantId = System.Configuration.ConfigurationManager.AppSettings["mId"].ToString();
        string saltKey = System.Configuration.ConfigurationManager.AppSettings["salt"].ToString();
        public ActionResult Index()
        {
            return View();
        }

        // POST: /Home/GeneratePaymentLink
        [HttpPost]
        public async Task<JsonResult> GeneratePaymentLink(VerifyRequestModel phonePePayment)
        {
            try
            {
                // ON LIVE URL YOU MAY GET CORS ISSUE, ADD Below LINE TO RESOLVE
                //ServicePointManager.Expect100Continue = true;
                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                //var PhonePeGatewayURL = "https://api-preprod.phonepe.com/apis/pg-sandbox";
                var PhonePeGatewayURL = "https://api.phonepe.com/apis/hermes";

                var httpClient = new HttpClient();
                var uri = new Uri($"{PhonePeGatewayURL}/pg/v1/pay");

                // Add headers
                httpClient.DefaultRequestHeaders.Add("accept", "application/json");
                httpClient.DefaultRequestHeaders.Add("X-VERIFY", phonePePayment.X_VERIFY);

                // Create JSON request body
                var jsonBody = $"{{\"request\":\"{phonePePayment.base64}\"}}";
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                // Send POST request
                var response = await httpClient.PostAsync(uri, content);
                response.EnsureSuccessStatusCode();

                // Read and deserialize the response content
                var responseContent = await response.Content.ReadAsStringAsync();
                //var onlineUrl = JsonConvert.DeserializeObject<string>(responseContent); 
                var details = JObject.Parse(responseContent);
                string url = details["data"]["instrumentResponse"]["redirectInfo"]["url"].ToString();
                // Return a response
                return Json(new { Success = true, Message = "Verification successful", phonepeResponse = responseContent });
            }
            catch (Exception ex)
            {
                // Handle errors and return an error response
                return Json(new { Success = false, Message = "Verification failed", Error = ex.Message });
            }
        }
        [HttpGet]
        public ActionResult CheckPaymentStatus()
        {
            return View();
        }
        // POST: /Home/CheckPaymentStatus
        [HttpPost]
        public async Task<JsonResult> CheckPaymentStatus(VerifyRequestModel phonePePayment)
        {
            try
            {
                // ON LIVE URL YOU MAY GET CORS ISSUE, ADD Below LINE TO RESOLVE
                //ServicePointManager.Expect100Continue = true;
                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                var PhonePeGatewayURL = "https://api.phonepe.com/apis/hermes";

                var httpClient = new HttpClient();
                var uri = new Uri($"{PhonePeGatewayURL}/pg/v1/status/{phonePePayment.MERCHANTID}/{phonePePayment.TransactionId}");

                // Add headers
                httpClient.DefaultRequestHeaders.Add("accept", "application/json");
                httpClient.DefaultRequestHeaders.Add("X-VERIFY", phonePePayment.X_VERIFY);
                httpClient.DefaultRequestHeaders.Add("X-MERCHANT-ID", phonePePayment.MERCHANTID);

                // Create JSON request body

                // Send GET request
                var response = await httpClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();

                // Read and deserialize the response content
                var responseContent = await response.Content.ReadAsStringAsync();

                // Return a response
                return Json(new { Success = true, Message = "Verification successful", phonepeResponse = responseContent });
            }
            catch (Exception ex)
            {
                // Handle errors and return an error response
                return Json(new { Success = false, Message = "Verification failed", Error = ex.Message });
            }
        }
        [HttpGet]
        public ActionResult RequestPayment()
        {
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> RequestPayment(VerifyRequestModel phonePePayment)
        {
            try
            {
                // ON LIVE URL YOU MAY GET CORS ISSUE, ADD Below LINE TO RESOLVE
                //ServicePointManager.Expect100Continue = true;
                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                //var PhonePeGatewayURL = "https://api-preprod.phonepe.com/apis/pg-sandbox";
                var PhonePeGatewayURL = "https://api.phonepe.com/apis/hermes";

                var httpClient = new HttpClient();
                var uri = new Uri($"{PhonePeGatewayURL}/v3/status/{phonePePayment.MERCHANTID}/{phonePePayment.TransactionId}");

                // Add headers
                httpClient.DefaultRequestHeaders.Add("accept", "application/json");
                httpClient.DefaultRequestHeaders.Add("X-VERIFY", phonePePayment.X_VERIFY);
                httpClient.DefaultRequestHeaders.Add("X-MERCHANT-ID", phonePePayment.MERCHANTID);

                // Create JSON request body

                // Send GET request
                var response = await httpClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();

                // Read and deserialize the response content
                var responseContent = await response.Content.ReadAsStringAsync();

                // Return a response
                return Json(new { Success = true, Message = "Verification successful", phonepeResponse = responseContent });
            }
            catch (Exception ex)
            {
                // Handle errors and return an error response
                return Json(new { Success = false, Message = "Verification failed", Error = ex.Message });
            }
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        [HttpGet]
        public async Task<ActionResult> GeneratePaymentLink()
        {
            var baseUrl = "https://mercury-t2.phonepe.com";
            var paymentLink = new PaymentLink()
            {
                merchantId = merchantId,
                transactionId = Guid.NewGuid().ToString(),
                merchantOrderId = Guid.NewGuid().ToString(),
                amount = 100,
                mobileNumber = "8296412345",
                message = "paylink for 1 order",
                expiresIn = 3600,
                storeId = $"store{50}",
                terminalId = "terminal10",
                shortName = "Test short Name",
                subMerchantId = "test"
            };
            //var Modelpayload = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(paymentLink)));
            //var Payload = $"{Modelpayload}/v3/payLink/init{saltKey}";
            //var x_VERIFY = ComputeSha256Hash(Payload, "1");
            var requestJSON = JsonConvert.SerializeObject(paymentLink);
            var PayloadString = Convert.ToBase64String(Encoding.UTF8.GetBytes(requestJSON));
            var payload = $"{PayloadString}/v3/payLink/init{saltKey}";
            var x_verify = ComputeSha256Hash(payload, "1");

            var httpClient = new HttpClient();
            var uri = new Uri($"{baseUrl}/v3/payLink/init");

            // Add headers
            httpClient.DefaultRequestHeaders.Add("accept", "application/json");
            //httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");
            httpClient.DefaultRequestHeaders.Add("X-VERIFY", x_verify);
            httpClient.DefaultRequestHeaders.Add("X-PROVIDER-ID", merchantId);

            // Create JSON request body
            var jsonBody = $"{{\"request\":\"{PayloadString}\"}}";
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            // Send POST request
            var response = await httpClient.PostAsync(uri, content);
            response.EnsureSuccessStatusCode();

            // Read and deserialize the response content
            var responseContent = await response.Content.ReadAsStringAsync();
            return View();
        }
        public string ComputeSha256Hash(string rawData, string saltindex)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString() + "###" + saltindex;
            }
        }
    }

    public class VerifyRequestModel
    {
        public string X_VERIFY { get; set; }
        public string base64 { get; set; }
        public string TransactionId { get; set; }
        public string MERCHANTID { get; set; }
        // Add other properties from the request if needed
    }
}