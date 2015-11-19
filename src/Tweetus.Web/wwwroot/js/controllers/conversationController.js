(function () {
    'use strict';

    angular
        .module('tweetusApp')
        .controller('conversationController', conversationController);

    conversationController.$inject = ['$scope', 'conversationService'];

    function conversationController($scope, conversationService) {
        $scope.title = 'conversationController';

        $scope.vm = [];
        $scope.recipient = "";
        $scope.message = "";

        $scope.openConversation = {
            id: "",
            conversationName: "",
            messages: []
        }

        $scope.init = function (model) {
            $scope.vm = model;
        }

        $scope.startNewConversation = function () {
            conversationService.startNewConversation($scope.recipient, $scope.message)
                .then(function (result) {
                    addConversation(result);
                });
        }

        function addConversation(result) {
            if (!result.IsValid) {
                BootstrapDialog.alert(result.Message)
            }
            else {
                $scope.vm.push(result.Value);

                $('#newConversationModal').modal('hide');

                $scope.recipient = "";
                $scope.message = "";
            }            
        }

        $scope.openConversation = function (conversationId) {
            conversationService.openConversation(conversationId)
                .then(function (conversation) {
                    openConversationModal(conversation);
                });
        }

        function openConversationModal(conversation) {
            $scope.openConversation.id = conversation.Value.ConversationId;
            $scope.openConversation.conversationName = conversation.Value.Name;
            $scope.openConversation.messages = conversation.Value.Messages;

            $('#conversationModal').modal('show');
        }

        $scope.sendNewMessage = function () {
            conversationService.sendNewMessage($scope.openConversation.id, $scope.message)
                .then(function (message) {
                    addMessage(message);
                });
        }

        function addMessage(message) {
            $scope.message = "";

            $scope.openConversation.messages.push(message.Value);
        }
    }
})();
