var app = angular.module('BCPhieuNhapApp', []);

app.controller('BCPhieuNhapController', function ($scope, $http) {
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
    }
 
  
    ///////////////////// KHAI BÁO CÁC MẶC ĐỊNH >>>>>>>>>>>>>>>>>>>>>
    // modal chọn chuỗi mã vật tư
    // Biến lưu trạng thái modal
    $scope.isSelectVTModalOpen = false;
    $scope.searchKeyword = '';  // Biến lưu từ khóa tìm kiếm

    // Hàm mở Modal chọn vật tư
    $scope.openSelectVTModal = function () {
        $scope.isSelectVTModalOpen = true;
        $scope.backup_ds_ma_vt = angular.copy($scope.ds_ma_vt);
    };

    // Hàm đóng Modal chọn vật tư
    $scope.closeSelectVTModal = function () {
        $scope.isSelectVTModalOpen = false;
        $scope.ds_ma_vt = angular.copy($scope.backup_ds_ma_vt);
        // Lọc các vật tư đã chọn và tạo chuỗi các mã vật tư ngăn cách nhau bởi dấu phẩy
        var selectedVTCodes = $scope.ds_ma_vt.filter(function (vt) {
            return vt.selected;  // Chỉ lấy các vật tư có 'selected' là true
        }).map(function (vt) {
            return vt.ma_vt;  // Chỉ lấy mã vật tư
        }).join(',');  // Ghép các mã vật tư thành chuỗi ngăn cách nhau bởi dấu phẩy

        // Gán chuỗi vào ma_vt_list
        $scope.search.ma_vt_list = selectedVTCodes;
    };
  
    // Hàm xác nhận lựa chọn vật tư
    $scope.confirmSelection = function () {
        // Lọc các vật tư đã chọn
        var selectedVTs = $scope.ds_ma_vt.filter(function (vt) {
            return vt.selected; // Lọc các vật tư có 'selected' là true
        });
        $scope.backup_ds_ma_vt = angular.copy($scope.ds_ma_vt);
        // Đóng Modal
        $scope.closeSelectVTModal();
    };
 
    // Hàm chọn/deselect tất cả các vật tư
    $scope.toggleSelectAll = function () {
        var isSelected = $scope.selectAll;
        $scope.ds_ma_vt.forEach(function (vt) {
            vt.selected = !isSelected;
        });
    };


    // modal chọn chuỗi MA
    // Biến lưu trạng thái modal
    $scope.isSelectKhoModalOpen = false;
    $scope.searchKeyword_kho = '';  // Biến lưu từ khóa tìm kiếm

    // Hàm mở Modal chọn kho
    $scope.openSelectKhoModal = function () {
   
        $scope.isSelectKhoModalOpen = true;
        $scope.backup_ds_ma_kho = angular.copy($scope.ds_ma_kho);
    };

    // Hàm đóng Modal chọn vật tư
    $scope.closeSelectKhoModal = function () {
        $scope.isSelectKhoModalOpen = false;
        $scope.ds_ma_kho = angular.copy($scope.backup_ds_ma_kho);
        // Lọc các kho đã chọn và tạo chuỗi các mã kho ngăn cách nhau bởi dấu phẩy
        var selectedKhoCodes = $scope.ds_ma_kho.filter(function (kho) {
            return kho.selected;  // Chỉ lấy các kho có 'selected' là true
        }).map(function (kho) {
            return kho.ma_kho;  // Chỉ lấy mã kho
        }).join(',');  // Ghép các mã kho thành chuỗi ngăn cách nhau bởi dấu phẩy

        // Gán chuỗi vào ma_kho_list
        $scope.search.ma_kho_list = selectedKhoCodes;

    };
    $scope.tich = function (kho) {
        kho.selected = !angular.copy(kho.selected);
    }
    // Hàm xác nhận lựa chọn vật tư
    $scope.confirmSelection_kho = function () {
        // Lọc các vật tư đã chọn
        var selectedKhos = $scope.ds_ma_kho.filter(function (kho) {
            return kho.selected; // Lọc các vật tư có 'selected' là true
        }); 
        $scope.backup_ds_ma_kho = angular.copy($scope.ds_ma_kho);
        // Đóng Modal
        $scope.closeSelectKhoModal();
    };
  

    // Hàm chọn/deselect tất cả các vật tư
    $scope.toggleSelectAll_kho = function () {
        var isSelected_kho = $scope.selectAll_kho;
        $scope.ds_ma_kho.forEach(function (kho) {
            kho.selected = !isSelected_kho;
        });
    };


    ///////////////
    //$http.get('/api/BCPhieuNhapAPI/GetPhieuNhap')
    //    .then(function (response) {
    //        $scope.ds_main = response.data;
            
    //    })
    //    .catch(function (error) {
    //        console.error('Lỗi khi lấy dữ liệu tồn kho:', error);
    //        $scope.errorMessage = 'Không thể tải dữ liệu tồn kho.';
    //        $scope.loading = false;
    //    });

    $scope.export = function () {
        $http({
            method: 'GET',
            url: '/api/BCPhieuNhapAPI/ExportPhieuNhap',
            responseType: 'arraybuffer'
        }).then(function (response) {
            var blob = new Blob([response.data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
            var downloadUrl = URL.createObjectURL(blob);
            var a = document.createElement("a");
            a.href = downloadUrl;
            a.download = "PhieuNhap.xlsx";
            document.body.appendChild(a);
            a.click();
        });


    };
    $scope.search.keyword = '';
    $scope.search.den_ngay = '';
    $scope.search.tu_ngay = '';
    $scope.SEARCH = function () {
        // Tạo chuỗi mã vật tư từ danh sách đã chọn
        var selectedVTCodes = $scope.ds_ma_vt
            .filter(function (vt) {
                return vt.selected === true;
            })
            .map(function (vt) {
                return vt.ma_vt;
            })
            .join(',');

        // Tạo chuỗi mã kho từ danh sách đã chọn
        var selectedKhoCodes = $scope.ds_ma_kho
            .filter(function (kho) {
                return kho.selected === true;
            })
            .map(function (kho) {
                return kho.ma_kho;
            })
            .join(',');

        // Gán vào model tìm kiếm
        $scope.search.ma_vt_list = selectedVTCodes;
        $scope.search.ma_kho_list = selectedKhoCodes;
    
        // Gọi API tìm kiếm
        $http.get('/api/BCPhieuNhapAPI/SearchPhieuNhap', {
            params: {
                ma_vt_list: $scope.search.ma_vt_list,
                ma_kho_list: $scope.search.ma_kho_list,
                keyword: $scope.search.keyword || '',
                tu_ngay: $scope.search.tu_ngay || '',
                den_ngay: $scope.search.den_ngay || ''
            }
        }).then(function (response) {
            $scope.ds_main = response.data;  // Danh sách kết quả
        }).catch(function (error) {
            console.error('Lỗi khi tìm kiếm:', error);
        });
    };
    // Thiết lập các danh sách phục vụ tìm kiếm truy xuất báo cáo
    $scope.ds_ma_vt = [];
    $scope.ds_ma_kho = [];
    $scope.search = {
        ma_vt_list: [],  // danh sách mã vật tư được chọn
        ma_kho_list: []  // danh sách mã kho được chọn
    };
    $scope.GET_VT = function () {
        $http.get('/api/DmvtAPI')
            .then(function (response) {
                $scope.ds_ma_vt = response.data;
            })
            .catch(function (error) {
                alert('Đã có lỗi xảy ra, liên hệ với quản trị viên');
            });
    };
    $scope.GET_KHO = function () {
        $http.get('/api/DmkhoAPI')
            .then(function (response) {
                $scope.ds_ma_kho = response.data;
                 
            })
            .catch(function (error) {
                alert('Đã có lỗi xảy ra, liên hệ với quản trị viên');
            });
    };
    $scope.GET_VT();
    $scope.GET_KHO();

 

 

   

});
