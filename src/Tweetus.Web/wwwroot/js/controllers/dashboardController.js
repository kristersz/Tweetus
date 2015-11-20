(function () {
    'use strict';

    angular
        .module('tweetusApp')
        .controller('dashboardController', controller);

    controller.$inject = ['$scope', 'tweetService'];

    function controller($scope, tweetService) {
        $scope.title = 'dashboardController';       
        $scope.currentTweet = "";

        $scope.init = function (model) {
            $scope.vm = model;
            initFileUpload();
        }

        $scope.likeTweet = function (tweet) {
            tweetService.likeTweet(tweet.TweetId)
                .then(function (result) {
                    proccessLikedTweet(result, tweet);
                });
        }

        $scope.openReply = function(tweet) {
            $scope.currentTweet = "@" + tweet.TweetedByUserName;
            $("#" + tweet.TweetId).collapse("show");
        }

        $scope.retweet = function (tweet) {
            tweetService.retweet(tweet.TweetId)
                .then(function (result) {
                    proccessRetweet(result, tweet);
                });
        }

        function proccessLikedTweet(result, tweet) {
            if (result.IsValid) {
                tweet.CurrentUserAlreadyLiked = true;
            }
        }

        function proccessRetweet(result, tweet) {
            if (result.IsValid) {
                loadTweets();
            }
        }

        function listTweets(result) {
            $scope.vm.Tweets = result.Value;
        }

        function loadTweets() {
            tweetService.getTweetsForDashboard()
                .then(function (result) {
                    listTweets(result);
                });
        }

        function initFileUpload() {
            var self = this;

            $("#uploadTarget").load(function () {
                loadTweets();

                $("#collapseMainTweet").collapse("hide");
                $("#fileUpload").val("");

                $scope.currentTweet = "";              

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
