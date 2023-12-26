using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Lab2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GetBook(uri.Text);
        }
        void GetBook(string uri)
        {
            WebClient wc = new WebClient();

            wc.DownloadStringCompleted += (s, eArgs) =>
            {
                string theBook = eArgs.Result;
                Dispatcher?.Invoke(() => text.Text = theBook);
            };
            wc.DownloadStringAsync(new Uri(uri));
        }
        private void CountDotsAndCommas(object sender, RoutedEventArgs e)
        {
            string selectedText = GetSelectedText();

            if (selectedText != null)
            {
                // поток
                Task.Run(() =>
                {
                    int dotCount = 0;
                    int commaCount = 0;

                    Parallel.ForEach(selectedText, (ch) =>
                    {
                        if (ch == '.')
                        {
                            Interlocked.Increment(ref dotCount);
                        }
                        else if (ch == ',')
                        {
                            Interlocked.Increment(ref commaCount);
                        }
                    });

                    // обновление ui
                    Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show($"Количество точек: {dotCount}\nКоличество запятых: {commaCount}");
                    });
                });
            }
            else
            {
                MessageBox.Show("Выделенного текста нет.");
            }
        }
        private void RemoveFirstQuotes(object sender, RoutedEventArgs e)
        {
            string selectedText = GetSelectedText();

            if (selectedText != null && selectedText.Length >= 2 && selectedText.StartsWith("\"") && selectedText.EndsWith("\""))
            {
                // поток
                Task.Run(() =>
                {
                    // изменяю строку
                    StringBuilder newTextBuilder = new StringBuilder(selectedText);
                    newTextBuilder.Remove(0, 1);  // Удаление первой кавычки
                    newTextBuilder.Remove(newTextBuilder.Length - 1, 1);  // Удаление второй кавычки

                    // обновляю ui
                    Dispatcher.Invoke(() =>
                    {
                        ReplaceSelectedText(newTextBuilder.ToString());
                    });
                });
            }
            else
            {
                MessageBox.Show("Выделенного текста с кавычками нет.");
            }
        }
        private void ReplaceSelectedText(string newText)
        {
            if (text.SelectionLength > 0)
            {
                text.SelectedText = newText;
            }
        }
        private string GetSelectedText()
        {
            if (text.SelectionLength > 0)
            {
                return text.SelectedText;
            }
            else
            {
                return null;
            }
        }
    }
}
