namespace Chameleon.Core.Util;

public static class IOtil
{
    public static Task CreateDirectory(string path)
    {
        return Task.Run(() =>
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        });
    }

    public static async Task DeleteFExists(string filePath)
    {
        if (File.Exists(filePath))
        {
            try
            {
                await Task.Run(() => File.Delete(filePath));
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

    public static async Task DeleteDExists(string filePath, bool recuersive = true)
    {
        if (Directory.Exists(filePath))
        {
            try
            {
                await Task.Run(() => Directory.Delete(filePath, recuersive));
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
