using Braintree;
using Microsoft.Extensions.Options;
using SMS_Utility.BrainTreePayment.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_Utility.BrainTreePayment
{
    public class BrainTreeGate : IBrainTreeGate
    {
        private BrainTreeSettings _settings { get; set; }

        private IBraintreeGateway _braintreeGateway { get; set; }

        public BrainTreeGate(IOptions<BrainTreeSettings> options) 
        {
            _settings = options.Value;
        }

        public IBraintreeGateway CreateGateway()
        {
            _braintreeGateway = new BraintreeGateway(_settings.Environment, _settings.MerchantID, _settings.PublicKey, _settings.PrivateKey);
            return _braintreeGateway;
        }

        public IBraintreeGateway GetGateway()
        {
            if (_braintreeGateway == null)
            {
                CreateGateway();
            }

            return _braintreeGateway;
        }
    }
}
