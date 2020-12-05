using ExploreCalifornia.Models;
using ExploreCalifornia.Services;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExploreCalifornia
{
    public class ChatHub :Hub
    {
        private readonly IChatRoomService _chatRoomService;

        public ChatHub(IChatRoomService chatRoomService)
        {
            this._chatRoomService = chatRoomService;
        }

        public override async Task OnConnectedAsync()
        {
            var roomId = await _chatRoomService.CreateRoom(Context.ConnectionId);

            await Groups.AddToGroupAsync(Context.ConnectionId, roomId.ToString());


            await Clients.Caller.SendAsync("ReceiveMessage", "Tayyab Bhatti", DateTimeOffset.UtcNow, "Hello! what can I help You");
            await base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }


        public async Task SendMessage(string senderName, string text)
        {
            var roomId = await _chatRoomService.GetRoomForConnectionId(Context.ConnectionId);


            var message = new ChatMessage()
            {
                SenderName = senderName,
                Text = text,
                SendAt = DateTimeOffset.UtcNow
            };


           await _chatRoomService.AddMessage(roomId, message);
            // Broadcast to all Group
            await Clients.Group(roomId.ToString())
                .SendAsync("ReceiveMessage", message.SenderName, message.SendAt, message.Text);
        }

        //public async Task SendMessage(string senderName, string text)
        //{
        //    var message = new ChatMessage()
        //    {
        //        SenderName = senderName,
        //        Text = text,
        //        SendAt = DateTimeOffset.UtcNow
        //    };

        //    // Broadcast to all clinets
        //    await Clients.All.SendAsync("ReceiveMessage", message.SenderName, message.SendAt, message.Text);
        //}



        public async Task SetName(string VisitorName)
        {
            var RoomName = $"Chat with {VisitorName} From the Web";

            var roomId = await _chatRoomService.GetRoomForConnectionId(Context.ConnectionId);

            await _chatRoomService.SetRoomName(roomId, RoomName);
        }
    }
}
