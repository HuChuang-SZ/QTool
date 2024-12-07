using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace QTool.Controls
{
    public static class QCommands
    {

        private static RoutedUICommand CreateCommand(string text, string name, params InputGesture[] inputGestures)
        {
            return new RoutedUICommand(text, name, typeof(QCommands), new InputGestureCollection(inputGestures));
        }

        /// <summary>
        /// 注册
        /// </summary>
        public static RoutedUICommand Register { get; } = CreateCommand("注册", nameof(Register));

        /// <summary>
        /// 登录
        /// </summary>
        public static RoutedUICommand Login { get; } = CreateCommand("登录", nameof(Login));

        /// <summary>
        /// 编辑
        /// </summary>
        public static RoutedUICommand Edit { get; } = CreateCommand("编辑", nameof(Edit));

        /// <summary>
        /// 删除
        /// </summary>
        public static RoutedUICommand Delete { get; } = CreateCommand("删除", nameof(Delete));

        /// <summary>
        /// 解绑
        /// </summary>
        public static RoutedUICommand Unbind { get; } = CreateCommand("解绑", nameof(Unbind));


        /// <summary>
        /// 保存
        /// </summary>
        public static RoutedUICommand Save { get; } = CreateCommand("保存", nameof(Save));


        /// <summary>
        /// 取消
        /// </summary>
        public static RoutedUICommand Cancel { get; } = CreateCommand("取消", nameof(Cancel));


        /// <summary>
        /// 确认
        /// </summary>
        public static RoutedUICommand OK { get; } = CreateCommand("确认", nameof(OK));

        /// <summary>
        /// Send
        /// </summary>
        public static RoutedUICommand Send { get; } = CreateCommand("发送", nameof(Send));

        /// <summary>
        /// 重置密码
        /// </summary>
        public static RoutedUICommand ResetPwd { get; } = CreateCommand("重置密码", nameof(ResetPwd));

        /// <summary>
        /// 关闭标签页
        /// </summary>
        public static RoutedUICommand CloseTab { get; } = CreateCommand("关闭标签页", nameof(CloseTab), new KeyGesture(Key.W, ModifierKeys.Control));

        /// <summary>
        /// 关闭其他标签页
        /// </summary>
        public static RoutedUICommand CloseOtherTabs { get; } = CreateCommand("关闭其他标签页", nameof(CloseOtherTabs));

        /// <summary>
        /// 关闭右侧标签页
        /// </summary>
        public static RoutedUICommand CloseOffsideTabs { get; } = CreateCommand("关闭右侧标签页", nameof(CloseOffsideTabs));

        /// <summary>
        /// 刷新
        /// </summary>
        public static RoutedUICommand RefreshTab { get; } = CreateCommand("刷新", nameof(RefreshTab), new KeyGesture(Key.R, ModifierKeys.Control), new KeyGesture(Key.F5));

        /// <summary>
        /// 复制
        /// </summary>
        public static RoutedUICommand CopyTab { get; } = CreateCommand("复制", nameof(CopyTab), new KeyGesture(Key.K, ModifierKeys.Control | ModifierKeys.Shift));


        /// <summary>
        /// 全选
        /// </summary>
        public static RoutedUICommand SelectAll { get; } = CreateCommand("全选", nameof(SelectAll), new KeyGesture(Key.A, ModifierKeys.Control));

        /// <summary>
        /// 反选
        /// </summary>
        public static RoutedUICommand Invert { get; } = CreateCommand("反选", nameof(SelectAll), new KeyGesture(Key.I, ModifierKeys.Control));




        /// <summary>
        /// 刷新
        /// </summary>
        public static RoutedUICommand Refresh { get; } = CreateCommand("刷新", nameof(Refresh));


        /// <summary>
        /// ClearUp
        /// </summary>
        public static RoutedUICommand ClearUp { get; } = CreateCommand("清除", nameof(ClearUp));


        /// <summary>
        /// LookShops
        /// </summary>
        public static RoutedUICommand LookShops { get; } = CreateCommand("查看店铺", nameof(LookShops));


        /// <summary>
        /// LookProducts
        /// </summary>
        public static RoutedUICommand LookProducts { get; } = CreateCommand("查看产品", nameof(LookShops));


        /// <summary>
        /// Start
        /// </summary>
        public static RoutedUICommand Start { get; } = CreateCommand("开启", nameof(Start));


        /// <summary>
        /// Stop
        /// </summary>
        public static RoutedUICommand Stop { get; } = CreateCommand("暂停", nameof(Stop));

        /// <summary>
        /// Update
        /// </summary>
        public static RoutedUICommand Update { get; } = CreateCommand("更新", nameof(Update));

        /// <summary>
        /// Create
        /// </summary>
        public static RoutedUICommand Create { get; } = CreateCommand("创建", nameof(Create));

        /// <summary>
        /// 添加
        /// </summary>
        public static RoutedUICommand Add { get; } = CreateCommand("添加", nameof(Add));

        /// <summary>
        /// 移除
        /// </summary>
        public static RoutedUICommand Remove { get; } = CreateCommand("移除", nameof(Remove));

        /// <summary>
        /// 插入
        /// </summary>
        public static RoutedUICommand Insert { get; } = CreateCommand("插入", nameof(Insert));

        /// <summary>
        /// 分享
        /// </summary>
        public static RoutedUICommand Share { get; } = CreateCommand("分享", nameof(Share));

        /// <summary>
        /// 查看
        /// </summary>
        public static RoutedUICommand Look { get; } = CreateCommand("查看", nameof(Look));

        /// <summary>
        /// 抢位
        /// </summary>
        public static RoutedUICommand GradBid { get; } = CreateCommand("抢位", nameof(GradBid));

        /// <summary>
        /// 管理
        /// </summary>
        public static RoutedUICommand Manage { get; } = CreateCommand("管理", nameof(Manage));

        /// <summary>
        /// 提交
        /// </summary>
        public static RoutedUICommand Submit { get; } = CreateCommand("提交", nameof(Submit));

        /// <summary>
        /// 撤销
        /// </summary>
        public static RoutedUICommand Revoke { get; } = CreateCommand("撤销", nameof(Revoke));

        /// <summary>
        /// 导出
        /// </summary>
        public static RoutedUICommand Import { get; } = CreateCommand("导出", nameof(Import));

        /// <summary>
        /// 批量编辑
        /// </summary>
        public static RoutedUICommand BatchEdit { get; } = CreateCommand("批量编辑", nameof(BatchEdit));

        /// <summary>
        /// 推荐
        /// </summary>
        public static RoutedUICommand Recommend { get; } = CreateCommand("推荐", nameof(Recommend));

        /// <summary>
        /// 最低
        /// </summary>
        public static RoutedUICommand Lowest { get; } = CreateCommand("最低", nameof(Lowest));


        /// <summary>
        /// 一键已读
        /// </summary>
        public static RoutedUICommand MarkAsRead { get; } = CreateCommand("一键已读", nameof(MarkAsRead));


        /// <summary>
        /// 详情
        /// </summary>
        public static RoutedUICommand Detail { get; } = CreateCommand("详情", nameof(Detail));


        /// <summary>
        /// 选择
        /// </summary>
        public static RoutedUICommand Select { get; } = CreateCommand("选择", nameof(Select));


        /// <summary>
        /// 指定
        /// </summary>
        public static RoutedUICommand Assign { get; } = CreateCommand("指定", nameof(Assign));


        /// <summary>
        /// 打印
        /// </summary>
        public static RoutedUICommand Print { get; } = CreateCommand("打印", nameof(Print));


        /// <summary>
        /// 加入
        /// </summary>
        public static RoutedUICommand Join { get; } = CreateCommand("加入", nameof(Join));

        /// <summary>
        /// 调整
        /// </summary>
        public static RoutedUICommand Adjust { get; } = CreateCommand("调整", nameof(Adjust));

        /// <summary>
        /// 取消调整
        /// </summary>
        public static RoutedUICommand CancelAdjust { get; } = CreateCommand("取消调整", nameof(CancelAdjust));

        /// <summary>
        /// 关闭
        /// </summary>
        public static RoutedUICommand Close { get; } = CreateCommand("关闭", nameof(Close));


        /// <summary>
        /// 申请删除
        /// </summary>
        public static RoutedUICommand ApplyDelete { get; } = CreateCommand("申请删除", nameof(ApplyDelete));


        /// <summary>
        /// 接受
        /// </summary>
        public static RoutedUICommand Accept { get; } = CreateCommand("接受", nameof(Accept));


        /// <summary>
        /// 恢复默认值
        /// </summary>
        public static RoutedUICommand ResetDefault { get; } = CreateCommand("恢复默认值", nameof(ResetDefault));


        /// <summary>
        /// 设置
        /// </summary>
        public static RoutedUICommand Settings { get; } = CreateCommand("设置", nameof(Settings));
    }
}
