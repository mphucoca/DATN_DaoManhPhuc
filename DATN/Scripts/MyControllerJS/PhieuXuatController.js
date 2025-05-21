var app = angular.module('PhieuXuatApp', []);

app.controller('PhieuXuatController', function ($scope, $http) {
    // Phần 1 khởi tạo các biến cần thiết
     ///////////////////// KHAI BÁO CÁC MẶC ĐỊNH <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
    // Khởi tạo danh sách và các biến cho phiếu
    $scope.ds_main = [];
    $scope.ds_ct_phieu = [];
    $scope.OtherMode = false;  // khi thêm hoặc sẽ mở ra một màn hình khác
    $scope.showConfirmModal = function (message, callback) {
        $scope.confirmMessage = message;  // Cập nhật nội dung thông báo
        $scope.confirmCallback = callback;  // Lưu callback
        $scope.isConfirmModalOpen = true;  // Mở modal
    };
    $scope.sum = {};
    $scope.closeConfirmModal = function () {
        $scope.isConfirmModalOpen = false;  // Đóng modal
    };
    // Biến lưu trữ trạng thái chỉnh sửa hoặc thêm
    $scope.edit_mode = false;
    $scope.add_mode = false;
    $scope.confirmAction = function () {
        if ($scope.confirmCallback) {
            $scope.confirmCallback();  // Thực hiện hành động sau khi xác nhận
        }
        $scope.isConfirmModalOpen = false;  // Đóng modal
    };
    //<<<<<<<Modal thêm mới chỉnh sửa

    $scope.ds_trangthai = [
        { ma_trangthai: 0, ten_trangthai: 'Chờ duyệt' },
        { ma_trangthai: 1, ten_trangthai: 'Đã duyệt' },
        { ma_trangthai: 2, ten_trangthai: 'Đã nhập kho' },
        { ma_trangthai: 3, ten_trangthai: 'Đã hủy' }
    ];

    $scope.ds_trangthai = [
        { ma_trangthai: 0, ten_trangthai: 'Chờ duyệt' },
        { ma_trangthai: 1, ten_trangthai: 'Đã duyệt' },
        { ma_trangthai: 2, ten_trangthai: 'Đã nhập kho' },
        { ma_trangthai: 3, ten_trangthai: 'Đã hủy' }
    ];
    $scope.ds_tt_thanh_toan = [
        { ma: 0, ten: 'Chọn giá trị ' },
        { ma: 1, ten: 'Thanh toán ngay' },
        { ma: 2, ten: 'Thanh toán sau 7 ngày' },
        { ma: 3, ten: 'Thanh toán sau 15 ngày' },
        { ma: 4, ten: 'Thanh toán sau 30 ngày' },
        { ma: 5, ten: 'Chuyển khoản ngay' },
        { ma: 6, ten: 'Chuyển khoản sau nhận hàng' },
        { ma: 7, ten: 'Thanh toán theo đợt 1' },
        { ma: 8, ten: 'Thanh toán theo đợt 2' },
        { ma: 9, ten: 'Trả góp theo kỳ' },
        { ma: 10, ten: 'Tạm ứng trước 30%' },
        { ma: 11, ten: 'Tạm ứng trước 50%' },
        { ma: 12, ten: 'Công nợ cuối tháng' },
        { ma: 13, ten: 'Chi hộ tiền mặt' },
        { ma: 14, ten: 'COD' },
        { ma: 15, ten: 'Thu hộ bên thứ ba' },
        { ma: 16, ten: 'Thu ngay khi giao hàng' },
        { ma: 17, ten: 'Thanh toán bằng tiền mặt' },
        { ma: 18, ten: 'Thanh toán bằng thẻ ngân hàng' },
        { ma: 19, ten: 'Tạm giữ tiền hàng' },
        { ma: 20, ten: 'Thu phí trước, thanh toán sau' },
        { ma: 21, ten: 'Thanh toán qua ví điện tử' },
        { ma: 22, ten: 'Thanh toán qua PayPal' },
        { ma: 23, ten: 'Chuyển khoản quốc tế' },
        { ma: 24, ten: 'Hợp đồng tín dụng' },
        { ma: 25, ten: 'Chi trả định kỳ' },
        { ma: 26, ten: 'Công nợ 60 ngày' },
        { ma: 27, ten: 'Đổi hàng' },
        { ma: 28, ten: 'Thanh toán khấu trừ' },
        { ma: 29, ten: 'Miễn phí, không thanh toán' },
        { ma: 30, ten: 'Tùy theo hợp đồng' }
    ];
    $scope.ds_chiet_khau = [
        { ma: 0, ten: 'Chọn giá trị ', gia_tri: 0 },
        { ma: 1, ten: 'Không chiết khấu', gia_tri: 0 },
        { ma: 2, ten: 'Chiết khấu 1%', gia_tri: 1 },
        { ma: 3, ten: 'Chiết khấu 2%', gia_tri: 2 },
        { ma: 4, ten: 'Chiết khấu 3%', gia_tri: 3 },
        { ma: 5, ten: 'Chiết khấu 4%', gia_tri: 4 },
        { ma: 6, ten: 'Chiết khấu 5%', gia_tri: 5 },
        { ma: 7, ten: 'Chiết khấu 6%', gia_tri: 6 },
        { ma: 8, ten: 'Chiết khấu 7%', gia_tri: 7 },
        { ma: 9, ten: 'Chiết khấu 8%', gia_tri: 8 },
        { ma: 10, ten: 'Chiết khấu 9%', gia_tri: 9 },
        { ma: 11, ten: 'Chiết khấu 10%', gia_tri: 10 },
        { ma: 12, ten: 'Khuyến mãi hàng tồn', gia_tri: 5 },
        { ma: 13, ten: 'Chiết khấu thanh toán sớm', gia_tri: 3 },
        { ma: 14, ten: 'Chiết khấu khách hàng VIP', gia_tri: 7 },
        { ma: 15, ten: 'Chiết khấu đại lý cấp 1', gia_tri: 8 },
        { ma: 16, ten: 'Chiết khấu đại lý cấp 2', gia_tri: 6 },
        { ma: 17, ten: 'Chiết khấu theo số lượng lớn', gia_tri: 10 },
        { ma: 18, ten: 'Chiết khấu hợp đồng năm', gia_tri: 12 },
        { ma: 19, ten: 'Chiết khấu hàng khuyến mãi', gia_tri: 15 },
        { ma: 20, ten: 'Chiết khấu sinh nhật khách hàng', gia_tri: 5 },
        { ma: 21, ten: 'Chiết khấu mùa lễ', gia_tri: 8 },
        { ma: 22, ten: 'Chiết khấu hoàn tiền', gia_tri: 4 },
        { ma: 23, ten: 'Chiết khấu đổi hàng cũ', gia_tri: 6 },
        { ma: 24, ten: 'Chiết khấu nhóm sản phẩm A', gia_tri: 5 },
        { ma: 25, ten: 'Chiết khấu nhóm sản phẩm B', gia_tri: 7 },
        { ma: 26, ten: 'Chiết khấu thanh lý kho', gia_tri: 20 },
        { ma: 27, ten: 'Chiết khấu tri ân khách hàng', gia_tri: 10 },
        { ma: 28, ten: 'Chiết khấu theo kỳ', gia_tri: 2 },
        { ma: 29, ten: 'Chiết khấu tích lũy', gia_tri: 9 },
        { ma: 30, ten: 'Chiết khấu không cố định', gia_tri: 0 }
    ];
    $scope.ds_thue = [
        { ma: 0, ten: 'Chọn giá trị', gia_tri: 0 },
        { ma: 1, ten: 'Không chịu thuế', gia_tri: 0 },
        { ma: 2, ten: 'VAT 5%', gia_tri: 5 },
        { ma: 3, ten: 'VAT 10%', gia_tri: 10 },
        { ma: 4, ten: 'Thuế xuất khẩu 0%', gia_tri: 0 },
        { ma: 5, ten: 'Thuế nhập khẩu 5%', gia_tri: 5 },
        { ma: 6, ten: 'Thuế nhập khẩu 10%', gia_tri: 10 },
        { ma: 7, ten: 'Thuế tiêu thụ đặc biệt 15%', gia_tri: 15 },
        { ma: 8, ten: 'Thuế bảo vệ môi trường 3%', gia_tri: 3 },
        { ma: 9, ten: 'Thuế tài nguyên 8%', gia_tri: 8 },
        { ma: 10, ten: 'VAT hàng miễn thuế', gia_tri: 0 },
        { ma: 11, ten: 'Thuế nhập khẩu ưu đãi 3%', gia_tri: 3 },
        { ma: 12, ten: 'VAT sản phẩm nông nghiệp', gia_tri: 5 },
        { ma: 13, ten: 'VAT sản phẩm công nghiệp', gia_tri: 10 },
        { ma: 14, ten: 'Thuế dịch vụ vận chuyển', gia_tri: 5 },
        { ma: 15, ten: 'Thuế máy móc nhập khẩu', gia_tri: 7 },
        { ma: 16, ten: 'Thuế phụ tùng cơ khí', gia_tri: 6 },
        { ma: 17, ten: 'Thuế hàng tiêu dùng cao cấp', gia_tri: 20 },
        { ma: 18, ten: 'Thuế hàng hóa dễ cháy', gia_tri: 12 },
        { ma: 19, ten: 'Thuế sản phẩm tái chế', gia_tri: 4 },
        { ma: 20, ten: 'Thuế dịch vụ hỗ trợ', gia_tri: 5 },
        { ma: 21, ten: 'VAT hóa chất', gia_tri: 8 },
        { ma: 22, ten: 'Thuế phụ phí vận tải', gia_tri: 2 },
        { ma: 23, ten: 'Thuế bảo hiểm hàng hóa', gia_tri: 1 },
        { ma: 24, ten: 'Thuế hàng nhập kho ngoại quan', gia_tri: 0 },
        { ma: 25, ten: 'Thuế hàng tạm nhập tái xuất', gia_tri: 0 },
        { ma: 26, ten: 'Thuế hàng viện trợ', gia_tri: 0 },
        { ma: 27, ten: 'Thuế tái nhập khẩu', gia_tri: 5 },
        { ma: 28, ten: 'Thuế linh kiện điện tử', gia_tri: 6 },
        { ma: 29, ten: 'Thuế hàng hải sản', gia_tri: 5 },
        { ma: 30, ten: 'Thuế hàng thủ công mỹ nghệ', gia_tri: 5 }
    ];
    //>> Modal thêm mới chỉnh sửa
    // Biến tìm kiếm mở rộng
    $scope.isExpandSearch = false;
    // Biến lưu trữ dữ liệu người dùng nhập vào tìm kiếm
    $scope.search = [];
    // function để show mà hình tìm kiếm mở rộng
    $scope.ExpandSearch = function () {
        $scope.isExpandSearch = !$scope.isExpandSearch;
        var today = new Date(); 
        // Ngày đầu tháng
        var firstDay = new Date(today.getFullYear(), today.getMonth(), 1); 
        // Ngày cuối tháng
        var lastDay = new Date(today.getFullYear(), today.getMonth() + 1, 0); 
        $scope.search.tu_ngay = firstDay;
        $scope.search.den_ngay = lastDay;
    }
    $scope.showAddModal = function () {
        $scope.edit_mode = false;
        $scope.add_mode = true;
        $scope.OtherMode = true;  // khi thêm sẽ mở ra một màn hình khác
        $scope.LoadComboBox();
       
        $scope.new_item = { 
            trang_thai: 0,
            ma_kh: $scope.ds_dmkh[0].ma_kh,
            ngay_ct: new Date(),
            thue: 0,
            chietkhau: 0,
            tt_thanhtoan: 0,
        };
        // khởi tạo các giá trị mặc định 
        $http.get('/api/phieuxuat/tao_soct').then(function (response) {
            $scope.new_item.so_ct = response.data.so_ct;
        });
        $scope.getMaSoThueByMaKH($scope.new_item.ma_kh);
        // khởi tạo danh sách chi tiết phiếu
        $scope.ds_ct_phieu = [];
        $scope.current_item = {};
     

    }
    $scope.Edit = function (item) {
        $scope.OtherMode = true;
        // lưu trữ biến cho biết người dùng đang sửa 
        $scope.edit_mode = true;
        $scope.add_mode = false;
    if(item != null || item != undefined) $scope.new_item = angular.copy(item);
        $scope.LoadComboBox();
        $scope.GET_CT($scope.new_item.so_ct);
    }
    ///////////////////// KHAI BÁO CÁC MẶC ĐỊNH >>>>>>>>>>>>>>>>>>>>>




    // Phần 2 các function chức năng của hệ thống
    // function lấy ra thông tin danh sách chính
    $scope.GET = function () {
        $http.get('/api/PhieuXuatAPI')
            .then(function (response) {
                $scope.ds_main = response.data;
            })
            .catch(function (error) {
                alert('Đã có lỗi xảy ra, liên hệ với quản trị viên');
            });
    };

 
    $scope.DELETE_All_checked = function () {
        var ds_checked = $scope.ds_main.filter(function (item) {
            return item.checked == true;
        });

        // Kiểm tra xem có dòng nào được chọn không
        if (ds_checked.length === 0) {
            alert('Chưa chọn phiếu nào để xóa');
            return;
        }

        // Hiển thị modal xác nhận
        $scope.showConfirmModal('Bạn có chắc chắn muốn xóa các phiếu đã chọn?', function () {
            // Nếu người dùng đồng ý, tiến hành xóa các phiếu
            $http.post('/api/PhieuXuatAPI/DeleteAll', ds_checked)
                .then(function (response) {
                    $scope.GET();  // reload danh sách
                })
                .catch(function (error) {
                    alert('Đã có lỗi xảy ra, liên hệ với quản trị viên');
                });
        });
    };
    $scope.DELETE = function (item) {
        var ds_checked = [];
        ds_checked.push(item);
        // Hiển thị modal xác nhận
        $scope.showConfirmModal('Bạn có chắc chắn muốn xóa phiếu này?', function () {
            // Nếu người dùng đồng ý, tiến hành xóa các phiếu
            $http.post('/api/PhieuXuatAPI/DeleteAll', ds_checked)
                .then(function (response) {
                    $scope.GET();  // reload danh sách
                })
                .catch(function (error) {
                    alert('Đã có lỗi xảy ra, liên hệ với quản trị viên');
                });
        });
    };

    // Gọi lấy danh sách ngay lần đầu
    $scope.GET();
  // Phần 3 các function cơ bản của hệ thống
    // function cho checkbox mỗi dòng
    $scope.click = function (item) {
        item.checked = !item.checked;
    };
    // function chọn nhiều dòng 
    $scope.clickall = function () {
        angular.forEach($scope.ds_main, function (item) {
            item.checked = !$scope.clickallcheckbox;
        });
    };
    // Thoát khỏi chế độ thêm hoặc sửa 
    $scope.CloseOtherMode = function () {
        $scope.showConfirmModal('Bạn có chắc chắn muốn thoát không?', function () {
            // nếu người dùng đồng ý
            $scope.OtherMode = false;
             $scope.SEARCH($scope.isExpandSearch);
        });
    }
    // reload lại phần dữ liệu tìm kiếm
    $scope.Refresh = function () {
        $scope.SEARCH($scope.isExpandSearch);
    }
    $scope.SEARCH = function (isExpandSearch) {
        var _data = {};
        if (!isExpandSearch) {
            _data = {
                keyword: $scope.search.keyword,
                so_ct: null,
                ma_kh: null,
                tu_ngay: null,
                den_ngay: null
            };
        } else {
            _data = {
                keyword: $scope.search.keyword,
                so_ct: $scope.search.so_ct,
                ma_kh: $scope.search.ma_kh,
                tu_ngay: $scope.search.tu_ngay,
                den_ngay: $scope.search.den_ngay

            };

        }
        $http.post('/api/PhieuXuatAPI/SEARCH', _data)
            .then(function (response) {
                if (response.data.success) {
                    $scope.ds_main = response.data.result;

                }
            }).catch(function (error) {
                alert('Không tồn tại bản ghi phù hợp');
                $scope.ds_main = {};
            });
      
    }
    $scope.Save = function () {
        var _data = angular.copy($scope.new_item);

        if ($scope.add_mode) {
            // xử lý khi thêm mới bản ghi
            $http.post('/api/PhieuXuatAPI/SaveAdd', _data)
                .then(function (response) {
                    if (response.data.success) {
                        $scope.SaveAll();
                        $scope.new_item = response.data.result;
                        $scope.edit_mode = false;
                        $scope.add_mode = false;
                    } else {
                        console.log(response.data.message);
                        alert(response.data.message);
                    }
                }).catch(function (error) {
                    alert('Đã có lỗi xảy ra liên hệ với quản trị viên');
                });
        } else if ($scope.edit_mode) {
            // xử lý khi sửa bản ghi
            // xử lý khi thêm mới bản ghi
            $http.put('/api/PhieuXuatAPI/SaveEdit', _data)
                .then(function (response) {
                    if (response.data.success) {
                        $scope.SaveAll();
                        alert('Cập nhật dữ liệu thành công');
                        $scope.new_item = response.data.result;
                        $scope.edit_mode = false;
                        $scope.add_mode = false;

                    } else {
                        alert('Dữ liệu không còn tồn tại');
                    }
                }).catch(function (error) {
                    alert('Đã có lỗi xảy ra liên hệ với quản trị viên');
                });

        } else {
            alert("Đã có lỗi xảy ra, liên hệ với quản trị viên");
        }
    }
    // click vào dòng trong danh sách chính
    $scope.Detail = function (item) {
        $scope.new_item = angular.copy(item);
        $scope.edit_mode = false;
        $scope.add_mode = false;
        $scope.OtherMode = true;
        $scope.LoadComboBox();
        if (!$scope.new_item.ngay_ct) $scope.new_item.ngay_ct = new Date();
        $scope.GET_CT($scope.new_item.so_ct);
    }



    //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< xử lý grid chi tiếtt
    /////////////////////////////////////////////////////////////////////////////////
    // Phần code cuối sẽ xử lý các phần giao diện đặc thù của từng chức năng
    //
    // Biến trạng thái sửa thêm chi tiết
    $scope.edit_mode_current_item = false;
 
 
 
    // Sửa dòng chi tiết
    $scope.Edit_current_item = function (item) {
        if (!$scope.edit_mode && !$scope.add_mode) return;
        $scope.current_item = angular.copy(item);
     
        $scope.edit_mode_current_item = true;
        $scope.backup_edit_item = angular.copy(item);
        $scope.GetDVTByMaVt($scope.current_item.ma_vt);
    };

    $scope.Save_current_item = function () {
        if (!$scope.current_item || !$scope.current_item.ma_vt || !$scope.current_item.ma_dvt || !$scope.current_item.ma_kho) {
            alert("Vui lòng nhập đầy đủ mã vật tư, mã đơn vị tính và mã kho.");
            return;
        }
        if (parseFloat($scope.current_item.so_luong_xuat) > parseFloat($scope.current_item.ton)) {
            alert("Số lượng xuất vượt tồn kho.");
            return;
        }
        $scope.current_item.so_ct = $scope.new_item.so_ct;
        $scope.current_item.ma_kh = $scope.new_item.ma_kh;
        // Tìm index theo khóa chính mới
        var newKeyIndex = $scope.ds_ct_phieu.findIndex(function (item) {
            return item.ma_vt === $scope.current_item.ma_vt &&
                item.ma_dvt === $scope.current_item.ma_dvt &&
                item.ma_kho === $scope.current_item.ma_kho;
        });

        if ($scope.edit_mode_current_item) {
            // Tìm index theo khóa cũ để xác định đúng dòng đang sửa
            var oldKeyIndex = $scope.ds_ct_phieu.findIndex(function (item) {
                return item.ma_vt === $scope.backup_edit_item.ma_vt &&
                    item.ma_dvt === $scope.backup_edit_item.ma_dvt &&
                    item.ma_kho === $scope.backup_edit_item.ma_kho;
            });

            // Nếu thay đổi khóa và đụng dòng khác thì mới báo trùng
            if (newKeyIndex !== -1 && newKeyIndex !== oldKeyIndex) {
                alert("Bản ghi với cùng mã vật tư, mã đơn vị tính và mã kho đã tồn tại.");
                return;
            }

            // Cập nhật lại dòng cũ
            if (oldKeyIndex !== -1) {
                $scope.ds_ct_phieu[oldKeyIndex] = angular.copy($scope.current_item);
            } else {
                // Nếu không tìm thấy dòng cũ, thêm mới
                $scope.ds_ct_phieu.push(angular.copy($scope.current_item));
            }
        } else {
            // Trường hợp thêm mới
            if (newKeyIndex !== -1) {
                alert("Bản ghi với cùng mã vật tư, mã đơn vị tính và mã kho đã tồn tại.");
                return;
            }
            $scope.ds_ct_phieu.push(angular.copy($scope.current_item));
        }

        // Reset trạng thái sau khi lưu
        $scope.current_item = null;
        $scope.edit_mode_current_item = false;
        $scope.backup_edit_item = null;
        $scope.UpdateTotal();
    };



 
    // clear nội dung phần thêm dòng
    $scope.clearContent = function () {
        $scope.current_item = null;
        $scope.edit_mode_current_item = false;
        $scope.backup_edit_item = null;

    }
    // xóa dòng chi tiết
    // xóa dòng chi tiết
    $scope.DELETE_current_item = function (item) {
        if (!$scope.edit_mode && !$scope.add_mode) return;
        if (!item) {
            alert("Không tìm thấy bản ghi để xóa");
            return;
        }

        // Hiển thị modal xác nhận
        $scope.showConfirmModal('Bạn có chắc chắn muốn xóa bản ghi này?', function () {
            // Tìm vị trí item trong ds_dmqddvt
            var index = $scope.ds_ct_phieu.findIndex(function (x) {
                return x.ma_vt === item.ma_vt && x.ma_dvt === item.ma_dvt && x.ma_kho === item.ma_kho;
            });

            if (index !== -1) {
                $scope.ds_ct_phieu.splice(index, 1);
                $scope.UpdateTotal();
                 
            } else {

            }
        });

    };
 
    // Update item
    $scope.change_value_xuat = function () {
        $scope.current_item.thanh_tien = parseFloat($scope.current_item.so_luong_xuat) * parseFloat($scope.current_item.don_gia_xuat);
    }
 
    $scope.GET_dmkh = function () {
        $http.get('/api/DMKhachHangAPI')
            .then(function (response) {
                $scope.ds_dmkh = response.data;
            })
            .catch(function (error) {
                alert('Đã có lỗi xảy ra, liên hệ với quản trị viên');
            });
    };
    $scope.GET_CT = function (so_ct) {
        $http.get('/api/PhieuXuatAPI/GET_CT', { params: { so_ct: so_ct } })
            .then(function (response) {
                if (response.data && response.data.length > 0) {
                    $scope.ds_ct_phieu = response.data;
                } else {
                    $scope.ds_ct_phieu = [];
                   
                }
                $scope.UpdateTotal();
            })
            .catch(function (error) {
                console.error('Lỗi khi gọi API GET_CT:', error);
                alert('Đã có lỗi xảy ra, vui lòng liên hệ quản trị viên');
            });
    };

    // Lấy danh sách vật tư để thêm vào grid chi tiết
    $scope.GET_dmvt = function () {
        $http.get('/api/DmvtAPI')
            .then(function (response) {
                $scope.ds_dmvt = response.data;
            })
            .catch(function (error) {
                alert('Đã có lỗi xảy ra, liên hệ với quản trị viên');
            });
    };
    $scope.GetDVTByMaVt = function (ma_vt) {
        if (ma_vt  ) {
            $http.get('/api/PhieuNhapAPI/GetDVTByMaVt', { params: { ma_vt: ma_vt } })
                .then(function (response) {
                    $scope.ds_dmdvt = response.data;
                    $scope.GetTonTheoKho($scope.current_item.ma_vt, $scope.current_item.ma_kho, $scope.current_item.ma_dvt);
                }, function (error) {

                });
        }
    };
    $scope.GetTonTheoKho = function (ma_vt, ma_kho, ma_dvt) {
        if (ma_vt && ma_dvt && ma_kho) {
            $http.get('/api/PhieuXuatAPI/GetTonTheoKho', { params: { ma_vt: ma_vt, ma_kho: ma_kho, ma_dvt: ma_dvt } })
                .then(function (response) {
                    $scope.current_item.ton = response.data;
                }, function (error) {
                    $scope.current_item.ton = 0;
                });
        } else {
            $scope.current_item.ton = 0;
        }
    };
    // lấy danh mục kho
    $scope.GET_kho = function () {
        $http.get('/api/DmkhoAPI')
            .then(function (response) {
                $scope.ds_dmkho = response.data;
            })
            .catch(function (error) {
                alert('Đã có lỗi xảy ra, liên hệ với quản trị viên');
            });
    };
    // Hàm gọi API để xóa và thêm các bản ghi chi tiết phiếu nhập
    $scope.SaveAll = function () {
        // Kiểm tra xem ds_ct_phieu có dữ liệu không
        if (!$scope.ds_ct_phieu || $scope.ds_ct_phieu.length === 0) {
            alert("Danh sách chi tiết phiếu nhập trống.");
            return;
        }
        $scope.ds_ct_phieu.forEach(function (item) {
            item.so_ct = $scope.new_item.so_ct;
        });
        // Chuẩn bị dữ liệu để gửi lên API
        var data = $scope.ds_ct_phieu;  // Giả sử $scope.ds_ct_phieu là danh sách chi tiết phiếu nhập

        // Gọi API để xóa và thêm các bản ghi
        $http.post('/api/PhieuXuatAPI/SaveAll', data)
            .then(function (response) {
                if (response.data.success) {
                    // Thông báo thành công
                   // alert(response.data.message);
                    $scope.GET_CT($scope.new_item.so_ct);  // Gọi lại API để lấy chi tiết phiếu nhập mới
                } else {
                    // Thông báo lỗi nếu có
                 //   alert(response.data.message);
                }
            })
            .catch(function (error) {
                // Xử lý lỗi nếu có
             //   console.error('Đã có lỗi xảy ra:', error);
                alert('Đã có lỗi xảy ra, vui lòng thử lại.');
            });
    };
    // function updateTotal
    // function updateTotal
    $scope.UpdateTotal = function () {
        $scope.sum = {
            so_luong: 0,
            thanh_tien: 0,
            chiet_khau: 0,
            thue: 0,
            tong_thanh_toan: 0
        };

        // Tính tổng số lượng và thành tiền
        $scope.ds_ct_phieu.forEach(function (item) {
            const thanhTien = item.so_luong_xuat * item.don_gia_xuat;
            $scope.sum.so_luong += item.so_luong_xuat;
            $scope.sum.thanh_tien += thanhTien;
        });

        // Lấy phần trăm chiết khấu từ mã
        let pt_chiet_khau = 0;
        const obj_ck = $scope.ds_chiet_khau.find(x => x.ma === $scope.new_item.chietkhau);
        if (obj_ck) pt_chiet_khau = obj_ck.gia_tri || 0;

        $scope.sum.chiet_khau = $scope.sum.thanh_tien * pt_chiet_khau / 100;

        // Lấy phần trăm thuế từ mã
        let pt_thue = 0;
        const obj_thue = $scope.ds_thue.find(x => x.ma === $scope.new_item.thue);
        if (obj_thue) pt_thue = obj_thue.gia_tri || 0;

        $scope.sum.thue = ($scope.sum.thanh_tien - $scope.sum.chiet_khau) * pt_thue / 100;

        // Tổng thanh toán cuối cùng
        $scope.sum.tong_thanh_toan = $scope.sum.thanh_tien - $scope.sum.chiet_khau + $scope.sum.thue;
        $scope.new_item.tong_thanh_toan = angular.copy($scope.sum.tong_thanh_toan);
        $scope.new_item.tong_chiet_khau = angular.copy($scope.sum.chiet_khau);
        $scope.new_item.tong_thue = angular.copy($scope.sum.thue);
    };

    $scope.getMaSoThueByMaKH = function (ma_kh) {
        if (ma_kh) {
            $http.get('/api/DMKhachHangAPI/GetMaSoThue', { params: { ma_kh: ma_kh } })
                .then(function (response) {
                    if (response.data.success) {
                        $scope.new_item.ma_so_thue = response.data.result.ma_so_thue;

                    } else {
                        $scope.new_item.ma_so_thue = "";
                    }
                }, function (error) {
                    $scope.new_item.ma_so_thue = "";
                });
        }


    };
    $scope.LoadComboBox = function () {
        $scope.GET_dmkh();
        $scope.GET_dmvt();
        $scope.GET_kho();
    }
    
    $scope.LoadComboBox();
    $scope.PrintPhieuXuat = function (so_ct) {
        var url = '/api/PhieuXuatAPI/ExportPdf_iText?so_ct=' + encodeURIComponent(so_ct);

        // Mở tab mới
        var printWindow = window.open('', '_blank');

        // Gọi API để lấy dữ liệu PDF
        fetch(url)
            .then(response => response.blob())
            .then(blob => {
                var blobUrl = URL.createObjectURL(blob);

                // Gán nội dung iframe PDF và script tự động in
                printWindow.document.write(`
                <html>
                <head>
                    <title>In phiếu xuất</title>
                </head>
                <body style="margin:0">
                    <iframe src="${blobUrl}" type="application/pdf" style="width:100%; height:100vh;" id="pdfFrame"></iframe>
                    <script>
                        const frame = document.getElementById('pdfFrame');
                        frame.onload = function() {
                            frame.contentWindow.focus();
                            frame.contentWindow.print();
                        };
                    </script>
                </body>
                </html>
            `);
                printWindow.document.close();
            });
    };
});
