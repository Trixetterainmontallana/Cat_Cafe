using Microsoft.AspNetCore.Mvc;
using Catpuccino_FinalProject.Data;
using Catpuccino_FinalProject.Models;
using System.Linq;
using System.Text.Json;

namespace Catpuccino_FinalProject.Controllers
{
    public class MenuController : Controller
    {
        private readonly AppDbContext _context;

        public MenuController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Menus()
        {
            var products = _context.Products.ToList();
            return View(products);
        }


       
        [HttpPost]
        public IActionResult PlaceOrder([FromBody] List<OrderItemModel> items)
        {
            // 1. Null / empty cart guard
            if (items == null || items.Count == 0)
            {
                return BadRequest(new { success = false, message = "Your cart is empty." });
            }

            // 2. Run DataAnnotations validation on every item
            var itemErrors = new List<string>();
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
                var ctx = new System.ComponentModel.DataAnnotations.ValidationContext(item);

                if (!System.ComponentModel.DataAnnotations.Validator.TryValidateObject(item, ctx, validationResults, true))
                {
                    foreach (var vr in validationResults)
                        itemErrors.Add($"Item {i + 1} ({item.Name}): {vr.ErrorMessage}");
                }
            }

            if (itemErrors.Count > 0)
            {
                return BadRequest(new { success = false, message = string.Join(" | ", itemErrors) });
            }

            var firstItem = items[0];
            var paymentMethod = firstItem.PaymentMethod;
            var details = firstItem.PaymentDetails ?? new Dictionary<string, string>();

            var paymentError = ValidatePaymentDetails(paymentMethod, details);
            if (paymentError != null)
            {
                return BadRequest(new { success = false, message = paymentError });
            }

            return Ok(new
            {
                success = true,
                redirectUrl = Url.Action("OrderSuccess", "Order")
            });
        }


       
        private static string? ValidatePaymentDetails(string method, Dictionary<string, string> d)
        {
            string Get(string key) =>
                d.TryGetValue(key, out var v) ? (v ?? "").Trim() : "";

            switch (method)
            {
                case "Maya":
                case "GCash":
                    {
                        var name = Get("accountName");
                        var mobile = Get("mobileNumber");
                        var refNo = Get("referenceNumber");

                        if (string.IsNullOrWhiteSpace(name))
                            return $"Account name is required for {method}.";

                        if (string.IsNullOrWhiteSpace(mobile))
                            return $"Mobile number is required for {method}.";

                        if (!System.Text.RegularExpressions.Regex.IsMatch(mobile, @"^09\d{9}$"))
                            return $"Please enter a valid {method} mobile number (e.g. 09XXXXXXXXX).";

                        if (string.IsNullOrWhiteSpace(refNo))
                            return $"Reference number is required for {method}.";

                        if (!System.Text.RegularExpressions.Regex.IsMatch(refNo, @"^\d{6,20}$"))
                            return $"{method} reference number must be 6–20 digits.";

                        break;
                    }

                case "Cash":
                    {
                        var name = Get("customerName");
                        if (string.IsNullOrWhiteSpace(name))
                            return "Your name is required for Cash payment.";
                        break;
                    }

                case "Card":
                    {
                        var cardHolder = Get("cardholderName");
                        var cardType = Get("cardType");
                        var lastFour = Get("lastFour");
                        var expiry = Get("expiry");

                        if (string.IsNullOrWhiteSpace(cardHolder))
                            return "Cardholder name is required.";

                        if (!new[] { "Mastercard", "Visa", "PayPal" }.Contains(cardType))
                            return "Please select a valid card type (Mastercard, Visa, or PayPal).";

                        if (string.IsNullOrWhiteSpace(lastFour) || !System.Text.RegularExpressions.Regex.IsMatch(lastFour, @"^\d{4}$"))
                            return "Card number is invalid.";

                        if (string.IsNullOrWhiteSpace(expiry) || !System.Text.RegularExpressions.Regex.IsMatch(expiry, @"^(0[1-9]|1[0-2])\/\d{2}$"))
                            return "Please enter expiry in MM/YY format.";

                        break;
                    }

                default:
                    return "Invalid payment method selected.";
            }

            return null; // no errors
        }
    }
}