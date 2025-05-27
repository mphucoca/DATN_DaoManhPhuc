var app = angular.module('DMKhachHangApp', []);

app.controller('DMKhachHangController', function ($scope, $http) {
    // Phần 1 khởi tạo các biến cần thiết
    //<<<<<<<<<<<<<<<<<<<< Lấy ra các thông tin đăng nhập của người sử dụng -- mặc đinh
    $scope.username = '';
    $scope.role = '';

    $http.get('/api/UserInfoAPI/GetSessionInfo')
        .then(function (response) {
            if (response.data.success) {
                $scope.username = response.data.username;
                $scope.role = response.data.role;
            }
        }, function (error) {

        })
    //>>>>>>>>>>>>>>>>>>>>>>Lấy ra các thông tin đăng nhập của người sử dụng -- mặc đinh
    ///////////////////// KHAI BÁO CÁC MẶC ĐỊNH <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
    // Khởi tạo danh sách và các biến cho phiếu
    $scope.ds_main = [];

    $scope.OtherMode = false;  // khi thêm hoặc sẽ mở ra một màn hình khác
    $scope.showConfirmModal = function (message, callback) {
        $scope.confirmMessage = message;  // Cập nhật nội dung thông báo
        $scope.confirmCallback = callback;  // Lưu callback
        $scope.isConfirmModalOpen = true;  // Mở modal
    };
    // Biến lưu trữ trạng thái chỉnh sửa hoặc thêm
    $scope.edit_mode = false;
    $scope.add_mode = false;
    $scope.closeConfirmModal = function () {
        $scope.isConfirmModalOpen = false;  // Đóng modal
    };

    $scope.confirmAction = function () {
        if ($scope.confirmCallback) {
            $scope.confirmCallback();  // Thực hiện hành động sau khi xác nhận
        }
        $scope.isConfirmModalOpen = false;  // Đóng modal
    };
    //<<<<<<<Modal thêm mới chỉnh sửa



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

        // khởi tạo các giá trị mặc định trước khi thêm
        $scope.new_item = { 
            trangthai: 1
        };
    }
    $scope.Edit = function (item) {
        $scope.OtherMode = true;
        // lưu trữ biến cho biết người dùng đang sửa 
        $scope.edit_mode = true;
        $scope.add_mode = false;
        if (item != null || item != undefined) $scope.new_item = angular.copy(item);
  
    }
    ///////////////////// KHAI BÁO CÁC MẶC ĐỊNH >>>>>>>>>>>>>>>>>>>>>




    // Phần 2 các function chức năng của hệ thống
    // function lấy ra thông tin danh sách chính
    $scope.GET = function () {
        $http.get('/api/DMKhachHangAPI')
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
            alert('Chưa chọn khách hàng nào để xóa');
            return;
        }

        // Hiển thị modal xác nhận
        $scope.showConfirmModal('Bạn có chắc chắn muốn xóa các khách hàng đã chọn?', function () {
            // Nếu người dùng đồng ý, tiến hành xóa các phiếu
            $http.post('/api/DMKhachHangAPI/DeleteAll', ds_checked)
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
        $scope.showConfirmModal('Bạn có chắc chắn muốn xóa khách hàng này?', function () {
            // Nếu người dùng đồng ý, tiến hành xóa các phiếu
            $http.post('/api/DMKhachHangAPI/DeleteAll', ds_checked)
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
                ma_kh: null,
                ten_kh: null,
                dien_thoai: null,
                dia_chi: null,
                mo_ta: null,
                ma_so_thue: null
            };
        } else {
            _data = {
                keyword: $scope.search.keyword,
                ma_kh: $scope.search.ma_kh,
                ten_kh: $scope.search.ten_kh,
                dien_thoai: $scope.search.dien_thoai,
                dia_chi: $scope.search.dia_chi,
                mo_ta: $scope.search.mo_ta,
                ma_so_thue: $scope.search.ma_so_thue

            };

        }
        $http.post('/api/DMKhachHangAPI/SEARCH', _data)
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
            $http.post('/api/DMKhachHangAPI/SaveAdd', _data)
                .then(function (response) {
                    if (response.data.success) {
                        $scope.new_item = response.data.result;
                        $scope.edit_mode = false;
                        $scope.add_mode = false;
                    } else {
                        alert('Tên người dùng đã tồn tại');
                    }
                }).catch(function (error) {
                    alert('Đã có lỗi xảy ra liên hệ với quản trị viên'); 
                });
        } else if ($scope.edit_mode) {
            // xử lý khi sửa bản ghi
            // xử lý khi thêm mới bản ghi
            $http.put('/api/DMKhachHangAPI/SaveEdit', _data)
                .then(function (response) {
                    if (response.data.success) {
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
    }
    // phần lịch sử chỉnh sửa
    // phần lịch sử chỉnh sửa
    $scope.isHistoryModalOpen = false;
    $scope.historyLogs = [];
    $scope.selectedRecord = null;

    function isSameJson(jsonStr1, jsonStr2) {
        try {
            let obj1 = JSON.parse(jsonStr1);
            let obj2 = JSON.parse(jsonStr2);
            return angular.equals(obj1, obj2);
        } catch (e) {
            return false;
        }
    }

    $scope.ShowLichSu = function (item) {
        $scope.new_item = null;

        // Gọi API lấy lịch sử log
        $http.get('/api/LOG1API/GetAuditLogByTable/')
            .then(function (response) {
                // Lọc bỏ các bản ghi UPDATE mà old_data và new_data giống nhau
                $scope.historyLogs = response.data.filter(function (log) {
                    if (log.operation === 'UPDATE') {
                        return !isSameJson(log.old_data, log.new_data);
                    }
                    return true;
                });
                $scope.isHistoryModalOpen = true;
            }, function (error) {

            });
    };
    $scope.ShowLichSuCT = function (item) {
        $http.get('/api/LOG1API/GetAuditLogByTableCT/')
            .then(function (response) {
                $scope.historyLogs = response.data.filter(function (log) {
                    try {
                        // Parse primary_key_data để lấy ma_kho
                        const primaryKeyObj = JSON.parse(log.primary_key_data || '{}');
                        const logMaKho = primaryKeyObj.ma_kh || null;

                        // So sánh ma_kho
                        const isSameMaKho = logMaKho === item.ma_kh;

                        if (!isSameMaKho) return false;

                        // Nếu là UPDATE, loại bỏ nếu old_data và new_data giống nhau
                        if (log.operation === 'UPDATE') {
                            return !isSameJson(log.old_data, log.new_data);
                        }

                        return true;
                    } catch (e) {

                        return false;
                    }
                });

                $scope.isHistoryModalOpen = true;
            }, function (error) {
            });
    };


    $scope.closeHistoryModal = function () {
        $scope.isHistoryModalOpen = false;
        $scope.historyLogs = [];
        $scope.selectedRecord = null;
    };

    $scope.parseJson = function (jsonString) {
        try {
            return JSON.parse(jsonString || "{}");
        } catch (e) {
            return {};
        }
    };

    $scope.getDiffClass = function (oldValue, newValue) {
        if (oldValue !== newValue) {
            return "table-warning";
        }
        return "";
    };

    // phần lịch sử chỉnh sửa
});
