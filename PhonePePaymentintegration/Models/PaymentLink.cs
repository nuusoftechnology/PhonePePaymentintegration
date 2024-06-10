using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PhonePePaymentintegration.Models
{
    public class PaymentLink
    {
        public string merchantId { get; set; }
        public string transactionId { get; set; }
        public string merchantOrderId { get; set; }
        public decimal amount { get; set; }
        public string mobileNumber { get; set; }
        public string message { get; set; }
        public int expiresIn { get; set; }
        public string storeId { get; set; }
        public string terminalId { get; set; }
        public string shortName { get; set; }
        public string subMerchantId { get; set; }
        //public PaymentInstrument paymentInstrument { get; set; }
    }
    public class PaymentInstrument
    {
        public string type { get; set; }
        public string vpa { get; set; }
    }
}