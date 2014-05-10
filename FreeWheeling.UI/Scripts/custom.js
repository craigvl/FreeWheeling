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
            $(keenuser).html(username + " is in!");
            $(keenuser).attr('class', 'label label-success');
            $(keenuser).css('text-decoration', 'none');
        }
        else {                     
            $("#keendiv_" + rideid).prepend('<p><span id=keen_' + username + rideid + ' class="label label-success">'+ username + ' is in!</span>');
        }                    
    }
                
    if (message == 'Out') {
        var keenuser = "#keen_" + username + rideid;
        $("#KeenCountSpan" + rideid).html("(" + keencount + ")");
        var keenuser = "#keen_" + username + rideid;
        if ($("#keen_" + username + rideid).length) {
            $(keenuser).html(username + " is Out!");
            $(keenuser).attr('class', 'label label-danger');
            $(keenuser).css("text-decoration","line-through");
        }
        else {
            $("#keendiv_" + rideid).prepend('<p><span id=keen_' + username + rideid + ' class="label label-danger">' + username + ' is out!</span>');
        }
    }
                
    if (message == 'OnWay') {
        var keenuser = "#keen_" + username + rideid;
        $("#KeenCountSpan" + rideid).html("(" + keencount + ")");
        var keenuser = "#keen_" + username + rideid;
        if ($("#keen_" + username + rideid).length) {
            $(keenuser).html(username + ' <span class="label label-success">Left  <abbr class="timeago" title="' + moment(leavetime).format('MM/DD/YYYY HH:mm:ss') + '"> </abbr></span>');
            $(keenuser).attr('class', 'label label-success');
            $(keenuser).css("text-decoration", "none");
            jQuery("abbr.timeago").timeago();
        }
        else {
            $("#keendiv_" + rideid).prepend('<p><span id=keen_' + username + rideid + ' class="label label-success" style"text-decoration: none;">' + username + '<span class="label label-success">Left  <abbr class="timeago" title="' + moment(leavetime).format('MM/DD/YYYY HH:mm:ss') + '"> </abbr></span>');
        }
    }
            
jQuery("abbr.timeago").timeago();

}

function UpdateKeenActions(message, rideid, keencount, username, leavetime)
{
    if (message == 'In') {
        $("#KeenCountSpan" + rideid).html("(" + keencount + ")");
        var keenuser = "#keen_" + username + rideid;
        if ($("#keen_" + username + rideid).length) {
            $(keenuser).html(username + " is in!");
            $(keenuser).attr('class', 'label label-success');
            $(keenuser).css('text-decoration', 'none');
        }
        else {
            $("#keendiv_" + rideid).prepend('<p><span id=keen_' + username + rideid + ' class="label label-success">' + username + ' is in!</span>');
        }
        $("#OutNext").show();
        $("#OnWayNext").show();
        $("#InNext").hide();
    }

    if (message == 'Out') {
        var keenuser = "#keen_" + username + rideid;
        $("#KeenCountSpan" + rideid).html("(" + keencount + ")");
        if ($("#keen_" + username + rideid).length) {
            $(keenuser).html(username + " is Out!");
            $(keenuser).attr('class', 'label label-danger');
            $(keenuser).css("text-decoration", "line-through");
        }
        else {
            $("#keendiv_" + rideid).prepend('<p><span id=keen_' + username + rideid + ' class="label label-danger">' + username + ' is out!</span>');
        }
        $("#OutNext").hide();
        $("#OnWayNext").hide();
        $("#InNext").show();
    }

    if (message == 'OnWay') {
        var keenuser = "#keen_" + username + rideid;
        $("#KeenCountSpan" + rideid).html("(" + keencount + ")");
        if ($("#keen_" + username + rideid).length) {
            $(keenuser).html(username + '<span class="label label-success">Left  <abbr class="timeago" title="' + moment(leavetime).format('MM/DD/YYYY HH:mm:ss') + '"> </abbr></span>');
            $(keenuser).attr('class', 'label label-success');
            $(keenuser).css("text-decoration", "none");
        }
        else {
            $("#keendiv_" + rideid).prepend('<p><span id=keen_' + username + rideid + ' class="label label-success" style"text-decoration: none;">' + username + '<span class="label label-success">Left  <abbr class="timeago" title="' + moment(leavetime).format('MM/DD/YYYY HH:mm:ss') + '"> </abbr></span>');
        }
        jQuery("abbr.timeago").timeago();

        $("#OutNext").show();
        $("#OnWayNext").hide();
        $("#InNext").hide();
    }
}