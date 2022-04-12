// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

function ConfirmDeny() {
    var comments = $('#EmpAgreement_Comments').val();
    var denyReason = $("#denyReasons").val();    
    var lang = $('#lang').val();
    if (comments.length == 0 || denyReason == '-1') {
        var msg = lang == "en" ? "You must enter a deny reason and a comment prior to submitting the decision." : "Vous devez saisir un commentaire avant de soumettre la demande.";
        alert(msg);
        $('#EmpAgreement_Comments').focus();
        return false;
    }
    else {
        $('.ui.moda.DenyConfirm').modal('show');
        $('#approve-modal').modal('hide');
        //var q = lang == "en" ? "Are you sure you want to deny this agreement?" : "Êtes-vous sûr de vouloir refuser cette demande?";
        //return confirm(q);
    }
}

function ReopenAgreement() {
    var lang = $('#lang').val();
    var msg = lang == "en" ? "Are you sure you want to return this agreement?" : "Êtes-vous sûr de vouloir retourner cet accord ?";

    if (confirm(msg)) {
        return true;
    }
    else {
        console.log('cancelling return');
        window.setTimeout(function () { $('#return-agreement-btn').removeClass('disabled').find('.icon').removeClass().addClass('folder open outline icon'); }, 500);
        return false;
    }
}

function formatDate(date) {
    var d = new Date(date),
        month = '' + (d.getMonth() + 1),
        day = '' + d.getDate(),
        year = d.getFullYear();

    if (month.length < 2)
        month = '0' + month;
    if (day.length < 2)
        day = '0' + day;

    return [year, month, day].join('-');
}

function addMonths(date, months) {
    var endDate = new Date(date);

    var d = new Date(endDate.setMonth(endDate.getMonth() + months)),
        month = '' + (d.getMonth()),
        day = '' + d.getDate(),
        year = d.getFullYear();

    if (month.length < 2)
        month = '0' + month;
    if (day.length < 2)
        day = '0' + day;

    return [year, month, day].join('-');
}

function debounce(func, threshold, execAsap) {
    var timeout;

    return function debounced() {
        var obj = this,
            args = arguments;

        function delayed() {
            if (!execAsap) {
                func.apply(obj, args);
            }
            timeout = null;
        }

        if (timeout) {
            clearTimeout(timeout);
        } else if (execAsap) {
            func.apply(obj, args);
        }
        timeout = setTimeout(delayed, threshold || 100);
    };
}

// add loading indicator to every button that is pressed
$(document).on("click", ".load-indicator.button", function () {
    $btn = $(this);

    // if this button is part of a form, only change icon if form data is valid
    if ($btn.parents('form').length > 0) {
        if ($btn.parents('form').first().form('is valid')) {
            $(this).addClass('disabled').find('.icon').removeClass().addClass('loading spinner icon');
        }
    } else {
        $(this).addClass('disabled').find('.icon').removeClass().addClass('loading spinner icon');
    }
});


