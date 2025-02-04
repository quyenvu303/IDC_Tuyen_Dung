var app = angular.module('myApp', ['ngRoute', 'ngMessages', 'ui.bootstrap']);
app.constant('CTX_FOLDER', '/views/ManagementDepartment');
app.config(function ($routeProvider, $locationProvider, CTX_FOLDER) {
    // Sử dụng HTML5 mode để loại bỏ hash (#)
    $locationProvider.html5Mode({
        enabled: true,
        requireBase: true // Đảm bảo có thẻ <base> trong HTML
    });
    $routeProvider
        .when('/Admin/ManagementDepartment', {
            templateUrl: CTX_FOLDER + '/index.html',
            controller: 'index',
            resolve: {
                permissions: function ($rootScope) {
                    return $rootScope.loadPermissions();
                },
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
        getUserPermissions: function (data, callback) {
            callApi('POST', '/ManagementDepartment/GetUserPermissions', JSON.stringify(data), callback);
        },
        jtable: function (callback) {
            callApi('POST', '/ManagementDepartment/JTable', {}, callback);
        },
        loadDepartment: function (callback) {
            callApi('POST', '/ManagementDepartment/LoadDepartment', {}, callback);
        },
        getItem: function (data, callback) {
            callApi('POST', '/ManagementDepartment/GetItem', JSON.stringify(data), callback);
        },
        delete: function (data, callback) {
            callApi('POST', '/ManagementDepartment/Delete', JSON.stringify(data), callback);
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
app.controller('Ctrl_ManagementDepartment', function ($scope, $rootScope, $uibModal, dataservice, CTX_FOLDER) {
    $rootScope.Title = "ManagementDepartment";
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
app.controller('index', function ($scope, $rootScope, $compile, $uibModal, dataservice, CTX_FOLDER, $timeout, $http) {
    $scope.title = "Quản lý phòng ban";
    $scope.model = {};
    $scope.datatable = [];
    $scope.staticParam = {};
    // Refresh dữ liệu
    $rootScope.ReloadData = function () {
        if ($rootScope.hasPermission('View')) {
            $scope.getTable();
        }
    };
    $scope.getTable = function () {
        showLoading();
        dataservice.jtable(function (result) {
            if (result.success) {
                setTimeout(function () {
                    $scope.renderTree(result.data);
                    $('.tree-basic').treegrid({
                        initialState: 'expanded' // Hoặc 'expanded' nếu bạn muốn mở rộng mặc định
                    });
                    hideLoading();
                }, 500);
            } else {
                console.error('Error:', result.Title);
            }
        });
    };
    $scope.renderTree = function (data) {
        let tbody = document.querySelector("tbody");
        tbody.innerHTML = ""; // Clear dữ liệu cũ

        function renderRow(node, ParenId) {
            let tr = document.createElement("tr");
            tr.classList.add(`treegrid-${node.Id}`);
            // Nếu cần, bổ sung class "expanded"
            tr.classList.add('expanded');
            if (ParenId !== null && ParenId !== undefined) {
                tr.classList.add(`treegrid-parent-${ParenId}`);
            }

            tr.innerHTML = `
            <td>${node.DepartmentCode}</td>
            <td>${node.DepartmentName || ""}</td>
            <td class="align-middle text-center ps-1 status">
                <span class="badge badge-phoenix fs-10 ${node.Status === 1 ? 'badge-phoenix-success' : 'badge-phoenix-danger'}">
                    ${node.Status === 1 ? 'Sử dụng' : 'Không sử dụng'}
                </span>
            </td>
            <td class="align-middle white-space-nowrap text-end pe-0">
                <div class="btn-reveal-trigger position-static" style="text-align: center !important;">
                    <button class="btn btn-sm dropdown-toggle dropdown-caret-none transition-none btn-reveal fs-10"
                            type="button" data-bs-toggle="dropdown" data-boundary="window" aria-haspopup="true" aria-expanded="false"
                            data-bs-reference="parent" style="background-color: #8ac4ff !important;">
                        <span class="fas fa-ellipsis-h fs-10"></span>
                    </button>
                    <div class="dropdown-menu dropdown-menu-end py-2">
                            <button class="dropdown-item" ng-click="editModal(${node.Id})" >
                                Cập nhật
                            </button>
                        ${node.Status === 1  ? `
                            <button class="dropdown-item" 
                                    ng-click="delete({ id: ${node.Id}, title: '${node.DepartmentName}' })" >
                                Xóa
                            </button>
                        ` : ''}
                        ${node.Status === 0 ? `
                            <button class="dropdown-item" 
                                    ng-click="restore({ id: ${node.Id}, title: '${node.DepartmentName}' })">
                                Khôi phục
                            </button>
                        ` : ''}
                    </div>
                </div>
            </td>
        `;
            $compile(tr)($scope);
            tbody.appendChild(tr);
            // Render các node con
            data
                .filter(child => child.ParenId === node.Id)
                .forEach(child => renderRow(child, node.Id));
        }

        // Render các node gốc
        data.filter(node => node.ParenId === null).forEach(node => renderRow(node, null));

        // Kích hoạt Treegrid
        $(".tree-basic").treegrid({
            initialState: 'expanded'
        });
        
    };
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
                    return { message: "Thêm phòng ban" };
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
                    return { message: "Cập nhật danh mục", id_item: id };
                }
            }
        });
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
            text: `Bạn có chắc chắn muốn xóa phòng ban "${title}" không?`,
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
                    Status: 0,
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
            text: `Bạn có chắc chắn muốn khôi phục phòng ban "${title}" không?`,
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
                    Status: 1,
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
});
// Controller điều khiển
app.controller('add', function ($scope, $rootScope, $uibModalInstance, dataservice, data) {
    $scope.title = data.message;
    $scope.model = {
        Status: "1",
    };
    $scope.submitted = false;
    // Hàm đóng modal
    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
    $scope.List_ParenId = [];
    dataservice.loadDepartment(function (result) {
        $scope.List_ParenId = result;
    });
    $scope.ParenIdChange = function () {
        let ParenId = $scope.List_ParenId.find(d => d.Id == $scope.selectedPanrenId);
        $scope.model.ParenId = ParenId.Id ? ParenId.Id : null;
    };
    $scope.submit = function (event) {
        event.preventDefault();
        var formName = document.activeElement.form.name;
        if (formName === 'addform') {
            var fd = new FormData();
            $scope.submitted = true;
            if (!$scope.model.DepartmentCode || !$scope.model.DepartmentName) {
                return;
            }
            var objchung = {
                ParenId: $scope.model.ParenId,
                DepartmentCode: $scope.model.DepartmentCode,
                DepartmentName: $scope.model.DepartmentName,
                Status: $scope.model.Status,
            }

            fd.append('submit', JSON.stringify(objchung));
            showLoading();
            $.ajax({
                type: "POST",
                url: "/ManagementDepartment/Insert",
                contentType: false,
                processData: false,
                data: fd,
                success: function (rs) {
                    if (!rs.Error) {
                        hideLoading();
                        showAlert('success', rs.Title);
                        $uibModalInstance.dismiss('cancel');
                        $rootScope.ReloadData();
                    } else {
                        hideLoading();
                        showAlert('danger', rs.Title);
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
    $scope.model = {};
    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
    $scope.init = function (id) {

        var requestData = { Id: id };

        // Tạo Promise bọc quanh lời gọi API
        function getParentId() {
            return new Promise(function (resolve, reject) {
                dataservice.loadDepartment(function (result) {
                    if (result) {
                        $scope.List_ParenId = result; // Lưu kết quả vào scope
                        resolve(); // Hoàn thành Promise
                    } else {
                        reject("Không thể tải danh sách phòng ban!");
                    }
                });
            });
        }


        Promise.all([getParentId()])
            .then(function () {
                dataservice.getItem(requestData, function (rs) {
                    if (rs.Error) {
                        alert("Có lỗi xảy ra!");
                    } else {
                        if (rs && rs.length > 0) {
                            $scope.model = rs[0];
                            $scope.model.Status = rs[0].Status ? "1" : "0";
                            $scope.model.ParenId = rs[0].ParenId ? rs[0].ParenId.toString() : '';
                            $scope.selectedPanrenId = $scope.model.ParenId ? $scope.model.ParenId.toString() : '';
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
    $scope.ParentIdChange = function () {
        let ParentId = $scope.List_ParenId.find(d => d.Id == $scope.selectedPanrenId);
        $scope.model.ParenId = ParentId.Id ? ParentId.Id : null;
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
                ParenId: $scope.model.ParenId,
                DepartmentCode: $scope.model.DepartmentCode,
                DepartmentName: $scope.model.DepartmentName,
                Status: $scope.model.Status,
            }
            
            fd.append('submit', JSON.stringify(objchung));
            $.ajax({
                type: "POST",
                url: "/ManagementDepartment/Update",
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