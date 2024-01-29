using Microsoft.AspNetCore.Mvc.Rendering;
using SMS_DataAccess.Data;
using SMS_DataAccess.Repository.IRepository;
using SMS_Models;
using SMS_Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DataAccess.Repository
{
    public class InquiryHeaderRepository : Repository<InquiryHeader>, IInquiryHeaderRepository
    {
        private readonly AppDbContext _database;

        public InquiryHeaderRepository(AppDbContext database) : base(database) 
        {
            _database = database;
        }

        public void Update(InquiryHeader inquiryHeader)
        {
            _database.InquiryHeaders.Update(inquiryHeader);
        }
    }
}
