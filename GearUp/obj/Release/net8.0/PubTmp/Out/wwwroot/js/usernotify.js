"use strict";

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/notificationHub")
    .build();

// Start connection
connection.start().then(function () {
    console.log("Connected to SignalR hub.");
}).catch(function (err) {
    return console.error(err.toString());
});

// Listen for car notifications
connection.on("ReceiveCarNotification", function (category, carHtml) {
    const container = document.querySelector(`[data-category='${category}']`);
    if (container) {
        container.insertAdjacentHTML('beforeend', carHtml); // Append car block
    }
});

