namespace WCNU;

class Program
{
    private const string PatchedName = "wow-wcnu";

    private static readonly Patch CharacterNameUnlockPatch = new()
    {
        Name = "Character Name Unlock",
        Pattern = [0x75, 0x37, 0x8B, 0x4D, 0x0C, 0x85, 0xC9, 0x74, 0x1C, 0x0F, 0xB7, 0x01, 0x66, 0x85, 0xC0, 0x74],
        Replace = [0xEB]
    };

    private static readonly Patch DeclensionsNameUnlockPatch = new()
    {
        Name = "Declensions Name Unlock",
        Pattern = [0x0F, 0x85, 0xDB, 0x00, 0x00, 0x00, 0x83, 0xC6, 0x01, 0x83, 0xFE, 0x05, 0x7C, 0xCE, 0x33, 0xF6],
        Replace = [0x90, 0x90, 0x90, 0x90, 0x90, 0x90]
    };

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
        
        var patchesApplied = false;
        byte[] originalBytes = File.ReadAllBytes(path);

        patchesApplied |= TryPatch(ref originalBytes, CharacterNameUnlockPatch);
        patchesApplied |= TryPatch(ref originalBytes, DeclensionsNameUnlockPatch);

        if (patchesApplied)
        {
            var newPath = GetNewPath(path);
            File.WriteAllBytes(newPath, originalBytes);
            Console.WriteLine($"Patched file saved as {newPath}");
        }

        PressAnyKeyToExit();
    }

    private static string GetNewPath(string path)
    {
        var count = 1;
        var directory = Path.GetDirectoryName(path)!;
        var newPath = Path.Combine(directory, $"{PatchedName}.exe");
        
        while (File.Exists(newPath))
        {
            count++;
            newPath = Path.Combine(directory, $"{PatchedName}_{count}.exe");
        }

        return newPath;
    }

    private static bool TryPatch(ref byte[] bytes, Patch patch)
    {
        try
        {
            if (IsPatched(bytes, patch))
            {
                Console.WriteLine($"[{patch.Name}] Already patched");
                return false;
            }
            
            var result = FindAndPatch(ref bytes, patch);
            if (!result)
            {
                Console.WriteLine($"[{patch.Name}] Target pattern not found in the file");
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return false;
        }
    }

    private static bool FindAndPatch(ref byte[] bytes, Patch patch)
    {
        var patternFound = false;

        for (var i = 0; i < bytes.Length - patch.Pattern.Length; i++)
        {
            if (bytes.Skip(i).Take(patch.Pattern.Length).SequenceEqual(patch.Pattern))
            {
                Console.WriteLine($"[{patch.Name}] Pattern founded: 0x{i:X8}");
                
                Buffer.BlockCopy(
                    patch.Replace, 
                    0,
                    bytes, 
                    i + patch.Offset,
                    patch.Replace.Length
                );

                patternFound = true;
                break;
            }
        }

        if (patternFound)
        {
            Console.WriteLine($"[{patch.Name}] Patched");
        }

        return patternFound;
    }

    private static bool IsPatched(byte[] bytes, Patch patch)
    {
        try
        {
            var patchedPattern = patch.GetExpectedResult();

            for (var i = 0; i < bytes.Length - patchedPattern.Length; i++)
            {
                if (bytes.Skip(i).Take(patchedPattern.Length).SequenceEqual(patchedPattern))
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