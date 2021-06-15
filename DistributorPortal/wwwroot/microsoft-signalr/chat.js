"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable send button until connection is established
//document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (userId, message) {
     
    if (UserId.toString() == message.userId) {
        document.getElementById('xyz').play();
        $(document).Toasts('create', {
            class: 'bg-info',
            delay: 1000,
            title: message.number,
            body: message.message
        });
        //$('#OrderNum').append('<a href="/Order/OrderView' + message.number + '" class="dropdown-item"><div class="media">  \
        //<div class="media-body"><h6 class="dropdown-item-">'+ message.number + ': ' + message.message + '<span class="float-right text-sm text-danger"> \
        //<i class="fas fa-star"></i></span></h6></div></div></a>');
        $('#AppendNotification').append('<div class="dropdown-item" style="background: #17a2b8d9; color: #fff;"><div class="media">  \
        <div class="media-body"><p class="dropdown-item-title">'+ message.number + '</p><p  class="text-sm">' + message.message + '</p><p><i class="far fa-clock mr-1"> Just ago</i></p> \
        </div></div></div><hr class="p-0 m-0" />');
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
