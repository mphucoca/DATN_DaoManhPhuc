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
using WH.Helpers;
using System.Threading.Tasks;

namespace WH.Controllers
{
    public class PhieuXuatAPIController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private AuditHelper auditHelper;

        public PhieuXuatAPIController()
        {
            auditHelper = new AuditHelper(db);
        }
        // GET all PhieuXuat
        public IHttpActionResult Get()
        {
            var query = @"
                SELECT 
                    p.so_ct, p.ngay_ct, p.dien_giai, p.ma_kh, p.trang_thai, 
    p.bien_so_xe, p.ngay_van_chuyen, p.tt_thanhtoan, 
    p.chietkhau, p.ma_so_thue, p.thue, 
    p.tong_thanh_toan, p.tong_chiet_khau, p.tong_thue,
    p.nguoi_giao, p.dia_diem_giao,
    b.ten_kh
                FROM phieu_xuat p left join dmkh b on p.ma_kh = b.ma_kh
                ORDER BY p.so_ct DESC";

            var phieuXuatList = db.Database.SqlQuery<PhieuXuatViewModel>(query).ToList();
            return Ok(phieuXuatList);
        }

        // GET ChiTiet PhieuXuat by so_ct
        [HttpGet]
        [Route("api/PhieuXuatAPI/GET_CT")]
        public IHttpActionResult GET_CT(string so_ct)
        {
            var query = @"
        SELECT 
            a.ma_vt, 
            a.ma_kho, 
            a.so_ct, 
            a.ma_kh, 
            a.ma_dvt, 
            a.so_luong_xuat, 
            a.don_gia_xuat, 
            a.ghi_chu,
             COALESCE(b.so_luong_ton, 0) AS ton
        FROM ct_phieu_xuat a
        LEFT JOIN tonkho b 
            ON a.ma_kho = b.ma_kho AND a.ma_vt = b.ma_vt AND a.ma_dvt = b.ma_dvt
        WHERE a.so_ct = @p0";

            var ctPhieuXuatList = db.Database.SqlQuery<ChiTietPhieuXuatViewClass>(query, so_ct).ToList();
            return Ok(ctPhieuXuatList);
        }
       
        // SEARCH with filters
        [HttpPost]
        [Route("api/PhieuXuatAPI/SEARCH")]
        public IHttpActionResult SEARCH(JObject data)
        {
            var keyword = (string)data["keyword"];
            var so_ct = (string)data["so_ct"];
            var ma_kh = (string)data["ma_kh"];
            var tu_ngay = data["tu_ngay"]?.ToObject<DateTime?>()?.Date;
            var den_ngay = data["den_ngay"]?.ToObject<DateTime?>()?.Date;

            var query = @"
                SELECT 
                       p.so_ct, p.ngay_ct, p.dien_giai, p.ma_kh, p.trang_thai, 
    p.bien_so_xe, p.ngay_van_chuyen, p.tt_thanhtoan, 
    p.chietkhau, p.ma_so_thue, p.thue, 
    p.tong_thanh_toan, p.tong_chiet_khau, p.tong_thue,
    p.nguoi_giao, p.dia_diem_giao,
    b.ten_kh
                FROM phieu_xuat p left join dmkh b on p.ma_kh = b.ma_kh
           
                WHERE 1=1";

            if (!string.IsNullOrEmpty(keyword))
                query += $" AND (p.so_ct ILIKE '%{keyword}%' OR p.dien_giai ILIKE '%{keyword}%' OR p.ma_kh ILIKE '%{keyword}%' OR p.ngay_ct::text ILIKE '%{keyword}%')";

            if (!string.IsNullOrEmpty(so_ct))
                query += $" AND p.so_ct ILIKE '%{so_ct}%'";

            if (!string.IsNullOrEmpty(ma_kh))
                query += $" AND p.ma_kh ILIKE '%{ma_kh}%'";

            if (tu_ngay.HasValue)
                query += $" AND CAST(p.ngay_ct AS date) >= '{tu_ngay.Value:yyyy-MM-dd}'";

            if (den_ngay.HasValue)
                query += $" AND CAST(p.ngay_ct AS date) <= '{den_ngay.Value:yyyy-MM-dd}'";

            query += " ORDER BY p.so_ct DESC";
            var result = db.Database.SqlQuery<PhieuXuatViewModel>(query).ToList();

            return Ok(new { success = true, result });
        }

