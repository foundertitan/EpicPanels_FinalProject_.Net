using Microsoft.AspNetCore.Mvc;
using EpicPanels_Group5_FinalProject.Models;
using EpicPanels_Group5_FinalProject.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using EpicPanels_Group5_FinalProject.Data;
using System.Security.Claims;

namespace EpicPanels_Group5_FinalProject.Controllers
{
    public class ComicsController : Controller
    {
        private readonly AppDbContext _context;

        public ComicsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? categoryId)
        {
            var categories = await _context.Categories.ToListAsync();
            ViewBag.Categories = categories;
            ViewBag.SelectedCategoryId = categoryId;

            IQueryable<Product> products = _context.Products.Include(p => p.Category);

            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryID == categoryId);
            }

            return View(await products.ToListAsync());
        }

        [HttpPost]
        public IActionResult AddToCart(int productId)
        {
            var product = _context.Products.Find(productId);
            if (product != null)
            {
                var cart = CartHelper.GetCart(HttpContext.Session);
                var cartItem = cart.FirstOrDefault(c => c.ProductID == product.ProductID);

                if (cartItem != null)
                {
                    cartItem.Quantity++;
                }
                else
                {
                    cart.Add(new CartItem
                    {
                        ProductID = product.ProductID,
                        Name = product.Name,
                        Price = product.Price,
                        ImageURL = product.ImageURL,
                        Quantity = 1
                    });
                }

                CartHelper.SaveCart(HttpContext.Session, cart);
            }

            return RedirectToAction("Cart");
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.CustomerReviews)
                .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(p => p.ProductID == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }



        public IActionResult Cart()
        {
            var cart = CartHelper.GetCart(HttpContext.Session);
            return View(cart);
        }
        [HttpPost]
        public IActionResult UpdateCart(Dictionary<int, int> Quantities)
        {
            var cart = CartHelper.GetCart(HttpContext.Session);

            foreach (var item in Quantities)
            {
                var cartItem = cart.FirstOrDefault(c => c.ProductID == item.Key);
                if (cartItem != null)
                {
                    cartItem.Quantity = Math.Clamp(item.Value, 1, 10);
                }
            }

            CartHelper.SaveCart(HttpContext.Session, cart);

            return RedirectToAction("Cart");
        }

        [HttpPost]
        public IActionResult RemoveCartItem(int ProductID)
        {
            var cart = CartHelper.GetCart(HttpContext.Session);

            var cartItem = cart.FirstOrDefault(c => c.ProductID == ProductID);
            if (cartItem != null)
            {
                cart.Remove(cartItem);
            }

            CartHelper.SaveCart(HttpContext.Session, cart);
            return RedirectToAction("Cart");
        }
        [HttpGet]
        public IActionResult Checkout()
        {
            var cart = CartHelper.GetCart(HttpContext.Session);

            if (cart == null || !cart.Any())
            {
                TempData["ErrorMessage"] = "Your cart is empty. Please add items to proceed.";
                return RedirectToAction("Cart");
            }

            var model = new CheckoutModel
            {
                OrderItems = cart
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult UpdateCartItem(int ProductID, int Quantity)
        {
            var cart = CartHelper.GetCart(HttpContext.Session);

            var cartItem = cart.FirstOrDefault(c => c.ProductID == ProductID);
            if (cartItem != null)
            {
                cartItem.Quantity = Math.Clamp(Quantity, 1, 10);
            }

            CartHelper.SaveCart(HttpContext.Session, cart);

            return RedirectToAction("Cart");
        }

        [HttpPost]
        public IActionResult ProcessCheckout(CheckoutModel model)
        {
            var cart = CartHelper.GetCart(HttpContext.Session);
            if (cart == null || !cart.Any())
            {
                TempData["ErrorMessage"] = "Your cart is empty. Please add items to proceed.";
                return RedirectToAction("Checkout");
            }

            if (ModelState.IsValid)
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        var order = new Order
                        {
                            UserID = GetLoggedInUserId(),
                            OrderDate = DateTime.Now,
                            TotalAmount = cart.Sum(item => item.Price * item.Quantity),
                            Status = "Pending"
                        };

                        _context.Orders.Add(order);
                        _context.SaveChanges();

                        foreach (var cartItem in cart)
                        {
                            var orderDetail = new OrderDetail
                            {
                                OrderID = order.OrderID,
                                ProductID = cartItem.ProductID,
                                Quantity = cartItem.Quantity,
                                Price = cartItem.Price
                            };

                            _context.OrderDetails.Add(orderDetail);
                        }

                        _context.SaveChanges();
                        transaction.Commit();
                        CartHelper.ClearCart(HttpContext.Session);

                        HttpContext.Session.SetInt32("OrderID", order.OrderID);

                        return RedirectToAction("OrderConfirmation");
                    }
                    catch
                    {
                        transaction.Rollback();
                        TempData["ErrorMessage"] = "There was an error processing your order. Please try again.";
                        return RedirectToAction("Checkout");
                    }
                }
            }

            TempData["ErrorMessage"] = "There was an error with your checkout details. Please try again.";
            return View("Checkout", model);
        }



        private int GetLoggedInUserId()
        {
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                throw new InvalidOperationException("User is not logged in.");
            }
            return int.Parse(userIdClaim.Value);
        }




        [HttpGet]
        public IActionResult OrderConfirmation()
        {
            var orderId = HttpContext.Session.GetInt32("OrderID");
            if (orderId == null)
            {
                TempData["ErrorMessage"] = "No order details found.";
                return RedirectToAction("Cart");
            }

            var order = _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefault(o => o.OrderID == orderId);

            if (order == null)
            {
                TempData["ErrorMessage"] = "Order not found.";
                return RedirectToAction("Cart");
            }

            var model = new CheckoutModel
            {
                OrderItems = order.OrderDetails.Select(od => new CartItem
                {
                    ProductID = od.ProductID,
                    Name = od.Product.Name,
                    Price = od.Price,
                    Quantity = od.Quantity,
                    ImageURL = od.Product.ImageURL
                }).ToList(),
                FullName = HttpContext.User.Identity.Name,
                Email = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value,
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddReview(CustomerReview review)
        {
            if (ModelState.IsValid)
            {
                review.UserID = GetLoggedInUserId();
                review.Date = DateTime.Now;

                _context.CustomerReviews.Add(review);
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", new { id = review.ProductID });
            }

            TempData["ErrorMessage"] = "There was an issue submitting your review. Please try again.";
            return RedirectToAction("Details", new { id = review.ProductID });
        }
    }
}
