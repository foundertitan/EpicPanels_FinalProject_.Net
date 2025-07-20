using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EpicPanels_Group5_FinalProject.Data;
using EpicPanels_Group5_FinalProject.Models;
using System.Linq;
using System.Threading.Tasks;

namespace EpicPanels_Group5_FinalProject.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders.Include(o => o.User).ToListAsync();
            var products = await _context.Products.Include(p => p.Category).ToListAsync();
            var categories = await _context.Categories.ToListAsync();
            var users = await _context.Users.ToListAsync();
            var reviews = await _context.CustomerReviews.Include(r => r.User).Include(r => r.Product).ToListAsync();

            ViewBag.Orders = orders;
            ViewBag.Products = products;
            ViewBag.Categories = categories;
            ViewBag.Users = users;
            ViewBag.Reviews = reviews;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, string status)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.Status = status;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AddOrUpdateCategory(Category category)
        {
            if (category.CategoryID == 0)
                _context.Categories.Add(category);
            else
                _context.Categories.Update(category);

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            var category = await _context.Categories.FindAsync(categoryId);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AddOrUpdateProduct(Product product)
        {
            if (product.ProductID == 0)
                _context.Products.Add(product);
            else
                _context.Products.Update(product);

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUserRole(int userId, string role)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.Role = role;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            var review = await _context.CustomerReviews.FindAsync(reviewId);
            if (review != null)
            {
                _context.CustomerReviews.Remove(review);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                var existingProduct = _context.Products.FirstOrDefault(p => p.ProductID == product.ProductID);
                if (existingProduct != null)
                {
                    existingProduct.Name = product.Name;
                    existingProduct.CategoryID = product.CategoryID;
                    existingProduct.Author = product.Author;
                    existingProduct.Price = product.Price;
                    existingProduct.Stock = product.Stock;
                    existingProduct.Description = product.Description;
                    existingProduct.ImageURL = product.ImageURL;

                    _context.Products.Update(existingProduct);
                    _context.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }


        [HttpPost]
        public IActionResult UpdateCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                var existingCategory = _context.Categories.FirstOrDefault(c => c.CategoryID == category.CategoryID);
                if (existingCategory != null)
                {
                    existingCategory.CategoryName = category.CategoryName;
                    existingCategory.Description = category.Description;

                    _context.Categories.Update(existingCategory);
                    _context.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult AddProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Products.Add(product);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Product added successfully.";
                return RedirectToAction("Index");
            }
            TempData["ErrorMessage"] = "Failed to add product. Please check the details and try again.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult AddCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Add(category);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Category added successfully.";
                return RedirectToAction("Index");
            }
            TempData["ErrorMessage"] = "Failed to add category. Please check the details and try again.";
            return RedirectToAction("Index");
        }


    }
}
