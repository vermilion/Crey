﻿namespace Crey.Message.Abstractions;

public interface IMessageEncoder
{
    Task<byte[]> EncodeAsync(object message);
}
