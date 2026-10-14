//using System.Net.Http.Json;
//using Sem_Fashion_Kids_Clothing_Website.Models;

//namespace Sem_Fashion_Kids_Clothing_Website.Services
//{
//    public class TelebirrService
//    {
//        private readonly HttpClient _http;
//        private readonly IConfiguration _config;

//        public TelebirrService(HttpClient http, IConfiguration config)
//        {
//            _http = http;
//            _config = config;
//        }

//        public async Task<string> CreatePaymentAsync(Order order)
//        {
//            try
//            {
//                // =========================
//                // CONFIG VALUES (FROM appsettings.json)
//                // =========================
//                var baseUrl = _config["Telebirr:BaseUrl"]; // sandbox or production
//                var notifyUrl = _config["Telebirr:NotifyUrl"];
//                var returnUrl = _config["Telebirr:ReturnUrl"];

//                // =========================
//                // REQUEST PAYLOAD
//                // =========================
//                var request = new
//                {
//                    amount = order.TotalAmount,
//                    merchantOrderNo = order.Id.ToString(),
//                    notifyUrl = notifyUrl,
//                    returnUrl = returnUrl
//                };

//                // =========================
//                // CALL TELEBIRR API
//                // =========================
//                var response = await _http.PostAsJsonAsync(
//                    $"{baseUrl}/pay",
//                    request
//                );

//                // =========================
//                // SAFE FAILOVER
//                // =========================
//                if (!response.IsSuccessStatusCode)
//                {
//                    // fallback instead of crash
//                    return $"/Order/Success?id={order.Id}";
//                }

//                var result = await response.Content.ReadFromJsonAsync<TelebirrResponse>();

//                return result?.paymentUrl ?? $"/Order/Success?id={order.Id}";
//            }
//            catch
//            {
//                // NEVER crash checkout
//                return $"/Order/Success?id={order.Id}";
//            }
//        }

//        // =========================
//        // RESPONSE MODEL
//        // =========================
//        private class TelebirrResponse
//        {
//            public string paymentUrl { get; set; }
//        }
//    }
//}