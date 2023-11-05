namespace eJournal.Web.Models
{
    public class Pagination
    {
        public int StartIteration { get; set; }
        public int EndIteration { get; set; }
        public int CurrentPage { get; set; }    
        public int LastPage { get; set; }
        public int TotalPage { get; set; }

        public Pagination(int totalBlogs, int blogsPerPage, int page)
        {
            CurrentPage = page;
            TotalPage = (int)Math.Ceiling(totalBlogs / Convert.ToDecimal(blogsPerPage));
            LastPage = TotalPage;
            StartIteration = CurrentPage - 2;
            if(StartIteration < 1)
            {
                StartIteration = 1;
            }
            EndIteration = CurrentPage + 2;
            EndIteration = Math.Min(EndIteration, LastPage);
            int totalPaginationPage = EndIteration - StartIteration + 1;
            if (totalPaginationPage < 5)
            {
                int moreToAdd = 5 - totalPaginationPage;
                EndIteration += moreToAdd;
                EndIteration = Math.Min(EndIteration, TotalPage);
            }
            if (totalPaginationPage < 5)
            {
                int moreToAdd = 5 - totalPaginationPage;
                StartIteration -= moreToAdd;
                StartIteration = Math.Max(StartIteration, 1);
            }
        }
    }
}
