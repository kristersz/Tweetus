(function () {
    'use strict';

    angular
        .module('tweetusApp')
        .factory('dashboardService', dashboardService);

    dashboardService.$inject = ['$http', '$q'];

    function dashboardService($http, $q) {
        var service = {
            getTweets: getTweets,
            postTweet: postTweet,
            followUser: followUser
        };

        return service;

        function getTweets(userId) {
            var request = $http({
                method: "get",
                url: "GetTweets",
                params: {
                    userId: userId
                }
            });

            return (request.then(handleSuccess, handleError));
        }

        function postTweet(content) {
            var request = $http({
                method: "post",
                url: "PostTweet",
                params: {
                    Content: content
                }
            });

            return (request.then(handleSuccess, handleError));
        }

        function followUser(userId) {
            var request = $http({
                method: "post",
                url: "FollowUser",
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