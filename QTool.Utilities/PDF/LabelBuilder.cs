using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;

namespace QTool.PDF
{
    public abstract class LabelBuilder<TData> : BaseBuilder
    {
        public LabelBuilder(TData[] datas, PSizeF paperSize, PThicknessF margin) : base(paperSize, margin)
        {
            Datas = datas;
        }

        public TData[] Datas { get; }

        protected override void Build(Document document, PdfWriter writer, Action<double> updateProgress)
        {
            int index = 0;
            foreach (var data in Datas)
            {
                updateProgress?.Invoke(index * 1d / Datas.Length);
                foreach (var element in BuildLabels(writer, data, index == 0))
                {
                    document.NewPage();
                    document.Add(element);
                }
                index++;
            }
        }

        protected abstract IEnumerable<IElement> BuildLabels(PdfWriter writer, TData data, bool isFirst);
    }
}