        // DELETE selected PhieuXuat
        [HttpPost]
        [Route("api/PhieuXuatAPI/DeleteAll")]
        public async Task<IHttpActionResult> DeleteAll([FromBody] List<PhieuXuatClass> list)
        {
            if (list == null || !list.Any())
                return BadRequest("Danh sách rỗng");

            foreach (var item in list)
            {
                var phieu = db.PhieuXuatObj.FirstOrDefault(p => p.so_ct == item.so_ct);
                if (phieu != null)
                {
                    var primaryKeyData = new { so_ct = phieu.so_ct };
                    await auditHelper.SaveAuditLogAsync(
                        tableName: "phieu_xuat",
                        operation: "DELETE",
                        primaryKeyData: primaryKeyData,
                        oldData: phieu,
                        newData: null
                    );
                    db.PhieuXuatObj.Remove(phieu);
                }
            }
            await db.SaveChangesAsync();
            return Ok("Đã xóa các phiếu được chọn");
        }

        // POST Save All ChiTiet PhieuXuat
        [HttpPost]
        [Route("api/PhieuXuatAPI/SaveAll")]
        public async Task<IHttpActionResult> SaveAll(List<ChiTietPhieuXuatClass> list)
        {
            try
            {
                if (list == null || !list.Any())
                    return BadRequest("Danh sách trống");

                var so_ct = list.First().so_ct;

                // Bước 1: Lấy các bản ghi cũ theo so_ct
                var oldItems = db.ChiTietPhieuXuatObj.Where(x => x.so_ct == so_ct).ToList();

                if (!oldItems.Any())
                {
                    // Nếu không có bản ghi cũ, log là INSERT từng dòng
                    foreach (var item in list)
                    {
                        db.ChiTietPhieuXuatObj.Add(item);

                        await auditHelper.SaveAuditLogAsync(
                            tableName: "ct_phieu_xuat",
                            operation: "INSERT",
                            primaryKeyData: new { so_ct = item.so_ct, ma_vt = item.ma_vt, ma_kho = item.ma_kho, ma_dvt = item.ma_dvt },
                            oldData: null,
                            newData: item
                        );
                    }
                }
                else
                {
                    // Bước 2: Xóa các bản ghi cũ
                    db.ChiTietPhieuXuatObj.RemoveRange(oldItems);
                    await db.SaveChangesAsync();

                    // Bước 3: Ghi lại log sửa (UPDATE) từng dòng
                    foreach (var item in list)
                    {
                        var old = oldItems.FirstOrDefault(x => x.ma_vt == item.ma_vt && x.ma_kho == item.ma_kho && x.ma_dvt == item.ma_dvt);

                        await auditHelper.SaveAuditLogAsync(
                            tableName: "ct_phieu_xuat",
                            operation: "UPDATE",
                            primaryKeyData: new { so_ct = item.so_ct, ma_vt = item.ma_vt, ma_kho = item.ma_kho, ma_dvt = item.ma_dvt },
                            oldData: old,
                            newData: item
                        );

                        db.ChiTietPhieuXuatObj.Add(item);
                    }
                }

                await db.SaveChangesAsync();

                return Ok(new { success = true, message = "Lưu thành công" });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        // POST Save new PhieuXuat
        [HttpPost]
        [Route("api/PhieuXuatAPI/SaveAdd")]
        public async Task<IHttpActionResult> SaveAdd(JObject data)
        {
            try
            {
                var phieu = data.ToObject<PhieuXuatClass>();
                if (phieu == null || string.IsNullOrEmpty(phieu.so_ct))
                    return BadRequest("Dữ liệu không hợp lệ");

                if (db.PhieuXuatObj.Any(x => x.so_ct == phieu.so_ct))
                {
                    return Ok(new { success = false, message = "Số chứng từ đã tồn tại." });
                }

                db.PhieuXuatObj.Add(phieu);
                await db.SaveChangesAsync();

                // Ghi log audit - INSERT
                var primaryKeyData = new { so_ct = phieu.so_ct };
                await auditHelper.SaveAuditLogAsync(
                    tableName: "phieu_xuat",
                    operation: "INSERT",
                    primaryKeyData: primaryKeyData,
                    oldData: null,
                    newData: phieu
                );

                return Ok(new { success = true, result = phieu });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }


        [HttpPut]
        [Route("api/PhieuXuatAPI/SaveEdit")]
        public async Task<IHttpActionResult> SaveEdit(JObject data)
        {
            try
            {
                var phieu = data.ToObject<PhieuXuatClass>();
                if (phieu == null || string.IsNullOrEmpty(phieu.so_ct))
                    return BadRequest("Dữ liệu không hợp lệ");

                var existing = db.PhieuXuatObj.FirstOrDefault(x => x.so_ct == phieu.so_ct);
                if (existing == null)
                {
                    return Ok(new { success = false, message = "Phiếu xuất không tồn tại." });
                }

                // Ghi lại dữ liệu cũ trước khi thay đổi
                var oldData = new PhieuXuatClass
                {
                    so_ct = existing.so_ct,
                    ngay_ct = existing.ngay_ct,
                    dien_giai = existing.dien_giai,
                    trang_thai = existing.trang_thai,
                    bien_so_xe = existing.bien_so_xe,
                    ngay_van_chuyen = existing.ngay_van_chuyen,
                    tt_thanhtoan = existing.tt_thanhtoan,
                    chietkhau = existing.chietkhau,
                    ma_so_thue = existing.ma_so_thue,
                    thue = existing.thue,
                    tong_thanh_toan = existing.tong_thanh_toan,
                    tong_chiet_khau = existing.tong_chiet_khau,
                    tong_thue = existing.tong_thue,
                    nguoi_giao = existing.nguoi_giao,
                    dia_diem_giao = existing.dia_diem_giao
                };

                // Cập nhật thông tin
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
                existing.nguoi_giao = phieu.nguoi_giao;
                existing.dia_diem_giao = phieu.dia_diem_giao;

                db.Entry(existing).State = EntityState.Modified;

                // Ghi log audit - UPDATE
                var primaryKeyData = new { so_ct = phieu.so_ct };
                await auditHelper.SaveAuditLogAsync(
                    tableName: "phieu_xuat",
                    operation: "UPDATE",
                    primaryKeyData: primaryKeyData,
                    oldData: oldData,
                    newData: phieu
                );

                await db.SaveChangesAsync();

                return Ok(new { success = true, result = existing });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        [Route("api/phieuxuat/tao_soct")]
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
            string so_ct = "PX" + paddedUsername + "-" + timestamp;

            // Trả về kết quả JSON
            return Ok(new { so_ct });
        }
        [HttpGet]
        [Route("api/PhieuXuatAPI/GetTonTheoKho")]
        public IHttpActionResult GetTonTheoKho(string ma_vt, string ma_kho, string ma_dvt)
        {
            try
            {
                if (string.IsNullOrEmpty(ma_vt) || string.IsNullOrEmpty(ma_kho) || string.IsNullOrEmpty(ma_dvt))
                {
                    return Ok(0); // Trả về 0 nếu thiếu dữ liệu
                }

                var tonKho = db.Database.SqlQuery<decimal?>(@"
            SELECT so_luong_ton 
            FROM tonkho 
            WHERE ma_vt = @p0 AND ma_kho = @p1 AND ma_dvt = @p2
            LIMIT 1
        ", ma_vt, ma_kho, ma_dvt).FirstOrDefault();

                return Ok(tonKho ?? 0); // Nếu null thì trả về 0
            }
            catch (Exception ex)
            {
                
                return Ok(0); // Trong trường hợp lỗi cũng trả về 0
            }
        }
        [HttpGet]
        [Route("api/PhieuXuatAPI/ExportPdf_iText")]
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

                // 2. Row: "PHIẾU Xuất KHO" chính giữa; QR code bên phải; khoảng trống bên trái
                Table headerTable = new Table(new float[] { 5, 3, 1 }); // cột trái = cột phải, cột giữa lớn hơn
                headerTable.SetWidth(iText.Layout.Properties.UnitValue.CreatePercentValue(100));

                // Cột trái: trống
                Cell leftEmptyCell = new Cell().SetBorder(iText.Layout.Borders.Border.NO_BORDER);
                headerTable.AddCell(leftEmptyCell);

                // Cột giữa: tiêu đề căn giữa cả ngang và dọc
                Cell titleCell = new Cell()
                    .Add(new Paragraph("PHIẾU XUẤT KHO")
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

                // Lấy dữ liệu phiếu xuất từ database
                var phieu = db.PhieuXuatObj.FirstOrDefault(p => p.so_ct == so_ct);
                var ncc = db.DmnccObj.FirstOrDefault(n => n.ma_ncc == phieu.ma_kh);

                string thongTinPhieu = $"Số phiếu: {phieu.so_ct}\n" +
                                       $"Ngày chứng từ: {phieu.ngay_ct:dd/MM/yyyy}\n" +
                                       $"Khách hàng: {phieu.ma_kh} - {ncc?.ten_ncc}\n" +
                                       $"Địa chỉ: {ncc?.dia_chi}\n" +
                                       $"Mã số thuế: {phieu.ma_so_thue}";

                Paragraph pThongTinPhieu = new Paragraph(thongTinPhieu).SetFontSize(10);
                document.Add(pThongTinPhieu);

                // TODO: Thêm phần chi tiết vật tư, tổng hợp, chữ ký...
                var result_ct = from ctpn in db.ChiTietPhieuXuatObj
                                join dm in db.DmvtObj on ctpn.ma_vt equals dm.ma_vt
                                join pn in db.PhieuXuatObj on new { ctpn.so_ct, ctpn.ma_kh } equals new { pn.so_ct, pn.ma_kh }
                                where ctpn.so_ct == so_ct
                                select new
                                {
                                    ctpn.ma_vt,
                                    dm.ten_vt,
                                    ctpn.so_luong_xuat,
                                    ctpn.don_gia_xuat,
                                    ctpn.ma_dvt,
                                    ctpn.ghi_chu,
                                    pn.tong_thanh_toan,
                                    pn.tong_chiet_khau,
                                    pn.tong_thue
                                };

                var chiTietVatTu = result_ct.ToList();

                // Tạo bảng chi tiết vật tư với 6 cột
                float[] columnWidths = { 1, 2, 5, 2, 3, 2, 4, 2 };
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
                    decimal thanhTien = item.so_luong_xuat * item.don_gia_xuat;
                    tongThanhTien += thanhTien;
                    detailsTable.AddCell(new Cell().Add(new Paragraph(stt.ToString())).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER));
                    detailsTable.AddCell(new Cell().Add(new Paragraph(item.ma_vt)));
                    detailsTable.AddCell(new Cell().Add(new Paragraph(item.ten_vt ?? "")));
                    detailsTable.AddCell(new Cell().Add(new Paragraph(item.so_luong_xuat.ToString() ?? "0")).SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT));
                    detailsTable.AddCell(new Cell().Add(new Paragraph(string.Format("{0:N2}", item.don_gia_xuat))).SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT));
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
                document.Add(new Paragraph("\nChi tiết vật tư xuất kho:").SetFont(fontBold).SetFontSize(12));
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
                    FileName = $"PhieuXuat_{so_ct}.pdf"
                };
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

                return result;
            }
        }
        [HttpPost]
        [Route("api/PhieuXuatAPI/CapNhatTrangThai")]
        public IHttpActionResult CapNhatTrangThai(string so_ct, int trang_thai)
        {
            try
            {
                var phieu = db.PhieuXuatObj.SingleOrDefault(p => p.so_ct == so_ct);
                if (phieu == null)
                    return NotFound();

                phieu.trang_thai = trang_thai;
                db.SaveChanges();

                return Ok(new { success = true, message = "Cập nhật trạng thái thành công." });
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception("Lỗi khi cập nhật trạng thái: " + ex.Message));
            }
        }
        [HttpPost]
        [Route("api/PhieuXuatAPI/XacNhanPhieuXuat")]
        public IHttpActionResult XacNhanPhieuNhap(string so_ct)
        {
            try
            {
                // Gọi hàm PostgreSQL process_phieu_nhap
                var query = "SELECT process_phieu_xuat(@p0)";
                db.Database.ExecuteSqlCommand(query, so_ct);

                return Ok(new { success = true, message = "Xác nhận phiếu xuất thành công." });
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception("Đã xảy ra lỗi khi xác nhận phiếu nhập: " + ex.Message));
            }
        }
        [HttpGet]
        [Route("api/LOG7API/GetAuditLogByTable")]
        public IHttpActionResult GetAuditLogByTable()
        {

            var query = @"
        SELECT 
            a.id AS id,
            a.table_name AS table_name,
            a.operation AS operation,
            a.primary_key_data AS primary_key_data,
            a.old_data AS old_data,
            a.new_data AS new_data,
            a.changed_by AS changed_by,
            a.changed_at AS changed_at,
            b.username AS username
        FROM audit_log a
        LEFT JOIN userinfo b ON a.changed_by = b.id
        WHERE a.table_name = 'phieu_xuat'
        ORDER BY a.changed_at DESC;
    ";

            var result = db.Database.SqlQuery<AuditLogView>(query).ToList();

            return Ok(result);
        }
        [HttpGet]
        [Route("api/LOG7API/GetAuditLogByTableCT")]
        public IHttpActionResult GetAuditLogByTableCT()
        {

            var query = @"
        SELECT 
            a.id AS id,
            a.table_name AS table_name,
            a.operation AS operation,
            a.primary_key_data AS primary_key_data,
            a.old_data AS old_data,
            a.new_data AS new_data,
            a.changed_by AS changed_by,
            a.changed_at AS changed_at,
            b.username AS username
        FROM audit_log a
        LEFT JOIN userinfo b ON a.changed_by = b.id
        WHERE a.table_name = 'ct_phieu_xuat'
        ORDER BY a.changed_at DESC;
    ";

            var result = db.Database.SqlQuery<AuditLogView>(query).ToList();

            return Ok(result);
        }



    }
}
