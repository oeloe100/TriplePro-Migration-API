// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(function () {
    LoadPartialView();
    TooltipManager();
    FaaManager();
});

//in case of onclick no need to call in document.ready
$(".fa-sync").on("click", function () {
    var fData = $(".WTA-form").serialize();

    if (localStorage.getItem("isAuthentication") === null) {
        AuthenticateClient(fData);
    }
    else {
        localStorage.removeItem("isAuthentication");
        StartMigrationProcess();
    }
});

//ajax call in case of authentication process
//when authenticated disable next authentication click
function AuthenticateClient(fData) {
    $.ajax({
        type: 'POST',
        url: '/WTA/Authenticate',
        data: fData,
        processData: false,
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8'
    }).done(function (result) {
        localStorage.setItem("isAuthentication", false);
        window.location.replace(result);
    });
}

//ajax call to C# controller to start migrating.
function StartMigrationProcess() {
    $.ajax({
        type: 'POST',
        url: '/WTA/Authenticate',
    }).done(function (result) {
        console.log(result);
    });
}

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