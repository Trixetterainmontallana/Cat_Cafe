using System.ComponentModel.DataAnnotations;

namespace Catpuccino_FinalProject.Models
{
    // ─── Base class shared by all payment methods ───────────────────────────────
    public abstract class PaymentDetailsBase
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
        [Display(Name = "Full Name")]
        public string Name { get; set; } = string.Empty;
    }


    // ─── Maya Payment ────────────────────────────────────────────────────────────
    public class MayaPaymentModel : PaymentDetailsBase
    {
        [Required(ErrorMessage = "Maya account name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Account name must be between 2 and 100 characters.")]
        [Display(Name = "Account Name")]
        public new string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Maya mobile number is required.")]
        [RegularExpression(@"^09\d{9}$", ErrorMessage = "Please enter a valid mobile number (e.g. 09XXXXXXXXX).")]
        [Display(Name = "Maya Mobile Number")]
        public string MobileNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Reference number is required.")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Reference number must be between 6 and 20 characters.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Reference number must contain digits only.")]
        [Display(Name = "Reference Number")]
        public string ReferenceNumber { get; set; } = string.Empty;
    }


    // ─── GCash Payment ───────────────────────────────────────────────────────────
    public class GCashPaymentModel : PaymentDetailsBase
    {
        [Required(ErrorMessage = "GCash account name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Account name must be between 2 and 100 characters.")]
        [Display(Name = "Account Name")]
        public new string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "GCash mobile number is required.")]
        [RegularExpression(@"^09\d{9}$", ErrorMessage = "Please enter a valid mobile number (e.g. 09XXXXXXXXX).")]
        [Display(Name = "GCash Mobile Number")]
        public string MobileNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Reference number is required.")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Reference number must be between 6 and 20 characters.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Reference number must contain digits only.")]
        [Display(Name = "Reference Number")]
        public string ReferenceNumber { get; set; } = string.Empty;
    }


    // ─── Cash Payment ────────────────────────────────────────────────────────────
    public class CashPaymentModel : PaymentDetailsBase
    {
        [Required(ErrorMessage = "Your name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
        [Display(Name = "Your Name")]
        public new string Name { get; set; } = string.Empty;
    }


    // ─── Card Payment ────────────────────────────────────────────────────────────
    public class CardPaymentModel
    {
        [Required(ErrorMessage = "Card type is required.")]
        [RegularExpression(@"^(Mastercard|Visa|PayPal)$", ErrorMessage = "Please select a valid card type.")]
        [Display(Name = "Card Type")]
        public string CardType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Cardholder name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Cardholder name must be between 2 and 100 characters.")]
        [Display(Name = "Cardholder Name")]
        public string CardholderName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Card number is required.")]
        [RegularExpression(@"^\d{16}$", ErrorMessage = "Card number must be exactly 16 digits.")]
        [Display(Name = "Card Number")]
        public string CardNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Expiration date is required.")]
        [RegularExpression(@"^(0[1-9]|1[0-2])\/\d{2}$", ErrorMessage = "Please enter expiry in MM/YY format.")]
        [Display(Name = "Expiration Date")]
        public string ExpiryDate { get; set; } = string.Empty;

        [Required(ErrorMessage = "CVC is required.")]
        [RegularExpression(@"^\d{3,4}$", ErrorMessage = "CVC must be 3 or 4 digits.")]
        [Display(Name = "CVC")]
        public string CVC { get; set; } = string.Empty;
    }


    // ─── Order Item (cart line) ───────────────────────────────────────────────────
    public class OrderItemModel
    {
        [Required(ErrorMessage = "Product ID is required.")]
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "Product name is required.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Size is required.")]
        [RegularExpression(@"^(Small|Medium|Large)$", ErrorMessage = "Size must be Small, Medium, or Large.")]
        public string Size { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public decimal Price { get; set; }

        [Range(1, 100, ErrorMessage = "Quantity must be between 1 and 100.")]
        public int Qty { get; set; }

        // ── Payment fields (flattened from JS payload) ──
        [Required(ErrorMessage = "Payment method is required.")]
        [RegularExpression(@"^(Maya|GCash|Cash|Card)$", ErrorMessage = "Invalid payment method.")]
        public string PaymentMethod { get; set; } = string.Empty;

        // Generic dictionary so any payment details can be deserialised without
        // a separate endpoint per method. Server-side fine-grained validation is
        // handled inside the controller using the typed models above.
        public Dictionary<string, string>? PaymentDetails { get; set; }
    }


    // ─── Full order payload (list of items) ──────────────────────────────────────
    public class PlaceOrderPayload
    {
        [Required(ErrorMessage = "Order must contain at least one item.")]
        [MinLength(1, ErrorMessage = "Order must contain at least one item.")]
        public List<OrderItemModel> Items { get; set; } = new();
    }
}