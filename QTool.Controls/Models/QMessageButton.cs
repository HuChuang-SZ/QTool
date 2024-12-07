using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace QTool.Controls.Models
{
    public class QMessageButton
    {
        public QMessageButton(int id, string text, QMessageButtonStyles style = QMessageButtonStyles.Default, bool isDefault = false, bool isCancel = false)
        {
            Id = id;
            Text = text;
            IsDefault = isDefault;
            IsCancel = isCancel;
            if (style == QMessageButtonStyles.Default && isDefault)
                Style = QMessageButtonStyles.Primary;
            else
                Style = style;
        }

        public int Id { get; }

        public string Text { get; }

        public bool IsDefault { get; }

        public bool IsCancel { get; }

        public QMessageButtonStyles Style { get; }


        public static QMessageButton OK { get; } = new QMessageButton(1, "确定", isDefault: true);
        public static QMessageButton Cancel { get; } = new QMessageButton(2, "取消", isCancel: true);
        public static QMessageButton Yes { get; } = new QMessageButton(3, "是", isDefault: true);
        public static QMessageButton No { get; } = new QMessageButton(4, "否", isCancel: true);

        public static QMessageButton[] GetButtons(MessageBoxButton buttons)
        {
            switch (buttons)
            {
                case MessageBoxButton.OK:
                    return new QMessageButton[] { OK };
                case MessageBoxButton.OKCancel:
                    return new QMessageButton[] { OK, Cancel };
                case MessageBoxButton.YesNoCancel:
                    return new QMessageButton[] { Yes, No, Cancel };
                case MessageBoxButton.YesNo:
                    return new QMessageButton[] { Yes, No };
                default:
                    return Array.Empty<QMessageButton>();
            }
        }

        public MessageBoxResult GetResult()
        {
            switch (Id)
            {
                case 1:
                    return MessageBoxResult.OK;
                case 2:
                    return MessageBoxResult.Cancel;
                case 3:
                    return MessageBoxResult.Yes;
                case 4:
                    return MessageBoxResult.No;
                default:
                    return MessageBoxResult.None;
            }
        }
    }
}
