﻿using PointToPoint.Server;
using PointToPoint.Protocol;
using Protocol;
using PointToPoint.Messenger;

namespace Server;

public class ChatMessageForwarder
{
    private readonly IMessageSender messageSender;

    public ChatMessageForwarder(IMessageSender messageSender)
    {
        this.messageSender = messageSender;
    }

    public void HandleMessage(PublishText message, IMessenger messenger)
    {
        Console.WriteLine($"Forwarding message '{message.Message}' to all.");
        messageSender.SendBroadcast(new Text(message.Message, DateTime.Now));

        switch (message.Message.ToLower())
        {
            case "hi":
            case "hello":
                Console.WriteLine($"Sending greeting.");
                messageSender.SendMessage(new Text($"And {message.Message} to you! /Server", DateTime.Now), messenger.Id);
                break;
        }
    }

    public void HandleMessage(KeepAlive message, IMessenger messenger)
    {
    }
}
