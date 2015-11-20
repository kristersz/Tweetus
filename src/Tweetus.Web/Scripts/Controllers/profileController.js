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

        $scope.likeTweet = function (tweet) {
            tweetService.likeTweet(tweet.TweetId)
                .then(function (result) {
                    proccessLikedTweet(result, tweet);
                });
        }

        $scope.openReply = function (tweet) {
            $scope.currentTweet = "@" + tweet.TweetedByUserName;
            $("#" + tweet.TweetId).collapse("show");
        }

        function proccessLikedTweet(result, tweet) {
            if (result.IsValid) {
                tweet.CurrentUserAlreadyLiked = true;
            }            
        }

        $scope.followUser = function () {
            userService.followUser($scope.vm.UserId)
                .then(function (result) {
                    $scope.vm.CurrentUserAlreadyFollowing = true;
                });
        }

        function initFileUpload() {
            var self = this;

            $("#uploadTarget").load(function () {
                $('#tweetToUserModal').modal('hide');
                $(".panel-collapse").collapse("hide");

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
