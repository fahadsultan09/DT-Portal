
var Toast;
var a = ['', 'one ', 'two ', 'three ', 'four ', 'five ', 'six ', 'seven ', 'eight ', 'nine ', 'ten ', 'eleven ', 'twelve ', 'thirteen ', 'fourteen ', 'fifteen ', 'sixteen ', 'seventeen ', 'eighteen ', 'nineteen '];
var b = ['', '', 'twenty', 'thirty', 'forty', 'fifty', 'sixty', 'seventy', 'eighty', 'ninety'];

$(document).ready(function () {

    //File upload
    $('.custom-file-input, input[type=file]').on("change", function () {

        const fsize = this.files[0].size;

        if (this.files[0].name.length > 100) {
            this.value = null;
            Swal.fire('Failed', 'File name characters should not be greater than 100.', 'error');
            return false;
        }
        if (fsize >= 5242880) {
            this.value = null;
            Swal.fire('Failed', 'File size should not be greater than 5mb.', 'error');
        }
        else {
            var fileName = $(this).val().split("\\").pop();
            $(this).next('.custom-file-label').html(fileName);
        }
    });

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
            Toast.fire({ icon: 'error', title: 'To date cannot be less than from date.' });
        } else {
            $('button[type=submit]').attr('disabled', false);
        }

    });

    $('#FromDate').on('change', function () {

        var d1 = new Date($('#ToDate').val());
        var d2 = new Date($('#FromDate').val());

        if (d1.getTime() < d2.getTime()) {
            $('button[type=submit]').attr('disabled', true);
            Toast.fire({ icon: 'error', title: 'To date can not be greater than from date.' });
        } else {
            $('button[type=submit]').attr('disabled', false);
        }
    });

    $('#input-group-append-From').click(function () {
        $("#FromDate").focus();
    });

    $('#input-group-append-To').click(function () {
        $("#ToDate").focus();
    });
});

$(function () {

    Toast = Swal.mixin({
        toast: true,
        position: 'top-end',
        showConfirmButton: false,
        timerProgressBar: true,
        timer: 7000
    });
});

function Begin() {
    BlockUI();
    $('#Spinner').show();
    $('.ladda-button').attr('disabled', true);
    if ($(".ladda-button")[0] != undefined) {
        Ladda.create($(".ladda-button")[0]).start();
    }
}

function OnSuccess(data) {
    
    if (data.data != undefined) {
        if (data.data.Status && data.data != undefined) {
            Toast.fire({
                icon: 'success',
                title: data.data.Message
            });
            if (data.data.SignalRResponse !== null) {
                var result = CallSignalR(data.data.SignalRResponse);
                if (result) {
                    window.location = data.data.RedirectURL;
                }
            }
            else {
                window.location = data.data.RedirectURL;
            }
        } else {
            Toast.fire({
                icon: 'error',
                title: data.data.Message
            });

            $('button[type="submit"]').attr('disabled', false);
            $('button[type="button"]').attr('disabled', false);

            if ($("button[type=submit]", this)[0] != undefined) {
                Ladda.create($("button[type=submit]", this)[0]).stop();
            }
            if ($("button[type=button]", this)[0] != undefined) {
                Ladda.create($("button[type=button]", this)[0]).stop();
            }
            if ($(".ladda-button")[0] != undefined) {
                Ladda.create($(".ladda-button")[0]).stop();
            }
            UnBlockUI();
            $("body").removeClass("loading");
        }
    }
}

function Complete() {
    UnBlockUI();
    $('#Spinner').hide('slow');

    $('button[type="submit"]').attr('disabled', false);
    if ($("button[type=submit]", this)[0] != undefined) {
        Ladda.create($("button[type=submit]", this)[0]).stop();
    }
    $('button[type="button]"]').attr('disabled', false);
    if ($("button[type=button]", this)[0] != undefined) {
        Ladda.create($("button[type=button]", this)[0]).stop();
    }
    $('.ladda-button').attr('disabled', false);
    if ($(".ladda-button")[0] != undefined) {
        Ladda.create($(".ladda-button")[0]).stop();
    }
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
                BlockUI();
                $.post(window.location.origin + "/" + controllerName + "/" + actionName, { Id: id, Status: val, Remarks: result.value }, function (data) {

                    if (data) {
                        if (data.data.Status) {
                            Toast.fire({ icon: 'success', title: data.data.Message });
                        } else {
                            Toast.fire({ icon: 'error', title: data.data.Message });
                        }
                        if (data.data.SignalRResponse !== null) {
                            CallSignalR(data.data.SignalRResponse);
                        }
                        window.location = data.data.RedirectURL;
                    } else {
                        UnBlockUI();
                        Toast.fire({ icon: 'error', title: 'Error occured while saving changes.' });
                    }
                });
            }
            else {
                UnBlockUI();
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
                BlockUI();
                $.post(window.location.origin + "/" + controllerName + "/" + actionName, { Id: id, Status: val }, function (data) {
                    if (data) {
                        if (data.data.Status) {
                            Toast.fire({ icon: 'success', title: data.data.Message });
                        } else {
                            Toast.fire({ icon: 'error', title: data.data.Message });
                        }
                        if (data.data.SignalRResponse !== null) {
                            CallSignalR(data.data.SignalRResponse);
                        }
                        window.location = data.data.RedirectURL;
                    } else {
                        UnBlockUI();
                        Toast.fire({ icon: 'error', title: 'Error occured while saving changes.' });
                    }
                });
            }
            else {
                UnBlockUI();
            }
        });
    }
}

function inWords(num) {
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
    if ($.blockUI != undefined) {
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
    if (ApplicationPage == "OrderReturn") {

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
                        $('#SiteTRNo').remove();
                    }
                    if (data.data.length > 0 && data.data.find(x => x.PlantLocationId == 2) != undefined) {
                        $('#KorangiTRNo').val(data.data.find(x => x.PlantLocationId == 2).TRNo);
                    }
                    else {
                        $('#KorangiTRNo').remove();
                    }
                    if (data.data.length > 0 && data.data.find(x => x.PlantLocationId == 3) != undefined) {
                        $('#SITEPhytek').val(data.data.find(x => x.PlantLocationId == 3).TRNo);
                    }
                    else {
                        $('#SITEPhytek').remove();
                    }
                    if (data.data.length > 0 && data.data.find(x => x.PlantLocationId == 4) != undefined) {
                        $('#KorangiPhytek').val(data.data.find(x => x.PlantLocationId == 4).TRNo);
                    }
                    else {
                        $('#KorangiPhytek').remove();
                    }
                    $('#modalTR').modal('toggle');
                    $('#modalTR').modal('show');
                } else {
                    Toast.fire({ icon: 'error', title: 'Error occured.' });
                }
            });
        }
    } else if (ApplicationPage == "Order") {
        window.open(window.location.origin + RedirectToUrl + "ApplicationPage=" + ApplicationPage + "&DPID=" + DPID, '_blank');
    }
}

function OrderReturnPrintForm() {

    $('.text-danger').empty();
    if ($('#SiteTRNo').val() == "") {
        $('#errorSiteTRNo').text('TR No is required.');
        return false;
    }
    if ($('#KorangiTRNo').val() == "") {
        $('#errorKorangiTRNo').text('TR No is required.');
        return false;
    }
    if ($('#SITEPhytek').val() == "") {
        $('#errorSITEPhytek').text('TR No is required.');
        return false;
    }
    if ($('#KorangiPhytek').val() == "") {
        $('#errorKorangiPhytek').text('TR No is required.');
        return false;
    }
    var form = event.target.form;
    $(form).submit();
}