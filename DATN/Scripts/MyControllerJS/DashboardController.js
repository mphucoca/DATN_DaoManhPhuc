 

    var app = angular.module("DashboardApp", []);
app.controller("DashboardController", function ($scope, $http) {
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
        let modalChart1 = null;
        let modalChart2 = null;
        let modalChartOverviewInstance = null;
        let modalChartDetailInstance = null;
        $scope.openModalWithTwoCharts = function (chartConfigs) {
            $scope.modalVisible = true;

            setTimeout(() => {
                // Chart 1
                const ctx1 = document.getElementById("modalChart1").getContext("2d");
                if (modalChart1) modalChart1.destroy();
                modalChart1 = new Chart(ctx1, {
                    type: chartConfigs[0].type,
                    data: {
                        labels: chartConfigs[0].labels,
                        datasets: [{
                            label: chartConfigs[0].label,
                            data: chartConfigs[0].data,
                            backgroundColor: chartConfigs[0].colors,
                            borderColor: chartConfigs[0].colors,
                            fill: chartConfigs[0].type === 'line' ? false : true,
                            tension: chartConfigs[0].type === 'line' ? 0.3 : 0
                        }]
                    },
                    options: chartConfigs[0].options
                });

                // Chart 2
                const ctx2 = document.getElementById("modalChart2").getContext("2d");
                if (modalChart2) modalChart2.destroy();
                modalChart2 = new Chart(ctx2, {
                    type: chartConfigs[1].type,
                    data: {
                        labels: chartConfigs[1].labels,
                        datasets: [{
                            label: chartConfigs[1].label,
                            data: chartConfigs[1].data,
                            backgroundColor: chartConfigs[1].colors,
                            borderColor: chartConfigs[1].colors,
                            fill: chartConfigs[1].type === 'line' ? false : true,
                            tension: chartConfigs[1].type === 'line' ? 0.3 : 0
                        }]
                    },
                    options: chartConfigs[1].options
                });
            }, 0);
        };
        $scope.closeModal = function (event) {
            if (event) event.stopPropagation(); // Tránh lỗi khi event là undefined
            $scope.modalVisible = false;

            // Hủy cả hai biểu đồ nếu có
            if (modalChartOverviewInstance) {
                modalChartOverviewInstance.destroy();
                modalChartOverviewInstance = null;
            }
            if (modalChartDetailInstance) {
                modalChartDetailInstance.destroy();
                modalChartDetailInstance = null;
            }
        };


        $scope.onDashboardCardClick = function (cardName) {
            switch (cardName) {
                case "tongNhap":
                    $scope.openModalWithTwoCharts([
                        {
                            type: "line",
                            label: "Tổng giá trị nhập (VNĐ)",
                            labels: $scope.tongNhapChartData.labels,
                            data: $scope.tongNhapChartData.values,
                            colors: "rgba(75,192,192,1)",
                            options: $scope.tongNhapChartOptions
                        },
                        {
                            type: "bar",
                            label: "Tổng giá trị nhập (Bar)",
                            labels: $scope.tongNhapChartData.labels,
                            data: $scope.tongNhapChartData.values,
                            colors: "rgba(75,192,192,0.6)",
                            options: { responsive: true, scales: { y: { beginAtZero: true } } }
                        }
                    ]);
                    break;

                case "tongXuat":
                    $scope.openModalWithTwoCharts([
                        {
                            type: "line",
                            label: "Tổng giá trị xuất (VNĐ)",
                            labels: $scope.tongXuatChartData.labels,
                            data: $scope.tongXuatChartData.values,
                            colors: "rgba(255,99,132,1)",
                            options: $scope.tongXuatChartOptions
                        },
                        {
                            type: "bar",
                            label: "Tổng giá trị xuất (Bar)",
                            labels: $scope.tongXuatChartData.labels,
                            data: $scope.tongXuatChartData.values,
                            colors: "rgba(255,99,132,0.6)",
                            options: { responsive: true, scales: { y: { beginAtZero: true } } }
                        }
                    ]);
                    break;

                case "tonKho":
                    $scope.openModalWithTwoCharts([
                        {
                            type: "bar",
                            label: "Tồn kho theo kho",
                            labels: $scope.tonKhoChartData.labels,
                            data: $scope.tonKhoChartData.values,
                            colors: "rgba(54, 162, 235, 0.6)",
                            options: { responsive: true, scales: { y: { beginAtZero: true } } }
                        },
                        {
                            type: "pie",
                            label: "Tỷ lệ tồn kho",
                            labels: $scope.tonKhoChartData.labels,
                            data: $scope.tonKhoChartData.values,
                            colors: ["#FF6384", "#36A2EB", "#FFCE56", "#4BC0C0", "#9966FF"],
                            options: { responsive: true }
                        }
                    ]);
                    break;

                case "sapHet":
                    $scope.openModalWithTwoCharts([
                        {
                            type: "bar",
                            label: "Vật tư sắp hết hàng",
                            labels: $scope.sapHetChartData.labels,
                            data: $scope.sapHetChartData.values,
                            colors: "rgba(255, 206, 86, 0.6)",
                            options: { responsive: true, scales: { y: { beginAtZero: true } } }
                        },
                        {
                            type: "doughnut",
                            label: "Phân bổ vật tư sắp hết",
                            labels: $scope.sapHetChartData.labels,
                            data: $scope.sapHetChartData.values,
                            colors: ["#FF6384", "#36A2EB", "#FFCE56", "#4BC0C0", "#9966FF"],
                            options: { responsive: true }
                        }
                    ]);
                    break;

                case "topNCC":
                    $scope.openModalWithTwoCharts([
                        {
                            type: "bar",
                            label: "Top 5 NCC theo giá trị nhập",
                            labels: $scope.topNCCChartData.labels,
                            data: $scope.topNCCChartData.values,
                            colors: "rgba(153, 102, 255, 0.6)",
                            options: {
                                responsive: true,
                                indexAxis: 'y',
                                scales: {
                                    x: {
                                        ticks: {
                                            callback: value => value.toLocaleString('vi-VN') + " ₫"
                                        }
                                    }
                                }
                            }
                        },
                        {
                            type: "pie",
                            label: "Tỷ trọng giá trị nhập theo NCC",
                            labels: $scope.topNCCChartData.labels,
                            data: $scope.topNCCChartData.values,
                            colors: ["#FF6384", "#36A2EB", "#FFCE56", "#4BC0C0", "#9966FF"],
                            options: { responsive: true }
                        }
                    ]);
                    break;

                case "tyTrongLoaiVT":
                    $scope.openModalWithTwoCharts([
                        {
                            type: "pie",
                            label: "Tỷ trọng nhập theo loại vật tư",
                            labels: $scope.tyTrongLoaiVTChartData.labels,
                            data: $scope.tyTrongLoaiVTChartData.values,
                            colors: ["#FF6384", "#36A2EB", "#FFCE56", "#4BC0C0", "#9966FF", "#FF9F40"],
                            options: { responsive: true }
                        },
                        {
                            type: "bar",
                            label: "Giá trị nhập theo loại vật tư",
                            labels: $scope.tyTrongLoaiVTChartData.labels,
                            data: $scope.tyTrongLoaiVTChartData.values,
                            colors: "rgba(75,192,192,0.6)",
                            options: { responsive: true, scales: { y: { beginAtZero: true } } }
                        }
                    ]);
                    break;
            }
        };

    // Các hàm load chart gốc, sửa lại để tạo chart data, labels, options, colors rồi gọi openModalWithChart khi click

    // Ví dụ cập nhật cho loadTongNhapTheoNgay:
    $scope.loadTongNhapTheoNgay = function () {
        $http.get("/api/DashboardAPI/TongGiaTriNhapTheoNgay")
            .then(function (response) {
                if (response.data.success) {
                    const data = response.data.result;
                    const labels = data.map(item => item.ngay);
                    const values = data.map(item => item.tong_gia_tri);

                    const ctx = document.getElementById("chartTongNhap").getContext("2d");
                    new Chart(ctx, {
                        type: "line",
                        data: {
                            labels: labels,
                            datasets: [{
                                label: "Tổng giá trị nhập (VNĐ)",
                                data: values,
                                fill: false,
                                borderColor: "rgba(75,192,192,1)",
                                tension: 0.3
                            }]
                        },
                        options: {
                            responsive: true,
                            scales: {
                                y: {
                                    beginAtZero: true,
                                    ticks: {
                                        callback: value => value.toLocaleString('vi-VN') + " ₫"
                                    }
                                }
                            }
                        }
                    });

                    // Gán dữ liệu để mở modal khi click
                    $scope.tongNhapChartData = { labels, values };
                    $scope.tongNhapChartOptions = {
                        responsive: true,
                        scales: {
                            y: {
                                beginAtZero: true,
                                ticks: {
                                    callback: value => value.toLocaleString('vi-VN') + " ₫"
                                }
                            }
                        }
                    };
                }
            });
    };

 

    // Sửa các hàm load tương tự loadTongNhapTheoNgay, thêm phần lưu data

    // Ví dụ loadTongXuatTheoNgay:
    $scope.loadTongXuatTheoNgay = function () {
        $http.get("/api/DashboardAPI/TongGiaTriXuatTheoNgay")
            .then(function (response) {
                if (response.data.success) {
                    const data = response.data.result;
                    const labels = data.map(item => item.ngay);
                    const values = data.map(item => item.tong_gia_tri);

                    const ctx = document.getElementById("chartTongXuat").getContext("2d");
                    new Chart(ctx, {
                        type: "line",
                        data: {
                            labels: labels,
                            datasets: [{
                                label: "Tổng giá trị xuất (VNĐ)",
                                data: values,
                                fill: false,
                                borderColor: "rgba(255,99,132,1)",
                                tension: 0.3
                            }]
                        },
                        options: {
                            responsive: true,
                            scales: {
                                y: {
                                    beginAtZero: true,
                                    ticks: {
                                        callback: value => value.toLocaleString('vi-VN') + " ₫"
                                    }
                                }
                            }
                        }
                    });

                    $scope.tongXuatChartData = { labels, values };
                    $scope.tongXuatChartOptions = {
                        responsive: true,
                        scales: {
                            y: {
                                beginAtZero: true,
                                ticks: {
                                    callback: value => value.toLocaleString('vi-VN') + " ₫"
                                }
                            }
                        }
                    };
                }
            });
    };

    // loadTonKhoTheoKho:
    $scope.loadTonKhoTheoKho = function () {
        $http.get("/api/DashboardAPI/TonKhoTheoKho")
            .then(function (response) {
                if (response.data.success) {
                    const data = response.data.result;
                    const labels = data.map(item => item.ten_kho);
                    const values = data.map(item => item.tong_so_luong);

                    const ctx = document.getElementById("chartTonKho").getContext("2d");
                    new Chart(ctx, {
                        type: "bar",
                        data: {
                            labels: labels,
                            datasets: [{
                                label: "Tổng số lượng tồn",
                                data: values,
                                backgroundColor: "rgba(54, 162, 235, 0.6)"
                            }]
                        },
                        options: {
                            responsive: true,
                            scales: {
                                y: { beginAtZero: true }
                            }
                        }
                    });

                    $scope.tonKhoChartData = { labels, values };
                }
            });
    };

    // loadVatTuSapHet:
    $scope.loadVatTuSapHet = function () {
        $http.get("/api/DashboardAPI/VatTuSapHetHang")
            .then(function (response) {
                if (response.data.success) {
                    const data = response.data.result;
                    const labels = data.map(item => item.ten_vt);
                    const values = data.map(item => item.tong_so_luong);

                    const ctx = document.getElementById("chartSapHet").getContext("2d");
                    new Chart(ctx, {
                        type: "bar",
                        data: {
                            labels: labels,
                            datasets: [{
                                label: "Số lượng còn lại",
                                data: values,
                                backgroundColor: "rgba(255, 206, 86, 0.6)"
                            }]
                        },
                        options: {
                            responsive: true,
                            scales: {
                                y: { beginAtZero: true }
                            }
                        }
                    });

                    $scope.sapHetChartData = { labels, values };
                }
            });
    };

    // loadTop5NCC:
    $scope.loadTop5NCC = function () {
        $http.get("/api/DashboardAPI/Top5NCCNhapCaoNhat")
            .then(function (response) {
                if (response.data.success) {
                    const data = response.data.result;
                    const labels = data.map(item => item.ten_ncc);
                    const values = data.map(item => item.tong_gia_tri);

                    const ctx = document.getElementById("chartTopNCC").getContext("2d");
                    new Chart(ctx, {
                        type: "bar",
                        data: {
                            labels: labels,
                            datasets: [{
                                label: "Giá trị nhập (VNĐ)",
                                data: values,
                                backgroundColor: "rgba(153, 102, 255, 0.6)"
                            }]
                        },
                        options: {
                            responsive: true,
                            indexAxis: 'y',
                            scales: {
                                x: {
                                    ticks: {
                                        callback: value => value.toLocaleString('vi-VN') + " ₫"
                                    }
                                }
                            }
                        }
                    });

                    $scope.topNCCChartData = { labels, values };
                }
            });
    };

    // loadTyTrongTheoLoaiVT:
    $scope.loadTyTrongTheoLoaiVT = function () {
        $http.get("/api/DashboardAPI/TyTrongNhapTheoLoaiVT")
            .then(function (response) {
                if (response.data.success) {
                    const data = response.data.result;
                    const labels = data.map(item => item.ten_loai_vt);
                    const values = data.map(item => item.tong_gia_tri);

                    const ctx = document.getElementById("chartLoaiVT").getContext("2d");
                    new Chart(ctx, {
                        type: "pie",
                        data: {
                            labels: labels,
                            datasets: [{
                                label: "Tỷ trọng giá trị nhập",
                                data: values,
                                backgroundColor: [
                                    "#FF6384", "#36A2EB", "#FFCE56", "#4BC0C0", "#9966FF", "#FF9F40"
                                ]
                            }]
                        },
                        options: {
                            responsive: true
                        }
                    });

                    $scope.tyTrongLoaiVTChartData = { labels, values };
                }
            });
    };
    // Tải dữ liệu các chart và lưu vào biến $scope để dùng cho modal
    $scope.loadTongNhapTheoNgay();
    $scope.loadTongXuatTheoNgay();
    $scope.loadTonKhoTheoKho();
    $scope.loadVatTuSapHet();
    $scope.loadTop5NCC();
    $scope.loadTyTrongTheoLoaiVT();
});
