using HereToYou.Cart;
using HereToYou.Models;
using Newtonsoft.Json;
using static System.Net.Mime.MediaTypeNames;

namespace ecommerce.Cart
{
    public class CartService : ICartService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISession _session;

        public CartService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _session = _httpContextAccessor.HttpContext.Session;
        }

        public void AddToCart(Product product, int quantity)
        {
            var cart = GetCart();
            var cartItem = cart.FirstOrDefault(item => item.ProductId == product.ProductId);
            if (cartItem == null)
            {
                cartItem = new CartItem
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    Price = product.Price,
                    Image = product.Image,
                    Quantity = quantity
                };
                cart.Add(cartItem);
            }
            else
            {
                cartItem.Quantity += quantity;
            }
            SaveCart(cart);
        }

        public List<CartItem> GetCart()
        {
            var cartJson = _session.GetString("Cart");
            if (string.IsNullOrEmpty(cartJson))
            {
                return new List<CartItem>();
            }
            return JsonConvert.DeserializeObject<List<CartItem>>(cartJson);
        }

        public void RemoveFromCart(int productId)
        {
            var cart = GetCart();
            var cartItem = cart.FirstOrDefault(item => item.ProductId == productId);
            if (cartItem != null)
            {
                cart.Remove(cartItem);
                SaveCart(cart);
            }
        }

        public void ClearCart()
        {
            _session.Remove("Cart");
        }

        public void SaveCart(List<CartItem> cart)
        {
            var cartJson = JsonConvert.SerializeObject(cart);
            _session.SetString("Cart", cartJson);
        }
    }
}
