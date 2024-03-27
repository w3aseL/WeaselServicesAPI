namespace WeaselServicesAPI
{
    public class PagingParams
    {
        public int Offset { get; set; } = 0;
        public int? Limit { get; set; }
    }

    public class DatePagingParams : PagingParams
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class PlaylistParams
    {
        public int PlaylistOption { get; set; }
    }
}
