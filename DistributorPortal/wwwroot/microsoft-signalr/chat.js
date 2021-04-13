"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable send button until connection is established
//document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (userId, message) {
    debugger;
    if (UserId.toString() == message.userId) {
        document.getElementById('xyz').play();
        $(document).Toasts('create', {
            class: 'bg-info',
            delay: 1000,
            title: message.number,
            body: message.message
        });
        $('#OrderNum').append('<a href="/Order/OrderView' + message.number + '" class="dropdown-item"><div class="media">  \
        <div class="media-body"><h6 class="dropdown-item-">'+ message.number + ': ' + message.message + '<span class="float-right text-sm text-danger"> \
        <i class="fas fa-star"></i></span></h6></div></div></a>');
    }
});

connection.start();


function CallSignalR(message) {
    var user = UserId.toString();
    connection.invoke("SendMessage", user, message).catch(function (err) {
        return console.error(err.toString());
    });
    return true;
}
