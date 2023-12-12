using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;

namespace Bulky.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(Product product)
        {
            var obj = _db.Products.FirstOrDefault(x => x.Id == product.Id);
            if (obj != null)
            {
                obj.Title = product.Title;
                obj.Description = product.Description;
                obj.Price = product.Price;
                obj.Price50 = product.Price50;
                obj.Price100 = product.Price100;
                obj.CategoryId = product.CategoryId;
                obj.Description = product.Description;
                obj.Author = product.Author;
                obj.ISBN = product.ISBN;
                obj.ListPrice = product.ListPrice;
               /* if (obj.ImageUrl != null)
                {
                    obj.ImageUrl = product.ImageUrl;
                }*/
            }
        }
    }
}
