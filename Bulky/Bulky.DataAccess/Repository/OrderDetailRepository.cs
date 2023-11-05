using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
    public class OrderDetailRepository: Repository<OrderDetail>, IOrderDetailRepository
    {
        public ApplicationDbContext _db { get; set; }
        public OrderDetailRepository(ApplicationDbContext db): base(db)
        {
                _db = db;
        }

        public void Update(OrderDetail orderDetail)
        {
            _db.OrderDetails.Update(orderDetail);
        }
    }
}
