﻿var opts = {
    lines: 7, // The number of lines to draw
    length: 20, // The length of each line
    width: 10, // The line thickness
    radius: 10, // The radius of the inner circle
    corners: 1, // Corner roundness (0..1)
    rotate: 0, // The rotation offset
    direction: 1, // 1: clockwise, -1: counterclockwise
    color: '#8dc63f', // #rgb or #rrggbb or array of colors
    speed: 1, // Rounds per second
    trail: 60, // Afterglow percentage
    shadow: false, // Whether to render a shadow
    hwaccel: false, // Whether to use hardware acceleration
    className: 'spinner', // The CSS class to assign to the spinner
    zIndex: 2e9, // The z-index (defaults to 2000000000)
    top: '50%', // Top position relative to parent
    left: '50%' // Left position relative to parent
};
var spinner = new Spinner(opts);
function ShowProgress() {
    var target = document.getElementById('loading');
    spinner.spin(target);
}
function HideProgress() {
    var target = document.getElementById('loading');
    spinner.stop(target);
}
function onsuccessjoinfav()
{
    $("#JoinFav").hide();
    $("#RemoveFav").show();
    HideProgress();
    displayMessageAjax("Added to favourites", "success", "BottomCentre", "tempoarayMessage");
}
function onsuccessremovefav() {
    $("#RemoveFav").hide();
    $("#JoinFav").show();
    HideProgress();
    displayMessageAjax("Removed from favourites", "success", "BottomCentre", "tempoarayMessage");
}
function HideInfoLink(GroupId) {
    $("#infoButton" + GroupId).hide();
    $("#hideinfo" + GroupId).show();
    $("#GroupInfo" + GroupId).show();
    HideProgress();
}
function hideinfodetails(GroupId) {
    $("#GroupInfo" + GroupId).hide();
    $("#infoButton" + GroupId).show();
    $("#hideinfo" + GroupId).hide();
}
function UpdateKeenActionsPusher(message, rideid, keencount, username, leavetime) {
    if (message == 'In') {
        $("#KeenCountSpan" + rideid).html("(" + keencount + ")");
        var keenuser = "#keen_" + username + rideid;
        if ($("#keen_" + username + rideid).length) {
            $(keenuser).html(username + " <span style='padding-left:5px;' class='glyphicon glyphicon-thumbs-up'></span>");
            $(keenuser).attr('class', '');
            $(keenuser).css('text-decoration', 'none');
        }
        else {                     
            $("#keendiv_" + rideid).prepend('<p><span id=keen_' + username + rideid + ' class="">' + username + " <span style='padding-left:5px;' class='glyphicon glyphicon-thumbs-up'></span>  </span>");
        }
        displayMessageAjax(username + " is in", "success", "", "tempoarayMessage");
    }               
    if (message == 'Out') {
        var keenuser = "#keen_" + username + rideid;
        $("#KeenCountSpan" + rideid).html("(" + keencount + ")");
        var keenuser = "#keen_" + username + rideid;
        if ($("#keen_" + username + rideid).length) {
            $(keenuser).html(username + "<span style='padding-left:5px;' class='glyphicon glyphicon-thumbs-down'></span>");
            $(keenuser).attr('class', 'greyout');
            $(keenuser).css("text-decoration", "line-through");
        }
        else {
            $("#keendiv_" + rideid).prepend('<p><span style="text-decoration:line-through;" id=keen_' + username + rideid + ' class="">' + username + " <span style='padding-left:5px;' class='glyphicon glyphicon-thumbs-down'></span>  </span>");
        }
        displayMessageAjax(username + " is out", "success", "", "tempoarayMessage");
    }                
    if (message == 'OnWay') {
        var keenuser = "#keen_" + username + rideid;
        $("#KeenCountSpan" + rideid).html("(" + keencount + ")");
        var keenuser = "#keen_" + username + rideid;
        if ($("#keen_" + username + rideid).length) {
            $(keenuser).html(username + '<span class="onway"> left  <abbr class="timeago" title="' + moment(leavetime).format('MM/DD/YYYY HH:mm:ss') + '"> </abbr></span>');
            $(keenuser).attr('class', '');
            $(keenuser).css("text-decoration", "none");
            jQuery("abbr.timeago").timeago();
        }
        else {
            $("#keendiv_" + rideid).prepend('<p><span id=keen_' + username + rideid + ' class="" style"text-decoration: none;">' + username + '<span class="onway"> Left  <abbr class="timeago" title="' + moment(leavetime).format('MM/DD/YYYY HH:mm:ss') + '"> </abbr></span>');
        }
        displayMessageAjax(username + " has left", "success", "", "tempoarayMessage");
    }           
jQuery("abbr.timeago").timeago();
}
function UpdateKeenActions(message, rideid, keencount, username, leavetime, position)
{
    var tagline = "'Life is better in a bunch'";
    position = position || "first";
    if (message == 'In') {
        $("#KeenCountSpan" + rideid).html("(" + keencount + ")");
        var keenuser = "#keen_" + username + rideid;
        if ($("#keen_" + username + rideid).length) {
            $(keenuser).html(username + ' <span style="padding-left:5px;" class="glyphicon glyphicon-thumbs-up"></span><span style="padding-left:5px;" class="pull-right"> <a href="#" onclick="fb_publish(' + rideid + ',' + rideid + ',' + tagline + ');"> <img src="/Content/Images/share_facebook.png" /></a></span></span>');
            $(keenuser).attr('class', '');
            $(keenuser).css('text-decoration', 'none');          
        }
        else {
            $("#keendiv_" + rideid).prepend('<p><span id=keen_' + username + rideid + ' class="">' + username + '<span style="padding-left:5px;" class="glyphicon glyphicon-thumbs-up"></span><span style="padding-left:5px;" class="pull-right"> <a href="#" onclick="fb_publish(' + rideid + ',' + rideid + ',' + tagline + ');"> <img src="/Content/Images/share_facebook.png" /></a></span> </span>');
        }

        if (position == "next") {
            $("#OutNext").show();
            $("#OnWayNext").show();
            $("#InNext").hide();
        }
        else {
            $("#OutFirst").show();
            $("#OnWayFirst").show();
            $("#InFirst").hide();
        }
        displayMessageAjax("You're in", "success", "", "tempoarayMessage");
    }
    if (message == 'Out') {
        var keenuser = "#keen_" + username + rideid;
        $("#KeenCountSpan" + rideid).html("(" + keencount + ")");
        if ($("#keen_" + username + rideid).length) {
            $(keenuser).html(username + "<span style='padding-left:5px;' class='glyphicon glyphicon-thumbs-down'></span>");
            $(keenuser).attr('class', 'greyout');
            $(keenuser).css("text-decoration", "line-through");
        }
        else {
            $("#keendiv_" + rideid).prepend('<p><span style="text-decoration:line-through;" id=keen_' + username + rideid + ' class="">' + username + " <span style='padding-left:5px;' class='glyphicon glyphicon-thumbs-down'></span>  </span>");
        }
        if (position == "next") {
            $("#OutNext").hide();
            $("#OnWayNext").hide();
            $("#InNext").show();
        }
        else {
            $("#OutFirst").hide();
            $("#OnWayFirst").hide();
            $("#InFirst").show();
        }
        displayMessageAjax("You're out", "success", "", "tempoarayMessage");
    }
    if (message == 'OnWay') {
        var keenuser = "#keen_" + username + rideid;
        $("#KeenCountSpan" + rideid).html("(" + keencount + ")");
        if ($("#keen_" + username + rideid).length) {
            $(keenuser).html(username + ' <span class="onway"> left  <abbr class="timeago" title="' + moment(leavetime).format('MM/DD/YYYY HH:mm:ss') + '"> </abbr></span>');
            $(keenuser).attr('class', '');
            $(keenuser).css("text-decoration", "none");
        }
        else {
            $("#keendiv_" + rideid).prepend('<p><span id=keen_' + username + rideid + ' class="" style"text-decoration: none;">' + username + '<span class="onway"> left  <abbr class="timeago" title="' + moment(leavetime).format('MM/DD/YYYY HH:mm:ss') + '"> </abbr></span>');
        }
        jQuery("abbr.timeago").timeago();

        if (position == "next") {
            $("#OutNext").show();
            $("#OnWayNext").hide();
            $("#InNext").hide();
        }
        else {
            $("#OutFirst").show();
            $("#OnWayFirst").hide();
            $("#InFirst").hide();
        }
        displayMessageAjax("On your way", "success", "", "tempoarayMessage");
    }
}
function UpdateCommentFields(message, rideid, username, commentcount) {
    $("#collapseCommentPanel" + rideid).append("<p><span>" + username + " : <span><span style='font-style:italic'>" + message + "</span><p>");
    $("#CommentCountSpan" + rideid).html("(" + commentcount + ")");
    $("#CommentCountSpanSeeAll" + rideid).html("View All (" + commentcount + ")");
    displayMessageAjax("Comment added", "success", "", "tempoarayMessage");
    //Clear comment textbox after submit for both rides.
    $("form #CommentStringFirst").val("");
    $("form #CommentStringSecond").val("");
}
function UpdateCommentFieldsPusher(message, rideid, username, commentcount) {
    //alert('hello from pusher');
    $("#collapseCommentPanel" + rideid).append("<p><span>" + username + " : <span><span style='font-style:italic'>" + message + "</span><p>");
    $("#CommentCountSpan" + rideid).html("(" + commentcount + ")");
    $("#CommentCountSpanSeeAll" + rideid).html("View All (" + commentcount + ")");
    displayMessageAjax(username + " added a new comment", "success", "", "tempoarayMessage");
}

