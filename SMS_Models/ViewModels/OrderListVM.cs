using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_Models.ViewModels
{
    public class OrderListVM
    {
        public IEnumerable<OrderHeader> OrderHeaders { get; set; }
        public IEnumerable<SelectListItem> StatusItems { get; set; }
        public string Status { get; set; }
    }
}
