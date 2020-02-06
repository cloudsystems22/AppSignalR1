﻿using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AppSignalR1
{

    public class ChatMessage : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public string Content { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

    }

    public class ChatVM : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public string Message { get; set; }

        private HubConnection Connection;
        private IHubProxy ChatHubProxy;
        public delegate void MessageReceived(string name, string message);
        public event MessageReceived OnMessageReceived;
        public Command SendCmd { get; set; }
        public ObservableCollection<ChatMessage> Messages { get; set; }
        public string ConnectionState { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public ChatVM(string url)
        {

            SendCmd = new Command(() =>
            {
                if (String.IsNullOrEmpty(Name) || String.IsNullOrEmpty(Message))
                {
                    return;
                }
                ChatHubProxy.Invoke("Send", Name, Message);
            });
            try
            {
                Messages = new ObservableCollection<ChatMessage>();
                Connection = new HubConnection(url);
                Connection.StateChanged += (StateChange obj) =>
                {
                    ConnectionState = obj.NewState.ToString();
                };
                OnMessageReceived += (name, message) =>
                {
                    Messages.Add(new ChatMessage() { Content = message, Name = name });
                };
                ChatHubProxy = Connection.CreateHubProxy("MyHub1");
                ChatHubProxy.On<string, string>("addNewMessageToPage", (username, text) => {
                    OnMessageReceived?.Invoke(username, text);
                });
                Connection.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }


    public partial class MainPage : ContentPage
    {
        public const string IP = "promo.cloudtracker.com.br";
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new ChatVM($"https://{IP}/Realpromo40Web");
        }
    }
}
