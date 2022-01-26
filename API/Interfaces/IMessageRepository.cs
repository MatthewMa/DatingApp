using API.DTOS;
using API.Entities;
using API.Tools;

namespace API.Interfaces
{
    public interface IMessageRepository
    {
        void AddMessage(Message message);
        void DeleteMessage(Message message);
        Task<Message> GetMessageByIdAsync(int id);
        Task<PagedList<MessageDTO>> GetMessagesForUserAsync(MessageParams messageParams);
        Task<IEnumerable<MessageDTO>> GetMessageThread(string senderUsername, string recipientUserName);
        Task<bool> SaveAllAsync();
    }
}