function toggleChevron(e) {
    $(e).toggleClass('glyphicon-minus glyphicon-plus');
    //alert($(e).attr("id"));
}
function MapCollapse() {
    jQuery('#CollapseMap').on('hidden.bs.collapse', function (event) {
        event.stopPropagation();
        toggleChevron('#MapPanel');
    });
    jQuery('#CollapseMap').on('shown.bs.collapse', function (event) {
        event.stopPropagation();
        toggleChevron('#MapPanel');
    });
}
//This function must be called from the _layout.cshtml or wherever @Html.RenderMessages() is being used 
function DisplayMessages() {
    if ($("#messagewrapper").children().length > 0) {
        //Apply a delay and fade the message in
        $("#messagewrapper").delay(150).fadeIn(300);    //Adjust timing if required
        //Apply a delay and fade out temp messages
        $(".tempoarayMessage").delay(1000).fadeOut(300);
        //Hide message on click
        $("#messagewrapper").click(function () {
            $("#messagewrapper").stop();
            ClearMessages();
        });
    }
    else {
        $("#messagewrapper").hide();
    }
}
function ClearMessages() {
    $("#messagewrapper").fadeOut(100, function () {
        $("#messagewrapper").empty();
    });
}
function displayMessageAjax(message, messageType, position, PersistMessage) {
    if (messageType == "error") {
        $("#messagewrapperajax").removeClass();
        $("#messagewrapperajax").addClass('alert btn-danger ' + ' ' + position + ' ' + PersistMessage);
    }
    if (messageType == "success") {
        $("#messagewrapperajax").removeClass();
        $("#messagewrapperajax").addClass('alert btn-success ' + ' ' + position + ' ' + PersistMessage);
    }
    if (messageType == "warning") {
        $("#messagewrapperajax").removeClass();
        $("#messagewrapperajax").addClass('alert btn-warning' + ' ' + position + ' ' + PersistMessage);
    }
    if (messageType == null) {
        $("#messagewrapperajax").removeClass();
        $("#messagewrapperajax").addClass('alert ' + ' ' + position + ' ' + PersistMessage);
    }
    $("#messagewrapperajax").html('<div id="messagebox" class="messagebox"></div>');
    $("#messagewrapperajax .messagebox").text(message);
    displayMessagesAjax();
}
function displayMessagesAjax() {
    //As per http://stackoverflow.com/questions/14931410/center-message-box-on-any-screen-resolution with some edits as per loading image.
    var windowWidth = document.documentElement.clientWidth;
    var windowHeight = document.documentElement.clientHeight;
    var el = $('#messagewrapperajax');
    var elWidth = el.width();
    var elHeight = el.height();
    var top = Math.max($(window).height() / 2 - el[0].offsetHeight / 2, 0);
    var left = Math.max($(window).width() / 2 - el[0].offsetWidth / 2, 0);
    if ($("#messagewrapperajax").children().length > 0) {
        $("#messagewrapperajax").delay(200).fadeIn(300).focus();
        el.css({
           // position: 'absolute',
            top: top,
            left: (windowWidth / 2) - (elWidth / 2),
        });
        $(".tempoarayMessage").delay(3000).fadeOut(300);
        $("#messagewrapperajax").click(function () {
            ClearMessagesAjaxClick();
        });
        $(".button .close").click(function () {
            ClearMessagesAjaxClick();
        });
    }
    else {
        $("#messagewrapperajax").hide();
    }
}
function ClearMessagesAjax() {
    $("#messagewrapperajax").hide();
}
function ClearMessagesAjaxClick() {
    $("#messagewrapperajax").hide();
}

//Show when user clicks on follow button
function FollowCompleteFollow() {
    HideProgress();
    $('a#followbutton').hide();
    $('a#unfollowbutton').show();
    displayMessageAjax("Following", "success", "BottomCentre", "tempoarayMessage");
}

//Show when user clicks on un follow button
function FollowCompleteUnFollow() {
    HideProgress();
    $('a#followbutton').show();
    $('a#unfollowbutton').hide();
    displayMessageAjax("Not Following", "success", "BottomCentre", "tempoarayMessage");
}

//Shown when user clicks the route voute button
function VoteComplete(data) {
    HideProgress();
    displayMessageAjax(data.message, "success", "BottomCentre", "tempoarayMessage");
    //alert(data.totalvotes);
    location.reload();
}