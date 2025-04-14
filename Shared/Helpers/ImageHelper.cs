using System.Diagnostics;
using System.Net.Http.Headers;

namespace Shared.Helpers
{
    public static class ImageHelper
    {
        public static async Task<Stream> DownloadImageAsync(string imageUrl, HttpClient httpClient)
        {
            if (string.IsNullOrEmpty(imageUrl))
                return null;

            try
            {
                var response = await httpClient.GetAsync(imageUrl);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStreamAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error downloading image: {ex.Message}");
                return null;
            }
        }

        public static async Task<MultipartFormDataContent> CreateImageContentAsync(Stream imageStream, string fileName, string paramName = "image")
        {
            if (imageStream == null)
                return null;

            try
            {
                // Reset stream position
                imageStream.Position = 0;

                // Create memory stream to get the bytes
                using var memoryStream = new MemoryStream();
                await imageStream.CopyToAsync(memoryStream);
                var imageBytes = memoryStream.ToArray();

                // Create form content
                var content = new MultipartFormDataContent();
                var imageContent = new ByteArrayContent(imageBytes);
                imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
                content.Add(imageContent, paramName, fileName);

                return content;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error creating image content: {ex.Message}");
                return null;
            }
        }

        public static string GetImageFileName(string url)
        {
            if (string.IsNullOrEmpty(url))
                return null;

            try
            {
                Uri uri = new Uri(url);
                string fileName = Path.GetFileName(uri.LocalPath);
                return fileName;
            }
            catch
            {
                return Path.GetRandomFileName() + ".jpg";
            }
        }
    }
}