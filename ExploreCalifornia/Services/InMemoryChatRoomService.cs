using ExploreCalifornia.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExploreCalifornia.Services
{
    public class InMemoryChatRoomService : IChatRoomService
    {
        public readonly Dictionary<Guid, ChatRoom> _roomInfo = new Dictionary<Guid, ChatRoom>();

        public readonly Dictionary<Guid, List<ChatMessage>> _messageHistory = new Dictionary<Guid,List<ChatMessage>>();

        public Task AddMessage(Guid roomId, ChatMessage chatMessage)
        {
            if(!_messageHistory.ContainsKey(roomId))
            {
                _messageHistory[roomId] = new List<ChatMessage>();
            }

            _messageHistory[roomId].Add(chatMessage);

            return Task.CompletedTask;
        }
        public Task<IEnumerable<ChatMessage>> GetMessageHistory(Guid roomId)
        {
            _messageHistory.TryGetValue(roomId, out var Messages);

            Messages = Messages ?? new List<ChatMessage>();

            var sortedMessages = Messages.OrderBy(s => s.SendAt).AsEnumerable();


            return Task.FromResult(sortedMessages);
        }



        public Task<Guid> CreateRoom(string connectionId)
        {
            var id = Guid.NewGuid();

            _roomInfo[id] = new ChatRoom()
            {
                OwnerConnectionId = connectionId
            };

            return Task.FromResult(id);
        }

        
        public Task<Guid> GetRoomForConnectionId(string connectionId)
        {
            var foundroom = _roomInfo.FirstOrDefault(s => s.Value.OwnerConnectionId == connectionId);

            if (foundroom.Key == Guid.Empty)
                throw new ArgumentException("Invalid Room");

            return Task.FromResult(foundroom.Key);
        }

        public Task SetRoomName(Guid roomId, string RoomName)
        {
            if(!_roomInfo.ContainsKey(roomId))
            {
                throw new ArgumentException("Invalid Room ID");
            }


            _roomInfo[roomId].Name = RoomName;

            return Task.CompletedTask;
        }
    }
}
