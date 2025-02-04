var app = angular.module('myApp', ['ngRoute', 'ngMessages', 'ui.bootstrap']);
app.constant('CTX_FOLDER', '/views/Home');
app.config(function ($routeProvider, $locationProvider, CTX_FOLDER) {
    // Sử dụng HTML5 mode để loại bỏ hash (#)
    $locationProvider.html5Mode({
        enabled: true,
        requireBase: true // Đảm bảo có thẻ <base> trong HTML
    });
    $routeProvider
        .when('/', {
            templateUrl: CTX_FOLDER + '/index.html',
            controller: 'index'
        })
        .otherwise({
            redirectTo: '/'
        });
});
app.factory('dataservice', function ($http, CTX_FOLDER) {
    var headers = {
        "Content-Type": "DocumentIns/json;odata=verbose",
        "Accept": "DocumentIns/json;odata=verbose",
    }
    return {
        ctxfolder: CTX_FOLDER,
    }
});

app.controller('Ctrl_Home', function ($scope, $rootScope, $uibModal, dataservice) {
    $rootScope.Title = "Trang chủ";
});

app.controller('index', function ($scope, $rootScope, $uibModal, dataservice) {
    $scope.model = {};
    $scope.datatable = [];
    var ctrl = $scope;
});