using Microsoft.AspNetCore.Identity;
using SMS_DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DataAccess.Initializer
{
    public interface IDBInitializer
    {
        void Initialize();
    }
}
