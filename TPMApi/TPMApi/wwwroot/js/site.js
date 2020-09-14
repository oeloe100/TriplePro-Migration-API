// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(function () {
    $(".woo-submit").on("click", function () {
        var fData = $("#woo-form").serialize();

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

    $(".login-span").on("click", function () {
        $(".partialView-body").load("/Form/LoginPartial");
    });


    $(".register-span").on("click", function () {

        $(".partialView-body").load("/Form/RegisterPartial");
    });
});