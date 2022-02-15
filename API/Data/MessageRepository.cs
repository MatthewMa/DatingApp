using API.DTOS;
using API.Entities;
using API.Interfaces;
using API.Tools;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public MessageRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<Message> GetMessageByIdAsync(int id)
        {
            return await _context.Messages.Include(u => u.Sender)
                .Include(u => u.Receipient).FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<PagedList<MessageDTO>> GetMessagesForUserAsync(MessageParams messageParams)
        {
            var query = _context.Messages.OrderByDescending(m => m.DateSend).AsQueryable();                       
            query = messageParams.Container switch
            {
                "Inbox" => query.Where(u => u.RecipientUserName.ToLower() == messageParams.UserName.ToLower() && u.RecipientDeleted == false),
                "Outbox" => query.Where(u => u.SenderUserName.ToLower() == messageParams.UserName.ToLower() && u.SenderDeleted == false),
                _ => query.Where(u => u.RecipientUserName.ToLower() == messageParams.UserName.ToLower() && u.DateRead == null && u.RecipientDeleted == false)
            };
            return await PagedList<MessageDTO>.CreateAsync(query.ProjectTo<MessageDTO>(_mapper.ConfigurationProvider).AsNoTracking(),
                messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDTO>> GetMessageThread(string senderUsername, string recipientUserName)
        {
            var messages = await _context.Messages.Include(u => u.Sender).ThenInclude(p => p.Photos)
                .Include(u => u.Receipient).ThenInclude(p => p.Photos)
                .Where(m => (m.RecipientUserName.ToLower() == recipientUserName.ToLower() && m.SenderDeleted == false && m.SenderUserName.ToLower() == senderUsername.ToLower())
                || (m.RecipientUserName.ToLower() == senderUsername.ToLower() && m.SenderUserName.ToLower() == recipientUserName.ToLower()
                && m.RecipientDeleted == false))
                .OrderByDescending(m => m.DateSend).ToListAsync();
            var unreadMessages = messages.Where(m => m.DateRead == null && m.RecipientUserName.ToLower() == senderUsername.ToLower())
                .ToList();
            if (unreadMessages.Any())
            {
                foreach (var unreadMessage in unreadMessages)
                {
                    unreadMessage.DateRead = DateTime.Now;
                }
                await SaveAllAsync();
            }
            return _mapper.Map<IEnumerable<MessageDTO>>(messages);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
