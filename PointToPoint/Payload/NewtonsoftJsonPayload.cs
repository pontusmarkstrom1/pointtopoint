﻿using System;
using System.Text;
using Newtonsoft.Json;

namespace PointToPoint.Payload
{
    public class NewtonsoftJsonPayload : IPayloadSerializer
    {
        private static readonly Encoding PayloadTextEncoding = Encoding.Unicode;
        private const string IdPayloadSeparator = " ";

        private readonly JsonSerializerSettings serializerSettings;

        public NewtonsoftJsonPayload(Formatting formatting = Formatting.None) : this(new JsonSerializerSettings() { Formatting = formatting })
        {
        }

        public NewtonsoftJsonPayload(JsonSerializerSettings serializerSettings)
        {
            this.serializerSettings = serializerSettings;
        }

        public byte[] MessageToPayload(object message)
        {
            var json = JsonConvert.SerializeObject(message, serializerSettings);
            var messageType = message.GetType();
            var assemblyName = messageType.Assembly.FullName.Split(',')[0];
            var messageString = $"{messageType},{assemblyName}{IdPayloadSeparator}{json}";
            return SerializeString(messageString);
        }

        public object PayloadToMessage(byte[] payloadBytes, int length)
        {
            var payload = DeserializeString(payloadBytes, length);

            var separatorIndex = payload.IndexOf(IdPayloadSeparator);
            if (separatorIndex < 1)
            {
                throw new Exception("Message type/json separator not found");
            }
            if (separatorIndex == payload.Length - 1)
            {
                throw new Exception("Message json not found");
            }

            var jsonTypeString = payload.Substring(0, separatorIndex);
            var jsonType = Type.GetType(jsonTypeString);
            if (jsonType == null)
            {
                throw new Exception("Unknown message type");
            }

            var json = payload.Substring(separatorIndex + 1);

            // Note: may throw JsonException
            return JsonConvert.DeserializeObject(json, jsonType);
        }

        private static byte[] SerializeString(string data)
        {
            return PayloadTextEncoding.GetBytes(data);
        }

        private string DeserializeString(byte[] bytes, int length)
        {
            return PayloadTextEncoding.GetString(bytes, 0, length);
        }
    }
}