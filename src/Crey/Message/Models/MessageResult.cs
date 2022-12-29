﻿using System.Net;

namespace Crey.Message.Models;

public class MessageResult : DMessage
{
    public int Code { get; set; } = 200;
    public string Message { get; set; }
    public object Content { get; set; }

    public MessageResult() { }

    public MessageResult(string message, int code = (int)HttpStatusCode.InternalServerError)
    {
        Message = message;
        Code = code;
    }
}
