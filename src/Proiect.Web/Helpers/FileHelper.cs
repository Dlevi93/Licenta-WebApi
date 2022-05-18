using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Proiect.Web.Helpers
{
    public static class FileHelper
    {
        public static bool IsValid(this IFormFile file)
        {
            var allowedExtensions = new List<string> { ".jpg", ".png", ".pdf", ".gif" };
            var extension = Path.GetExtension(file.FileName);
            return allowedExtensions.Contains(extension.ToLower());
        }

        public static async Task<(bool, bool, string)> GenerateAndSave(this IFormFile file, string id, bool isAdvertisement = false)
        {
            var extension = Path.GetExtension(file.FileName);
            var imageType = new List<string> { ".jpg", ".png" };
            var isImage = imageType.Contains(extension.ToLower());

            string folderName;
            if (isAdvertisement)
            {
                folderName = Path.Combine("Uploads", "Interests", id);
            }
            else if (isImage)
            {
                folderName = Path.Combine("Uploads", "Images", id);
            }
            else if (extension == ".pdf")
            {
                folderName = Path.Combine("Uploads", "Pdf", id);
            }
            else
            {
                folderName = Path.Combine("Uploads", "Other", id);
            }
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), folderName);

            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            var generatedName = new RandomGenerator();
            var generateString = generatedName.RandomString(10);
            var uniqueFileName = $"{generateString}{extension}";
            var dbPath = Path.Combine(folderName, uniqueFileName);

            try
            {
                if (isImage)
                {
                    using var image = Image.Load(file.OpenReadStream());

                    if (isAdvertisement)
                    {
                        var imgHeight = 1024 * image.Height / image.Width;
                        image.Mutate(x => x.Resize(new Size(1024, imgHeight)));
                        image.Save(Path.Combine(filePath, uniqueFileName));
                    }
                    else
                    {
                        var imgHeightL = 1024 * image.Height / image.Width;
                        var imgHeightM = 730 * image.Height / image.Width;
                        var imgHeightS = 380 * image.Height / image.Width;

                        image.Mutate(x => x.Resize(new Size(1024, imgHeightL)));
                        image.Save(Path.Combine(filePath, $"{generateString}-L{extension}"));

                        image.Mutate(x => x.Resize(new Size(730, imgHeightM)));
                        image.Save(Path.Combine(filePath, $"{generateString}-M{extension}"));

                        image.Mutate(x => x.Resize(new Size(380, imgHeightS)));
                        image.Save(Path.Combine(filePath, $"{generateString}-S{extension}"));
                    }
                }
                else
                {
                    await using var fileStream = new FileStream(Path.Combine(filePath, uniqueFileName), FileMode.Create);
                    await file.CopyToAsync(fileStream);
                }
            }
            catch (Exception ex)
            {
                return (false, false, ex.Message);
            }


            return (true, isImage, dbPath);
        }
    }
}
