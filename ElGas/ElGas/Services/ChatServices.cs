
using System;
using Microsoft.AspNet.SignalR.Client;
using Xamarin.Forms;
using ElGas;
using System.Threading.Tasks;
using ElGas.Services;
using ElGas.Models;

[assembly: Dependency(typeof(ChatServices))]

namespace ElGas.Services
{
    public class ChatServices : IChatServices
    {
        private readonly HubConnection _connection;
        private readonly IHubProxy _proxy;

        public event EventHandler<ChatMessage> OnMessageReceived;

        public ChatServices()
        {
            _connection = new HubConnection("http://xamarin-chat.azurewebsites.net/");
            _proxy = _connection.CreateHubProxy("ChatHub");
        }

        #region IChatServices implementation

        public async Task Connect()
        {
            await _connection.Start();

            _proxy.On("GetMessage", (string name, string message) => OnMessageReceived(this, new ChatMessage
            {
                Name = name,
                Message = message
            }));
        }

        public async Task Send(ChatMessage message, string roomName)
        {
            _proxy.Invoke("SendMessage", message.Name, message.Message, roomName);
        }

        public async Task JoinRoom(string roomName)
        {
            _proxy.Invoke("JoinRoom", roomName);
        }

        #endregion
    }
}
