namespace WCNU;

public class Patch
{
    public required string Name { get; init; }
    public required byte[] Pattern { get; init; }
    public required byte[] Replace { get; init; }
    public int Offset { get; init; }

    public byte[] GetExpectedResult()
    {
        var result = new byte[Pattern.Length];
        Array.Copy(Pattern, result, Pattern.Length);
        
        for (var i = Offset; i < Replace.Length; i++)
        {
            result[i] = Replace[i];
        }

        return result;
    }
}