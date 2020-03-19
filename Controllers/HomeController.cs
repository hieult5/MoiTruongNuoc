using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MTN.Common;
using MTN.Common.Models;
using MTN.Models;
using MTN.Util;

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
        

        public JsonResult GetBaoCao(string fromDate, string toDate, string isDuBao, string diadanhid)
        {
            try
            {
                var bc = from a in db.NV_MauBaocao
                         where a.Diadanh_ID == diadanhid && a.Trangthai == true
                         select new
                         {
                             a.MauBC_ID,
                             listbcdd = db.NV_MaubaocaoDiadanh.Where(n => n.MauBC_ID == a.MauBC_ID).OrderBy(x => x.STT).Select(n => new {
                                n.Diadanh_ID,
                                listdd = db.TD_Diadanh.Where(m => m.Diadanh_ID == n.Diadanh_ID).Select(m => new
                                {
                                    m.Tendiadanh
                                }),
                                n.BaocaoDiadanh_ID
                             }),
                             listbctt = db.NV_MaubaocaoThuoctinh.Where(n => n.MauBC_ID == a.MauBC_ID).OrderBy(x => x.STT).Select(n => new {
                                n.Thuoctinh_ID,
                                listtt = db.TD_Thuoctinh.Where(m => m.Thuoctinh_ID == n.Thuoctinh_ID).Select(m => new 
                                {
                                    m.Tenthuoctinh,
                                    m.Donvitinh,
                                }),
                                n.BaocaoThuoctinh_ID
                             }),

                         };
                Dictionary<string, string[,]> list = new Dictionary<string, string[,]>();
                list = db.GetDataSearch(fromDate, toDate, isDuBao, diadanhid);
                        
                return Json(new { header = bc, body = list, error = 0 }, JsonRequestBehavior.AllowGet);
        }
            catch (Exception e)
            {
                return Json(new { error = 1 }, JsonRequestBehavior.AllowGet);
            }
        }

        #region Get Data Search DateTime
        [HttpPost]
        public JsonResult GetBodyBaoCao(string fromDate, string toDate, string bc, string diadanhid)
        {
            try
            {
                Dictionary<string, string[,]> list = new Dictionary<string, string[,]>();
                using (var db = new Models.DbEntities())
                {
                    list = db.GetDataSearch(fromDate, toDate, bc, diadanhid);
                }
                return Json(new { date = list.Keys, arrDatas = list.Values, status = true });
            }
            catch (Exception ex)
            {
                return Json(new { messenger = ex.Message, status = false });
            }
        }

        
        #endregion
    }
}