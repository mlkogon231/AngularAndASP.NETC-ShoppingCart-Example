using DutchTreat.Data.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DutchTreat.Data
{
    public class DutchSeeder
    {
        private readonly DutchContext _ctx;
        private readonly IHostingEnvironment _hosting;
        private readonly UserManager<StoreUser> _userManager;

        public DutchSeeder(DutchContext ctx, IHostingEnvironment hosting, UserManager<StoreUser> userManager)
        {
            _ctx = ctx;
            _hosting = hosting;
            _userManager = userManager;
        }

        public async Task SeedAsync()
        {
          _ctx.Database.EnsureCreated();

            StoreUser user = await _userManager.FindByEmailAsync("mlkogon231@gmail.com");
			      if (user == null)
			      {
			      	user = new StoreUser()
			      	{
			      		FirstName = "Mark",
			      		LastName = "Kogon",
			      		Email = "mlkogon231@gmail.com",
			      		UserName = "mlkogon231@gmail.com"
			      	};
			      
				var result = await _userManager.CreateAsync(user, "Possum23!");
				if (result != IdentityResult.Success)
				{
					throw new InvalidOperationException("Could not create new user in seeder");
				}
			}
          if (!_ctx.Products.Any())
          {
            // need to create sample data
            var filepath = Path.Combine(_hosting.ContentRootPath, "Data/art.json");
            var json = File.ReadAllText(filepath);
            var products = JsonConvert.DeserializeObject<IEnumerable<Product>>(json);
            _ctx.Products.AddRange(products);

            var order = _ctx.Orders.Where(o => o.Id == 1)
                .FirstOrDefault();
            if (order != null)
            {
								order.User = user;
                order.Items = new List<OrderItem>()
                {
                    new OrderItem()
                    {
                        Product = products.First(),
                        Quantity = 5,
                        UnitPrice = products.First().Price
                    }
                };
            }

            _ctx.SaveChanges();
          }
        }

    }
}
