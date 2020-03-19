using MTN.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MTN.Areas.Admin.Controllers
{
    public class ReportsController : Controller
    {
        // GET: Admin/Reports
        public ActionResult Index()
        {
            return View();
        }
        #region Import
        [HttpPost]
        public JsonResult ImportExcel(HttpPostedFileBase file, string diadanhid, string bc)
        {
            try
            {
                string filePath = string.Empty;
                if (file != null)
                {
                    string path = Server.MapPath("~/Uploads/");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    filePath = path + Path.GetFileName(file.FileName);

                    //delete the file exits
                    if (!System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                    string extension = Path.GetExtension(file.FileName);
                    file.SaveAs(filePath);

                    if (!filePath.ReadAndWriteDataToExcel(diadanhid, "true".Equals(bc)))
                        return Json(new { status = false });
                }
                else
                    return Json(new { status = false, messenger = "chưa chọn file" });
                return Json(new { status = true });
            }
            catch (Exception ex)
            {
                return Json(new { status = false });
            }
        }
        #endregion
    }
}