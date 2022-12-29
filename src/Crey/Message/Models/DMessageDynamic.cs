﻿using Crey.Extensions;
using Crey.Helper;
using Crey.Message.Abstractions;

namespace Crey.Message.Models;

public class DMessageDynamic
{
    private readonly IMessageSerializer _serialize;

    public virtual string ContentType { get; set; }
    public virtual byte[] Content { get; set; }

    public DMessageDynamic() { }

    public DMessageDynamic(IMessageSerializer serialize)
    {
        _serialize = serialize;
    }

    public void SetValue(object value)
    {
        if (value == null)
            return;

        var type = value.GetType();
        ContentType = type.TypeName();
        var code = Type.GetTypeCode(type);
        Content = _serialize.Serialize(code == TypeCode.Object ? JsonHelper.ToJson(value) : value);
    }

    public object GetValue()
    {
        if (Content == null || string.IsNullOrWhiteSpace(ContentType))
            return null;

        var type = Type.GetType(ContentType);
        var code = Type.GetTypeCode(type);
        if (code == TypeCode.Object)
        {
            var content = _serialize.Deserialize<string>(Content);
            return JsonHelper.FromJson(content, type);
        }

        return _serialize.Deserialize(Content, type);
    }
}
