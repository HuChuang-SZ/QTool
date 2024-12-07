using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows;
using System.Diagnostics;

namespace QTool.Controls
{
    public static class HtmlHelper
    {

        public static IEnumerable<Inline> ToInlines(this string content)
        {
            if (string.IsNullOrWhiteSpace(content) || content.IndexOf('<') == -1 || content.IndexOf('>') == -1)
            {
                return new Inline[] { new Run(content) };
            }
            else
            {
                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(content);
                return ToInlines(document.DocumentNode.ChildNodes);
            }
        }

        private static IEnumerable<Inline> ToInlines(HtmlNodeCollection nodes)
        {
            List<Inline> inlines = new List<Inline>();
            foreach (HtmlNode item in nodes)
            {
                inlines.Add(ToInline(item));
            }
            return inlines;
        }

        private static Inline ToInline(HtmlNode node)
        {
            if (node.NodeType == HtmlNodeType.Element)
            {
                string tagName = node.Name.ToUpper();
                if (tagName == "BR")
                {
                    return new LineBreak();
                }
                else
                {
                    Span span;
                    switch (tagName)
                    {
                        case "B":
                            span = new Span() { FontWeight = FontWeights.Bold };
                            break;
                        case "I":
                            span = new Span() { FontStyle = FontStyles.Italic };
                            break;
                        case "A":
                            Hyperlink link = new Hyperlink();
                            var href = node.GetAttributeValue("href", string.Empty);
                            if (Uri.TryCreate(href, UriKind.Absolute, out Uri uri))
                            {
                                link.CommandParameter = uri;
                                link.Click += Hyperlink_Click;
                            }
                            span = link;
                            break;
                        case "FONT":
                            span = new Span();
                            int size = node.GetAttributeValue("size", 0);
                            if (size > 0)
                            {
                                span.FontSize = size;
                            }
                            try
                            {
                                string color = node.GetAttributeValue("color", string.Empty);
                                if (string.IsNullOrWhiteSpace(color))
                                {
                                    span.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
                                }
                            }
                            catch { }
                            break;
                        default:
                            span = new Span();
                            break;
                    }
                    span.Inlines.AddRange(ToInlines(node.ChildNodes));
                    return span;
                }


            }
            else
            {
                return new Run(node.InnerHtml);
            }
        }

        private static void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink link = (Hyperlink)sender;
            ((Uri)link.CommandParameter).OriginalString.Start();
        }
    }
}
