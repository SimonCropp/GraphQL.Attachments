using System;
using GraphQL.Language.AST;
using GraphQL.Types;

public class AttachmentInput : InputObjectGraphType<Attachment>
{
    public AttachmentInput()
    {
        Field<NonNullGraphType<StringGraphType>>("name");
        Field<NonNullGraphType<StreamGraphType>>("stream");
    }
}
public class StreamGraphType : ScalarGraphType
{
    public override object Serialize(object value)
    {
        return value;
    }

    public override object ParseValue(object value)
    {
        return value;
    }

    public override object ParseLiteral(IValue value)
    {
        var stringValue = value as StreamFuncValue;
        return stringValue?.Value;
    }
}