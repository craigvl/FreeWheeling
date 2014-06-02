function ShowProgress() {
    var loading = $(".loading");
    loading.show();
    var top = Math.max($(window).height() / 2 - loading[0].offsetHeight / 2, 0);
    var left = Math.max($(window).width() / 2 - loading[0].offsetWidth / 2, 0);
    loading.css({ top: top, left: left });
}

function HideProgress() {
    var loading = $(".loading");
    loading.hide();
    var top = Math.max($(window).height() / 2 - loading[0].offsetHeight / 2, 0);
    var left = Math.max($(window).width() / 2 - loading[0].offsetWidth / 2, 0);
    loading.css({ top: top, left: left });
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
            $("#keendiv_" + rideid).prepend('<p><span id=keen_' + username + rideid + ' class="" style"text-decoration: none;">' + username + '<span class=""> Left  <abbr class="timeago" title="' + moment(leavetime).format('MM/DD/YYYY HH:mm:ss') + '"> </abbr></span>');

        }
    }
            
jQuery("abbr.timeago").timeago();

}

function UpdateKeenActions(message, rideid, keencount, username, leavetime, position)
{
    position = position || "first";

    if (message == 'In') {
        $("#KeenCountSpan" + rideid).html("(" + keencount + ")");
        var keenuser = "#keen_" + username + rideid;
        if ($("#keen_" + username + rideid).length) {
            $(keenuser).html(username + ' <span style="padding-left:5px;" class="glyphicon glyphicon-thumbs-up"></span><span style="padding-left:5px;" class="pull-right"> <a href="#" onclick="fb_publish();"> <img src="/Content/Images/share_facebook.png" /></a></span></span>');
            $(keenuser).attr('class', '');
            $(keenuser).css('text-decoration', 'none');          
        }
        else {
            $("#keendiv_" + rideid).prepend('<p><span id=keen_' + username + rideid + ' class="">' + username + "<span style='padding-left:5px;' class='glyphicon glyphicon-thumbs-up'></span><span style='padding-left:5px;' class='pull-right'> <a href='#' onclick='fb_publish();'> <img src='/Content/Images/share_facebook.png' /></a></span> </span>");
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
            $("#keendiv_" + rideid).prepend('<p><span id=keen_' + username + rideid + ' class="" style"text-decoration: none;">' + username + '<span class=""> is on the way! Left  <abbr class="timeago" title="' + moment(leavetime).format('MM/DD/YYYY HH:mm:ss') + '"> </abbr></span>');
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
    }
}

function UpdateCommentFields(message, rideid, username, commentcount) {
    $("#collapseCommentPanel" + rideid).prepend("<p>" + username + " : " + message + "</p>");
    $("#CommentCountSpan" + rideid).html("(" + commentcount + ")");
    $("#CommentCountSpanSeeAll" + rideid).html("View All (" + commentcount + ")");

    //Clear comment textbox after submit for both rides.
    $("form #CommentStringFirst").val("");
    $("form #CommentStringSecond").val("");
}

function UpdateCommentFieldsPusher(message, rideid, username, commentcount) {
    //alert('hello from pusher');
    $("#collapseCommentPanel" + rideid).prepend("<p>" + username + " : " + message + "</p>");
    $("#CommentCountSpan" + rideid).html("(" + commentcount + ")");
    $("#CommentCountSpanSeeAll" + rideid).html("View All (" + commentcount + ")");
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
        $("#messagewrapper").delay(200).fadeIn(300);    //Adjust timing if required

        //Apply a delay and fade out temp messages
        $(".tempoarayMessage").delay(2000).fadeOut(300);

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
    if ($("#messagewrapperajax").children().length > 0) {      
        $("#messagewrapperajax").delay(200).fadeIn(300);
        $(".tempoarayMessage").delay(2000).fadeOut(300);
       
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