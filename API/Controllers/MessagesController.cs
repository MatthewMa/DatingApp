using API.DTOS;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using API.Tools;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class MessagesController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;

        public MessagesController(IUserRepository userRepository, IMessageRepository messageRepository,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _messageRepository = messageRepository;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<MessageDTO>> AddMessage([FromBody] MessageCreateDTO messageCreateDTO)
        {
            var userId = User.GetUserId();
            var userName = User.GetUserName();
            var senderUser =  await _userRepository.GetUserByUserNameAsync(userName);
            var recipientUser = await _userRepository.GetUserByUserNameAsync(messageCreateDTO.RecipientUserName);
            if (recipientUser == null) return BadRequest("Recipient cannot find");
            if (userId == recipientUser.Id) return BadRequest("You cannot send message to yourself.");
            var messageCreated = new Message()
            {
                Sender = senderUser,
                Receipient = recipientUser,
                SenderId = senderUser.Id,
                SenderUserName = senderUser.UserName,
                RecipientUserName = recipientUser.UserName,
                RecipientId = recipientUser.Id,
                Content = messageCreateDTO.Content
            };
            _messageRepository.AddMessage(messageCreated);
            if (!await _messageRepository.SaveAllAsync()) return BadRequest("Message added failed");
            return CreatedAtRoute("GetMessages", null, _mapper.Map<MessageDTO>(messageCreated));
        }

        [HttpGet(Name = "GetMessages")]
        public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessages([FromQuery] MessageParams messageParams)
        {
            string userName = User.GetUserName();
            messageParams.UserName = userName;
            var messages = await _messageRepository.GetMessagesForUserAsync(messageParams);
            if (messages == null) return NotFound("Messages not found");
            Response.AddPaginationHeader(messageParams.PageNumber, messageParams.PageSize,
                messages.TotalCount, messages.TotalPages);
            return Ok(messages.List);
        }

        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessages(string username)
        {
            if (string.IsNullOrEmpty(username)) return BadRequest("Username is null");
            var senderUserName = User.GetUserName();
            var thread = await _messageRepository.GetMessageThread(senderUserName, username);
            if (thread == null) return NotFound("Thread not found");
            return Ok(thread);
        }
    }
}
