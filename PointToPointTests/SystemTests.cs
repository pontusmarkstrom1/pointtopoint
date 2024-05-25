﻿using PointToPoint.MessageRouting;
using PointToPoint.Messenger;
using PointToPoint.Messenger.Tcp;
using PointToPoint.Payload;
using PointToPoint.Protocol;
using PointToPoint.Server;

namespace PointToPointTests;

public class SystemTests
{
    private readonly List<object> clientReceivedMessages = [];

    [Fact]
    public void MessagesBetweenClientAndServer()
    {
        // Arrange - start the server
        var port = 12345;
        var keepAliveInterval = TimeSpan.FromSeconds(1);

        var clientHandler = new ClientsHandler(
            new NewtonsoftJsonPayloadSerializer(typeof(ClientToServerMessage).Namespace!),
            typeof(TestClientHandler),
            keepAliveInterval);

        var tcpServer = new TcpServer(port);
        var tcpServerThread = new Thread(() => tcpServer.Run(clientHandler));
        tcpServerThread.Start();

        // Arrange - start the client
        var clientMessenger = new TcpMessenger("127.0.0.1", port,
            new NewtonsoftJsonPayloadSerializer(typeof(ClientToServerMessage).Namespace!),
            new ReflectionMessageRouter(this),
            new SocketFactory(),
            keepAliveInterval);
        clientMessenger.Start();

        // Act - send message from client to server
        clientMessenger.Send(new ClientToServerMessage());

        Thread.Sleep(3000);

        // Shutdown
        tcpServer.Stop();
        clientHandler.Stop();
        clientMessenger.Stop();

        TestUtils.WaitFor(clientHandler.IsStopped);
        TestUtils.WaitFor(clientMessenger.IsStopped);

        // Assert
        Assert.Single(clientReceivedMessages.OfType<ServerToClientMessage>());
        Assert.Single(clientReceivedMessages.OfType<ServerToClientBroadcastMessage>());
        Assert.True(clientReceivedMessages.OfType<KeepAlive>().Count() > 1);
    }

    public void HandleMessage(ServerToClientMessage message, IMessenger messenger)
    {
        clientReceivedMessages.Add(message);
    }

    public void HandleMessage(ServerToClientBroadcastMessage message, IMessenger messenger)
    {
        clientReceivedMessages.Add(message);
    }

    public void HandleMessage(KeepAlive message, IMessenger messenger)
    {
        clientReceivedMessages.Add(message);
    }
}

public record ClientToServerMessage();
public record ServerToClientMessage();
public record ServerToClientBroadcastMessage();

public class TestClientHandler : IClientHandler
{
    public readonly List<object> receivedMessages = [];

    public void Init(IMessageSender messageSender)
    {
        messageSender.SendMessage(new ServerToClientMessage(), this);
        messageSender.SendBroadcast(new ServerToClientBroadcastMessage());
    }

    public void Exit(Exception? _)
    {
        Assert.Single(receivedMessages.OfType<ClientToServerMessage>());
        Assert.True(receivedMessages.OfType<KeepAlive>().Count() > 1);
    }

    public void HandleMessage(ClientToServerMessage message, IMessenger messenger)
    {
        receivedMessages.Add(message);
    }

    public void HandleMessage(KeepAlive message, IMessenger messenger)
    {
        receivedMessages.Add(message);
    }
}
