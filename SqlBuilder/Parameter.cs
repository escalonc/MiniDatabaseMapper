namespace SqlBuilder;

internal class Parameter
{
    public required string Name { get; set; }

    public required Type Type { get; set; }

    public required object Value { get; set; }
}