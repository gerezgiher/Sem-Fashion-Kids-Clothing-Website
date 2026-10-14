//using Microsoft.AspNetCore.Mvc;
//using System.Text.Json;

//namespace Sem_Fashion_Kids_Clothing_Website.Controllers
//{
//    [ApiController]
//    [Route("api/payment")]
//    public class PaymentController : ControllerBase
//    {
//        private readonly ApplicationDbContext _context;

//        public PaymentController(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        // =========================
//        // TELEBIRR WEBHOOK (SAFE)
//        // =========================
//        [HttpPost("webhook")]
//        public IActionResult TelebirrWebhook([FromBody] JsonElement data)
//        {
//            var orderId = data.GetProperty("merchantOrderNo").GetString();
//            var status = data.GetProperty("status").GetString();
//            var transactionId = data.GetProperty("transactionId").GetString();

//            if (!int.TryParse(orderId, out int id))
//                return BadRequest();

//            var order = _context.Orders.FirstOrDefault(o => o.Id == id);
//            if (order == null) return NotFound();

//            if (status == "SUCCESS")
//            {
//                order.IsPaid = true;
//                order.Status = "Approved";
//                order.TelebirrTransactionId = transactionId;
//            }
//            else
//            {
//                order.Status = "Failed";
//            }

//            _context.SaveChanges();
//            return Ok();
//        }
//    }
//}