var app = angular.module('myApp', ['ngRoute', 'ngMessages', 'ui.bootstrap']);
app.constant('CTX_FOLDER', '/views/ManagementUser');
app.config(function ($routeProvider, $locationProvider, CTX_FOLDER) {
    // Sử dụng HTML5 mode để loại bỏ hash (#)
    $locationProvider.html5Mode({
        enabled: true,
        requireBase: true // Đảm bảo có thẻ <base> trong HTML
    });
    $routeProvider
        .when('/Admin/ManagementUser', {
            templateUrl: CTX_FOLDER + '/index.html',
            controller: 'index',
            resolve: {
                permissions: function ($rootScope) {
                    return $rootScope.loadPermissions();
                }
            }
        })
});
app.factory('dataservice', function ($http, CTX_FOLDER) {
    function callApi(method, url, data, callback) {
        $http({
            method: method,
            url: url,
            data: data,
            headers: { 'Content-Type': 'application/json' }
        }).then(function (response) {
            callback(response.data); // Trả dữ liệu về callback
        }).catch(function (error) {
            console.error(`Error in API call [${url}]:`, error); // Bắt lỗi nếu có
        });
    }

    return {
        getNumberUserNoAuto: function (callback) {
            callApi('POST', '/ManagementUser/GetNumberUserNoAuto', {}, callback);
        },
        jtable: function (data, callback) {
            callApi('POST', '/ManagementUser/JTable', JSON.stringify(data), callback);
        },
        getDepartment: function (callback) {
            callApi('POST', '/ManagementUser/GetDepartment', {}, callback);
        },
        getJobTitle: function (callback) {
            callApi('POST', '/ManagementUser/GetJobTitle', {}, callback);
        },
        getItem: function (data, callback) {
            callApi('POST', '/ManagementUser/GetItem', JSON.stringify(data), callback);
        },
        delete: function (data, callback) {
            callApi('POST', '/ManagementUser/Delete', JSON.stringify(data), callback);
        },
        changeIsActive: function (data, callback) {
            callApi('POST', '/ManagementUser/ChangeIsActive', JSON.stringify(data), callback);
        },
        getUserPermissions: function (data, callback) {
            callApi('POST', '/ManagementUser/GetUserPermissions', JSON.stringify(data), callback);
        },
        loadDepartment: function (data, callback) {
            callApi('POST', '/ManagementUser/LoadDepartment', JSON.stringify(data), callback);
        },
        loadController: function (callback) {
            callApi('POST', '/ManagementUser/LoadController', {}, callback);
        },
        get_item_permission: function (data, callback) {
            callApi('POST', '/ManagementUser/getItemPermission', JSON.stringify(data), callback);
        },
        reset_password: function (data, callback) {
            callApi('POST', '/ManagementUser/ResetPassword', JSON.stringify(data), callback);
        },
    };
});
app.run(function ($rootScope, dataservice, $q) {
    // Khởi tạo $rootScope.userPermissions và $rootScope.hasPermission
    $rootScope.userPermissions = {};
    $rootScope.hasPermission = function (permission) {
        return $rootScope.userPermissions[permission] || false;
    };

    // Tải quyền người dùng qua API và trả về Promise
    $rootScope.loadPermissions = function () {
        var deferred = $q.defer();
        dataservice.getUserPermissions({}, function (result) {
            if (!result.Error) {
                $rootScope.userPermissions = result;
                deferred.resolve();
            } else {
                console.error('Error loading permissions:', result.Title);
                deferred.reject();
            }
        });
        return deferred.promise;
    };

    // Gọi hàm loadPermissions() ngay khi ứng dụng khởi chạy
    $rootScope.loadPermissions();
});
app.controller('Ctrl_ManagementUser', function ($scope, $rootScope, $uibModal, dataservice, CTX_FOLDER) {
    $rootScope.Title = "ManagementUser";


    $rootScope.validationOptions = {
        rules: {
            UserName: {
                required: true,
                maxlength: 100
            },
            FullName: {
                required: true,
                maxlength: 100
            },
            Department_id: {
                required: true,
            },
            JobTitle_id: {
                required: true
            },
            Email: {
                required: true
            },
            Phone: {
                required: true
            },
            Avatar: {
                required: true
            }
        },
        messages: {
            UserName: {
                required: "Yêu cầu nhập User Name",
                maxlength: "User Name không vượt quá 100 ký tự."
            },
            FullName: {
                required: "Yêu cầu nhập Full Name",
                maxlength: "Full Name đến không vượt quá 100 ký tự."
            },
            JobTitle_id: {
                required: "Yêu cầu chọn JobTitle_id"
            },
            Email: {
                required: "Yêu cầu nhập Email"
            },
            Phone: {
                required: "Yêu cầu nhập Phone"
            },
            Avatar: {
                required: "Yêu cầu nhập Avatar"
            }
        }
    };
    $rootScope.convertToJSONDate = function (strDate) {
        if (strDate !== null && strDate !== "") {
            var Str = strDate.toString();
            if (Str.indexOf("/Date") >= 0) {
                return Str;
            } else {
                var newDate = new Date(strDate);
                return '/Date(' + newDate.getTime() + ')/';
            }
        }
    }
});
app.directive('fileInput', function () {
    return {
        restrict: 'A',
        scope: {
            fileInput: '=', // Liên kết với một biến trong scope để lưu danh sách file
            onFileChange: '&' // Hàm callback để xử lý khi file thay đổi
        },
        link: function (scope, element, attrs) {
            element.bind('change', function (event) {
                var files = event.target.files; // Lấy danh sách file đã chọn
                scope.$apply(function () {
                    scope.fileInput = files; // Lưu file vào biến được liên kết
                    if (scope.onFileChange) {
                        scope.onFileChange({ files: files }); // Gọi hàm callback nếu có
                    }
                });
            });
        }
    };
});
app.controller('index', function ($scope, $rootScope, $uibModal, dataservice, CTX_FOLDER, $timeout) {
    $scope.model = {};
    $scope.datatable = [];
    var ctrl = $scope;
    $scope.staticParam = {
        Search: "",
        IsActive: "1",
        Department_id: null
    };
    //Table
    $scope.pageIndex = 1;    // Trang hiện tại
    $scope.pageSize = 10;    // Số bản ghi mỗi trang
    $scope.totalPages = 0;   // Tổng số trang
    $scope.onEnterPress = function (event) {
        if (event.keyCode === 13) { // 13 là mã phím Enter
            $scope.getTable(); // Gọi hàm getTable khi nhấn Enter
        }
    };

    // Gọi hàm lấy dữ liệu ngay khi controller được khởi tạo
    if ($rootScope.hasPermission('View')) {
        $scope.initTree = function () {
            const treeContainer = document.getElementById('div_tree');
            if (!treeContainer) {
                console.error("Phần tử '#div_tree' không tồn tại.");
                return;
            }
            var requestData = {
                IsActive: $scope.staticParam.IsActive,
            };
            dataservice.loadDepartment(requestData, function (result) {
                let tree = new Tree('#div_tree', {
                    data: result,
                    closeDepth: 3,
                    onChange: function () {
                        $scope.staticParam.Department_id = this.values.join(",");
                        //$scope.staticParam.Department_id = this.selectedNodes.id.join(",");
                        $scope.getTable();
                    }
                });
                if (result.length > 0) {
                    $scope.staticParam.Department_id = ""; // Gán giá trị mặc định là phần tử đầu tiên
                    $scope.getTable();
                }
            });
        };
    }
    $scope.getTable = function () {
        showLoading();
        var requestData = {
            Department_id: $scope.staticParam.Department_id,
            Search: $scope.staticParam.Search,
            IsActive: $scope.staticParam.IsActive,
            pageIndex: $scope.pageIndex,
            pageSize: $scope.pageSize
        };
        showLoading();
        dataservice.jtable(requestData, function (response) {
            if (!response.Error) {
                if (JSON.stringify($scope.List_ManagementUser) !== JSON.stringify(response.Data)) {
                    $scope.List_ManagementUser = response.Data;
                    $scope.totalPages = Math.ceil(response.TotalRecords / $scope.pageSize);
                    hideLoading();
                }
            } else {
                console.error('Error:', response.Title);
            }
        });
    };
    // Chuyển sang trang
    $scope.previousPage = function () {
        if ($scope.pageIndex > 1) {
            $scope.pageIndex--;
            $scope.getTable();
        }
    };
    $scope.nextPage = function () {
        if ($scope.pageIndex < $scope.totalPages) {
            $scope.pageIndex++;
            $scope.getTable();
        }
    };
    $scope.goToPage = function (page) {
        if (page >= 1 && page <= $scope.totalPages) {
            $scope.pageIndex = page; // Cập nhật trang hiện tại
            $scope.getTable(); // Gọi lại hàm để lấy dữ liệu cho trang mới
        }
    };
    // Refresh dữ liệu
    $rootScope.ReloadData = function () {
        if ($rootScope.hasPermission('View')) {
            $scope.initTree();
        }
    };
    // Điều khiển cá Controller, nút button
    $scope.addModal = function () {

        var modalInstance = $uibModal.open({
            templateUrl: CTX_FOLDER + '/add.html',
            controller: 'add',
            backdrop: 'static',
            keyboard: false,
            //size: 'lg', // Kích thước: 'sm', 'lg', hoặc 'xl'
            windowClass: 'custom-modal-size',
            resolve: {
                data: function () {
                    return { message: "Thêm người dùng" };
                }
            }
        });
        // Khi modal mở, chuyển focus đến modal
        modalInstance.opened.then(function () {
            document.querySelector('.custom-modal-size').focus();
        });

        // Xử lý kết quả khi modal đóng
        modalInstance.result.then(
            function (result) {
                // Thành công
            },
            function (reason) {
                if (reason === 'cancel') {
                    // Xử lý hủy modal
                }
            }
        );
    };
    $scope.editModal = function (id) {
        var modalInstance = $uibModal.open({
            templateUrl: CTX_FOLDER + '/edit.html',
            controller: 'edit',
            backdrop: 'static',
            keyboard: true,
            //size: 'lg', // Kích thước: 'sm', 'lg', hoặc 'xl'
            windowClass: 'custom-modal-size',
            resolve: {
                data: function () {
                    return { message: "Cập nhật người dùng", id_item: id };
                }
            }
        });
        // Khi modal mở, chuyển focus đến modal
        modalInstance.opened.then(function () {
            document.querySelector('.custom-modal-size').focus();
        });
        // Xử lý kết quả khi modal đóng
        modalInstance.result.then(
            function (result) {
                // Thành công
            },
            function (reason) {
                if (reason === 'cancel') {
                    // Xử lý hủy modal
                }
            }
        );
    };
    $scope.openModal = function (id) {
        var modalInstance = $uibModal.open({
            templateUrl: CTX_FOLDER + '/open.html',
            controller: 'open',
            backdrop: 'static',
            keyboard: true,
            //size: 'lg', // Kích thước: 'sm', 'lg', hoặc 'xl'
            windowClass: 'custom-modal-size',
            resolve: {
                data: function () {
                    return { message: "Xem bản ghi", id_item: id };
                }
            }
        });
        // Khi modal mở, chuyển focus đến modal
        modalInstance.opened.then(function () {
            document.querySelector('.custom-modal-size').focus();
        });
        // Xử lý kết quả khi modal đóng
        modalInstance.result.then(
            function (result) {
                // Thành công
            },
            function (reason) {
                if (reason === 'cancel') {
                    // Xử lý hủy modal
                }
            }
        );
    };
    $scope.delete = function (temp) {
        const { id, title } = temp;

        swal({
            title: "Bạn có chắc chắn?",
            text: `Bạn có chắc chắn muốn xóa bản ghi có tiêu đề "${title}" không?`,
            icon: "warning",
            buttons: {
                cancel: {
                    text: "Hủy",
                    value: false,
                    visible: true,
                    className: "",
                    closeModal: true,
                },
                confirm: {
                    text: "Xóa",
                    value: true,
                    visible: true,
                    className: "btn-danger",
                    closeModal: true,
                },
            },
            dangerMode: true,
        }).then((confirmDelete) => {
            if (confirmDelete) {
                var requestData = {
                    Id: id,
                    IsActive: 0,
                };
                dataservice.delete(requestData, function (result) {
                    if (!result.Error) {
                        showAlert("success", result.Title);
                        $rootScope.ReloadData();
                    } else {
                        showAlert("danger", result.Title);
                    }
                });
            } else {
                console.log("Hủy hành động xóa");
            }
        });
    };
    $scope.restore = function (temp) {
        const { id, title } = temp;

        swal({
            title: "Bạn có chắc chắn?",
            text: `Bạn có chắc chắn muốn khôi phục tài khoản "${title}" không?`,
            icon: "warning",
            buttons: {
                cancel: {
                    text: "Hủy",
                    value: false,
                    visible: true,
                    className: "",
                    closeModal: true,
                },
                confirm: {
                    text: "Khôi phục",
                    value: true,
                    visible: true,
                    className: "btn-danger",
                    closeModal: true,
                },
            },
            dangerMode: true,
        }).then((confirmDelete) => {
            if (confirmDelete) {
                var requestData = {
                    Id: id,
                    IsActive: 1,
                };
                dataservice.delete(requestData, function (result) {
                    if (!result.Error) {
                        showAlert('success', "Khôi phục thành công!");
                        $rootScope.ReloadData();
                    } else {
                        showAlert('danger', result.Title);
                    }
                });
            } else {
                console.log("Hủy hành động xóa");
            }
        });
    };
    $scope.addpermission = function (temp) {
        const { id, title } = temp;
        var modalInstance = $uibModal.open({
            templateUrl: CTX_FOLDER + '/addpermission.html',
            controller: 'addpermission',
            backdrop: 'static',
            keyboard: true,
            //size: 'lg', // Kích thước: 'sm', 'lg', hoặc 'xl'
            windowClass: 'custom-modal-size',
            resolve: {
                data: function () {
                    return { message: "Phân quyền: " + title, id_item: id, title_item: title };
                }
            }
        });
        modalInstance.result.then(
            function (result) {
            },
            function (reason) {
                // Xử lý từ chối
                if (reason === 'cancel') {
                } else {
                }
            }
        );

    };
    $scope.resetpassword = function (temp) {
        const { id, title } = temp;

        swal({
            title: "Bạn có chắc chắn?",
            text: `Bạn có chắc chắn muốn đặt lại mật khẩu tài khoản "${title}" ?`,
            icon: "warning",
            buttons: {
                cancel: {
                    text: "Hủy",
                    value: false,
                    visible: true,
                    className: "",
                    closeModal: true,
                },
                confirm: {
                    text: "Đặt lại mật khẩu",
                    value: true,
                    visible: true,
                    className: "btn-danger",
                    closeModal: true,
                },
            },
            dangerMode: true,
        }).then((confirmDelete) => {
            if (confirmDelete) {
                var requestData = {
                    Id: id,
                };
                dataservice.reset_password(requestData, function (result) {
                    if (!result.Error) {
                        showAlert('success', "Đặt lại mật khẩu thành công!");
                        $rootScope.ReloadData();
                    } else {
                        showAlert('danger', result.Title);
                    }
                });
            } else {
                console.log("Hủy hành động xóa");
            }
        });
    };
});
// Controller điều khiển
app.controller('add', function ($scope, $rootScope, $uibModalInstance, dataservice, data) {
    $scope.title = data.message; // Nhận dữ liệu truyền vào
    $scope.model = {
        //StartTime: $rootScope.convertToJSONDate(new Date()),
        UserNo: null,
        UserName: null,
        FullName: null,
        Department_id: null,
        JobTitle_id: null,
        Email: null,
        Phone: null,
        IsActive: null,
        Avatar: [],
    };
    // Hàm đóng modal
    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
    $scope.ListDepartment = [];
    $scope.ListPosition = [];

    dataservice.getNumberUserNoAuto(function (result) {
        $scope.model.UserNo = result.NextUserNo;
    });
    dataservice.getDepartment(function (result) {
        $scope.ListDepartment = result;
    });
    dataservice.getJobTitle(function (result) {
        $scope.ListPosition = result;
    });
    $scope.onDepartmentChange = function () {
        let department = $scope.ListDepartment.find(d => d.Id == $scope.selectedDepartment);
        $scope.model.Department_id = department ? department.Id : null;
    };
    $scope.onJobTitleChange = function () {
        let jobtitle = $scope.ListPosition.find(d => d.Id == $scope.selectedPosition);
        $scope.model.JobTitle_id = jobtitle ? jobtitle.Id : null;
    };
    $scope.handleFiles = function (files) {
        if (files && files.length > 0) {
            $scope.model.Avatar = []; // Xóa danh sách file trước đó (nếu cần reset)

            // Duyệt qua từng file và thêm vào danh sách file
            for (let i = 0; i < files.length; i++) {
                $scope.model.Avatar.push(files[i]);
            }

            // Hiển thị ảnh đầu tiên (nếu cần)
            var reader = new FileReader();
            reader.onload = function (e) {
                $scope.$apply(function () {
                    $scope.model.DisplayImage = e.target.result; // Lưu chuỗi base64 cho mục đích hiển thị
                });
            };
            reader.readAsDataURL(files[0]); // Chỉ đọc file đầu tiên để hiển thị
        }
    };
    $scope.submit = function (event) {
        event.preventDefault();
        var formName = document.activeElement.form.name;
        if (formName === 'addform') {
            showLoading();
            var fd = new FormData();
            $scope.lstTitlefile = [];
            var list = [];
            var objchung = {
                user_no: $scope.model.UserNo,
                UserName: $scope.model.UserName,
                FullName: $scope.model.FullName,
                DepartmentId: $scope.model.Department_id,
                JobTitleId: $scope.model.JobTitle_id,
                Email: $scope.model.Email,
                Phone: $scope.model.Phone,
                Avatar: null,
                IsActive: $scope.model.IsActive
            }
            if ($scope.model.Avatar && $scope.model.Avatar.length > 0) {
                for (var i = 0; i < $scope.model.Avatar.length; i++) {
                    fd.append('files', $scope.model.Avatar[i]); // Chỉ thêm file gốc
                }
            }

            fd.append('submit', JSON.stringify(objchung));
            $.ajax({
                type: "POST",
                url: "/ManagementUser/Insert",
                contentType: false,
                processData: false,
                data: fd,
                success: function (rs) {
                    if (!rs.Error) {
                        showAlert('success', rs.Title);
                        $uibModalInstance.dismiss('cancel');
                        $rootScope.ReloadData();
                        hideLoading();
                    } else {
                        showAlert('danger', rs.Title);
                        hideLoading();
                    }
                },
                Error: function (rs) {
                    showAlert('danger', "Có lỗi xảy ra!");
                }
            });
        };
    }
});
app.controller('edit', function ($scope, $rootScope, $uibModalInstance, dataservice, data) {
    $scope.title = data.message;
    $scope.model = {
        StartTime: $rootScope.convertToJSONDate(new Date()),
        UserName: null,
        FullName: null,
        Department_id: null,
        JobTitle_id: null,
        Email: null,
        Phone: null,
        IsActive: null,
        Avatar: [],
    };
    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
    $scope.handleFiles = function (files) {
        if (files && files.length > 0) {
            $scope.model.Avatar = []; // Xóa danh sách file trước đó (nếu cần reset)

            // Duyệt qua từng file và thêm vào danh sách file
            for (let i = 0; i < files.length; i++) {
                $scope.model.Avatar.push(files[i]);
            }

            // Hiển thị ảnh đầu tiên (nếu cần)
            var reader = new FileReader();
            reader.onload = function (e) {
                $scope.$apply(function () {
                    $scope.model.DisplayImage = e.target.result; // Lưu chuỗi base64 cho mục đích hiển thị
                });
            };
            reader.readAsDataURL(files[0]); // Chỉ đọc file đầu tiên để hiển thị
        }
    };
    $scope.ListDepartment = [];
    $scope.ListPosition = [];

    $scope.init = function (id) {

        var requestData = { Id: id };

        // Tạo Promise bọc quanh lời gọi API
        function getDepartments() {
            return new Promise(function (resolve, reject) {
                dataservice.getDepartment(function (result) {
                    if (result) {
                        $scope.ListDepartment = result; // Lưu kết quả vào scope
                        resolve(); // Hoàn thành Promise
                    } else {
                        reject("Không thể tải danh sách phòng ban!");
                    }
                });
            });
        }

        function getJobTitles() {
            return new Promise(function (resolve, reject) {
                dataservice.getJobTitle(function (result) {
                    if (result) {
                        $scope.ListPosition = result; // Lưu kết quả vào scope
                        resolve(); // Hoàn thành Promise
                    } else {
                        reject("Không thể tải danh sách chức danh!");
                    }
                });
            });
        }

        Promise.all([getDepartments(), getJobTitles()])
            .then(function () {
                // Khi tất cả lời gọi hoàn tất
                dataservice.getItem(requestData, function (rs) {
                    if (rs.Error) {
                        alert("Có lỗi xảy ra!");
                    } else {
                        if (rs && rs.length > 0) {
                            $scope.model = rs[0];
                            $scope.model.IsActive = $scope.model.IsActive ? "1" : "0";
                            $scope.model.UserNo = $scope.model.user_no;
                            $scope.model.Department_id = $scope.model.DepartmentId ? $scope.model.DepartmentId.toString() : '';
                            $scope.model.Position_id = $scope.model.JobTitleId ? $scope.model.JobTitleId.toString() : '';
                            $scope.selectedDepartment = $scope.model.DepartmentId ? $scope.model.DepartmentId.toString() : '';
                            $scope.selectedPosition = $scope.model.JobTitleId ? $scope.model.JobTitleId.toString() : '';
                            const baseUrl = window.location.origin; // Lấy domain hiện tại
                            $scope.model.DisplayImage = baseUrl + $scope.model.Avatar;
                            $scope.model.Phone = parseInt($scope.model.Phone);
                        } else {
                            alert("Không tìm thấy dữ liệu!");
                        }
                    }
                });
            })
            .catch(function (error) {
                // Xử lý lỗi nếu có bất kỳ lỗi nào xảy ra
                console.error(error);
                alert(error);
            });
    };
    $scope.init(data.id_item);
    $scope.onDepartmentChange = function () {
        let department = $scope.ListDepartment.find(d => d.Id == $scope.selectedDepartment);
        $scope.model.Department_id = department ? department.Id : null;
    };
    $scope.onJobTitleChange = function () {
        let jobtitle = $scope.ListPosition.find(d => d.Id == $scope.selectedPosition);
        $scope.model.Position_id = jobtitle ? jobtitle.Id : null;
    };
    $scope.submit = function (event) {
        event.preventDefault();
        var formName = document.activeElement.form.name;
        if (formName === 'editform') {
            showLoading();
            var fd = new FormData();
            $scope.lstTitlefile = [];
            var list = [];
            var objchung = {
                Id: $scope.model.Id,
                UserName: $scope.model.UserName,
                FullName: $scope.model.FullName,
                DepartmentId: $scope.model.Department_id,
                JobTitleId: $scope.model.Position_id,
                Email: $scope.model.Email,
                Phone: $scope.model.Phone,
                Avatar: null,
                IsActive: $scope.model.IsActive
            }
            if ($scope.model.Avatar && $scope.model.Avatar.length > 0) {
                for (var i = 0; i < $scope.model.Avatar.length; i++) {
                    fd.append('files', $scope.model.Avatar[i]); // Chỉ thêm file gốc
                }
            }
            fd.append('submit', JSON.stringify(objchung));
            $.ajax({
                type: "POST",
                url: "/ManagementUser/Update",
                contentType: false,
                processData: false,
                data: fd,
                success: function (rs) {
                    if (!rs.Error) {
                        showAlert('success', rs.Title);
                        $uibModalInstance.dismiss('cancel');
                        $rootScope.ReloadData();
                        hideLoading();
                    } else {
                        alert(rs.Title);
                        hideLoading();
                    }
                },
                Error: function (rs) {
                    alert("Có lỗi xảy ra trong quá trình gửi yêu cầu.");
                }
            });
        };
    }
});
app.controller('open', function ($scope, $rootScope, $uibModalInstance, dataservice, data) {
    $scope.title = data.message;
    $scope.model = {
        StartTime: $rootScope.convertToJSONDate(new Date()),
        UserName: null,
        FullName: null,
        Department_id: null,
        JobTitle_id: null,
        Email: null,
        Phone: null,
        IsActive: null,
        Avatar: [],
    };
    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
    $scope.handleFiles = function (files) {
        if (files && files.length > 0) {
            $scope.model.Avatar = []; // Xóa danh sách file trước đó (nếu cần reset)

            // Duyệt qua từng file và thêm vào danh sách file
            for (let i = 0; i < files.length; i++) {
                $scope.model.Avatar.push(files[i]);
            }

            // Hiển thị ảnh đầu tiên (nếu cần)
            var reader = new FileReader();
            reader.onload = function (e) {
                $scope.$apply(function () {
                    $scope.model.DisplayImage = e.target.result; // Lưu chuỗi base64 cho mục đích hiển thị
                });
            };
            reader.readAsDataURL(files[0]); // Chỉ đọc file đầu tiên để hiển thị
        }
    };
    $scope.ListDepartment = [];
    $scope.ListPosition = [];
    $scope.init = function (id) {

        var requestData = { Id: id };

        // Tạo Promise bọc quanh lời gọi API
        function getDepartments() {
            return new Promise(function (resolve, reject) {
                dataservice.getDepartment(function (result) {
                    if (result) {
                        $scope.ListDepartment = result; // Lưu kết quả vào scope
                        resolve(); // Hoàn thành Promise
                    } else {
                        reject("Không thể tải danh sách phòng ban!");
                    }
                });
            });
        }

        function getJobTitles() {
            return new Promise(function (resolve, reject) {
                dataservice.getJobTitle(function (result) {
                    if (result) {
                        $scope.ListPosition = result; // Lưu kết quả vào scope
                        resolve(); // Hoàn thành Promise
                    } else {
                        reject("Không thể tải danh sách chức danh!");
                    }
                });
            });
        }

        Promise.all([getDepartments(), getJobTitles()])
            .then(function () {
                // Khi tất cả lời gọi hoàn tất
                dataservice.getItem(requestData, function (rs) {
                    if (rs.Error) {
                        alert("Có lỗi xảy ra!");
                    } else {
                        if (rs && rs.length > 0) {
                            $scope.model = rs[0];
                            $scope.model.IsActive = $scope.model.IsActive ? "1" : "0";
                            $scope.model.UserNo = $scope.model.user_no;
                            $scope.model.Department_id = $scope.model.DepartmentId ? $scope.model.DepartmentId.toString() : '';
                            $scope.model.Position_id = $scope.model.JobTitleId ? $scope.model.JobTitleId.toString() : '';
                            $scope.selectedDepartment = $scope.model.DepartmentId ? $scope.model.DepartmentId.toString() : '';
                            $scope.selectedPosition = $scope.model.JobTitleId ? $scope.model.JobTitleId.toString() : '';
                            const baseUrl = window.location.origin; // Lấy domain hiện tại
                            $scope.model.DisplayImage = baseUrl + $scope.model.Avatar;
                            $scope.model.Phone = parseInt($scope.model.Phone);
                        } else {
                            alert("Không tìm thấy dữ liệu!");
                        }
                    }
                });
            })
            .catch(function (error) {
                // Xử lý lỗi nếu có bất kỳ lỗi nào xảy ra
                console.error(error);
                alert(error);
            });
    };
    $scope.init(data.id_item);
});
app.controller('addpermission', function ($scope, $rootScope, $uibModalInstance, dataservice, data) {
    $scope.title = data.message + ""; // Nhận dữ liệu truyền vào
    $scope.model = {
        userId: data.id_item,
    };
    $scope.jxcelAddDetailInit = null;
    // Hàm đóng modal
    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
    function Load_Controller() {
        return new Promise((resolve, reject) => {
            dataservice.loadController(function (result) {
                if (result && Array.isArray(result.data)) {
                    const controllerData = result.data.map(item => ({
                        id: item.id,
                        name: item.name
                    }));
                    resolve(controllerData);
                } else {
                    console.error('Dữ liệu trả về không hợp lệ:', result);
                    reject('Dữ liệu không hợp lệ');
                }
            }, function (error) {
                console.error('Lỗi khi gọi API:', error);
                reject(error);
            });
        });
    }

    // Hàm tạo bảng jspreadsheet
    async function createSpreadsheet(data) {
        const spreadsheetElement = document.getElementById('spreadsheet');
        if (!spreadsheetElement) {
            console.error('Phần tử #spreadsheet không tồn tại.');
            return;
        }
        const controllerData = await Load_Controller();
        $scope.jxcelAddDetailInit = jspreadsheet(spreadsheetElement, {
            worksheets: [{
                data: data,
                columns: [
                    { type: 'text', width: '0px', title: 'id' },
                    {
                        type: 'dropdown', width: '200px', title: 'Controller', source: controllerData, autocomplete: true, readOnly: true, name: 'MenuId'
                    },
                    { type: 'checkbox', title: 'Full Control', name: 'full_control' },
                    { type: 'checkbox', title: 'Access', name: 'Access' },
                    { type: 'checkbox', title: 'View', name: 'View' },
                    { type: 'checkbox', title: 'Insert', name: 'Insert' },
                    { type: 'checkbox', title: 'Edit', name: 'Edit' },
                    { type: 'checkbox', title: 'Delete', name: 'Delete' },
                ],
                style: (() => {
                    const styles = {};
                    for (let i = 0; i < 100; i++) { // Giả sử bạn có tối đa 100 hàng
                        styles[`B${i + 1}`] = 'background-color: orange; color: black;text-align: left;';
                    }
                    return styles;
                })(),
            }],
            contextMenu: function () {
                return false;
            },
            minSpareRows: 0, // Không thêm hàng tự động
            onbeforeinsertrow: function (instance, rowNumber, amount) {
                return false; // Hủy sự kiện thêm hàng
            },
            onchange: function (instance, cell, x, y, value) {
                if (x === "2") { // "Full Control" là cột thứ 3 (chỉ số bắt đầu từ 0)
                    const startCol = 3; // Bắt đầu từ cột "Access"
                    const endCol = 7; // Kết thúc ở cột "Delete"
                    if (value === true) {
                        for (let i = startCol; i <= endCol; i++) {
                            instance.setValueFromCoords(i, y, true); // Đặt giá trị của ô thành true
                        }
                    } else {
                        for (let i = startCol; i <= endCol; i++) {
                            instance.setValueFromCoords(i, y, false); // Đặt giá trị của ô thành false
                        }
                    }
                }
            },
        });
        document.getElementById('spreadsheet').addEventListener('keydown', function (e) {
            if (e.key === 'Enter') {
                e.preventDefault(); // Ngăn hành động mặc định
            }
        });
        console.log('Đối tượng Jexcel:',);
    }
    $scope.init = async function (id) {
        $scope.lstdata = []; // Dữ liệu đầu vào cho bảng

        var requestData = { Id: id };
        try {
            await new Promise((resolve, reject) => {
                dataservice.get_item_permission(requestData, function (rs) {
                    if (rs.Error) {
                        alert("Có lỗi xảy ra!");
                        reject('Error in API response');
                    } else {
                        if (rs && rs.length > 0) {
                            for (var i = 0; i < rs.length; i++) {
                                var row = [
                                    rs[i].id,
                                    rs[i].MenuId,
                                    rs[i].full_control,
                                    rs[i].access,
                                    rs[i].view,
                                    rs[i].insert,
                                    rs[i].edit,
                                    rs[i].delete
                                ];
                                $scope.lstdata.push(row);
                            }
                            resolve(); // Dữ liệu đã sẵn sàng
                        } else {
                            alert("Không tìm thấy dữ liệu!");
                            reject('No data found');
                        }
                    }
                });
            });

            await createSpreadsheet($scope.lstdata);
        } catch (error) {
            console.error('Error in init:', error);
        }
    };
    $scope.init(data.id_item);
    $scope.submit = function () {
        event.preventDefault();
        var formName = document.activeElement.form.name;
        if (formName === 'addpermission') {
            showLoading();
            var fd = new FormData();
            var List_permission = [];

            var datatable = $scope.jxcelAddDetailInit[0].getData();
            var columns = $scope.jxcelAddDetailInit[0].options.columns;
            console.log("Dữ liệu lấy được từ bảng:", datatable);
            for (var i = 0; i < datatable.length; i++) {
                var row = datatable[i];

                List_permission.push({
                    userId: $scope.model.userId,
                    Id: row[0],
                    MenuId: row[1],
                    FullControl: row[2] ? 1 : 0,
                    Access: row[3] ? 1 : 0,
                    View: row[4] ? 1 : 0,
                    Insert: row[5] ? 1 : 0,
                    Edit: row[6] ? 1 : 0,
                    Delete: row[7] ? 1 : 0
                });
            };

            fd.append('UpdatePermission', JSON.stringify(List_permission));
            $.ajax({
                type: "POST",
                url: "/ManagementUser/UpdatePermission",
                contentType: false,
                processData: false,
                data: fd,
                success: function (rs) {
                    if (!rs.Error) {
                        showAlert('success', rs.Title);
                        $uibModalInstance.dismiss('cancel');
                        $rootScope.ReloadData();
                        hideLoading();
                    } else {
                        alert(rs.Title);
                        hideLoading();
                    }
                },
                Error: function (rs) {
                    alert("Có lỗi xảy ra trong quá trình gửi yêu cầu.");
                }
            });
        };
    }
});



