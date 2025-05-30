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
    public class CBTonKhoAPIController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private string _ma_vt_list = "";
        private string _ma_kho_list = "";

        [HttpGet]
        [Route("api/CBTonKhoAPI/GetTonKho")]
        public IHttpActionResult GetTonKho()
        {
            var inputJson = "{\"ds_ma_vt\": \"\", \"ds_ma_kho\": \"\", \"group_yn\": true}";
            var sql = "SELECT * FROM cbtontheokho(@p0::json)";
            var data = db.Database.SqlQuery<CBTonKho>(sql, inputJson).ToList();

            return Ok(data);
        }

        [HttpGet]
        [Route("api/CBTonKhoAPI/SearchTonKho")]
        public IHttpActionResult SearchTonKho(string ma_vt_list, string ma_kho_list)
        {
            _ma_vt_list = ma_vt_list;
            _ma_kho_list = ma_kho_list;
            var inputJson = "{\"ds_ma_vt\": \"" + ma_vt_list + "\", \"ds_ma_kho\": \"" + ma_kho_list + "\", \"group_yn\": true}";

            var sql = "SELECT * FROM cbtontheokho(@p0::json)";
            var data = db.Database.SqlQuery<CBTonKho>(sql, inputJson).ToList();

            return Ok(data);
        }

        [HttpGet]
        [Route("api/CBTonKhoAPI/ExportTonKho")]
        public HttpResponseMessage ExportTonKho()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var inputJson = "{\"ds_ma_vt\": \"" + _ma_vt_list + "\", \"ds_ma_kho\": \"" + _ma_kho_list + "\", \"group_yn\": true}";
            var sql = "SELECT * FROM cbtontheokho(@p0::json)";
            var data = db.Database.SqlQuery<CBTonKho>(sql, inputJson).ToList();

            using (var package = new ExcelPackage())
            {
                var ws = package.Workbook.Worksheets.Add("TonKho");

                // Add headers
                string[] headers = {
                    "STT", "Mã kho", "Tên kho", "Mô tả kho", "Địa chỉ kho", "Mã vật tư", "Tên vật tư",
                    "Rộng", "Cao", "Khối lượng", "Màu sắc", "Kiểu dáng", "Mã ĐVT", "Tên ĐVT",
                    "Mô tả ĐVT", "Tỷ lệ quy đổi", "Số lượng tồn", "Số lượng định xuất",
                    "Số lượng đang nhập", "Cảnh báo tồn kho", "Ngày cập nhật"
                };

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
                    ws.Cells[row, 2].Value = item.ma_kho;
                    ws.Cells[row, 3].Value = item.ten_kho;
                    ws.Cells[row, 4].Value = item.mo_ta_kho;
                    ws.Cells[row, 5].Value = item.dia_chi_kho;
                    ws.Cells[row, 6].Value = item.ma_vt;
                    ws.Cells[row, 7].Value = item.ten_vt;
                    ws.Cells[row, 8].Value = item.rong;
                    ws.Cells[row, 9].Value = item.cao;
                    ws.Cells[row, 10].Value = item.khoi_luong;
                    ws.Cells[row, 11].Value = item.mau_sac;
                    ws.Cells[row, 12].Value = item.kieu_dang;
                    ws.Cells[row, 13].Value = item.ma_dvt;
                    ws.Cells[row, 14].Value = item.ten_dvt;
                    ws.Cells[row, 15].Value = item.mo_ta_dvt;
                    ws.Cells[row, 16].Value = item.ty_le_quy_doi;
                    ws.Cells[row, 17].Value = item.so_luong_ton;
                    ws.Cells[row, 18].Value = item.so_luong_dinh_xuat;
                    ws.Cells[row, 19].Value = item.so_luong_dang_nhap;
                    ws.Cells[row, 20].Value = item.canh_bao_ton_kho;

                    if (item.ngay_cap_nhat.HasValue)
                    {
                        ws.Cells[row, 21].Value = item.ngay_cap_nhat.Value;
                        ws.Cells[row, 21].Style.Numberformat.Format = "dd/MM/yyyy";
                    }

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


                    var dataRange = ws.Cells[row, 1, row, 21];

                    if (!string.IsNullOrEmpty(colorHex))
                    {
                        dataRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        dataRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#" + colorHex));
                    }

                    // Border
                    for (int col = 1; col <= 21; col++)
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
                    FileName = "CANHBAOTONKHO.xlsx"
                };
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

                return result;
            }
        }
    }
}
