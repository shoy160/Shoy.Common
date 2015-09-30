
namespace Shoy.Assistant.Solr
{
    public class SolrResult<T> where T : SolrEntity
    {
        public Header ResponseHeader { get; set; }
        public SolrResponse<T> Response { get; set; }
    }

    public class Header
    {
        public int Status { get; set; }
        public int QTime { get; set; }
    }

    public class SolrResponse<T>
    {
        public int NumFound { get; set; }
        public int Start { get; set; }

        public T[] Docs { get; set; }
    }
}