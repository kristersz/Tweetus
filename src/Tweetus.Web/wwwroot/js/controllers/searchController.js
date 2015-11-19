(function () {
    'use strict';

    angular
        .module('tweetusApp')
        .controller('searchController', searchController);

    searchController.$inject = ['$scope', 'userService'];

    function searchController($scope, userService) {
        $scope.title = 'searchController';

        $scope.searchParam = "";
        $scope.result = [];
        $scope.userHasSearched = false;

        $scope.searchUsers = function () {
            userService.searchUsers($scope.searchParam)
                .then(function (result) {
                    showSearchResult(result);
                });
        }

        function showSearchResult(result) {
            $scope.userHasSearched = true;

            if (result.IsValid) {
                $scope.result = result.Value;
            }
            else {
                BootstrapDialog.alert(result.Message)
            }
        }
    }
})();
