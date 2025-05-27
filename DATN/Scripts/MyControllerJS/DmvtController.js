var app = angular.module('DmvtApp', []);

app.controller('DmvtController', function ($scope, $http) {
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
         
            trangthai: 1,
            ma_loai_vt: '-', 

        };
  
        $scope.LoadComboBox();
    }
    $scope.Edit = function (item) {
        $scope.OtherMode = true;
        // lưu trữ biến cho biết người dùng đang sửa 
        $scope.edit_mode = true;
        $scope.add_mode = false;
        if (item != null || item != undefined) $scope.new_item = angular.copy(item);
        // Sinh QR_code chỉ sử dụng cho các trường hợp chức năng đặc biệt
        $scope.getQRCode();
        $scope.LoadComboBox();

    }
    ///////////////////// KHAI BÁO CÁC MẶC ĐỊNH >>>>>>>>>>>>>>>>>>>>>




    // Phần 2 các function chức năng của hệ thống
    // function lấy ra thông tin danh sách chính
    $scope.GET = function () {
        $http.get('/api/DmvtAPI')
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
            alert('Chưa chọn vật tư nào để xóa');
            return;
        }

        // Hiển thị modal xác nhận
        $scope.showConfirmModal('Bạn có chắc chắn muốn xóa các vật tư đã chọn?', function () {
            // Nếu người dùng đồng ý, tiến hành xóa các phiếu
            $http.post('/api/DmvtAPI/DeleteAll', ds_checked)
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
        $scope.showConfirmModal('Bạn có chắc chắn muốn xóa vật tư này?', function () {
            // Nếu người dùng đồng ý, tiến hành xóa các phiếu
            $http.post('/api/DmvtAPI/DeleteAll', ds_checked)
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
        $scope.get_LoaiVatTu();
    }
   
    $scope.SEARCH = function (isExpandSearch) {
        var _data = {};
        if (!isExpandSearch) {
            _data = {
                keyword: $scope.search.keyword,
                ma_vt: null,
                ten_vt: null,
                mo_ta: null,
                trangthai: null
            };
        } else {
            _data = {
                keyword: $scope.search.keyword,
                ma_vt: $scope.search.ma_vt,
                ten_vt: $scope.search.ten_vt,
                mo_ta: $scope.search.mo_ta,
                trangthai: $scope.search.trangthai

            };

        }
        $http.post('/api/DmvtAPI/SEARCH', _data)
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
            $http.post('/api/DmvtAPI/SaveAdd', _data)
                .then(function (response) {
                    if (response.data.success) {
                        $scope.SaveDmqddvtList();
                        $scope.new_item = response.data.result;
                        $scope.edit_mode = false;
                        $scope.add_mode = false;
                        $scope.getQRCode();
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
            $http.put('/api/DmvtAPI/SaveEdit', _data)
                .then(function (response) {
                    if (response.data.success) {
                        $scope.SaveDmqddvtList();
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
        // Sinh QR_code chỉ sử dụng cho các trường hợp chức năng đặc biệt
        $scope.getQRCode();
        $scope.LoadComboBox();
    }

    // Functinon tạo QR code cho vật tư
    $scope.getQRCode = function () {
        // Tạo QR Code khi controller được khởi tạo
        $scope.data = $scope.new_item.ma_vt; 
        // Tạo QR Code khi controller được khởi tạo
        $scope.$watch('data', function (newVal, oldVal) {
            if (newVal) {
                var qrCodeContainer = document.getElementById('qrcode');
                // Xóa QR Code cũ trước khi tạo mới
                qrCodeContainer.innerHTML = "";
                // Tạo QR Code mới
                new QRCode(qrCodeContainer, {
                    text: newVal,
                    width: 128,
                    height: 128,
                    colorDark: "#000000",
                    colorLight: "#ffffff",
                    correctLevel: 3
                });
            }
        });
    }

    /////////////////////////////////////////////////////////////////////////////////
    // Phần code cuối sẽ xử lý các phần giao diện đặc thù của từng chức năng
    //
    // Biến trạng thái sửa thêm chi tiết
    $scope.edit_mode_current_item = false;
    $scope.get_LoaiVatTu = function () {
        $http.get('/api/LoaiVatTuAPI')
            .then(function (response) {
                $scope.ds_loai_vat_tu = response.data;
                $scope.ds_loai_vat_tu.unshift({
                    ma_loai_vt: '-',
                    ten_loai_vt: ' Chọn loại vật tư --'
                });
            })
            .catch(function (error) {
                alert('Đã có lỗi xảy ra, liên hệ với quản trị viên');
            });
    };
    $scope.get_Dmqddvt = function () {
        $http.get('/api/DmqddvtAPI')
            .then(function (response) {
                $scope.ds_dmqddvt = response.data;
                $scope.ds_dmqddvt = angular.copy($scope.ds_dmqddvt.filter(function (item) {
                    return item.ma_vt == $scope.new_item.ma_vt;
                }));

             
            })
            .catch(function (error) {
                alert('Đã có lỗi xảy ra, liên hệ với quản trị viên');
            });
    };
    $scope.get_Dmdvt = function () {
        $http.get('/api/DmdvtAPI')
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

    // Lưu dòng chi tiết
    $scope.Save_current_item = function () {
        if (!$scope.current_item) {
            alert("Hãy nhập giá trị cho bản ghi");
            return;
        }
        if (!$scope.current_item.ma_dvt) {
            alert("Hãy nhập giá trị cho bản ghi");
            return;
        }
        if (!$scope.current_item.ty_le_quy_doi || $scope.current_item.ty_le_quy_doi <= 0) {
            alert("Hãy nhập tỷ lệ quy đổi lớn hơn 0");
            return;
        }

        // Gán lại giá trị ma_vt cho bản ghi
        $scope.current_item.ma_vt = angular.copy($scope.new_item.ma_vt);

        if ($scope.edit_mode_current_item) {
            // Tìm vị trí phần tử cần cập nhật
            var index = $scope.ds_dmqddvt.findIndex(function (item) {
                return item.ma_vt === $scope.current_item.ma_vt && item.ma_dvt === $scope.current_item.ma_dvt;
            });

            if (index !== -1) {
                $scope.ds_dmqddvt[index] = angular.copy($scope.current_item);
            } else {
                alert("Không tìm thấy bản ghi để cập nhật, sẽ thêm mới.");
                $scope.ds_dmqddvt.push(angular.copy($scope.current_item));
            }
        } else {
            $scope.ds_dmqddvt.push(angular.copy($scope.current_item));
        }

        $scope.current_item = null;
        $scope.edit_mode_current_item = false;
        $scope.backup_edit_item = null;
        $scope.reload_combobox();
    };

    // loại bỏ các giá trị ma_dvt đã tồn tại trong ds_dmqddvt
    $scope.reload_combobox = function () {
        $scope.ds_dmdvt = angular.copy($scope.back_up_ds_dmdvt);

        if (!$scope.ds_dmqddvt || $scope.ds_dmqddvt.length === 0) {
            return;
        }

        var existingMaDvt = $scope.ds_dmqddvt
            .filter(function (item) {
                // Bỏ qua mã của dòng đang sửa
                return !$scope.edit_mode_current_item || item.ma_dvt !==  $scope.backup_edit_item?.ma_dvt;
            })
            .map(function (item) {
                return item.ma_dvt;
            });
        // Thêm mã của new_item vào danh sách loại trừ (nếu có)
        if ($scope.new_item && $scope.new_item.ma_dvt) {
            existingMaDvt.push($scope.new_item.ma_dvt);
        }
        $scope.ds_dmdvt = $scope.ds_dmdvt.filter(function (item) {
            return existingMaDvt.indexOf(item.ma_dvt) === -1;
        });
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
            var index = $scope.ds_dmqddvt.findIndex(function (x) {
                return x.ma_dvt === item.ma_dvt;
            });

            if (index !== -1) {
                $scope.ds_dmqddvt.splice(index, 1);
                $scope.reload_combobox(); 
            } else {
                
            }
        });
       
    };
    $scope.SaveDmqddvtList = function () {
        if (!$scope.ds_dmqddvt || $scope.ds_dmqddvt.length === 0) {
            alert("Danh sách chi tiết đang trống");
            return;
        }

        $http.post('/api/DmqddvtAPI/SaveAll', $scope.ds_dmqddvt)
            .then(function (response) {
                if (response.data.success) {
                    
                    $scope.get_Dmqddvt();  // reload lại danh sách từ DB
                } else {
                 //   alert('Lưu thất bại: ' + response.data.message);
                }
            })
            .catch(function (error) {
                alert('Đã có lỗi xảy ra, liên hệ với quản trị viên');
            });
    };


    $scope.LoadComboBox = function () {
        $scope.get_LoaiVatTu();
        $scope.get_Dmqddvt();
        $scope.get_Dmdvt();
    };
    $scope.LoadComboBox();
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
        $http.get('/api/LOG4API/GetAuditLogByTable/')
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
        $http.get('/api/LOG4API/GetAuditLogByTableCT/')
            .then(function (response) {
                $scope.historyLogs = response.data.filter(function (log) {
                    try {
                        // Parse primary_key_data để lấy ma_kho
                        const primaryKeyObj = JSON.parse(log.primary_key_data || '{}');
                        const logMaKho = primaryKeyObj.ma_vt || null;

                        // So sánh ma_kho
                        const isSameMaKho = logMaKho === item.ma_vt;

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
    $scope.uploadImage = function () {
        var formData = new FormData();
        formData.append("file", $scope.fileImage);

        $http.post('/api/DmvtAPI/UploadImage', formData, {
            transformRequest: angular.identity,
            headers: { 'Content-Type': undefined }
        }).then(function (response) {
            if (response.data.success) {
                $scope.new_item.url = response.data.url;
                alert("Upload ảnh thành công");
            } else {
                alert("Upload thất bại");
            }
        }).catch(function (error) {
            alert("Lỗi khi upload ảnh");
        });
    };

});
app.directive('fileModel', ['$parse', function ($parse) {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            var model = $parse(attrs.fileModel);
            var modelSetter = model.assign;

            element.bind('change', function () {
                scope.$apply(function () {
                    modelSetter(scope, element[0].files[0]);
                });
            });
        }
    };
}]);
