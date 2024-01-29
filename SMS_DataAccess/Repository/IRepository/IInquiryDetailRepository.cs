using Microsoft.AspNetCore.Mvc.Rendering;
using SMS_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DataAccess.Repository.IRepository
{
    public interface IInquiryDetailRepository : IRepository<InquiryDetail>
    {
        void Update(InquiryDetail inquiryDetail);
    }
}
