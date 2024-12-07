using QTool.Controls;
using QTool.View.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
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

namespace QTool.View.Controls
{
    public class ModuleBox : Control
    {
        static ModuleBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ModuleBox), new FrameworkPropertyMetadata(typeof(ModuleBox)));
        }

        public ModuleBox()
        {
            SetCurrentValue(GroupsProperty, QModuleInfo.Groups);
            CommandBindings.Add(new CommandBinding(QCommands.Select, Select_Executed));
        }

        private void Select_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is QModuleInfo module)
            {
                SetCurrentValue(ContentProperty, module);
                SetCurrentValue(IsOpenProperty, false);
            }
        }

        public QModuleInfo Content
        {
            get { return (QModuleInfo)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Content.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(QModuleInfo), typeof(ModuleBox), new PropertyMetadata(null));



        public ReadOnlyCollection<QModuleGroup> Groups
        {
            get { return (ReadOnlyCollection<QModuleGroup>)GetValue(GroupsProperty); }
        }

        // Using a DependencyProperty as the backing store for Groups.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GroupsProperty =
            DependencyProperty.Register("Groups", typeof(ReadOnlyCollection<QModuleGroup>), typeof(ModuleBox), new PropertyMetadata(null));



        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsOpen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(ModuleBox), new PropertyMetadata(false));


        /// <summary>
        /// 延时关闭弹出层
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            DelayExecute(() =>
            {
                if (!IsMouseOver)
                    SetCurrentValue(IsOpenProperty, false);

            }, DelayMilliseconds * 2);
        }


        /// <summary>
        /// 延迟执行代码
        /// </summary>
        /// <param name="method">执行方法</param>
        /// <param name="delayMilliseconds">延时毫秒</param>
        private void DelayExecute(Action method, int delayMilliseconds)
        {
            Task.Factory.StartNew(() =>
            {
                System.Threading.Thread.Sleep(delayMilliseconds);
                Dispatcher.BeginInvoke(method);
            });
        }

        /// <summary>
        /// 延时毫秒（默认100毫秒）
        /// </summary>
        public int DelayMilliseconds
        {
            get { return (int)GetValue(DelayMillisecondsProperty); }
            set { SetValue(DelayMillisecondsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DelayMilliseconds.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DelayMillisecondsProperty =
            DependencyProperty.Register("DelayMilliseconds", typeof(int), typeof(ModuleBox), new PropertyMetadata(100));
    }
}
