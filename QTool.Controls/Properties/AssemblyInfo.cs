using System.Reflection;
using System.Windows;
using System.Windows.Markup;

[assembly: XmlnsDefinition("http://wpf.qtool.com.cn/Controls", "QTool.Controls")]
[assembly: XmlnsDefinition("http://wpf.qtool.com.cn/Controls/Converters", "QTool.Controls.Converters")]
[assembly: XmlnsDefinition("http://wpf.qtool.com.cn/Controls/Models", "QTool.Controls.Models")]

[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, //主题特定资源词典所处位置
                                     //(未在页面中找到资源时使用，
                                     //或应用程序资源字典中找到时使用)
    ResourceDictionaryLocation.SourceAssembly //常规资源词典所处位置
                                              //(未在页面中找到资源时使用，
                                              //、应用程序或任何主题专用资源字典中找到时使用)
)]


// 程序集的版本信息由下列四个值组成: 
//
//      主版本
//      次版本
//      生成号
//      修订号
//
//可以指定所有这些值，也可以使用“生成号”和“修订号”的默认值
//通过使用 "*"，如下所示:
[assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyProduct("QTool")]
[assembly: AssemblyCompany("QTool")]
[assembly: AssemblyCopyright("Copyright © 2023 QTool")]
