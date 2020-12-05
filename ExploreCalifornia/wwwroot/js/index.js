var chatterName = 'Visitor';


var dialogE1 = document.getElementById('chatDialog');
// Initialize the SinglR client

var connection = new signalR.HubConnectionBuilder()
    .withUrl('/ChatHub').build();


connection.on('ReceiveMessage', renderMessage);

//connection.start();

connection.onclose(function () {
    onDisconnected();
    console.log('Reconnecting the Connection in 5 sec');

    setTimeout(startConnection, 5000);
});


function startConnection() {
    connection.start().
        then(onConnected)
        .catch(function (err) {
            console.error(err);
        });
}


function onDisconnected() {
    dialogE1.classList.add('disconnected');
}



function onConnected() {
    dialogE1.classList.remove('disconnected');

    var messageTextboxEl = document.getElementById('messageTextbox');
    messageTextboxEl.focus();


    connection.invoke("SetName", chatterName);
}



function showChatDialog() {
  

    dialogE1.style.display = 'block';
}


function sendMessage(text) {
    if (text && text.length) {
        connection.invoke('SendMessage', chatterName, text);
    }
}


function ready() {
    setTimeout(showChatDialog, 750);


    var chatFormE1 = document.getElementById('chatForm');

    chatFormE1.addEventListener('submit', function (e) {
        e.preventDefault();

        var text = e.target[0].value;
        e.target[0].value = '';

        sendMessage(text);  
    });


    var welcomePanelEl = document.getElementById('chatWelcomePanel');
    welcomePanelEl.addEventListener('submit', function (e) {
        e.preventDefault();

        var name = e.target[0].value;
        if (name && name.length) {
            welcomePanelEl.style.display = 'none';
            chatterName = name;
            startConnection();
        }
    });
}



function renderMessage(name, time, message) {
    var nameSpan = document.createElement('span');
    nameSpan.className = 'name';
    nameSpan.textContent = name;

    var timeSpan = document.createElement('span');
    timeSpan.className = 'time';
    var friendlyTime = moment(time).format('H:mm');
    timeSpan.textContent = friendlyTime;

    var headerDiv = document.createElement('div');
    headerDiv.appendChild(nameSpan);
    headerDiv.appendChild(timeSpan);

    var messageDiv = document.createElement('div');
    messageDiv.className = 'message';
    messageDiv.textContent = message;

    var newItem = document.createElement('li');
    newItem.appendChild(headerDiv);
    newItem.appendChild(messageDiv);

    var chatHistoryEl = document.getElementById('chatHistory');
    chatHistoryEl.appendChild(newItem);
    chatHistoryEl.scrollTop = chatHistoryEl.scrollHeight - chatHistoryEl.clientHeight;
}


document.addEventListener('DOMContentLoaded', ready);