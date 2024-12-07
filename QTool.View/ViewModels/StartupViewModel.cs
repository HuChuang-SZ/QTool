using QTool.Controls;
using QTool.View.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace QTool.View.ViewModels
{

    class StartupViewModel : ModelBase
    {

        public StartupViewModel(Window owner, string[] args)
        {
            Owner = owner;
        }

        public Window Owner { get; }

        public void Start()
        {

            var dialog = new MainWindow();
            dialog.Show();
            Owner.Close();
        }


        #region UpgradeMsg Property
        private string _upgradeMsg;
        /// <summary>
        /// 
        /// </summary>
        public string UpgradeMsg
        {
            get
            {
                return _upgradeMsg;
            }
            private set
            {
                if (_upgradeMsg != value)
                {
                    _upgradeMsg = value;
                    OnPropertyChanged(nameof(UpgradeMsg));
                }
            }
        }
        #endregion UpgradeMsg Property

        #region UpgradeProgress Property
        private double _upgradeProgress;
        /// <summary>
        /// 
        /// </summary>
        public double UpgradeProgress
        {
            get
            {
                return _upgradeProgress;
            }
            private set
            {
                if (_upgradeProgress != value)
                {
                    _upgradeProgress = value;
                    OnPropertyChanged(nameof(UpgradeProgress));
                }
            }
        }
        #endregion UpgradeProgress Property



    }
}
