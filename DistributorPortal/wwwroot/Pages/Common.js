$(document).ready(function () {
    $('#Spinner').hide();
});

var Toast;
$(function () {
    debugger;
    Toast = Swal.mixin({
        toast: true,
        position: 'top-end',
        showConfirmButton: false,
        timer: 3000
    });
});

function Begin() {
    debugger;
    $('#Spinner').show();
}
function OnSuccess(data) {
    debugger;
    if (data.data.Status) {
        Toast.fire({
            icon: 'success',
            title: data.data.Message
        })
        window.location = data.data.RedirectURL;
    }
    else {
        Toast.fire({
            icon: 'error',
            title: data.data.Message
        })
    }
}
function Complete() {
    debugger;
    $('#Spinner').hide();
}