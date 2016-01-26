(function () {
    'use strict';

    angular
        .module('tweetusApp')
        .controller('dashboardController', controller);

    controller.$inject = ['$scope', 'tweetService'];

    function controller($scope, tweetService) {
        $scope.title = 'dashboardController';       
        $scope.currentTweet = "";

        $scope.locationEnabled = false;
        $scope.locationButtonText = 'Enable location';

        $scope.currentLatitude = 0;
        $scope.currentLongitude = 0;
        $scope.currentAddress = "";

        var currentAddressSuccessFunc = function (res) {
            $scope.$apply(function () {
                if (res.results.length > 0) {
                    $scope.currentAddress = res.results[0].formatted_address;
                }
            });
        }

        $scope.$watch('currentLatitude', function () { getAddress($scope.currentLatitude, $scope.currentLongitude, currentAddressSuccessFunc); }, true);
        $scope.$watch('currentLongitude', function () { getAddress($scope.currentLatitude, $scope.currentLongitude, currentAddressSuccessFunc); }, true);

        getLocation();

        $scope.init = function (model) {
            model.Tweets.forEach(function (tweet) {
                getTweetAddress(tweet);
            });

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

        $scope.enableDisableLocation = function () {
            if ($scope.locationEnabled) {
                $scope.locationButtonText = 'Enable location';
                $scope.locationEnabled = false;
            }
            else {
                $scope.locationButtonText = 'Disable location';
                $scope.locationEnabled = true;
            }
        }

        function getLocation() {
            if (navigator.geolocation) {
                navigator.geolocation.getCurrentPosition(function (position) {
                    var mapCanvas = document.getElementById('map');
                    var currentLatLng = new google.maps.LatLng(position.coords.latitude, position.coords.longitude);

                    var mapOptions = {
                        center: currentLatLng,
                        zoom: 12,
                        mapTypeId: google.maps.MapTypeId.ROADMAP
                    }

                    var map = new google.maps.Map(mapCanvas, mapOptions);
                    var marker = new google.maps.Marker({
                        position: currentLatLng,
                        map: map,
                        title: 'Location'
                    });

                    $("#mapModal").on("shown.bs.modal", function () {
                        google.maps.event.trigger(map, "resize");
                    });

                    google.maps.event.addListener(map, "click", function (event) {
                        marker.setPosition(event.latLng);

                        $scope.$apply(function () {
                            $scope.currentLatitude = event.latLng.lat();
                            $scope.currentLongitude = event.latLng.lng();
                        });
                    });

                    getAddress(position.coords.latitude, position.coords.longitude, currentAddressSuccessFunc);

                    $scope.$apply(function () {
                        $scope.currentLatitude = position.coords.latitude;
                        $scope.currentLongitude = position.coords.longitude;
                    });
                });

            } else {
                alert("Geolocation is not supported by this browser.");
            }
        }

        function getAddress(latitude, longitude, successFunc) {
            $.ajax({
                url: 'https://maps.googleapis.com/maps/api/geocode/json?latlng=' + latitude + ',' + longitude + '&key=AIzaSyDh191LxSw1lPNMWDVoOvOVEOug2Nqly5U',
                type: 'GET',
                success: successFunc
            });
        }

        function getTweetAddress(tweet) {
            if (tweet.Latitude > 0 && tweet.Longitude > 0) {
                getAddress(tweet.Latitude, tweet.Longitude, function (res) {
                    if (res.results.length > 0) {
                        tweet.Address = res.results[0].formatted_address;
                    }
                });
            }
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
            result.Value.forEach(function (tweet) {
                getTweetAddress(tweet);
            });

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

                var result = $.parseJSON($("#uploadTarget").contents().text());

                if (result.IsValid) {
                    $("#collapseMainTweet").collapse("hide");
                    $("#fileUpload").val("");

                    $scope.currentTweet = "";

                    var n = noty({
                        text: 'Your tweet was successfully posted!',
                        type: 'success',
                        timeout: 3000,
                        theme: 'bootstrapTheme'
                    });
                }
                else {
                    BootstrapDialog.alert(result.Message);
                }
            });
        }
    }
})();
