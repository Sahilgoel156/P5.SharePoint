angular.module('SharePoint')
    .factory('SharePoint.webApi', ['$resource', function ($resource) {
        return $resource('api/share-point');
    }]);
