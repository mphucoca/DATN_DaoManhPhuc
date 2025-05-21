using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using WH.DataContext;
using WH.Models;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using System.Net.Http;
using System.Net.Http.Headers;
using ApplicationDbContext = WH.DataContext.ApplicationDbContext;
using System.IO;
using iText.Kernel.Font;
using iText.IO.Font;
using System.Windows;
using iText.Layout.Properties;
using iText.Kernel.Colors;

namespace WH.Controllers
{
    public class PhieuNhapAPIController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
  
        public IHttpActionResult Get()
        {
            var query = @"
        SELECT 
              p.so_ct, 
    p.ma_ncc, 
    p.ngay_ct, 
    p.dien_giai, 
    p.trang_thai, 
    p.bien_so_xe,
    p.ngay_van_chuyen,
    p.tt_thanhtoan,
    p.chietkhau,
    p.ma_so_thue,p.tong_thanh_toan,
p.tong_chiet_khau,
p.tong_thue,
    p.thue,
    n.ten_ncc as ten_ncc
        FROM phieu_nhap p
        LEFT JOIN dmncc n ON p.ma_ncc = n.ma_ncc order by p.so_ct desc;";
            // Sử dụng View Model chỉ để hiển thị dũ liệu
            var phieuNhapList = db.Database.SqlQuery<PhieuNhapViewModel>(query).ToList();
            return Ok(phieuNhapList);
        }
        [HttpGet]
        [Route("api/PhieuNhapAPI/GET_CT")]
        public IHttpActionResult GET_CT(string so_ct)
        {
            var query = @"
        SELECT * 
        FROM ct_phieu_nhap 
        WHERE so_ct = @p0";

            // Sử dụng View Model chỉ để hiển thị dữ liệu
            var ctphieuNhapList = db.Database.SqlQuery<ChiTietPhieuNhapClass>(query, so_ct).ToList();

            return Ok(ctphieuNhapList);
        }

        [HttpPost]
        [Route("api/PhieuNhapAPI/SEARCH")]
        public IHttpActionResult SEARCH(JObject data)
        {
            var keyword = (string)data["keyword"];
            var so_ct = (string)data["so_ct"];
            var ma_ncc = (string)data["ma_ncc"];
            var tu_ngay = data["tu_ngay"]?.ToObject<DateTime?>()?.Date;
            var den_ngay = data["den_ngay"]?.ToObject<DateTime?>()?.Date;


            var query = @"
        SELECT 
                p.so_ct, 
    p.ma_ncc, 
    p.ngay_ct, 
    p.dien_giai, 
    p.trang_thai, 
    p.bien_so_xe,
    p.ngay_van_chuyen,
    p.tt_thanhtoan,
    p.chietkhau,
    p.ma_so_thue,
    p.thue,p.tong_thanh_toan,
p.tong_chiet_khau,
p.tong_thue,
    n.ten_ncc as ten_ncc
        FROM phieu_nhap p
        LEFT JOIN dmncc n ON p.ma_ncc = n.ma_ncc
        WHERE 1=1";

            if (!string.IsNullOrEmpty(keyword))
                query += $" AND (p.so_ct ILIKE '%{keyword}%' OR p.dien_giai ILIKE '%{keyword}%'OR n.ten_ncc ILIKE '%{keyword}%'OR p.ma_ncc ILIKE '%{keyword}%'OR p.ngay_ct::text ILIKE '%{keyword}%')";

            if (!string.IsNullOrEmpty(so_ct))
                query += $" AND p.so_ct ILIKE '%{so_ct}%'";

            if (!string.IsNullOrEmpty(ma_ncc))
                query += $" AND p.ma_ncc ILIKE '%{ma_ncc}%'";

            if (tu_ngay.HasValue)
                query += $" AND CAST(p.ngay_ct AS date) >= '{tu_ngay.Value:yyyy-MM-dd}'";

            if (den_ngay.HasValue)
                query += $" AND CAST(p.ngay_ct AS date) <= '{den_ngay.Value:yyyy-MM-dd}'";
            query += $"order by p.so_ct desc";
            var result = db.Database.SqlQuery<PhieuNhapViewModel>(query).ToList();

            return Ok(new { success = true, result });
        }

