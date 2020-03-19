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
                List<DataBaoCao> lstData = new List<DataBaoCao>();
                list = db.GetDataSearch(fromDate, toDate, isDuBao, diadanhid);
                var baocaoId = db.NV_MauBaocao.Where(x => x.Trangthai && x.Diadanh_ID == diadanhid).Select(x => x.MauBC_ID).FirstOrDefault();
                if (string.IsNullOrEmpty(baocaoId))
                    return Json(new { messenger = "không có mã địa danh", status = false }); ;
                var lstTT = db.NV_MaubaocaoThuoctinh.Where(x => x.MauBC_ID.Equals(baocaoId)).OrderBy(x => x.STT).Select(row => row.Thuoctinh_ID);
                var lstDD = db.NV_MaubaocaoDiadanh.Where(x => x.MauBC_ID.Equals(baocaoId)).OrderBy(x => x.STT).Select(row => row.Diadanh_ID);
                int? rowIndex, colIndex;
                foreach (KeyValuePair<string, string[,]> item in list)
                {
                    List<DataViewRows> datas = new List<DataViewRows>();
                    DataBaoCao data = new DataBaoCao();
                    data.datetime = item.Key;
                    rowIndex = null;
                    lstTT.AsEnumerable().ForEach(ref rowIndex, row =>
                    {
                        List<string> arr = new List<string>();
                        colIndex = null;
                        lstDD.AsEnumerable().ForEach(ref colIndex, col =>
                        {
                            arr.Add(item.Value[rowIndex.Value, colIndex.Value]);
                        });
                        datas.Add(new DataViewRows() { data = arr });
                    });
                    data.datas = datas;
                    lstData.Add(data);
                }
                return Json(new { header = bc, body = lstData, diadanhid, error = 0 }, JsonRequestBehavior.AllowGet);
        }
            catch (Exception e)
            {
                return Json(new { error = 1 }, JsonRequestBehavior.AllowGet);
            }
        }

        
    }
}