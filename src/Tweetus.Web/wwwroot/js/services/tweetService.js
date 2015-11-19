(function () {
    'use strict';

    angular
        .module('tweetusApp')
        .factory('tweetService', tweetService);

    tweetService.$inject = ['$http', '$q'];

    function tweetService($http, $q) {
        var service = {
            getTweetsForDashboard: getTweetsForDashboard,
            likeTweet: likeTweet,
            followUser: followUser
        };

        return service;

        function getTweetsForDashboard(userId) {
            var request = $http({
                method: "post",
                url: "/Tweet/GetTweetsForDashboard",
                params: {
                    userId: userId
                }
            });

            return (request.then(handleSuccess, handleError));
        }

        function likeTweet(tweetId) {
            var request = $http({
                method: "post",
                url: "/Tweet/LikeTweet",
                params: {
                    tweetId: tweetId
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