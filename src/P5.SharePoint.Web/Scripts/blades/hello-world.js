angular.module('SharePoint')
    .controller('SharePoint.helloWorldController', ['$scope', 'SharePoint.webApi', function ($scope, api) {
        var blade = $scope.blade;
        blade.title = 'SharePoint';

        blade.refresh = function () {
            api.get(function (data) {
                blade.title = 'SharePoint.blades.hello-world.title';
                blade.data = data.result;
                blade.isLoading = false;
            });
        };

        blade.refresh();
    }]);
