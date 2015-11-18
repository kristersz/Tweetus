(function () {
    'use strict';

    angular
        .module('tweetusApp')
        .controller('profileController', profileController);

    profileController.$inject = ['$scope', 'dashboardService'];

    function profileController($scope, dashboardService) {
        $scope.title = 'profileController';
        $scope.currentTweet = "";

        $scope.init = function (model) {
            $scope.vm = model;

            $scope.currentTweet = "@" + model.UserName;
            initFileUpload();
        }

        $scope.postTweet = function () {
            dashboardService.postTweet($scope.currentTweet)
                .then(function (tweet) {
                    addTweet(tweet);
                });
        }

        $scope.followUser = function () {
            dashboardService.followUser($scope.vm.UserId)
                .then(function (result) {
                    $scope.vm.ViewerAlreadyFollowing = true;
                });
        }

        function addTweet(tweet) {
            $scope.currentTweet = "@" + $scope.vm.UserName;
        }

        function initFileUpload() {
            var self = this;

            $("#uploadTarget").load(function () {
                $('#tweetToUserModal').modal('hide');

                $scope.currentTweet = "@" + $scope.vm.UserName;
                $("#fileUpload").val("");

                $scope.$apply();
            });
        }
    }
})();
