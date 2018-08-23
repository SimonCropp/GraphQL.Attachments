using System;
using System.IO;
using GraphQL;
using GraphQL.Language.AST;
using GraphQL.Types;

public class Schema : GraphQL.Types.Schema
{
    public Schema(IDependencyResolver resolver) :
        base(resolver)
    {
        Query = resolver.Resolve<Query>();
        Mutation = resolver.Resolve<Mutation>();
        RegisterValueConverter(new StreamFuncValueConverter());
    }
}
public class StreamFuncValueConverter : IAstFromValueConverter
{
    public bool Matches(object value, IGraphType type)
    {
        return value is Func<Stream>;
    }

    public IValue Convert(object value, IGraphType type)
    {
        return new StreamFuncValue((Func<Stream>)value);
    }
}

public class StreamFuncValue : ValueNode<Func<Stream>>
{
    public StreamFuncValue(Func<Stream> value)
    {
        Value = value;
    }

    protected override bool Equals(ValueNode<Func<Stream>> node)
    {
        return Value == node?.Value;
    }
}