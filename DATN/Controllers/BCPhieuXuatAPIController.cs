using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using Newtonsoft.Json;
using OfficeOpenXml;
using WH.DataContext;
using WH.Models;
 
using ApplicationDbContext = WH.DataContext.ApplicationDbContext;

namespace WH.Controllers
{
    public class BCPhieuXuatAPIController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // Biến lưu lại thông tin tìm kiếm cho xuất Excel
        private static string _ma_vt_list;
        private static string _ma_kho_list;
        private static string _searchKeyword;
        private static DateTime? _tu_ngay;
        private static DateTime? _den_ngay;
        private static List<BCPhieuXuat> data = null;
        [HttpGet]
        [Route("api/BCPhieuXuatAPI/SearchPhieuXuat")]
        public IHttpActionResult SearchPhieuXuat(string ma_vt_list, string ma_kho_list, string keyword = "", string tu_ngay = "", string den_ngay = "")
        {
            try
            {
                _ma_vt_list = ma_vt_list ?? "";
                _ma_kho_list = ma_kho_list ?? "";
                _searchKeyword = keyword?.Trim().ToLower() ?? "";

                if (DateTime.TryParse(tu_ngay, out var startDate))
                    _tu_ngay = startDate;
                else
                    _tu_ngay = new DateTime(2000, 1, 1);

                if (DateTime.TryParse(den_ngay, out var endDate))
                    _den_ngay = endDate;
                else
                    _den_ngay = DateTime.Now;

                var inputJson = new
                {
                    ds_ma_vt = _ma_vt_list,
                    ds_ma_kho = _ma_kho_list,
                    search_keyword = _searchKeyword,
                    tu_ngay = _tu_ngay?.ToString("yyyy-MM-dd"),
                    den_ngay = _den_ngay?.ToString("yyyy-MM-dd")
                };

                string jsonStr = JsonConvert.SerializeObject(inputJson);
                var sql = "SELECT * FROM bc_tonghop_phieu_xuat(@p0::json)";
                data = db.Database.SqlQuery<BCPhieuXuat>(sql, jsonStr).ToList();

                return Ok(data);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [HttpGet]
        [Route("api/BCPhieuXuatAPI/ExportPhieuXuat")]
        public HttpResponseMessage ExportPhieuXuat()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            if (data == null || !data.Any())
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Không có dữ liệu để xuất.");
            }

            using (var package = new ExcelPackage())
            {
                var ws = package.Workbook.Worksheets.Add("PhieuXuat");

                // Danh sách tiêu đề
                string[] headers = {
            "STT", "Số CT", "Ngày CT", "Mã KH", "Tên KH", "Mã VT", "Tên VT",
            "Mã kho", "Tên kho", "Mã ĐVT", "Tên ĐVT", "Số lượng nhập",
            "Đơn giá nhập", "Thành tiền"
        };

                // Ghi tiêu đề
                for (int i = 0; i < headers.Length; i++)
                {
                    ws.Cells[1, i + 1].Value = headers[i];
                    ws.Cells[1, i + 1].Style.Font.Bold = true;
                    ws.Cells[1, i + 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                }

                int row = 2;
                foreach (var item in data)
                {
                    ws.Cells[row, 1].Value = item.stt;
                    ws.Cells[row, 2].Value = item.so_ct;
                    ws.Cells[row, 3].Value = item.ngay_ct.ToString("dd/MM/yyyy");
                    ws.Cells[row, 4].Value = item.ma_kh;
                    ws.Cells[row, 5].Value = item.ten_kh;
                    ws.Cells[row, 6].Value = item.ma_vt;
                    ws.Cells[row, 7].Value = item.ten_vt;
                    ws.Cells[row, 8].Value = item.ma_kho;
                    ws.Cells[row, 9].Value = item.ten_kho;
                    ws.Cells[row, 10].Value = item.ma_dvt;
                    ws.Cells[row, 11].Value = item.ten_dvt;
                    ws.Cells[row, 12].Value = item.so_luong_xuat;
                    ws.Cells[row, 13].Value = item.don_gia_xuat;
                    ws.Cells[row, 14].Value = item.thanh_tien;

                    // Định dạng số
                    ws.Cells[row, 12].Style.Numberformat.Format = "#,##0.00##";
                    ws.Cells[row, 13].Style.Numberformat.Format = "#,##0.00##";
                    ws.Cells[row, 14].Style.Numberformat.Format = "#,##0.00##";

                    // Tô màu dòng theo sys_order
                    string colorHex = "";
                    switch (item.sys_order)
                    {
                        case 0:
                            colorHex = "FFD580";
                            break;
                        case 1:
                            colorHex = "D4EDDA";
                            break;
                        case 2:
                            colorHex = "FFF3CD";
                            break;
                        case 3:
                            colorHex = "E6E6FA";
                            break;
                        default:
                            colorHex = "";
                            break;
                    }


                    var dataRange = ws.Cells[row, 1, row, headers.Length];
                    if (!string.IsNullOrEmpty(colorHex))
                    {
                        dataRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        dataRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#" + colorHex));
                    }

                    // Border từng ô
                    for (int col = 1; col <= headers.Length; col++)
                    {
                        ws.Cells[row, col].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    }

                    row++;
                }

                ws.Cells[ws.Dimension.Address].AutoFitColumns();

                var result = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(package.GetAsByteArray())
                };
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = "PhieuXuat.xlsx"
                };
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

                return result;
            }
        }

    }
}
