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
        //var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
        //var encodedMsg = UserId.toString() + " says: " + msg;
        //var li = document.createElement("li");
        //li.textContent = encodedMsg;
        //document.getElementById("messagesList").appendChild(li);
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
