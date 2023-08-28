namespace SqlBuilder;

internal class Parameter
{
    public string FieldName { get; set; }
    
    public required string Name { get; set; }

    public required Type Type { get; set; }

    public required object? Value { get; set; }
}