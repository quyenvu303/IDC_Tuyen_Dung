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
            controller: 'index'
        })
        .otherwise({
            redirectTo: '/Admin/ManagementUser'
        })
        .otherwise({
            redirectTo: '/'
        });
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
        jtable: function (data, callback) {
            callApi('POST', '/JTable', JSON.stringify(data), callback);
        },
        getDepartment: function (callback) {
            callApi('POST', '/GetDepartment', {}, callback);
        },
        getJobTitle: function (callback) {
            callApi('POST', '/GetJobTitle', {}, callback);
        },
        getItem: function (data, callback) {
            callApi('POST', '/GetItem', JSON.stringify(data), callback);
        },
        delete: function (data, callback) {
            callApi('POST', '/Delete', JSON.stringify(data), callback);
        },
    };
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
app.controller('index', function ($scope, $rootScope, $uibModal, dataservice, CTX_FOLDER) {
    $scope.model = {};
    $scope.datatable = [];
    var ctrl = $scope;

    //Table
    $scope.pageIndex = 1;    // Trang hiện tại
    $scope.pageSize = 10;    // Số bản ghi mỗi trang
    $scope.totalPages = 0;   // Tổng số trang
    $scope.getTable = function () {
        var requestData = {
            id: null,
            pageIndex: $scope.pageIndex,
            pageSize: $scope.pageSize
        };
        dataservice.jtable(requestData, function (response) {
            if (!response.Error) {
                $scope.List_ManagementUser = response.Data;
                $scope.totalPages = Math.ceil(response.TotalRecords / $scope.pageSize);
            } else {
                console.error('Error:', response.Title);
            }
        });
    };
    // Chuyển sang trang trước
    $scope.previousPage = function () {
        if ($scope.pageIndex > 1) {
            $scope.pageIndex--;
            $scope.getTable();
        }
    };

    // Chuyển sang trang tiếp theo
    $scope.nextPage = function () {
        if ($scope.pageIndex < $scope.totalPages) {
            $scope.pageIndex++;
            $scope.getTable();
        }
    };
    // Gọi hàm lấy dữ liệu ngay khi controller được khởi tạo
    $scope.getTable();
    $rootScope.ReloadData = function () {
        $scope.getTable();
    };
    // Hàm mở modal
    $scope.addModal = function () {
        
        var modalInstance = $uibModal.open({
            templateUrl: CTX_FOLDER + '/add.html',
            controller: 'add',
            backdrop: 'static',
            keyboard: true,
            //size: 'lg', // Kích thước: 'sm', 'lg', hoặc 'xl'
            windowClass: 'custom-modal-size',
            resolve: {
                data: function () {
                    return { message: "Thêm người dùng" };
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

    $scope.delete = function (temp) {
        const { id, title } = temp; 
        const confirmDelete = confirm(`Bạn có chắc chắn muốn xóa bản ghi có tiêu đề "${title}" không?`);

        if (confirmDelete) {
            var requestData = { Id: id };
            dataservice.delete(requestData, function (result) {
                if (!result.Error) {
                    alert(result.Title);
                    $rootScope.ReloadData();
                } else {
                    alert(result.Title);
                }
            });
        } else {
            // Người dùng hủy hành động xóa
            console.log("Hủy hành động xóa");
        }
    };

});
// Controller điều khiển
app.controller('add', function ($scope, $rootScope, $uibModalInstance, dataservice, data) {
    $scope.title = data.message; // Nhận dữ liệu truyền vào
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
    // Hàm đóng modal
    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
    $scope.ListDepartment = [];
    dataservice.getDepartment(function (result) {
        $scope.ListDepartment = result;
    });
    dataservice.getJobTitle(function (result) {
        $scope.ListJobTitle = result;
    });
    $scope.onDepartmentChange = function () {
        $scope.model.Department_id = document.querySelector('select').value;
    };
    $scope.onJobTitleChange = function () {
        $scope.model.JobTitle_id = document.querySelector('select').value;
    };
    $scope.handleFiles = function (files) {
        if (files && files.length > 0) {
            var reader = new FileReader();
            reader.onload = function (e) {
                $scope.$apply(function () {
                    $scope.model.Avatar = e.target.result; // Hiển thị ảnh mới
                });
            };
            reader.readAsDataURL(files[0]); // Chỉ xử lý file đầu tiên
            $scope.model.Avatar = []; // Reset danh sách file
                for (let i = 0; i < files.length; i++) {
                    $scope.model.Avatar.push(files[i]); // Lưu từng file vào danh sách
                }
        }
    };
    $scope.submit = function () {
        var formName = document.activeElement.form.name;
        if (formName === 'addform') {

            var fd = new FormData();
            $scope.lstTitlefile = [];
            var list = [];
            var objchung = {
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
                    fd.append('files', $scope.model.Avatar[i]); // Dùng key 'files' cho nhiều file
                }
            }

            fd.append('data_model', JSON.stringify($scope.model));
            fd.append('submit', JSON.stringify(objchung));

            $.ajax({
                type: "POST",
                url: "/Insert",
                contentType: false,
                processData: false,
                data: fd,
                success: function (rs) {
                    if (!rs.Error) {
                        alert(rs.Title);
                    } else {
                        alert(rs.Title);
                    }
                },
                Error: function (rs) {
                    alert("Có lỗi xảy ra trong quá trình gửi yêu cầu.");
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
            var reader = new FileReader();
            reader.onload = function (e) {
                $scope.$apply(function () {
                    $scope.model.Img = e.target.result; // Hiển thị ảnh mới
                });
            };
            reader.readAsDataURL(files[0]); // Chỉ xử lý file đầu tiên
        }
        $scope.model.Avatar = []; // Reset danh sách file
        for (let i = 0; i < files.length; i++) {
            $scope.model.Avatar.push(files[i]); // Lưu từng file vào danh sách
        }
    };

    $scope.ListDepartment = [];
    $scope.init = function (id) {
        dataservice.getDepartment(function (result) {
            $scope.ListDepartment = result;
        });
        dataservice.getJobTitle(function (result) {
            $scope.ListJobTitle = result;
        });
        var requestData = { Id: id };
        dataservice.getItem(requestData, function (rs) {
            if (rs.Error) {
                alert("Có lỗi xảy ra!");
            } else {
                if (rs && rs.length > 0) {
                    $scope.model = rs[0]; 
                    $scope.model.IsActive = $scope.model.IsActive ? "1" : "0";
                    $scope.model.Department_id = $scope.model.DepartmentId.toString(); // Gán Department_id
                    $scope.model.JobTitle_id = $scope.model.JobTitleId.toString(); 
                    const baseUrl = window.location.origin; // Lấy domain hiện tại
                    $scope.model.Img = baseUrl + $scope.model.Avatar;
                } else {
                    alert("Không tìm thấy dữ liệu!");
                }
            }
        });
    };
    $scope.init(data.id_item);
    $scope.onDepartmentChange = function () {
        //$scope.model.Department_id = document.querySelector('select').value;
    };
    $scope.onJobTitleChange = function () {
        //$scope.model.JobTitle_id = document.querySelector('select').value;
    };

    $scope.submit = function () {
        var formName = document.activeElement.form.name;
        if (formName === 'editform') {
            var fd = new FormData();
            $scope.lstTitlefile = [];
            var list = [];
            var objchung = {
                Id: $scope.model.Id,
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
                    fd.append('files', $scope.model.Avatar[i]); // Dùng key 'files' cho nhiều file
                }
            }
            fd.append('data_model', JSON.stringify($scope.model));
            fd.append('submit', JSON.stringify(objchung));
            $.ajax({
                type: "POST",
                url: "/Update",
                contentType: false,
                processData: false,
                data: fd,
                success: function (rs) {
                    if (!rs.Error) {
                        alert(rs.Title);
                        $uibModalInstance.dismiss('cancel');
                        $rootScope.ReloadData();
                    } else {
                        alert(rs.Title);
                    }
                },
                Error: function (rs) {
                    alert("Có lỗi xảy ra trong quá trình gửi yêu cầu.");
                }
            });


        };
    }
});