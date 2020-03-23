using MTN.Common.Models;
using MTN.Models;
using MTN.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MTN.Common
{
    public static class Common
    {
        public static Dictionary<string, string[,]> GetDataSearch(this DbEntities db, string fromDate, string toDate, string isQuantrac, string diadanhId)
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
            if ("true".Equals(isQuantrac))
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
                    lstTT.AsEnumerable().ForEach(ref rowIndex, col =>
                    {
                        colIndex = null;
                        arr = arr ?? new string[lstTT.Count(), lstDD.Count()];
                        lstDD.AsEnumerable().ForEach(ref colIndex, row =>
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
    }
}