using System;
using System.Collections.Generic;
using System.Linq;
using Raven.Client.Documents.Session;
using Shop.Model;
using Shop.Raven;

namespace Shop
{
    class Program
    {
        static void Main(string[] args)
        {
            // 1
            //CreateProduct("Tea", 11.24);
            //CreateProduct("Apple", 9.00);

            // 2
            //GetProduct("products/65-A");

            // 3
            // GetAllProducts();

            // 4
            // CreateProduct("Banana", 8.60);
            // CreateProduct("Coffee", 19.49);
            // CreateProduct("Carrot", 27.97);

            // 5
            // GetProducts(2, 3);

            // 6
            // CreateCart("john@doe.com");

            // 7
            // AddProductToCart("carts/1-A", "products/1-A", 5);
            // AddProductToCart("carts/1-A", "products/2-A", 1);

            // 8
            // list cart content
            // PrintCart("john@doe.com");

            //Console.WriteLine("Hello World!");
        }

        // 1
        static void CreateProduct(string name, double price)
        {
            Product p = new Product();

            p.Name = name;
            p.Price = price;

            using (var session = DocumentStoreHolder.Store.OpenSession())
            {
                session.Store(p);
                session.SaveChanges();
            }
        }

        // 2
        static void GetProduct(string id)
        {
            using (var session = DocumentStoreHolder.Store.OpenSession())
            {
                Product p = session.Load<Product>(id);
                Console.WriteLine($"Product: {p.Name} \t\t price: {p.Price}");
            }
        }

        // 3
        static void GetAllProducts()
        {
            using (var session = DocumentStoreHolder.Store.OpenSession())
            {
                List<Product> all = session.Query<Product>().ToList();

                foreach (Product p in all)
                {
                    Console.WriteLine($"{p.Name} \t\t {p.Price}");
                }
            }
        }

        // 4
        static void GetProducts(int pageNdx, int pageSize)
        {
            int skip = (pageNdx - 1) * pageSize;
            int take = pageSize;

            using (var session = DocumentStoreHolder.Store.OpenSession())
            {
                List<Product> page = session.Query<Product>()
                    .Statistics(out QueryStatistics stats)
                    .Skip(skip)
                    .Take(take)
                    .ToList();

                Console.WriteLine($"Showing results {skip + 1} to {skip + page.Count} of {stats.TotalResults}");
                foreach (Product p in page)
                {
                    Console.WriteLine($"{p.Name} \t\t {p.Price}");
                }
            }
        }

        // 5
        static void CreateCart(string customer)
        {
            Cart cart = new Cart();
            cart.Customer = customer;

            using (var session = DocumentStoreHolder.Store.OpenSession())
            {
                session.Store(cart);
                session.SaveChanges();
            }
        }

        // 6
        static void AddProductToCart(string cartId, string productId, int quantity)
        {
            using (var session = DocumentStoreHolder.Store.OpenSession())
            {
                Cart cart = session.Load<Cart>(cartId);
                Product product = session.Load<Product>(productId);

                cart.Lines.Add(new CartLine
                {
                    ProductName = product.Name,
                    ProductPrice = product.Price,
                    Quantity = quantity
                });

                session.SaveChanges();
            }
        }

        // 7
        static void PrintCart(string customer)
        {
            using (var session = DocumentStoreHolder.Store.OpenSession())
            {
                Cart cart = session.Query<Cart>().Single(x => x.Customer == customer);

                Console.WriteLine($"Cart content for {customer}");

                foreach (CartLine line in cart.Lines)
                {
                    Console.WriteLine($"{line.ProductName}: {line.Quantity} x {line.ProductPrice}");
                }
            }
        }
    }
}
