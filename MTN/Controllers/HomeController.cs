using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using MTN.Common;
using MTN.Common.Models;
using MTN.Models;
using MTN.Util;
using Newtonsoft.Json;

namespace MTN.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        private DbEntities db = new DbEntities();
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult ListDanhmucDiadanh()
        {
            try
            {
                var dm = from a in db.TD_Danhmuc
                         select new
                         {
                             a.Danhmuc_ID,
                             a.TenDanhmuc,
                             dd = db.TD_Diadanh.Where(x => x.Danhmuc_ID == a.Danhmuc_ID && x.Diadanhcha_ID == null).Select(x => new
                             {
                                 x.Tendiadanh,
                                 x.Diadanh_ID
                             })
                         };
                return Json(new { data = dm, error = 0 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { error = 1 }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult UpdateBaoCao(List<NV_DulieuQuantrac> arr)
        {
            try
            {
                foreach(var qt in arr)
                {
                    var gt = db.NV_DulieuQuantrac.Where(x => x.BaocaoDiadanh_ID == qt.BaocaoDiadanh_ID && x.BaocaoThuoctinh_ID == qt.BaocaoThuoctinh_ID).FirstOrDefault();
                    if(gt != null)
                    {
                        gt.Giatri = qt.Giatri;
                    }
                }
                db.SaveChanges();
                return Json(new { error = 0 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { error = 1 }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult DelBaoCao(string id)
        {
            try
            {
                var bc = db.NV_MauBaocao.Find(id);
                if(bc != null)
                {
                    db.NV_MauBaocao.Remove(bc);
                }
                db.SaveChanges();
                return Json(new { error = 0 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { error = 1 }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult GetBaoCao(string diadanhid, DateTime fromDate, DateTime toDate, bool isQuantrac)
        {
            try
            {
                var bc = (from a in db.TD_Thuoctinh
                          join b in db.NV_MaubaocaoThuoctinh
                          on a.Thuoctinh_ID equals b.Thuoctinh_ID
                          select new
                          {
                              a.Thuoctinh_ID,
                              a.Tenthuoctinh,
                              a.Donvitinh,
                              a.STT
                          }).Distinct().OrderBy(n => n.STT).ToList();
                var lstData = db.GetGiaTri(diadanhid, fromDate, toDate, isQuantrac);
                var result = Json(new { header = bc, body = lstData, diadanhid, fromDate, toDate, error = 0 }, JsonRequestBehavior.AllowGet);
                result.MaxJsonLength = int.MaxValue;
                return result;
        }
            catch (Exception e)
            {
                return Json(new { error = 1 }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult GetBaoCaoExcel(string diadanhid, DateTime fromDate, DateTime toDate, bool isQuantrac)
        {
            try
            {
                var lstData = db.GetGiaTri(diadanhid, fromDate, toDate, isQuantrac);
                var result = Json(new { body = lstData }, JsonRequestBehavior.AllowGet);
                result.MaxJsonLength = int.MaxValue;
                return result;
            }
            catch (Exception e)
            {
                return Json(new { error = 1 }, JsonRequestBehavior.AllowGet);
            }
        }
        #region Import Excel     
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
        //#region ExportExcel
        //[HttpPost]
        //public JsonResult ExportData(string diadanhid, DateTime fromDate, DateTime toDate, bool isQuantrac)
        //{

        //    lấy dữ liệu xử lý
        //    using (var db = new Models.DbEntities())
        //    {
        //        string fileName;
        //        var list = GetBaoCaoExcel(diadanhid, fromDate, toDate, isQuantrac);
        //        string json = new JavaScriptSerializer().Serialize(list);
        //        Dictionary<string, string[,]> dic = JsonConvert.DeserializeObject<Dictionary<string, string[,]>>(json);
        //        string excelFile = Excel.getCopyExcelTemplateFile(out fileName);

        //        var excelBytes = Excel.WriteExcel(dic, excelFile);
        //        if (excelBytes != null)
        //        {
        //            string fullPath = Path.Combine(Server.MapPath("~/TemplateFile/temp"), fileName);
        //            using (var exportData = new MemoryStream())
        //            {
        //                exportData.Write(excelBytes, 0, excelBytes.Length);
        //                exportData.Position = 0;

        //                FileStream file = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
        //                exportData.WriteTo(file);
        //                file.Close();
        //            }
        //            return Json(new { messenger = "success", fileName, status = true });
        //        }
        //        else
        //            return Json(new { messenger = dic.Count() > 0 ? "success" : "not_data", status = false });
        //    }
        //}
        //#endregion
    }
}