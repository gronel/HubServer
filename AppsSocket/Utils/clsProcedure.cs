using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace AppsSocket.Utils
{
    public class clsProcedure
    {
        public void writeLog(string strLog)
        {
            try
            {
                string menupath = HttpContext.Current.Request.PhysicalApplicationPath + "ErrorLog//error.log";



            }
            catch (Exception ex)
            {
                //
            }
        }
    }
}