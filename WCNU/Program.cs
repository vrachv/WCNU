namespace WCNU;

class Program
{
    private const string PatchedName = "wow-wcnu.exe";

    private static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Usage: WCNU <wow.exe path>");
            PressAnyKeyToExit();
            return;
        }
        
        var path = args[0];
        if (!File.Exists(path))
        {
            Console.WriteLine("File not found: " + path);
            PressAnyKeyToExit();
            return;
        }

        try
        {
            if (IsPatched(path))
            {
                Console.WriteLine("File is already patched");
                PressAnyKeyToExit();
                return;
            }
            
            var result = FindAndPatch(path);
            if (!result)
            {
                Console.WriteLine("Target pattern not found in the file");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        PressAnyKeyToExit();
    }

    private static bool FindAndPatch(string filePath)
    {
        byte[] searchPattern =
        [
            0x75, 0x37, 0x8B, 0x4D, 0x0C, 0x85, 0xC9, 0x74, 
            0x1C, 0x0F, 0xB7, 0x01, 0x66, 0x85, 0xC0, 0x74
        ];
            
        byte[] replacePattern = [0xEB, 0x37];
        
        var fileBytes = File.ReadAllBytes(filePath);
        var patternFound = false;

        for (var i = 0; i < fileBytes.Length - searchPattern.Length; i++)
        {
            if (fileBytes.Skip(i).Take(searchPattern.Length).SequenceEqual(searchPattern))
            {
                Console.WriteLine($"Pattern founded: 0x{i:X8}");
                
                Buffer.BlockCopy(
                    replacePattern, 
                    0, 
                    fileBytes, 
                    i,
                    replacePattern.Length
                );

                patternFound = true;
                break;
            }
        }

        if (patternFound)
        {
            File.Copy(filePath, PatchedName, true);
            var newPath = Path.Combine(Path.GetDirectoryName(filePath)!, PatchedName);
            File.WriteAllBytes(newPath, fileBytes);
            Console.WriteLine($"Patched file created at: {newPath}");
        }

        return patternFound;
    }

    private static bool IsPatched(string filePath)
    {
        try
        {
            byte[] patchedPattern =
            [
                0xEB, 0x37, 0x8B, 0x4D, 0x0C, 0x85, 0xC9, 0x74, 
                0x1C, 0x0F, 0xB7, 0x01, 0x66, 0x85, 0xC0, 0x74
            ];

            var fileBytes = File.ReadAllBytes(filePath);
            
            for (var i = 0; i < fileBytes.Length - patchedPattern.Length; i++)
            {
                if (fileBytes.Skip(i).Take(patchedPattern.Length).SequenceEqual(patchedPattern))
                {
                    return true;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Patch verification failed: {ex.Message}");
        }
        
        return false;
    }

    private static void PressAnyKeyToExit()
    {
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}