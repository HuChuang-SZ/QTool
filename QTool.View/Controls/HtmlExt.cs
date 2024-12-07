using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace QTool.View.Controls
{
    public static class HtmlExt
    {
        public static IEnumerable<HtmlNode> GetElementByTagName(this HtmlNode node, string tagName)
        {
            if (node != null && node.HasChildNodes)
            {
                foreach (var cNode in node.ChildNodes)
                {
                    if (cNode.NodeType == HtmlNodeType.Element && string.Compare(cNode.Name, tagName, true) == 0)
                    {
                        yield return cNode;
                    }

                    foreach (var ccNode in cNode.GetElementByTagName(tagName))
                    {
                        yield return ccNode;
                    }
                }
            }
        }
    }
}