        [HttpPost]
        [Route("api/PhieuNhapAPI/DeleteAll")]
        public IHttpActionResult DeleteAll([FromBody] List<PhieuNhapClass> list)
        {
            if (list == null || !list.Any())
                return BadRequest("Danh sách rỗng");

            foreach (var item in list)
            {
                var phieu = db.PhieuNhapObj.FirstOrDefault(p => p.so_ct == item.so_ct.ToString());
                if (phieu != null)
                {
                    db.PhieuNhapObj.Remove(phieu);
                }
            } 
            db.SaveChanges();
            return Ok("Đã xóa các phiếu được chọn");
        }
        [HttpGet]
        [Route("api/PhieuNhapAPI/GetDVTByMaVt")]
        public IHttpActionResult GetDVTByMaVt(string ma_vt)
        {
            var query = @"
    SELECT 
        qd.ma_vt, 
        qd.ma_dvt, 
        dvt.ten_dvt, 
        dvt.mo_ta,
        dvt.trangthai,
        qd.ty_le_quy_doi
    FROM dmqddvt qd
    LEFT JOIN dmdvt dvt ON qd.ma_dvt = dvt.ma_dvt
    WHERE qd.ma_vt = @p0

    UNION

    SELECT 
        dmvt.ma_vt, 
        dmvt.ma_dvt, 
        dvt.ten_dvt, 
        dvt.mo_ta,
        dvt.trangthai,
        1 AS ty_le_quy_doi
    FROM dmvt
    LEFT JOIN dmdvt dvt ON dmvt.ma_dvt = dvt.ma_dvt
    WHERE dmvt.ma_vt = @p0
      AND NOT EXISTS (
          SELECT 1 
          FROM dmqddvt 
          WHERE dmqddvt.ma_vt = dmvt.ma_vt 
            AND dmqddvt.ma_dvt = dmvt.ma_dvt
      );";

            var result = db.Database.SqlQuery<DmdvtQddvtViewModel>(query, ma_vt).ToList();
            return Ok(result);
        }

