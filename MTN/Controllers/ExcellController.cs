using System;
using System.IO;
using System.Web;
using System.Text;
using System.Linq;
using System.Web.Mvc;
using MTN.Common.Enum;
using System.Collections.Generic;
using MTN.Util;
using MTN.Common.Models;
using MTN.Attributes;

namespace MTN.Controllers
{
    public class ExcellController : Controller
    {
        // GET: Export
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [DeleteFileAttribute]
        public ActionResult Download(string file)
        {
            string fullPath = Path.Combine(Server.MapPath("~/TemplateFile/temp"), file);

            return File(fullPath, "application/vnd.ms-excel", file);
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

        #region Get Data Search DateTime
        [HttpPost]
        public JsonResult GetData(string fromDate, string toDate, string bc, string id)
        {
            try
            {
                Dictionary<string, string[,]> list = new Dictionary<string, string[,]>();
                List<DataBaoCao> lstData = new List<DataBaoCao>();
                using (var db = new Models.DbEntities())
                {
                    list = GetDataSearch(db, fromDate, toDate, bc, id);
                    var baocaoId = db.NV_MauBaocao.Where(x => x.Trangthai && x.Diadanh_ID == id).Select(x => x.MauBC_ID).FirstOrDefault();
                    if (string.IsNullOrEmpty(baocaoId))
                        return Json(new { messenger = "không có mã địa danh", status = false }); ;
                    var lstTT = db.NV_MaubaocaoThuoctinh.Where(x => x.MauBC_ID.Equals(baocaoId)).OrderBy(x => x.STT).Select(row => row.Thuoctinh_ID);
                    var lstDD = db.NV_MaubaocaoDiadanh.Where(x => x.MauBC_ID.Equals(baocaoId)).OrderBy(x => x.STT).Select(row => row.Diadanh_ID);
                    int? rowIndex, colIndex;
                    List<DataViewRows> datas = new List<DataViewRows>();
                    List<string> arr = new List<string>();
                    foreach (KeyValuePair<string, string[,]> item in list)
                    {
                        DataBaoCao data = new DataBaoCao();
                        data.datetime = item.Key;
                        rowIndex = null; datas.Clear();
                        lstDD.AsEnumerable().ForEach(ref rowIndex, row =>
                        {
                            colIndex = null; arr.Clear();
                            lstTT.AsEnumerable().ForEach(ref colIndex, col =>
                            {
                                arr.Add(item.Value[rowIndex.Value, colIndex.Value]);
                            });
                            datas.Add(new DataViewRows() { data = arr });
                        });
                        data.datas = datas;
                        lstData.Add(data);
                    }
                }

                return Json(new { data = lstData, status = true });
            }
            catch (Exception ex)
            {
                return Json(new { messenger = ex.Message, status = false });
            }
        }

        private Dictionary<string, string[,]> GetDataSearch(Models.DbEntities db, string fromDate, string toDate, string isDuBao, string diadanhId)
        {
            // fix data request
            DateTime? fromdate = fromDate.ConvertStringToDate();
            DateTime? todate = toDate.ConvertStringToDate();

            Dictionary<string, string[,]> list = new Dictionary<string, string[,]>();
            string[,] arr;
            var baocaoId = db.NV_MauBaocao.Where(x => x.Trangthai && x.Diadanh_ID == diadanhId).Select(x => x.MauBC_ID).FirstOrDefault();
            if (string.IsNullOrEmpty(baocaoId))
                return list;
            var lstTT = db.NV_MaubaocaoThuoctinh.Where(x => x.MauBC_ID.Equals(baocaoId)).OrderBy(x => x.STT).Select(row => row.Thuoctinh_ID);
            var lstDD = db.NV_MaubaocaoDiadanh.Where(x => x.MauBC_ID.Equals(baocaoId)).OrderBy(x => x.STT).Select(row => row.Diadanh_ID);
            if (!"true".Equals(isDuBao))
            {
                var dataNgayQuanTrac = db.NV_DulieuQuantrac.Select(x => x.NgayQuantrac).Distinct()
                    .WhereIf(fromdate.HasValue, row => DateTime.Compare(fromdate.Value, row) <= 0)
                    .WhereIf(todate.HasValue, row => DateTime.Compare(row, todate.Value) <= 0);

                // loaddata to string[,]
                int? rowIndex, colIndex;
                dataNgayQuanTrac.ForEach(date =>
                {
                    var lstTTs = from thuoctinh in db.TD_Thuoctinh
                                 select new DanhSachDiaDanh()
                                 {
                                     thuocTinhId = thuoctinh.Thuoctinh_ID,
                                     diadanh = (from dd in db.TD_Diadanh
                                                join dlQT in db.NV_DulieuQuantrac on dd.Diadanh_ID equals dlQT.BaocaoDiadanh_ID
                                                where dlQT.BaocaoThuoctinh_ID == thuoctinh.Thuoctinh_ID && dlQT.NgayQuantrac == date
                                                select new DanhSachThuocTinh() { DiaDanhId = dd.Diadanh_ID, GiaTri = dlQT.Giatri.ToString() })
                                 };
                    arr = null;
                    rowIndex = null;
                    lstDD.AsEnumerable().ForEach(ref rowIndex, row =>
                    {
                        colIndex = null;
                        arr = arr ?? new string[lstDD.Count(), lstTT.Count()];
                        lstTT.AsEnumerable().ForEach(ref colIndex, col =>
                        {
                            var danhSachDiaDanhs = lstTTs.Where(r => r.thuocTinhId.Equals(col)).FirstOrDefault();
                            if (danhSachDiaDanhs == null)
                                arr[rowIndex.Value, colIndex.Value] = "0";
                            else
                            {
                                var tt = danhSachDiaDanhs.diadanh.Where(d => d.DiaDanhId.Equals(row)).FirstOrDefault();
                                arr[rowIndex.Value, colIndex.Value] = tt == null ? "0" : tt.GiaTri;
                            }
                        });
                    });
                    list.Add(date.ToString("dd/MM/yyyy"), arr);
                }
                );
            }
            else
            {
                var dataNgayDuBao = db.NV_Dulieudubao.Select(x => x.Ngaydubao).Distinct()
                    .WhereIf(fromdate.HasValue, row => DateTime.Compare(fromdate.Value, row) <= 0)
                    .WhereIf(todate.HasValue, row => DateTime.Compare(row, todate.Value) <= 0);

                // loaddata to string[,]
                int? rowIndex, colIndex;
                dataNgayDuBao.ForEach(date =>
                {
                    var lstTTs = from thuoctinh in db.TD_Thuoctinh
                                 select new DanhSachDiaDanh()
                                 {
                                     thuocTinhId = thuoctinh.Thuoctinh_ID,
                                     diadanh = (from dd in db.TD_Diadanh
                                                join dlDB in db.NV_Dulieudubao on dd.Diadanh_ID equals dlDB.BaocaoDiadanh_ID
                                                where dlDB.BaocaoThuoctinh_ID == thuoctinh.Thuoctinh_ID && dlDB.Ngaydubao == date
                                                select new DanhSachThuocTinh() { DiaDanhId = dd.Diadanh_ID, GiaTri = dlDB.Giatri.ToString() })
                                 };
                    arr = null;
                    rowIndex = null;
                    lstDD.AsEnumerable().ForEach(ref rowIndex, row =>
                    {
                        colIndex = null;
                        arr = arr ?? new string[lstDD.Count(), lstTT.Count()];
                        lstTT.AsEnumerable().ForEach(ref colIndex, col =>
                        {
                            var danhSachDiaDanhs = lstTTs.Where(r => r.thuocTinhId.Equals(col)).FirstOrDefault();
                            if (danhSachDiaDanhs == null)
                                arr[rowIndex.Value, colIndex.Value] = "0";
                            else
                            {
                                var tt = danhSachDiaDanhs.diadanh.Where(d => d.DiaDanhId.Equals(row)).FirstOrDefault();
                                arr[rowIndex.Value, colIndex.Value] = tt == null ? "0" : tt.GiaTri;
                            }
                        });
                    });
                    list.Add(date.ToString("dd/MM/yyyy"), arr);
                }
                );
            }

            return list;
        }
        #endregion
    }
}