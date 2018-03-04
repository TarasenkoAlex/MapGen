using System;
using System.Drawing;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MapGen.View.Source.Interfaces;

namespace MapGen.View.GUI.Windows
{
    /// <summary>
    /// Логика взаимодействия для MessageWindow.xaml
    /// </summary>
    public partial class MessageWindow : IMessage
    {
        #region Region events.

        /// <summary>
        /// Событие результата сообщения.
        /// </summary>
        public event Action<bool> ReturnResult;

        #endregion

        #region Region constructor.

        /// <summary>
        /// Создает окно сообщения.
        /// </summary>
        public MessageWindow()
        {
            InitializeComponent();
            BindingEventsOfWindow();
        }

        #endregion

        #region Region public methods.

        /// <summary>
        /// Отображает окно сообщения.
        /// </summary>
        /// <param name="title">Заголовок сообщения.</param>
        /// <param name="text">Текст сообщения.</param>
        /// <param name="butonType">Тип кнопок выбора.</param>
        /// <param name="messageType">Тип сообщения.</param>
        public void ShowMessage(string title, string text, MessageButton butonType, MessageType messageType)
        {
            Dispatcher.Invoke(() =>
            {
                LabelHeader.Content = title;
                TextBlockContent.Text = text;
                InitMessageButtons(butonType);
                InitMessageType(messageType);
            });

            ShowDialog();
        }

        #endregion

        #region Region private methods.
        
        /// <summary>
        /// Подписка на события окна сообщения.
        /// </summary>
        private void BindingEventsOfWindow()
        {
            // Обработка кнопки закрытия.
            ButtonClose.Click += (s, e) => Close();

            // Обработка кнопки "Ok".
            ButtonOk.Click += (s, e) =>
            {
                Close();
                ReturnResult?.Invoke(true);
            };

            // Обработка кнопки "Да".
            ButtonYes.Click += (s, e) =>
            {
                Close();
                ReturnResult?.Invoke(true);
            };

            // Обработка кнопки "Нет".
            ButtonNo.Click += (s, e) =>
            {
                Close();
                ReturnResult?.Invoke(false);
            };
        }

        /// <summary>
        /// Инициализация изображения в сообщении в зависимости от типа сообщения.
        /// </summary>
        /// <param name="messageType"></param>
        private void InitMessageType(MessageType messageType)
        {
            switch (messageType)
            {
                case MessageType.Error:
                    {
                        InitImageMessage(ResourcesView.message_error);
                        break;
                    }
                case MessageType.Information:
                    {
                        InitImageMessage(ResourcesView.message_info);
                        break;
                    }
                case MessageType.Question:
                    {
                        InitImageMessage(ResourcesView.message_question);
                        break;
                    }
                case MessageType.Warning:
                    {
                        InitImageMessage(ResourcesView.message_warning);
                        break;
                    }
            }
        }

        /// <summary>
        /// Инициализация изображения.
        /// </summary>
        /// <param name="source">Изображение.</param>
        private void InitImageMessage(Bitmap source)
        {
            IntPtr hsource = source.GetHbitmap();
            ImageSource wpfBitmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                hsource,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            ImageMessage.Source = wpfBitmap;
        }

        /// <summary>
        /// Инициализация кнопок в зависимости от выбранного типа кнопок.
        /// </summary>
        /// <param name="messageButton"></param>
        private void InitMessageButtons(MessageButton messageButton)
        {
            switch (messageButton)
            {
                case MessageButton.Ok:
                    {
                        ButtonYes.Visibility = Visibility.Hidden;
                        ButtonNo.Visibility = Visibility.Hidden;
                        ButtonOk.Visibility = Visibility.Visible;
                        break;
                    }
                case MessageButton.YesNo:
                    {
                        ButtonYes.Visibility = Visibility.Visible;
                        ButtonNo.Visibility = Visibility.Visible;
                        ButtonOk.Visibility = Visibility.Hidden;
                        break;
                    }
            }
        }

        #endregion
    }

    #region Region enums.

    /// <summary>
    /// Тип сообщения. Необходим для отображения картинки в окне диалога
    /// </summary>
    public enum MessageType
    {
        Information,
        Error,
        Warning,
        Question
    }

    /// <summary>
    /// Тип кнопок, которые необходимо отобразить
    /// </summary>
    public enum MessageButton
    {
        Ok,
        YesNo
    }

    #endregion
}
