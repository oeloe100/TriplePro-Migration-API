// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(".woo-submit").on("click", function () {
    var fData = $("#woo-form").serialize();

    console.log('Woo-Submit started');

    $.ajax({
        type: 'POST',
        url: '/api/woo',
        data: fData,
        processData: false,
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8'
    }).done(function (result) {
        console.log(result);
    });
});

$(".afosto-submit").on("click", function () {
    var fData = $("#afosto-form").serialize();

    console.log('afosto-Submit started');

    $.ajax({
        type: 'POST',
        url: '/AfostoAuthorization/Authenticate',
        data: fData,
        processData: false,
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8'
    }).done(function (result) {
        console.log(result);
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