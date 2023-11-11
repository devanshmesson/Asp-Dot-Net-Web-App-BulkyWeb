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
    public class OrderHeaderRepository: Repository<OrderHeader>, IOrderHeaderRepository
    {
        public ApplicationDbContext _db;
        public OrderHeaderRepository(ApplicationDbContext db): base(db)
        { 
            _db = db;
        }

        public void Update(OrderHeader orderHeader)
        {
            _db.OrderHeaders.Update(orderHeader);
        }

        public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
        {
            var orderHeader = _db.OrderHeaders.FirstOrDefault(orderHeader => orderHeader.Id == id);
            if (orderHeader != null)
            {
                orderHeader.OrderStatus = orderStatus;
                if(!string.IsNullOrEmpty(paymentStatus))
                {
                    orderHeader.PaymentStatus = paymentStatus;
                }
            }
        }

        public void UpdateStripePaymentID(int id, string sessionId, string paymentIntentId)
        {
            var orderHeader = _db.OrderHeaders.FirstOrDefault(orderHeader => orderHeader.Id == id);
            if (orderHeader != null)
            {
                if (!string.IsNullOrEmpty(sessionId))
                {
                    orderHeader.SessionId = sessionId;
                }
                if (!string.IsNullOrEmpty(paymentIntentId))
                {
                    orderHeader.PaymentIntentId = paymentIntentId;
                    orderHeader.PaymentDate = DateTime.Now;
                }
            }
        }
    }
}
