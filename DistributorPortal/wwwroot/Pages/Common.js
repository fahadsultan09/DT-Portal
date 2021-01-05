
$(document).ready(function () {
    $('#Spinner').hide();
});

var Toast;
$(function () {

    Toast = Swal.mixin({
        toast: true,
        position: 'top-end',
        showConfirmButton: false,
        timer: 3000
    });
});

function Begin() {

    $('#Spinner').show();
    $('button[type="submit"]').attr('disabled', true);
    Ladda.create($("button[type=submit]", this)[0]).start();
}

function OnSuccess(data) {
    if (data.data.Status) {
        Toast.fire({
            icon: 'success',
            title: data.data.Message
        })
        setTimeout(function () {
            window.location = data.data.RedirectURL;
        }, 1000);
    }
    else {
        Toast.fire({
            icon: 'error',
            title: data.data.Message
        })
    }
    Ladda.create($("button[type=submit]", this)[0]).stop();
}


function Complete() {
    $('#Spinner').hide('slow');
    $('button[type="submit"]').attr('disabled', false);
    Ladda.create($("button[type=submit]", this)[0]).stop();
}

//Save
function Save() {
    var form = event.target.form; // storing the form
    $(form).each(function () {
        $(this).children().find('textarea').each(function () {
            $(this).val($(this).val().trim());
        });
        $(this).children().find('input[type="text"]').each(function () {
            $(this).val($(this).val().trim());
        });
    });

    $.validator.unobtrusive.parse(form);

    if ($(form).valid()) {

        event.preventDefault();
        event.stopImmediatePropagation();
        $('#Spinner').show();
        $(form).submit();
    }
}

function bindDropDownList(dropdown, url, params, defaultvalue = "") {
    $.ajax({
        type: "GET",
        url: url,
        contentType: "application/json",
        dataType: "json",
        data: params,
        async: false,
        success: function (response) {
            dropdown.empty();
            dropdown.append($("<option readonly disabled></option>").val(defaultvalue).html("--Select option--"));
            $.each(response, function () {
                dropdown.append($("<option></option>").val(this['Value']).html(this['Text']));
            });
        },
    });
}

//Approve
function UpdateStatus(e, controllerName, actionName, id) {
    
    Swal.fire({
        title: "Are you sure you want to verify?",
        type: "warning",
        showCancelButton: true,
        confirmButtonColor: "#DD6B55",
        confirmButtonText: "Yes",
        closeOnConfirm: true,
        showLoaderOnConfirm: true
    }).then((result) => {
        if (result.value) {
            $.post(window.location.origin + "/" + controllerName + "/" + actionName, { Id: id, paymentStatus: e.value }, function (data) {
                if (data) {
                    Toast.fire({ icon: 'success', title: 'Verified successfully.' });
                    setTimeout(function () {
                        window.location.reload();
                    }, 1000);
                } else {
                    Toast.fire({ icon: 'error', title: 'Error occured while saving changes.' });
                }
            });
        }
    });
}