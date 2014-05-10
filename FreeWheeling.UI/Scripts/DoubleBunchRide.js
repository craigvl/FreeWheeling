var socketId = null;

function toggleChevron(e) {
    $(e).toggleClass('glyphicon-chevron-down glyphicon-chevron-up');
}

function KeenCompleteFirst(data) {
    HideProgress();
    //alert(data.message);

    var client = new WindowsAzure.MobileServiceClient(
              "http://bunchydev.azure-mobile.net/",
              "HIdMfCPoYsHPanowdOvYsYOnmbZbsq45"
               );

    var item = {
        text: data.message,
        bunchid: data.rideid,
        username: data.username,
        keencount: data.keencount,
        leavetime: data.leavetime,
        channel_id: data.parentid,
        socket_id: socketId
    };
    client.getTable("Keen").insert(item);

    UpdateKeenActions(data.message, data.rideid, data.keencount, data.username, data.leavetime);
}

function KeenCompleteNext(data) {
    HideProgress();
    //alert(data.message);

    var client = new WindowsAzure.MobileServiceClient(
              "http://bunchydev.azure-mobile.net/",
              "HIdMfCPoYsHPanowdOvYsYOnmbZbsq45"
               );

    var item = {
        text: data.message,
        bunchid: data.rideid,
        username: data.username,
        keencount: data.keencount,
        leavetime: data.leavetime,
        channel_id: data.parentid,
        socket_id: socketId
    };
    client.getTable("Keen").insert(item);

    UpdateKeenActions(data.message, data.rideid, data.keencount, data.username, data.leavetime);
}

function CommentComplete(data) {
    HideProgress();
    //Azure Mobile Storage

    //Comments

    var client = new WindowsAzure.MobileServiceClient(
              "http://bunchydev.azure-mobile.net/",
              "HIdMfCPoYsHPanowdOvYsYOnmbZbsq45"
               );

    var item = {
        text: data.message,
        bunchid: data.rideid,
        username: data.username,
        commentcount: data.commentcount,
        channel_id: data.parentid,
        socket_id: socketId
    };
    client.getTable("Item").insert(item);

    $("#collapseCommentPanel" + data.rideid).prepend("<p>" + data.username + " : " + data.message + "</p>");
    $("#CommentCountSpan" + data.rideid).html("(" + data.commentcount + ")");
    $("#CommentCountSpanSeeAll" + data.rideid).html("View All (" + data.commentcount + ")");

    //Clear comment textbox after submit for both rides.
    $("form #CommentStringFirst").val("");
    $("form #CommentStringSecond").val("");
                     
}

//IDs for collapse user remember
// 1 = FirstBunchCollapse
// 2 = FirstKeen
// 3 = FirstComment
// 4 = SecondBunchCollapse
// 5 = SecondKeen
// 6 = SecondComment

jQuery(document).ready(function () {
    HideProgress();
    //Pusher
    var rideId = jQuery('#RideIdHidden').val();
    var NextRideIdHidden = jQuery('#NextRideIdHidden').val();
    var pusher = new Pusher('dba777635636cbc16582');
    var channel = pusher.subscribe('BunchyRide' + rideId);
    var userName = jQuery('#RideUserNameHidden').val();
    var commentCount = jQuery('#CommentCount').val();
    var commentCountNext = jQuery('#CommentCountNext').val();
    var keenCount = jQuery('#KeenCount').val();
    var keenCountNext = jQuery('#KeenCountNext').val();

    channel.bind('New-Comments', function (data) {
        //alert('hello from pusher');
        $("#collapseCommentPanel" + data.rideid).prepend("<p>" + data.username + " : " + data.message + "</p>");
        $("#CommentCountSpan" + data.rideid).html("(" + data.commentcount + ")");
        $("#CommentCountSpanSeeAll" + data.rideid).html("View All (" + data.commentcount + ")");
    });

    pusher.connection.bind('connected', function () {
        socketId = pusher.connection.socket_id;
    });
  
    channel.bind('You-In', function (data) {
        UpdateKeenActionsPusher(data.message, data.rideid, data.keencount, data.username, data.leavetime);
    });
            
    //End Pusher

    jQuery("abbr.timeago").timeago();
           
    //End Azure

    //Collapse user save

    jQuery('#collapseKeenFirst').on('hidden.bs.collapse', function (event) {
        event.stopPropagation();
        toggleChevron('#collapseFirst #KeenCount');

        var data = {
            'id': 2,
            'collapsed': false
        };

        extendDataService.save(data);
    });

    jQuery('#collapseKeenFirst').on('shown.bs.collapse', function (event) {
        event.stopPropagation();
        toggleChevron('#collapseFirst #KeenCount');

        var data = {
            'id': 2,
            'collapsed': true
        };

        extendDataService.save(data);

    });

    jQuery('#collapseCommentFirst').on('hidden.bs.collapse', function (event) {
        event.stopPropagation();
        toggleChevron('#CommentCollapseArea #CommentCount');

        var data = {
            'id': 3,
            'collapsed': false
        };

        extendDataService.save(data);

    });

    jQuery('#collapseCommentFirst').on('shown.bs.collapse', function (event) {
        event.stopPropagation();
        toggleChevron('#CommentCollapseArea #CommentCount');

        var data = {
            'id': 3,
            'collapsed': true
        };

        extendDataService.save(data);

    });

    jQuery('#collapseFirst').on('hidden.bs.collapse', function (event) {
        event.stopPropagation();
        toggleChevron('#FirstDetails');

        var data = {
            'id': 1,
            'collapsed': false
        };

        extendDataService.save(data);

    });

    jQuery('#collapseFirst').on('shown.bs.collapse', function (event) {
        event.stopPropagation();
        toggleChevron('#FirstDetails');

        var data = {
            'id': 1,
            'collapsed': true
        };

        extendDataService.save(data);

    });

    /////////////Second Collapse functions

    jQuery('#collapseKeenSecond').on('hidden.bs.collapse', function (event) {
        event.stopPropagation();
        toggleChevron('#KeenCountSecond');

        var data = {
            'id': 5,
            'collapsed': false
        };

        extendDataService.save(data);
    });

    jQuery('#collapseKeenSecond').on('shown.bs.collapse', function (event) {
        event.stopPropagation();
        toggleChevron('#KeenCountSecond');

        var data = {
            'id': 5,
            'collapsed': true
        };

        extendDataService.save(data);

    });

    jQuery('#collapseCommentSecond').on('hidden.bs.collapse', function (event) {
        event.stopPropagation();
        toggleChevron('#SecondCommentCollapseArea #CommentCountSecond');

        var data = {
            'id': 6,
            'collapsed': false
        };

        extendDataService.save(data);


    });

    jQuery('#collapseCommentSecond').on('shown.bs.collapse', function (event) {
        event.stopPropagation();
        toggleChevron('#SecondCommentCollapseArea #CommentCountSecond');

        var data = {
            'id': 6,
            'collapsed': true
        };

        extendDataService.save(data);

    });

    jQuery('#collapseSecond').on('hidden.bs.collapse', function (event) {
        event.stopPropagation();
        toggleChevron('#SecondDetails');

        var data = {
            'id': 4,
            'collapsed': false
        };

        extendDataService.save(data);

    });

    jQuery('#collapseSecond').on('shown.bs.collapse', function (event) {
        event.stopPropagation();
        toggleChevron('#SecondDetails');

        var data = {
            'id': 4,
            'collapsed': true
        };

        extendDataService.save(data);

    });

    //End Collapse user save 

});