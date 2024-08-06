using Chameleon.Interfaces.WebBrowser;
using System.Diagnostics;
using System.IO.Compression;

namespace Chameleon.Core.Util;

public static class IOtil
{
    public static async Task DC(string directoryPath)
    {
        await DeleteDExistsAsync(directoryPath);
        await CreateDirectory(directoryPath);
    }

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

    public static Task CreateZipAsync(string zipFilePath, string directoryPath)
    {
        return Task.Run(() =>
        {
            if (!Directory.Exists(directoryPath))
            {
                throw new DirectoryNotFoundException($"The directory '{directoryPath}' does not exist.");
            }

            ZipFile.CreateFromDirectory(directoryPath, zipFilePath, CompressionLevel.Fastest, false);
        });
    }

    public static Task CreateZipAsync(string filePath, Dictionary<string,string> files)
        => Task.Run(async () => 
        {
            using (var fileStream = new FileStream(filePath, FileMode.CreateNew))
            {
                using var archive = new ZipArchive(fileStream, ZipArchiveMode.Create, true);
                foreach (var file in files)
                {
                    var zipArchiveManifest = archive.CreateEntry(file.Key, CompressionLevel.Fastest);
                    using var zipStream = zipArchiveManifest.Open();
                    using var writer = new StreamWriter(zipStream);
                    await writer.WriteAsync(file.Value);
                    writer.Close();
                    zipStream.Close();

                    await DeleteFExists(Path.Combine(filePath, file.Key));
                }
            }
        });
    public static string[] ReadDirectory(string path)
    {
        if (Directory.Exists(path))
        {
            return Directory.GetFiles(path);
        }

        return [];
    }
    public static Task<string[]> ReadDirectoryAsync(string path)
        => Task.Run(() => ReadDirectory(path));

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

    public static Task DeleteDExistsAsync(string filePath, bool recuersive = true)
    {
        return Task.Run(() =>
        {
            DeleteDExists(filePath, recuersive);
        });
    }

    public static void DeleteDExists(string filePath, bool recuersive = true)
    {
        if (Directory.Exists(filePath))
        {
            try
            {
                Directory.Delete(filePath, recuersive);
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

    public static bool IsNeedUpdate(string newer, string older)
    {
        if (!Path.Exists(older))
        {
            return true;
        }

        FileVersionInfo systemFirefoxInfo = FileVersionInfo.GetVersionInfo(newer);
        FileVersionInfo chamelonFirefoxInfo = FileVersionInfo.GetVersionInfo(older);

        bool isEqual = chamelonFirefoxInfo.ProductMajorPart == systemFirefoxInfo.ProductMajorPart
            && chamelonFirefoxInfo.ProductMinorPart == systemFirefoxInfo.ProductMinorPart;

        return !isEqual;
    }
    public static Task CopyFolderAsync(string directory, string directoryForCopy) =>
        Task.Run(() => CopyFolder(directory, directoryForCopy));
    public static void CopyFolder(string directory, string directoryForCopy)
    {
        Directory.CreateDirectory(directoryForCopy);

        string[] filePaths = Directory.GetFiles(directory);
        foreach (string filePath in filePaths)
        {
            string fileName = Path.GetFileName(filePath);
            string newFile = Path.Combine(directoryForCopy, fileName);

            File.Copy(filePath, newFile);
        }

        string[] subdirectoryPaths = Directory.GetDirectories(directory);
        foreach (string subdirectory in subdirectoryPaths)
        {
            string subdirectoryName = Path.GetFileName(subdirectory);
            string newSubdirectory = Path.Combine(directoryForCopy, subdirectoryName);

            CopyFolder(subdirectory, newSubdirectory);
        }
    }

    public static async Task WriteTextToFileAsync(string filePath, string content, int maxRetries = 3, int delayMilliseconds = 1000)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));

        if (content == null)
            throw new ArgumentNullException(nameof(content), "Content cannot be null.");

        int attempt = 0;

        while (attempt < maxRetries)
        {
            try
            {
                await File.WriteAllTextAsync(filePath, content);
                Console.WriteLine("File written successfully.");
                return; // Exit if write is successful
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Access denied: {ex.Message}");
                break; // Don't retry on access denied
            }
            catch (DirectoryNotFoundException ex)
            {
                Console.WriteLine($"Directory not found: {ex.Message}");
                break; // Don't retry if the directory doesn't exist
            }
            catch (IOException ex)
            {
                attempt++;
                Console.WriteLine($"IO error (attempt {attempt}): {ex.Message}");
                if (attempt >= maxRetries)
                {
                    throw; // Re-throw the exception if maximum retries are reached
                }
                await Task.Delay(delayMilliseconds); // Wait before retrying
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                throw; // Re-throw unexpected exceptions
            }
        }
    }
}
