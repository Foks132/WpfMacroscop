using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfMacroscop.Models;

namespace WpfMacroscop
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CancellationTokenSource cancellationTokenSource;
        List<TextBox> inputs = new List<TextBox>();
        List<Image> images = new List<Image>();

        public MainWindow()
        {
            InitializeComponent();
            Element element = new Element();
            inputs = element.GetInputs(this.form); // Элементы полей ввода 
            images = element.GetImages(this.form); // Элементы изображений 
        }


        /// <summary>
        /// Производит обработку загрузки изображений 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="imageButton"></param>
        /// <returns></returns>
        private async Task Load(TypeLoad type = TypeLoad.one, Button imageButton = null)
        {
            barDownloadStatus.IsIndeterminate = true;
            ImageDownloader imageDownloader = new ImageDownloader();
            switch (type)
            {
                case TypeLoad.all:
                    try
                    {
                        // Создаем список задач для параллельной загрузки
                        var downloadTasks = inputs.Select(async input =>
                        {
                            // Получаем компонент изображение соответствующий компоненту ввода
                            Image image = images.FirstOrDefault(x => x.Name.Contains(input.Name.Split('_')[1]));
                            if (image == null)
                            {
                                return Task.CompletedTask; // Если изображение не найдено, возвращаем завершенную задачу
                            }
                            cancellationTokenSource = new CancellationTokenSource();
                            // Запускаем загрузку изображения асинхронно
                            image.Source = await imageDownloader.DownloadImageToForm(input, cancellationTokenSource.Token);
                            return Task.CompletedTask;
                        }).ToList();
                        // Ожидаем завершения всех задач загрузки
                        await Task.WhenAll(downloadTasks);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Возникла ошибка:\n{ex.Message}", "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    break;
                case TypeLoad.one:
                    try
                    {
                        if (imageButton == null)
                        {
                            return;
                        }
                        TextBox input = inputs.FirstOrDefault(x => x.Name.Contains(imageButton.Name.Split('_')[2]));
                        if (input == null)
                        {
                            return;
                        }
                        Image image = images.FirstOrDefault(x => x.Name.Contains(input.Name.Split('_')[1]));
                        if (image == null)
                        {
                            break;
                        }
                        cancellationTokenSource = new CancellationTokenSource();
                        image.Source = await imageDownloader.DownloadImageToForm(input, cancellationTokenSource.Token);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Возникла ошибка:\n{ex.Message}", "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    break;
                default:
                    break;
            }
            barDownloadStatus.IsIndeterminate = false;
        }

        enum TypeLoad
        {
            all,
            one
        }

        private async void btn_downloadAll_Click(object sender, RoutedEventArgs e)
        {
            await Load(TypeLoad.all);
        }

        private async void btn_download_Click(object sender, RoutedEventArgs e)
        {
            await Load(TypeLoad.one, sender as Button);
        }

        private void btn_stop_Click(object sender, RoutedEventArgs e)
        {
            cancellationTokenSource?.Cancel();
        }
    }
}
