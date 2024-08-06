using System.Collections.Generic;
using System.Threading.Tasks;
using Mantis.Domain.Carriers.Models;
using Microsoft.EntityFrameworkCore;

namespace Mantis.Shared.DataAccess
{
    public class OrderDataAccessLayer
    {
        OrderContext db = new OrderContext();

        public DbSet<Carrier> GetAllOrders()
        {
            try
            {
                return db.Carriers;
            }
            catch
            {
                throw;
            }
        }
        public void AddOrder(Carrier carrier)
        {
            try
            {
                db.Carriers.Add(carrier);
                db.SaveChanges();
            }
            catch
            {
                throw;
            }
        }
        public void UpdateOrder(Carrier carrier)
        {
            try
            {
                db.Entry(carrier).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch
            {
                throw;
            }
        }
        public void DeleteOrder(int id)
        {
            try
            {
                Carrier ord = db.Carriers.Find(id);
                db.Carriers.Remove(ord);
                db.SaveChanges();
            }
            catch
            {
                throw;
            }
        }
    }
}
