// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

var specialReqArray = [];

$(function () {
    LoadPartialView();
    TooltipManager();

    setTimeout(function () {
        PopUp();
    }, 1000); // 1000 to load it after 1 seconds from page load
});

$(".fa-settings").on("click", function () {
    PopUp();
});

function PopUp() {
    if ($(".modal").css("display") == "none") {
        $(".modal").css("display", "block");
    }
}

$(".modal-close").on("click", function () {
    $(".modal").css("display", "none");
});

$(".modal-save").on("click", function () {
    specialReqArray = [];
    var customs = $(".customs-form").find("#customsSelect");

    $(customs).each(function (index, value) {
        var value = $(value).val();
        specialReqArray.push(value);
    });

    $(".modal").css("display", "none");
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
    var bundled = $("#bundled").prop("checked");

    $(specialReqArray).each(function (index, value) {
        if (bundled && value.toLowerCase() != "choose") {
            FaaManager();

            $.ajax({
                type: 'POST',
                url: '/Migration/StartWTAProductBundleMigration',
                traditional: true,
                data: { "specialsArray": specialReqArray },
            }).done(function (result) {
                $(".sync-process-box").html(result);
            });
        }

        if (bundled == false) {
            FaaManager();

            $.ajax({
                type: 'POST',
                url: '/Migration/StartWTAMigration',
                traditional: true,
                data: { "specialsArray": specialReqArray },
            }).done(function (result) {
                $(".sync-process-box").html(result);
            });
        }

        if (bundled && value.toLowerCase() == "choose") {
            alert("Please select a customs");
        }
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
    if (!$(".fa-sync").hasClass("fa-spin")) {
        $(".fa-sync").addClass("fa-spin");
    }
    else if ($(".fa-sync").hasClass("fa-spin")) {
        $(".fa-sync").removeClass("fa-spin");
    }
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