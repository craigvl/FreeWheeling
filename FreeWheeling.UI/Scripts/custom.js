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
            $(keenuser).attr('class', 'line-through');
            $(keenuser).css("text-decoration", "line-through");
        }
        else {
            $("#keendiv_" + rideid).prepend('<p><span id=keen_' + username + rideid + ' class="">' + username + " <span style='padding-left:5px;' class='glyphicon glyphicon-thumbs-down'></span>  </span>");
        }
    }
                
    if (message == 'OnWay') {
        var keenuser = "#keen_" + username + rideid;
        $("#KeenCountSpan" + rideid).html("(" + keencount + ")");
        var keenuser = "#keen_" + username + rideid;
        if ($("#keen_" + username + rideid).length) {
            $(keenuser).html(username + '<span class=""> Left  <abbr class="timeago" title="' + moment(leavetime).format('MM/DD/YYYY HH:mm:ss') + '"> </abbr></span>');
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
            $(keenuser).html(username + " <span style='padding-left:5px;' class='glyphicon glyphicon-thumbs-up'></span>");
            $(keenuser).attr('class', '');
            $(keenuser).css('text-decoration', 'none');          
        }
        else {
            $("#keendiv_" + rideid).prepend('<p><span id=keen_' + username + rideid + ' class="">' + username + " <span style='padding-left:5px;' class='glyphicon glyphicon-thumbs-up'></span>  </span>");
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
            $(keenuser).attr('class', '');
            $(keenuser).css("text-decoration", "line-through");
        }
        else {
            $("#keendiv_" + rideid).prepend('<p><span id=keen_' + username + rideid + ' class="">' + username + " <span style='padding-left:5px;' class='glyphicon glyphicon-thumbs-down'></span>  </span>");
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
            $(keenuser).html(username + ' <span class=""> Left  <abbr class="timeago" title="' + moment(leavetime).format('MM/DD/YYYY HH:mm:ss') + '"> </abbr></span>');
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