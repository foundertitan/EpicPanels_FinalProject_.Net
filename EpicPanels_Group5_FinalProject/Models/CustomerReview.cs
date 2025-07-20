namespace EpicPanels_Group5_FinalProject.Models
{
    public class CustomerReview
    {
        public int CustomerReviewID { get; set; }
        public int UserID { get; set; }
        public int ProductID { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime Date { get; set; }
        public User? User { get; set; }
        public Product? Product { get; set; }
    }

}
