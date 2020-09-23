// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(".fa-sync").on("click", function () {
    var fData = $("#WTA-form").serialize();

    $.ajax({
        type: 'POST',
        url: '/AfostoAuthorization/Authenticate',
        data: fData,
        processData: false,
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8'
    }).done(function (result) {
        window.location.replace(result);
    });
});

$(function () {

    $(".login-span").on("click", function () {
        $(".partialView-body").load("/Form/LoginPartial");
    });


    $(".register-span").on("click", function () {

        $(".partialView-body").load("/Form/RegisterPartial");
    });

    $(".faa-click").on("click", function () {
        if (!$(".fa-sync").hasClass("fa-spin")) {
            $(".fa-sync").addClass("fa-spin");
        }
        else if ($(".fa-sync").hasClass("fa-spin")) {
            $(".fa-sync").removeClass("fa-spin");
        }
    });

    $(document).ready(function () {
        $('[data-toggle="tooltip"]').tooltip();
    });
});