using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using OfficeOpenXml;
using WH.DataContext;
using WH.Models;
 
using ApplicationDbContext = WH.DataContext.ApplicationDbContext;

namespace WH.Controllers
{
    public class BCPhieuNhapAPIController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
         
      
    }
}
