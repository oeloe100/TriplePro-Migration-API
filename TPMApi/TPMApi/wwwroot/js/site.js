﻿// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(function () {
    LoadPartialView();
    TooltipManager();
    FaaManager();
});

//in case of onclick no need to call in document.ready
$(".afosto-form-authorize").on("click", function () {
    var fData = $(".afosto-form").serialize();

    $.ajax({
        type: 'POST',
        url: '/AfostoAuthentication/Authenticate',
        data: fData,
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8'
    }).done(function (result) {
        if (IsValidUrl(result)) {
            window.location.replace(result);
        }
    });
});

$(".woo-form-authorize").on("click", function () {
    var fData = $(".woo-form").serialize();

    $.ajax({
        type: 'POST',
        url: '/WooCommerceAuthentication/Authenticate',
        data: fData,
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8'
    }).done(function (result) {
        if (IsValidUrl(result)) {
            window.location.replace(result);
        }
    });
});

$(".faa-click").on("click", function () {
    $.ajax({
        type: 'POST',
        url: '/Migration/StartWTAMigration',
    }).done(function (result) {
        $(".sync-process-box").html(result);
    });
});


//load partial view(s)
function LoadPartialView() {
    $(".login-span").on("click", function () {
        $(".partialView-body").load("/Form/LoginPartial");
    });

    $(".register-span").on("click", function () {

        $(".partialView-body").load("/Form/RegisterPartial");
    });
}

//manage faa state on click
function FaaManager() {
    $(".faa-click").on("click", function () {
        if (!$(".fa-sync").hasClass("fa-spin")) {
            $(".fa-sync").addClass("fa-spin");
        }
        else if ($(".fa-sync").hasClass("fa-spin")) {
            $(".fa-sync").removeClass("fa-spin");
        }
    });
}

//simple tooltip
function TooltipManager() {
    $(document).ready(function () {
        $('[data-toggle="tooltip"]').tooltip();
    });
}

function IsValidUrl(url) {
    try {
        new URL(url);
    }
    catch (_) {
        return false;
    }

    return true;
}