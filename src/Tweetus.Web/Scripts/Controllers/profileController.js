(function () {
    'use strict';

    angular
        .module('tweetusApp')
        .controller('profileController', profileController);

    profileController.$inject = ['$scope', 'tweetService', 'userService'];

    function profileController($scope, tweetService, userService) {
        $scope.title = 'profileController';
        $scope.currentTweet = "";

        $scope.init = function (model) {
            $scope.vm = model;

            $scope.currentTweet = "@" + model.UserName;
            initFileUpload();
        }

        $scope.likeTweet = function (tweetId) {
            tweetService.likeTweet(tweetId)
                .then(function (result) {
                    
                });
        }

        $scope.followUser = function () {
            userService.followUser($scope.vm.UserId)
                .then(function (result) {
                    $scope.vm.ViewerAlreadyFollowing = true;
                });
        }

        function initFileUpload() {
            var self = this;

            $("#uploadTarget").load(function () {
                $('#tweetToUserModal').modal('hide');

                $scope.currentTweet = "@" + $scope.vm.UserName;
                $("#fileUpload").val("");

                $scope.$apply();

                var n = noty({
                    text: 'Your tweet was successfully posted!',
                    type: 'success',
                    timeout: 3000,
                    theme: 'bootstrapTheme'
                });
            });
        }
    }
})();
