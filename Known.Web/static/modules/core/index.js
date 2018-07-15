///////////////////////////////////////////////////////////////////////
$(document).ajaxSend(function (evt, xhr, settings) {
    if (settings.type === 'POST') {
        var token = $('input[name="__RequestVerificationToken"]').val();
        xhr.setRequestHeader('X-XSRF-TOKEN', token);
    }
});

$(function () {
    $('input[onenter]').keydown(function (event) {
        if (event.which === 13) {
            event.preventDefault();
            var method = $(this).attr("onenter");
            eval(method);
        }
    });
});