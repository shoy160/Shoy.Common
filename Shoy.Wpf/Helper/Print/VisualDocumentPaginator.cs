using System.Printing;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Shoy.Wpf.Helper.Print
{
    /// <summary> DrawingVisual 分页打印 </summary>
    public class VisualDocumentPaginator : DocumentPaginator
    {
        private readonly ContainerVisual _containerVisual;
        private readonly PageMediaSize _mediaSize;
        private Size _pageSize;

        /// <summary> DrawingVisual 分页打印 构造函数 </summary>
        public VisualDocumentPaginator(ContainerVisual containerVisual,
            PageMediaSizeName sizeName = PageMediaSizeName.ISOA4)
        {
            _containerVisual = containerVisual;
            _mediaSize = PrintHelper.GetSize(sizeName);
        }

        public override DocumentPage GetPage(int pageNumber)
        {
            //我们使用A4纸张大小
            if (_mediaSize != null)
            {
                _pageSize = new Size(_mediaSize.Width ?? 0, _mediaSize.Height ?? 1100);
            }
            return new DocumentPage(_containerVisual.Children[pageNumber], _pageSize, new Rect(_pageSize),
                new Rect(_pageSize));
        }

        public override bool IsPageCountValid => true;

        public override int PageCount => _containerVisual.Children.Count;

        public override Size PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value; }
        }

        public override IDocumentPaginatorSource Source => null;
    }
}
