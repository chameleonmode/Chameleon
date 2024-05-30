namespace Chameleon.Core.Util;

public static class IOUtil
{
    public static void CreateDirectory(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    public static void DeleteFileIfExists(string filePath)
    {
        if (File.Exists(filePath))
        {
            try
            {
                File.Delete(filePath);
            }
            catch (IOException ex)
            {
                // Handle I/O exception, e.g., log it
                Console.WriteLine($"I/O error occurred: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                // Handle unauthorized access exception, e.g., log it
                Console.WriteLine($"Access error occurred: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Handle any other exception, e.g., log it
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}
