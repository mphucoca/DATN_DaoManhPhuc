var app = angular.module('DmkhoApp', []);

app.controller('DmkhoController', function ($scope, $http) {
    // Phần 1 khởi tạo các biến cần thiết
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
            role: 1 ,
            trangthai: 1
        };
        $scope.LoadComboBox();
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
        $http.get('/api/DmkhoAPI')
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
            alert('Chưa chọn kho nào để xóa');
            return;
        }

        // Hiển thị modal xác nhận
        $scope.showConfirmModal('Bạn có chắc chắn muốn xóa các kho đã chọn?', function () {
            // Nếu người dùng đồng ý, tiến hành xóa các phiếu
            $http.post('/api/DmkhoAPI/DeleteAll', ds_checked)
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
        $scope.showConfirmModal('Bạn có chắc chắn muốn xóa kho này?', function () {
            // Nếu người dùng đồng ý, tiến hành xóa các phiếu
            $http.post('/api/DmkhoAPI/DeleteAll', ds_checked)
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
                ma_kho: null,
                ten_kho: null,
                dia_chi: null,
                mo_ta: null,

            };
        } else {
            _data = {
                keyword: $scope.search.keyword,
                ma_kho: $scope.search.ma_kho,
                ten_kho: $scope.search.ten_kho,
                dia_chi: $scope.search.dia_chi,
                mo_ta: $scope.search.mo_ta,
                

            };

        }
        $http.post('/api/DmkhoAPI/SEARCH', _data)
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
            $http.post('/api/DmkhoAPI/SaveAdd', _data)
                .then(function (response) {
                    if (response.data.success) {
                        $scope.saveTonKho();
                        $scope.new_item = response.data.result;
                        $scope.edit_mode = false;
                        $scope.add_mode = false;
                    } else {
                        alert('Mã kho đã tồn tại');
                    }
                }).catch(function (error) {
                    alert('Đã có lỗi xảy ra liên hệ với quản trị viên'); 
                });
        } else if ($scope.edit_mode) {
            // xử lý khi sửa bản ghi
            // xử lý khi thêm mới bản ghi
            $http.put('/api/DmkhoAPI/SaveEdit', _data)
                .then(function (response) {
                    if (response.data.success) {
                        $scope.saveTonKho();
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
    }
    /////////////////////////////////////////////////////////////////////////////////



      /////////////////////////////////////////////////////////////////////////////////

      /////////////////////////////////////////////////////////////////////////////////

      /////////////////////////////////////////////////////////////////////////////////

      /////////////////////////////////////////////////////////////////////////////////
    // Phần code cuối sẽ xử lý các phần giao diện đặc thù của từng chức năng
    //
    $scope.ds_ton_kho = [];
    $scope.ds_dmdvt = [];

    // Biến trạng thái sửa thêm chi tiết
    $scope.edit_mode_current_item = false;
    $scope.GET_dmvt = function () {
        $http.get('/api/DmvtAPI')
            .then(function (response) {
                $scope.back_up_ds_dmvt = response.data;
                $scope.ds_dmvt = angular.copy($scope.back_up_ds_dmvt);
            })
            .catch(function (error) {
                alert('Đã có lỗi xảy ra, liên hệ với quản trị viên');
            });
    };
    $scope.GET_tonkho = function (ma_kho) {
        if (!ma_kho) {
           // alert('Vui lòng chọn mã kho');
            return;
        }

        $http.get('/api/DmkhoAPI/GetTonTheoKho', { params: { ma_kho: ma_kho } })
            .then(function (response) {
                $scope.back_up_ds_ton_kho = response.data;
                $scope.ds_ton_kho = angular.copy($scope.back_up_ds_ton_kho);
            })
            .catch(function (error) {
                console.error('Lỗi khi gọi API tồn kho:', error);
                alert('Đã có lỗi xảy ra khi lấy tồn kho, vui lòng liên hệ quản trị viên.');
            });
    };

    $scope.get_Dmdvt = function (ma_vt) {
        $http.get('/api/DmdvtAPI/GetDVTByMaVT', { params: { ma_vt: ma_vt } })
            .then(function (response) {
                $scope.back_up_ds_dmdvt = response.data;
                $scope.ds_dmdvt = angular.copy($scope.back_up_ds_dmdvt);
            })
            .catch(function (error) {
                alert('Đã có lỗi xảy ra, liên hệ với quản trị viên');
            });
    };

    // Sửa dòng chi tiết
    $scope.Edit_current_item = function (item) {
        if (!$scope.edit_mode && !$scope.add_mode) return;
        $scope.current_item = angular.copy(item);
        $scope.edit_mode_current_item = true;
        $scope.backup_edit_item = angular.copy(item);
        $scope.reload_combobox();
    };

    $scope.Save_current_item = function () {
        if (!$scope.current_item) {
            alert("Hãy nhập giá trị cho bản ghi");
            return;
        }
        if (!$scope.current_item.ma_vt) {
            alert("Hãy chọn vật tư");
            return;
        }
        if (!$scope.current_item.ma_dvt) {
            alert("Hãy chọn đơn vị tính");
            return;
        }
        if (!$scope.current_item.so_luong_ton || $scope.current_item.so_luong_ton < 0) {
            alert("Số lượng tồn thực tế >= 0");
            return;
        }

        // Kiểm tra trùng mã vt + mã dvt
        var isDuplicate = $scope.ds_ton_kho.some(function (item) {
            if ($scope.edit_mode_current_item) {
                // Bỏ qua dòng đang sửa (dựa vào backup_edit_item)
                if (item.ma_vt === $scope.backup_edit_item.ma_vt && item.ma_dvt === $scope.backup_edit_item.ma_dvt) {
                    return false;
                }
            }
            return item.ma_vt === $scope.current_item.ma_vt && item.ma_dvt === $scope.current_item.ma_dvt;
        });

        if (isDuplicate) {
            alert("Vật tư và đơn vị tính đã tồn tại trong danh sách!");
            return;
        }

        // Gán lại giá trị ma_kho cho bản ghi
        $scope.current_item.ma_kho = angular.copy($scope.new_item.ma_kho);
        // Gán ngày hiện tại
        $scope.current_item.ngay_cap_nhat = new Date();

        if ($scope.edit_mode_current_item) {
            // Tìm vị trí phần tử cần cập nhật dựa vào backup_edit_item
            var index = $scope.ds_ton_kho.findIndex(function (item) {
                return item.ma_vt === $scope.backup_edit_item.ma_vt && item.ma_dvt === $scope.backup_edit_item.ma_dvt;
            });

            if (index !== -1) {
                $scope.ds_ton_kho[index] = angular.copy($scope.current_item);
            } else {
                alert("Không tìm thấy bản ghi để cập nhật, sẽ thêm mới.");
                $scope.ds_ton_kho.push(angular.copy($scope.current_item));
            }
        } else {
            $scope.ds_ton_kho.push(angular.copy($scope.current_item));
        }

        $scope.current_item = null;
        $scope.edit_mode_current_item = false;
        $scope.backup_edit_item = null;
        $scope.reload_combobox();
    };



    // loại bỏ các giá trị ma_dvt đã tồn tại trong ds_dmqddvt
    $scope.reload_combobox = function () {
       
 
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
            // Tìm vị trí item trong ds_ton_kho
            var index = $scope.ds_ton_kho.findIndex(function (x) {
                return x.ma_dvt === item.ma_dvt;
            });

            if (index !== -1) {
                $scope.ds_ton_kho.splice(index, 1);
                $scope.reload_combobox();
            } else {

            }
        });

    };
  
    $scope.saveTonKho = function () {
        var data = {
            ma_kho: $scope.new_item.ma_kho,
            ds_ton_kho: $scope.ds_ton_kho
        };

        $http.post('/api/TonKhoAPI/SaveTonKho', data)
            .then(function (response) {
                if (response.data.success) {
                     
                } else {
                    alert("Đã có lỗi xảy ra, liên hệ với quản trị viên");
                }
            }, function (error) {
                alert('Lỗi kết nối server');
            });
    };

    $scope.LoadComboBox = function () {
        $scope.GET_dmvt();
      
        if ($scope.new_item) $scope.GET_tonkho($scope.new_item.ma_kho);
    };
    $scope.LoadComboBox();

});
