using HereToYou.Cart;
using HereToYou.Models;
using Newtonsoft.Json;

namespace ecommerce.Cart
{
    public interface ICartService
    {
            void AddToCart(Product product, int quantity);
            List<CartItem> GetCart();
            void RemoveFromCart(int productId);
            void ClearCart();
           void SaveCart(List<CartItem> cart);



    }
}
