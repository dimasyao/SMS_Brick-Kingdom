using Braintree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_Utility.BrainTreePayment.Interface
{
    public interface IBrainTreeGate
    {
        public IBraintreeGateway CreateGateway();

        public IBraintreeGateway GetGateway();
    }
}
