using QTool.Controls.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace QTool.Controls
{
    /// <summary>
    /// QMessageWindow.xaml 的交互逻辑
    /// </summary>
    public partial class QMessageWindow : QWindow
    {
        public QMessageWindow()
        {
            InitializeComponent();
            Loaded += QMessageWindow_Loaded;
        }

        private void QMessageWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (InputMode != QMessageInputMode.None)
            {
                txtInput.SelectAll();
                txtInput.Focus();
            }
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            OnInputModeChanged();
        }

        public QMessageButton Result { get; private set; }


        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        private const string DefaultPlaceholder = "请填写您要输入的值";
        private const string DefaultCaption = "确定";

        // Using a DependencyProperty as the backing store for Message.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(QMessageWindow), new PropertyMetadata(null));


        public IList<QMessageButton> Buttons
        {
            get { return (IList<QMessageButton>)GetValue(ButtonsProperty); }
            set { SetValue(ButtonsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Buttons.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ButtonsProperty =
            DependencyProperty.Register("Buttons", typeof(IList<QMessageButton>), typeof(QMessageWindow), new PropertyMetadata(null));


        public Func<string, string> InputCheck { get; set; }


        public QMessageInputMode InputMode
        {
            get { return (QMessageInputMode)GetValue(InputModeProperty); }
            set { SetValue(InputModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InputMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InputModeProperty =
            DependencyProperty.Register("InputMode", typeof(QMessageInputMode), typeof(QMessageWindow), new PropertyMetadata(QMessageInputMode.None, OnInputModeChanged));

        private static void OnInputModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((QMessageWindow)d).OnInputModeChanged();
        }

        protected void OnInputModeChanged()
        {
            if (IsInitialized)
            {
                switch (InputMode)
                {
                    case QMessageInputMode.None:
                        txtInput.Visibility = Visibility.Collapsed;
                        break;
                    case QMessageInputMode.Single:
                        txtInput.Visibility = Visibility.Visible;
                        txtInput.Height = double.NaN;
                        txtInput.AcceptsReturn = false;
                        txtInput.AcceptsTab = false;
                        break;
                    case QMessageInputMode.Multiple:
                        txtInput.Visibility = Visibility.Visible;
                        txtInput.Height = 50;
                        txtInput.AcceptsReturn = true;
                        txtInput.AcceptsTab = true;
                        break;
                }
            }
        }



        public string InputText
        {
            get { return (string)GetValue(InputTextProperty); }
            set { SetValue(InputTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InputText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InputTextProperty =
            DependencyProperty.Register("InputText", typeof(string), typeof(QMessageWindow), new PropertyMetadata(null));



        public string InputPlaceholder
        {
            get { return (string)GetValue(InputPlaceholderProperty); }
            set { SetValue(InputPlaceholderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InputPlaceholder.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InputPlaceholderProperty =
            DependencyProperty.Register("InputPlaceholder", typeof(string), typeof(QMessageWindow), new PropertyMetadata(null));





        private void Select_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is QMessageButton button)
            {
                if (button.IsDefault && InputMode != QMessageInputMode.None)
                {
                    var msg = InputCheck?.Invoke(InputText);
                    if (!string.IsNullOrWhiteSpace(msg))
                    {
                        QMessageBox.Show(msg, this);
                        txtInput.SelectAll();
                        txtInput.Focus();
                        return;
                    }
                }
                Result = button;
                DialogResult = true;
            }
        }


        public static MessageBoxResult Show(string message, string caption = DefaultCaption, MessageBoxButton buttons = MessageBoxButton.OKCancel, Window owner = null)
        {
            if (Application.Current == null) return MessageBoxResult.None;

            if (Application.Current.CheckAccess())
            {
                QMessageWindow dialog = new QMessageWindow()
                {
                    Title = caption,
                    Message = message,
                    Buttons = QMessageButton.GetButtons(buttons),
                    Owner = owner,
                };

                if (dialog.ShowDialog() == true)
                {
                    return dialog.Result?.GetResult() ?? MessageBoxResult.None;
                }
                else
                {
                    return MessageBoxResult.None;
                }
            }
            else
            {
                return Application.Current.Dispatcher.Invoke(() =>
                {
                    return Show(message, caption, buttons, owner);
                });
            }
        }

        public static bool Confirm(string message, string caption = DefaultCaption)
        {
            return Show(message, caption) == MessageBoxResult.OK;
        }

        public static string Prompt(string message, Func<string, string> inputCheck, string defaultText = null, string placeholder = DefaultPlaceholder, string caption = DefaultCaption, QMessageInputMode inputMode = QMessageInputMode.Single, Window owner = null)
        {
            if (Application.Current != null)
            {
                if (Application.Current.CheckAccess())
                {
                    QMessageWindow dialog = new QMessageWindow()
                    {
                        Title = caption,
                        Message = message,
                        Buttons = QMessageButton.GetButtons(MessageBoxButton.OKCancel),
                        InputText = defaultText,
                        InputPlaceholder = placeholder,
                        InputMode = inputMode,
                        InputCheck = inputCheck,
                        Owner = owner,
                    };

                    if (dialog.ShowDialog() == true && dialog.Result?.IsDefault == true)
                    {
                        return dialog.InputText;
                    }
                }
                else
                {
                    return Application.Current.Dispatcher.Invoke(() =>
                    {
                        return Prompt(message, inputCheck, defaultText, placeholder, caption, inputMode, owner);
                    });
                }
            }
            return null;
        }

        public static string Prompt(string message, string checkErrorMsg = DefaultPlaceholder, string defaultText = null, string placeholder = null, string caption = null, QMessageInputMode inputMode = QMessageInputMode.Single, Window owner = null)
        {
            return Prompt(message, inputValue => CheckRequire(inputValue, checkErrorMsg), defaultText, placeholder ?? checkErrorMsg, caption ?? checkErrorMsg, inputMode, owner);
        }

        public static string CheckRequire(string inputValue, string errorMsg = DefaultPlaceholder)
        {
            if (string.IsNullOrWhiteSpace(inputValue))
            {
                return errorMsg; ;
            }
            else
            {
                return null;
            }
        }
    }
}
