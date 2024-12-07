using QTool.View.DAL;
using QTool.View.Models;
using QTool.View.ViewModels;
using System.Threading.Tasks;
using System.Windows;

namespace QTool.View
{
    /// <summary>
    /// HelpBox.xaml 的交互逻辑
    /// </summary>
    public partial class HelpTip : Window
    {
        private readonly HelpTipViewModel _viewModel;

        private HelpTip(Window owner, HelpTipStep[] steps)
        {
            DataContext = _viewModel = new HelpTipViewModel(owner, steps);
            InitializeComponent();
            Owner = owner;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Prev_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Prev();
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (!_viewModel.Next())
            {
                Close();
            }
        }


        public static void AddSteps(Window window, HelpTipStep[] steps, uint version = 1)
        {
            string identity = window.GetType().FullName;

            if (version == 0 || AccountDAL.Current.GetHelpTip(identity) != version)
            {
                window.Loaded += (sender, e) =>
                {
                    AsyncHelper.Exec(() =>
                    {
                        Task.Delay(500).Wait();
                    }, result =>
                    {
                        HelpTip dialog = new HelpTip(window, steps);
                        dialog.Show();
                        if (version > 0)
                            AccountDAL.Current.SetHelpTip(identity, version);

                    });

                };
            }
        }
    }
}
