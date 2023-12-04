using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        public ICategoryRepository Category { get; }
        public IProductRepository Product { get;}
        public ICompanyRepository Company { get;}
        public IShoppingCartRepository ShoppingCart { get; set; }
    
        public IApplicationUserRepository ApplicationUser { get; set; }

        public IOrderDetailRepository OrderDetail { get; set; }
        public IOrderHeaderRepository OrderHeader { get; set; }



        void Save();
    }
}
