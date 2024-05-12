using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WpfMacroscop.Models
{
    public class Element
    {
        /// <summary>
        /// Возвращает элементы ввода из формы
        /// </summary>
        /// <param name="form">Форма</param>
        /// <returns>Элементы ввода</returns>
        public List<TextBox> GetInputs(StackPanel form)
        {
            List<TextBox> inputsUrl = new List<TextBox>();
            if (form == null)
            {
                return inputsUrl;
            }
            foreach (FrameworkElement element in form.Children)
            {
                if (element is StackPanel stackPanel && stackPanel.Name.Contains("item"))
                {
                    foreach (var item in stackPanel.Children)
                    {
                        if (item is TextBox textBox && textBox.Name.Contains("input"))
                        {
                            inputsUrl.Add(textBox);
                        }
                    }
                }
            }
            return inputsUrl;
        }

        /// <summary>
        /// Возвращает элементы изображений из формы
        /// </summary>
        /// <param name="form">Форма</param>
        /// <returns>Элементы изображений</returns>
        public List<Image> GetImages(StackPanel form)
        {
            List<Image> images = new List<Image>();
            if (form == null)
            {
                return images;
            }
            foreach (FrameworkElement element in form.Children)
            {
                if (element is StackPanel stackPanel && stackPanel.Name.Contains("item"))
                {
                    foreach (var item in stackPanel.Children)
                    {
                        if (item is Border border)
                        {
                            var image = border.Child as Image;
                            images.Add(image);
                        }
                    }
                }
            }
            return images;
        }
    }
}
