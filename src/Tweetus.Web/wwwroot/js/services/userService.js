(function () {
    'use strict';

    angular
        .module('tweetusApp')
        .factory('userService', userService);

    userService.$inject = ['$http', '$q'];

    function userService($http, $q) {
        var service = {
            searchUsers: searchUsers,
            followUser: followUser
        };

        return service;

        function searchUsers(searchParam) {
            var request = $http({
                method: "post",
                url: "/User/SearchUsers",
                params: {
                    searchParam: searchParam
                }
            });

            return (request.then(handleSuccess, handleError));
        }

        function followUser(userId) {
            var request = $http({
                method: "post",
                url: "/User/FollowUser",
                params: {
                    userId: userId
                }
            });

            return (request.then(handleSuccess, handleError));
        }

        function handleError(response) {
            if (!angular.isObject(response.data) || !response.data.message) {
                return ($q.reject("An unknown error occurred."));
            }

            return ($q.reject(response.data.message));
        }

        function handleSuccess(response) {
            return (response.data);
        }
    }
})();