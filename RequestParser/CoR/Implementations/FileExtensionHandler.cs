namespace RequestParser.CoR.Implementations;

public class FileExtensionHandler(string requiredExtension) : AbstractHandler
{
    public override bool Handle(string input)
    {
        if (!input.EndsWith(requiredExtension, StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine($"Error: The file must have a '{requiredExtension}' extension.");
            Console.WriteLine($"Provided file: {input}");
            return false;
        }

        return base.Handle(input);
    }
}