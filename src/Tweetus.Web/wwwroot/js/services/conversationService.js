(function () {
    'use strict';

    angular
        .module('tweetusApp')
        .factory('conversationService', conversationService);

    conversationService.$inject = ['$http', '$q'];

    function conversationService($http, $q) {
        var service = {
            startNewConversation: startNewConversation,
            openConversation: openConversation,
            sendNewMessage: sendNewMessage
        };

        return service;

        function startNewConversation(username, message) {
            var request = $http({
                method: "post",
                url: "/Conversation/StartNewConversation",
                params: {
                    username: username,
                    message: message
                }
            });

            return (request.then(handleSuccess, handleError));
        }

        function openConversation(conversationId) {
            var request = $http({
                method: "post",
                url: "/Conversation/OpenConversation",
                params: {
                    conversationId: conversationId
                }
            });

            return (request.then(handleSuccess, handleError));
        }

        function sendNewMessage(conversationId, message) {
            var request = $http({
                method: "post",
                url: "/Conversation/SendNewMessage",
                params: {
                    conversationId: conversationId,
                    message: message
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