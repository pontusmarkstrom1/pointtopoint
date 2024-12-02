﻿using PointToPoint.Payload;
using PointToPoint.Payload.NewtonsoftJson;
using PointToPoint.Protocol;
using System.Text;

namespace PointToPointTests.Payload;

public class NewtonsoftJsonPayloadSerializerTests
{
    [Fact]
    public void SerializeDeserialize_PayloadRecord()
    {
        // Arrange
        var message = new PayloadForTest(10, "text");
        var serializer = new NewtonsoftJsonPayloadSerializer(typeof(PayloadForTest).Assembly);

        // Act
        var payload = serializer.MessageToPayload(message);
        var deserializedMessage = serializer.PayloadToMessage(payload, payload.Length);

        // Assert
        Assert.Equal(message, deserializedMessage);
    }

    [Fact]
    public void SerializeDeserialize_PayloadWithParameterlessConstructor()
    {
        // Arrange
        var message = new PayloadWithParameterlessConstructorForTest() { Value = 10, Text = "text" };
        var serializer = new NewtonsoftJsonPayloadSerializer(typeof(PayloadWithParameterlessConstructorForTest).Assembly);

        // Act
        var payload = serializer.MessageToPayload(message);
        var deserializedMessage = (PayloadWithParameterlessConstructorForTest)serializer.PayloadToMessage(payload, payload.Length);

        // Assert
        Assert.Equal(message.Value, deserializedMessage.Value);
        Assert.Equal(message.Text, deserializedMessage.Text);
    }

    [Fact]
    public void SerializeDeserialize_KeepAlive()
    {
        // Arrange
        var message = new KeepAlive();
        var serializer = new NewtonsoftJsonPayloadSerializer(typeof(PayloadForTest).Assembly);

        // Act
        var payload = serializer.MessageToPayload(message);
        var deserializedMessage = serializer.PayloadToMessage(payload, payload.Length);

        // Assert
        Assert.Equal(message, deserializedMessage);
    }

    [Fact]
    public void PayloadToMessage_NoJsonIncluded()
    {
        // Arrange
        var protocolNamespace = typeof(PayloadForTest).Namespace!;
        var bytes = Encoding.Unicode.GetBytes($"{protocolNamespace}.{nameof(PayloadForTest)} ");
        var serializer = new NewtonsoftJsonPayloadSerializer(typeof(PayloadForTest).Assembly);

        // Act
        Assert.Throws<PayloadDeserializeException>(() => serializer.PayloadToMessage(bytes, bytes.Length));
    }
}

