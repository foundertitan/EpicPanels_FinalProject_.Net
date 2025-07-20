namespace EpicPanels_Group5_FinalProject.Models
{
    public class Product
    {
        public int ProductID { get; set; }
        public string Name { get; set; }
        public int CategoryID { get; set; }
        public string Author { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string Description { get; set; }
        public string ImageURL { get; set; }
        public Category? Category { get; set; }
        public ICollection<CustomerReview>? CustomerReviews { get; set; }
    }
}