namespace CarAuction.Application.Common
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; }
        public PagingMetadata Metadata { get; set; } = new PagingMetadata();
    }
    public class PagingMetadata
    {
        public int TotalCount {  get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string SearchKeyword { get; set; } = string.Empty;
    }
}
