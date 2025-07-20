using EpicPanels_Group5_FinalProject.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace EpicPanels_Group5_FinalProject.Helpers
{
    public static class CartHelper
    {
        private const string CartSessionKey = "Cart";

        // Get the current cart from session
        public static List<CartItem> GetCart(ISession session)
        {
            var cart = session.GetString(CartSessionKey);
            return string.IsNullOrEmpty(cart) ? new List<CartItem>() : JsonConvert.DeserializeObject<List<CartItem>>(cart);
        }

        // Save the cart back to session
        public static void SaveCart(ISession session, List<CartItem> cart)
        {
            session.SetString(CartSessionKey, JsonConvert.SerializeObject(cart));
        }

        // Clear the cart from session
        public static void ClearCart(ISession session)
        {
            session.Remove(CartSessionKey);
        }
    }
}
