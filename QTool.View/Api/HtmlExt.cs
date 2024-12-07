using HtmlAgilityPack;
using NPOI.HSSF.Record.Chart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace QTool.Api
{
    public static class HtmlExt
    {
        public static string GetInnerText(this string htmlString)
        {
            try
            {
                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(htmlString);
                return string.Concat(GetInnerText(document.DocumentNode));
            }
            catch
            {
                return htmlString;
            }
        }

        private const char IndentChar = '\t';
        private readonly static string[] IndentElements = { "dd", "li" };
        private readonly static string[] WrapElements = { "div", "p", "table", "tr", "dd", "dt", "li" };
        private static IEnumerable<string> GetInnerText(this HtmlNode node, int indentCount = 0)
        {
            if (node.NodeType == HtmlNodeType.Text)
            {
                yield return node.InnerText;
            }
            else
            {
                if (node.NodeType == HtmlNodeType.Element)
                {
                    string name = node.Name.ToLower();
                    if (IndentElements.Contains(name))
                    {
                        indentCount++;
                        yield return GetIndentChar(indentCount);
                    }
                }

                if (node.HasChildNodes)
                {
                    int index = 0;
                    foreach (var cNode in node.ChildNodes)
                    {
                        if (index > 0)
                        {
                            if (cNode.Name?.ToLower() == "td")
                            {
                                yield return "\t";
                            }
                        }
                        foreach (var text in cNode.GetInnerText(indentCount))
                        {
                            yield return text;
                        }
                        index++;
                    }
                }


                if (node.NodeType == HtmlNodeType.Element)
                {
                    string name = node.Name.ToLower();
                    if (WrapElements.Contains(name))
                    {
                        yield return Environment.NewLine;
                    }
                }
            }
        }

        public static string GetIndentChar(int indentCount)
        {
            if (indentCount > 0)
                return new string(' ', indentCount * 4);
            else
                return string.Empty;
        }
    }
}
