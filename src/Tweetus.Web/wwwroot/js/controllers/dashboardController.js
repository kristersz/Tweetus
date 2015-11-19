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
