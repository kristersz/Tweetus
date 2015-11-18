(function () {
    'use strict';

    angular
        .module('tweetusApp')
        .controller('dashboardController', controller);

    controller.$inject = ['$scope', 'dashboardService']; 

    function controller($scope, dashboardService) {
        $scope.title = 'dashboardController';       
        $scope.currentTweet = "";

        $scope.init = function (model) {
            $scope.vm = model;
            initFileUpload();
        }

        function listTweets(tweets) {
            $scope.tweets = tweets;
        }

        function loadTweets() {
            dashboardService.getTweets()
                .then(function (tweets) {
                    listTweets(tweets);
                });
        }        

        $scope.postTweet = function () {
            dashboardService.postTweet($scope.currentTweet)
                .then(function (tweet) {
                    addTweet(tweet);
                });
        }

        function addTweet(tweet) {
            $scope.currentTweet = "";
            $scope.vm.Tweets.push(tweet);

            $("#collapseOne").collapse("hide");
        }

        function initFileUpload() {
            var self = this;

            $("#uploadTarget").load(function () {
                //AprHelper.hideAjaxLoading();

                var contents = $("#uploadTarget").contents().text();

                //if (contents.indexOf("Maximum request length exceeded") >= 0) {
                //    showMessage("Datnes izmērs nevar būt lielāks par 3 MB.");
                //}

                var data = JSON.parse(contents);

                $("#collapseOne").collapse("hide");

                $scope.currentTweet = "";
                $("#fileUpload").val("");

                $scope.vm.Tweets.push(data);                
                $scope.$apply();
            });
        }
    }
})();
