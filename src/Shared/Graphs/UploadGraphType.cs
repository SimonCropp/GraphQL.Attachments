using GraphQL.Types;
using GraphQLParser.AST;

public class UploadGraphType : ScalarGraphType
{
    public UploadGraphType() => Name = "Upload";

    public override object ParseLiteral(GraphQLValue value) => new Upload();

    public override object ParseValue(object? value) => new Upload();

    public override object Serialize(object? value) => "";
}