﻿using BeetleX;
using BeetleX.Buffers;
using SpanJson;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PlatformBenchmarks
{
    public partial class HttpHandler
    {
        private readonly static uint _jsonPayloadSize = (uint)System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(new JsonMessage { message = "Hello, World!" }, SerializerOptions).Length;

        private readonly static AsciiString _jsonPreamble =
            _headerContentLength + _jsonPayloadSize.ToString();

        private static readonly JsonSerializerOptions SerializerOptions = new JsonSerializerOptions();

        private static Utf8JsonWriter GetUtf8JsonWriter(PipeStream stream, HttpToken token)
        {
            var buffer = stream.CreateBufferWriter();
            if (token.Utf8JsonWriter == null)
            {
                token.Utf8JsonWriter = new Utf8JsonWriter(buffer, new JsonWriterOptions { SkipValidation = true });
            }
            var writer = token.Utf8JsonWriter;
            writer.Reset(buffer);
            return writer;
        }

        public ValueTask Json(PipeStream stream, HttpToken token, ISession session)
        {
            if (Program.Debug)
            {
                System.Text.Json.JsonSerializer.Serialize<JsonMessage>(GetUtf8JsonWriter(stream, token), new JsonMessage { message = "Hello, World!" }, SerializerOptions);   
            }
            else
            {
                SpanJson.JsonSerializer.NonGeneric.Utf8.SerializeAsync(new JsonMessage { message = "Hello, World!" }, stream);
            }
            OnCompleted(stream, session, token);
            return ValueTask.CompletedTask;
        }
    }
}
