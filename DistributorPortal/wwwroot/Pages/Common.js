
var Toast;
var a = ['', 'one ', 'two ', 'three ', 'four ', 'five ', 'six ', 'seven ', 'eight ', 'nine ', 'ten ', 'eleven ', 'twelve ', 'thirteen ', 'fourteen ', 'fifteen ', 'sixteen ', 'seventeen ', 'eighteen ', 'nineteen '];
var b = ['', '', 'twenty', 'thirty', 'forty', 'fifty', 'sixty', 'seventy', 'eighty', 'ninety'];

$(document).ready(function () {

    //File upload
    $('.custom-file-input').on("change", function () {
        var fileName = $(this).val().split("\\").pop();
        $(this).next('.custom-file-label').html(fileName);
    });

    $('.select2').select2();

    $('#Spinner').hide();

    $('.datetimepicker-input').keypress(function (e) {
        e.preventDefault();
    });

    $('body').on('click', '#disclaimer', function () {
        $('#modal-disclaimer').modal('toggle');
        $('#modal-disclaimer').modal('show');
    });

    $('#ToDate').on('change', function () {

        var d1 = new Date($('#ToDate').val());
        var d2 = new Date($('#FromDate').val());

        if (d1.getTime() < d2.getTime()) {
            $('button[type=submit]').attr('disabled', true);
            Toast.fire({ icon: 'error', title: 'To date cannot be small than from date.' });
        } else {
            $('button[type=submit]').attr('disabled', false);
        }

    });


    $('#FromDate').on('change', function () {

        var d1 = new Date($('#ToDate').val());
        var d2 = new Date($('#FromDate').val());

        if (d1.getTime() < d2.getTime()) {
            $('button[type=submit]').attr('disabled', true);
            Toast.fire({ icon: 'error', title: 'To Date can not be greater than from date.' });
        } else {
            $('button[type=submit]').attr('disabled', false);
        }
    });
});

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
    if ($("button[type=submit]", this)[0] != undefined) {
        Ladda.create($("button[type=submit]", this)[0]).start();
    }
    BlockUI();
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
    $("body").removeClass("loading");
}

function Complete() {
    $('#Spinner').hide('slow');
    $('button[type="submit"]').attr('disabled', false);
    Ladda.create($("button[type=submit]", this)[0]).stop();
    if ($("#btnOrderNow", this)[0] != undefined) {
        Ladda.create($("#btnOrderNow", this)[0]).stop();
    }

    if ($("#btnDraft", this)[0] != undefined) {
        Ladda.create($("#btnDraft", this)[0]).stop();
    }
    UnBlockUI();
}

function MessageDisappear() {
    setInterval(function () {
        $("#alertmessage").hide('slow');
    }, 5000);
    UnBlockUI();
}

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

function UpdateStatus(e, controllerName, actionName, id) {
    var val = '';
    if (e.value == undefined) {
        val = e;
    }
    else {
        val = e.value;
    }
    if (e.value == "Resolved" || e.value == "Reject" || val == "Rejected") {
        Swal.fire({
            type: "warning",
            confirmButtonText: "Yes",
            title: 'Enter Remarks',
            input: 'text',
            inputLabel: 'Remarks',
            inputValue: '',
            showCancelButton: true,
            inputValidator: (value) => {
                if (!value) {
                    return 'You need to write something!'
                }
                if (value.length > 255) {
                    return 'You have exceeded 255 characters';
                }
            }
        }).then(function (result) {

            if (result.value) {
                $.post(window.location.origin + "/" + controllerName + "/" + actionName, { Id: id, Status: val, Remarks: result.value }, function (data) {

                    if (data) {

                        if (data.data.Status) {
                            Toast.fire({ icon: 'success', title: data.data.Message });
                        } else {
                            Toast.fire({ icon: 'error', title: data.data.Message });
                        }
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
    else {
        Swal.fire({
            title: "Are you sure you want to continue?",
            type: "warning",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: "#DD6B55",
            confirmButtonText: "Yes",
            closeOnConfirm: true,
            showLoaderOnConfirm: true
        }).then((result) => {
            if (result.value) {
                $.post(window.location.origin + "/" + controllerName + "/" + actionName, { Id: id, Status: val }, function (data) {
                    if (data) {
                        if (data.data.Status) {
                            Toast.fire({ icon: 'success', title: data.data.Message });
                        } else {
                            Toast.fire({ icon: 'error', title: data.data.Message });
                        }
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
}

function inWords(num) {

    if ((num = num.toString()).length > 9) return 'overflow';
    n = ('000000000' + num).substr(-9).match(/^(\d{2})(\d{2})(\d{2})(\d{1})(\d{2})$/);
    if (!n) return; var str = '';
    str += (n[1] != 0) ? (a[Number(n[1])] || b[n[1][0]] + ' ' + a[n[1][1]]) + 'Crore ' : '';
    str += (n[2] != 0) ? (a[Number(n[2])] || b[n[2][0]] + ' ' + a[n[2][1]]) + 'Lac ' : '';
    str += (n[3] != 0) ? (a[Number(n[3])] || b[n[3][0]] + ' ' + a[n[3][1]]) + 'Thousand ' : '';
    str += (n[4] != 0) ? (a[Number(n[4])] || b[n[4][0]] + ' ' + a[n[4][1]]) + 'Hundred ' : '';
    str += (n[5] != 0) ? ((str != '') ? 'and ' : '') + (a[Number(n[5])] || b[n[5][0]] + ' ' + a[n[5][1]]) + 'only ' : '';
    return str.charAt(0).toUpperCase() + str.slice(1);
}

function BlockUI() {

    if ($.blockUI != undefined) {
        $.blockUI.defaults.css = {
            padding: 0,
            margin: 0,
            width: '30%',
            top: '40%',
            left: '35%',
            textAlign: 'center',
            cursor: 'wait'
        };
        $.blockUI({
            Message: ('<img src="/Images/loading-spinner-grey.gif" />')
        });
    }
}

function UnBlockUI() {
    if($.blockUI != undefined) {
        $.unblockUI();
    }
}

function isEmailValidate(email) {

    var re = /^(([^<>()\[\]\\.,;:\s@@"]+(\.[^<>()\[\]\\.,;:\s@@"]+)*)|(".+"))@@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return re.test(String(email).toLowerCase());
}

//Print Report
function Print(RedirectToUrl, ApplicationPage, DPID, Reason) {

    $('#DPID').val(DPID);

    if (Reason == 3) {
        window.open(window.location.origin + RedirectToUrl + "ApplicationPage=" + ApplicationPage + "&DPID=" + DPID, '_blank');
    }
    else {
        $.post(window.location.origin + "/OrderReturn/GetTRNoById", { DPID: DPID }, function (data) {

            if (data) {
                if (data.data.length > 0 && data.data.find(x => x.PlantLocationId == 1) != undefined) {
                    $('#SiteTRNo').val(data.data.find(x => x.PlantLocationId == 1).TRNo);
                }
                else {
                    debugger
                    $('#SiteTRNo').remove();
                }
                if (data.data.length > 0 && data.data.find(x => x.PlantLocationId == 2) != undefined) {
                    $('#KorangiTRNo').val(data.data.find(x => x.PlantLocationId == 2).TRNo);
                }
                else {
                    debugger
                    $('#KorangiTRNo').remove();
                }
                $('#modalTR').modal('toggle');
                $('#modalTR').modal('show');
            } else {
                Toast.fire({ icon: 'error', title: 'Error occured.' });
            }
        });
    }
}