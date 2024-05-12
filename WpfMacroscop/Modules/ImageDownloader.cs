using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Threading;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Net;
using System.Security.Policy;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;

namespace WpfMacroscop.Models
{
    public class ImageDownloader
    {
        /// <summary>
        /// Выполняет загрузку изображений по Url
        /// </summary>
        /// <param name="input">Элемент ввода</param>
        /// <returns>Изображение</returns>
        public async Task<BitmapImage> DownloadImageToForm(TextBox input, CancellationToken tokenSource)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetAsync(input.Text, HttpCompletionOption.ResponseHeadersRead, tokenSource);
                    response.EnsureSuccessStatusCode();
                    Stream stream = await response.Content.ReadAsStreamAsync();
                    var bitmapImage = new BitmapImage();
                    using (var memoryStream = new MemoryStream())
                    {
                        await stream.CopyToAsync(memoryStream);
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        bitmapImage.BeginInit();
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.StreamSource = memoryStream;
                        bitmapImage.EndInit();
                        bitmapImage.Freeze();
                    }
                    return bitmapImage;
                }
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("Загрузка отменена.", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (HttpRequestException httpEx)
            {
                MessageBox.Show($"Ошибка HTTP: {httpEx.Message}", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Возникло исключение: {ex.Message}", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return null;
        }
    }
}