        [HttpPost]
        [Route("api/PhieuNhapAPI/SaveAll")]
        public IHttpActionResult SaveAll(List<ChiTietPhieuNhapClass> list)
        {
            try
            {
                if (list == null || !list.Any())
                    return BadRequest("Danh sách trống");

                var so_ct = list.First().so_ct;  // Lấy so_ct từ bản ghi đầu tiên

                // Bước 1: Xóa hết các bản ghi cũ trong bảng ct_phieu_nhap theo so_ct
                var oldItems = db.ChiTietPhieuNhapObj.Where(x => x.so_ct == so_ct).ToList();
                db.ChiTietPhieuNhapObj.RemoveRange(oldItems);
                db.SaveChanges();  // Lưu thay đổi sau khi xóa

                // Bước 2: Thêm mới các bản ghi gửi lên
                foreach (var item in list)
                {
                    db.ChiTietPhieuNhapObj.Add(item);  // Thêm từng bản ghi vào bảng
                    db.SaveChanges();
                }

                  // Lưu thay đổi sau khi thêm mới

                return Ok(new { success = true, message = "Lưu thành công" });
            }
            catch (Exception ex)
            {
                // Trả về lỗi nếu có
                return Ok(new { success = false, message = ex.Message });
            }
        }
        [HttpPost]
        [Route("api/PhieuNhapAPI/SaveAdd")]
        public IHttpActionResult SaveAdd(JObject data)
        {
            try
            {
                var phieu = data.ToObject<PhieuNhapClass>();
                if (phieu == null || string.IsNullOrEmpty(phieu.so_ct))
                    return BadRequest("Dữ liệu không hợp lệ");

                if (db.PhieuNhapObj.Any(x => x.so_ct == phieu.so_ct))
                {
                    return Ok(new { success = false, message = "Số chứng từ đã tồn tại." });
                }

                db.PhieuNhapObj.Add(phieu);
                db.SaveChanges();

                return Ok(new { success = true, result = phieu });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }
        [HttpPut]
        [Route("api/PhieuNhapAPI/SaveEdit")]
        public IHttpActionResult SaveEdit(JObject data)
        {
            try
            {
                var phieu = data.ToObject<PhieuNhapClass>();
                if (phieu == null || string.IsNullOrEmpty(phieu.so_ct))
                    return BadRequest("Dữ liệu không hợp lệ");

                var existing = db.PhieuNhapObj.FirstOrDefault(x => x.so_ct == phieu.so_ct);
                if (existing == null)
                {
                    return Ok(new { success = false, message = "Phiếu nhập không tồn tại." });
                }

                existing.ma_ncc = phieu.ma_ncc;
                existing.ngay_ct = phieu.ngay_ct;
                existing.dien_giai = phieu.dien_giai;
                existing.trang_thai = phieu.trang_thai;
                existing.bien_so_xe = phieu.bien_so_xe;
                existing.ngay_van_chuyen = phieu.ngay_van_chuyen;
                existing.tt_thanhtoan = phieu.tt_thanhtoan;
                existing.chietkhau = phieu.chietkhau;
                existing.ma_so_thue = phieu.ma_so_thue;
                existing.thue = phieu.thue;
                existing.tong_thanh_toan = phieu.tong_thanh_toan;
                existing.tong_chiet_khau = phieu.tong_chiet_khau;
                existing.tong_thue = phieu.tong_thue;
                db.Entry(existing).State = EntityState.Modified;
                db.SaveChanges();

                return Ok(new { success = true, result = existing });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        [Route("api/phieunhap/tao_soct")]
        public IHttpActionResult TaoSoChungTu()
        {
            // Lấy username từ cookie
            var cookie = HttpContext.Current.Request.Cookies["UserSessionCookie"];

            // Kiểm tra nếu cookie tồn tại và lấy giá trị UserName từ cookie
            var username = cookie?.Value ?? "000000";  // Nếu cookie không tồn tại, dùng giá trị mặc định "000000"

            // Đảm bảo chuỗi số có đủ 6 ký tự, thêm '0' ở đầu nếu cần
            string paddedUsername = username.PadLeft(6, '0');

            // Lấy thời gian hiện tại theo định dạng ddMMyyHHmmss
            string timestamp = DateTime.Now.ToString("ddMMyyHHmmss");

            // Tạo số chứng từ theo định dạng PX+user+timestamp
            string so_ct = "PN" + paddedUsername + "-" + timestamp;

            // Trả về kết quả JSON
            return Ok(new { so_ct });
        }
        [HttpGet]
        [Route("api/PhieuNhapAPI/ExportPdf_iText")]
        public HttpResponseMessage ExportPdf_iText(string so_ct)
        {
            using (var ms = new MemoryStream())
            {
                var writer = new PdfWriter(ms);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf);

                // Font
                string fontBoldPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arialbd.ttf");
                PdfFont fontBold = PdfFontFactory.CreateFont(fontBoldPath, PdfEncodings.IDENTITY_H);
                string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf");
                PdfFont unicodeFont = iText.Kernel.Font.PdfFontFactory.CreateFont(fontPath, iText.IO.Font.PdfEncodings.IDENTITY_H);
                document.SetFont(unicodeFont);

                // 1. Header row: logo - company name - address
                Table companyTable = new Table(new float[] { 1, 4 });
                companyTable.SetWidth(iText.Layout.Properties.UnitValue.CreatePercentValue(100));

                string logoPath = HttpContext.Current.Server.MapPath("~/Content/IMAGE/warehouse_17883543.png");
                Image logoImage = new Image(iText.IO.Image.ImageDataFactory.Create(logoPath)).SetWidth(40).SetHeight(40);

                Cell logoCell = new Cell().Add(logoImage).SetBorder(iText.Layout.Borders.Border.NO_BORDER);
                companyTable.AddCell(logoCell);

                Paragraph companyInfo = new Paragraph()
                    .Add(new Text("CÔNG TY CỔ PHẦN NHÔM ĐÔ THÀNH\n").SetFont(fontBold).SetFontSize(11))
                    .Add(new Text("Khu Công nghiệp vừa và nhỏ, Xã Phú Thị, Huyện Gia Lâm, Hà Nội\n").SetFontSize(10))
                    .Add(new Text("Điện thoại: 0123456789 | MST: 1234567890").SetFontSize(10));
                Cell infoCell = new Cell().Add(companyInfo).SetBorder(iText.Layout.Borders.Border.NO_BORDER);
                companyTable.AddCell(infoCell);

                document.Add(companyTable);

                // 2. Row: "PHIẾU NHẬP KHO" chính giữa; QR code bên phải; khoảng trống bên trái
                Table headerTable = new Table(new float[] { 5, 3, 1 }); // cột trái = cột phải, cột giữa lớn hơn
                headerTable.SetWidth(iText.Layout.Properties.UnitValue.CreatePercentValue(100));

                // Cột trái: trống
                Cell leftEmptyCell = new Cell().SetBorder(iText.Layout.Borders.Border.NO_BORDER);
                headerTable.AddCell(leftEmptyCell);

                // Cột giữa: tiêu đề căn giữa cả ngang và dọc
                Cell titleCell = new Cell()
                    .Add(new Paragraph("PHIẾU NHẬP KHO")
                        .SetFont(fontBold)
                        .SetFontSize(16)
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                    )
                    .SetVerticalAlignment(iText.Layout.Properties.VerticalAlignment.MIDDLE)
                    .SetBorder(iText.Layout.Borders.Border.NO_BORDER);
                headerTable.AddCell(titleCell);

                

                document.Add(headerTable);
                //
                Table headerTable1 = new Table(new float[] { 1 }); // cột trái = cột phải, cột giữa lớn hơn
                headerTable1.SetWidth(iText.Layout.Properties.UnitValue.CreatePercentValue(100));

                // Cột trái: trống
                Cell leftEmptyCell1 = new Cell().SetBorder(iText.Layout.Borders.Border.NO_BORDER);
                headerTable1.AddCell(leftEmptyCell1);

                // Cột giữa: tiêu đề căn giữa cả ngang và dọc
               

                // Cột phải: QR Code
                var qrCode1 = new iText.Barcodes.BarcodeQRCode(so_ct);
                var qrImageData1 = qrCode1.CreateFormXObject(pdf);
                var qrImage1 = new Image(qrImageData1).SetWidth(60).SetHeight(60);

                Cell cellQR1 = new Cell()
                    .Add(qrImage1)
                    .SetBorder(iText.Layout.Borders.Border.NO_BORDER)
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT)
                    .SetVerticalAlignment(iText.Layout.Properties.VerticalAlignment.MIDDLE);
                headerTable1.AddCell(cellQR1);

                document.Add(headerTable1);

                // Lấy dữ liệu phiếu nhập từ database
                var phieu = db.PhieuNhapObj.FirstOrDefault(p => p.so_ct == so_ct);
                var ncc = db.DmnccObj.FirstOrDefault(n => n.ma_ncc == phieu.ma_ncc);

                string thongTinPhieu = $"Số phiếu: {phieu.so_ct}\n" +
                                       $"Ngày chứng từ: {phieu.ngay_ct:dd/MM/yyyy}\n" +
                                       $"Nhà cung cấp: {phieu.ma_ncc} - {ncc?.ten_ncc}\n" +
                                       $"Địa chỉ NCC: {ncc?.dia_chi}\n" +
                                       $"Mã số thuế: {phieu.ma_so_thue}";

                Paragraph pThongTinPhieu = new Paragraph(thongTinPhieu).SetFontSize(10);
                document.Add(pThongTinPhieu);

                // TODO: Thêm phần chi tiết vật tư, tổng hợp, chữ ký...
                var result_ct = from ctpn in db.ChiTietPhieuNhapObj
                             join dm in db.DmvtObj on ctpn.ma_vt equals dm.ma_vt
                             join pn in db.PhieuNhapObj on new { ctpn.so_ct, ctpn.ma_ncc } equals new { pn.so_ct, pn.ma_ncc }
                             where ctpn.so_ct == so_ct
                             select new
                             {
                                 ctpn.ma_vt,
                                 dm.ten_vt,
                                 ctpn.so_luong_nhap,
                                 ctpn.don_gia_nhap,
                                 ctpn.ma_dvt,
                                 ctpn.ghi_chu,
                                 pn.tong_thanh_toan,
                                 pn.tong_chiet_khau,
                                 pn.tong_thue
                             };

                var chiTietVatTu = result_ct.ToList();

                // Tạo bảng chi tiết vật tư với 6 cột
                float[] columnWidths = {1, 2, 5, 2, 3, 2, 4 ,2};
                Table detailsTable = new Table(columnWidths);
                detailsTable.SetWidth(UnitValue.CreatePercentValue(100));

                // Thêm header bảng, in đậm
                // Tạo màu xanh dương nhạt
                Color lightBlue = new DeviceRgb(173, 216, 230);

                // Thêm header bảng, in đậm, có background xanh dương nhạt
                detailsTable.AddHeaderCell(new Cell().Add(new Paragraph("STT").SetFont(fontBold))
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                    .SetBackgroundColor(lightBlue));
                detailsTable.AddHeaderCell(new Cell().Add(new Paragraph("Mã VT").SetFont(fontBold))
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                    .SetBackgroundColor(lightBlue));
                detailsTable.AddHeaderCell(new Cell().Add(new Paragraph("Tên VT").SetFont(fontBold))
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                    .SetBackgroundColor(lightBlue));
                detailsTable.AddHeaderCell(new Cell().Add(new Paragraph("Số lượng").SetFont(fontBold))
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                    .SetBackgroundColor(lightBlue));
                detailsTable.AddHeaderCell(new Cell().Add(new Paragraph("Đơn giá").SetFont(fontBold))
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                    .SetBackgroundColor(lightBlue));
                detailsTable.AddHeaderCell(new Cell().Add(new Paragraph("Đơn vị").SetFont(fontBold))
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                    .SetBackgroundColor(lightBlue));
                detailsTable.AddHeaderCell(new Cell().Add(new Paragraph("Ghi chú").SetFont(fontBold))
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                    .SetBackgroundColor(lightBlue));
                detailsTable.AddHeaderCell(new Cell().Add(new Paragraph("Thành tiền").SetFont(fontBold))
                 .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                 .SetBackgroundColor(lightBlue));
                // Thêm dữ liệu chi tiết
                int stt = 0;
                decimal tongThanhTien = 0;
                foreach (var item in chiTietVatTu)
                {
                    stt++;
                    decimal thanhTien = item.so_luong_nhap * item.don_gia_nhap;
                    tongThanhTien += thanhTien;
                    detailsTable.AddCell(new Cell().Add(new Paragraph(stt.ToString())).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER));
                    detailsTable.AddCell(new Cell().Add(new Paragraph(item.ma_vt)));
                    detailsTable.AddCell(new Cell().Add(new Paragraph(item.ten_vt ?? "")));
                    detailsTable.AddCell(new Cell().Add(new Paragraph(item.so_luong_nhap.ToString() ?? "0")).SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT));
                    detailsTable.AddCell(new Cell().Add(new Paragraph(string.Format("{0:N2}", item.don_gia_nhap))).SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT));
                    detailsTable.AddCell(new Cell().Add(new Paragraph(item.ma_dvt ?? "")));
                    detailsTable.AddCell(new Cell().Add(new Paragraph(item.ghi_chu ?? "")));
                    detailsTable.AddCell(new Cell().Add(new Paragraph(string.Format("{0:N2}", thanhTien))).SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT));
                }
                // Thêm dòng tổng cuối bảng (gộp các cột đầu, căn phải, in đậm)
                Cell totalLabelCell = new Cell(1, 7).Add(new Paragraph("Tổng cộng").SetFont(fontBold).SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT))
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT);
                detailsTable.AddCell(totalLabelCell);

                Cell totalThanhTienCell = new Cell().Add(new Paragraph(string.Format("{0:N2}", tongThanhTien)).SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT))
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT);
                detailsTable.AddCell(totalThanhTienCell);


                // Tổng chiết khấu
                Cell totalLabelCell2 = new Cell(1, 7).Add(new Paragraph("Tổng chiết khấu").SetFont(fontBold).SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT));
                
                detailsTable.AddCell(totalLabelCell2);

                Cell totalThanhTienCell2 = new Cell().Add(new Paragraph(string.Format("{0:N2}", phieu.tong_chiet_khau)).SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT));
                detailsTable.AddCell(totalThanhTienCell2);

                // Tổng thuế
                Cell totalLabelCell3 = new Cell(1, 7).Add(new Paragraph("Tổng thuế").SetFont(fontBold).SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT));
                
                detailsTable.AddCell(totalLabelCell3);

                Cell totalThanhTienCell3 = new Cell().Add(new Paragraph(string.Format("{0:N2}", phieu.tong_thue)).SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT));
      
                detailsTable.AddCell(totalThanhTienCell3);

                // Tổng thanh toán
                Cell totalLabelCell4 = new Cell(1, 7).Add(new Paragraph("Tổng thanh toán").SetFont(fontBold).SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT));
          
                detailsTable.AddCell(totalLabelCell4);

                Cell totalThanhTienCell4 = new Cell().Add(new Paragraph(string.Format("{0:N2}", phieu.tong_thanh_toan)).SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT));
 
                detailsTable.AddCell(totalThanhTienCell4);

                // Thêm bảng vào document
                document.Add(new Paragraph("\nChi tiết vật tư nhập kho:").SetFont(fontBold).SetFontSize(12));
                document.Add(detailsTable);

               


                // Phần ký tên cuối trang
                Table signatureTable = new Table(2);
                signatureTable.SetWidth(UnitValue.CreatePercentValue(100));
                signatureTable.SetMarginTop(50);

                signatureTable.AddCell(new Cell().Add(new Paragraph("Người lập phiếu").SetFontSize(12).SetFont(fontBold))
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER).SetBorder(iText.Layout.Borders.Border.NO_BORDER));

                signatureTable.AddCell(new Cell().Add(new Paragraph("Thủ kho").SetFontSize(12).SetFont(fontBold))
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER).SetBorder(iText.Layout.Borders.Border.NO_BORDER));

                // Dòng trống cho ký tên
                signatureTable.AddCell(new Cell().Add(new Paragraph("\n")).SetBorder(iText.Layout.Borders.Border.NO_BORDER));
                signatureTable.AddCell(new Cell().Add(new Paragraph("\n")).SetBorder(iText.Layout.Borders.Border.NO_BORDER));

                // Tên người ký (có thể lấy từ DB hoặc để trống)
                signatureTable.AddCell(new Cell().Add(new Paragraph("(Ký, họ tên)").SetFontSize(10))
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER).SetBorder(iText.Layout.Borders.Border.NO_BORDER));

                signatureTable.AddCell(new Cell().Add(new Paragraph("(Ký, họ tên)").SetFontSize(10))
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER).SetBorder(iText.Layout.Borders.Border.NO_BORDER));

                document.Add(signatureTable);
                //
                document.Close();

                var result = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(ms.ToArray())
                };

                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline")
                {
                    FileName = $"PhieuNhap_{so_ct}.pdf"
                };
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

                return result;
            }
        }





    }
}
