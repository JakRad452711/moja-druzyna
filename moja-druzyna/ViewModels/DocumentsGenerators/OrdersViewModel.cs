using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace moja_druzyna.ViewModels.DocumentsGenerators
{
    public class OrdersViewModel
    {
        public OrdersViewModel()
        {
            OrderViewModel_Orders = new List<OrdersViewModel_Order>();
        }

        public string AddedOrderName { get; set; }

        public List<OrdersViewModel_Order> OrderViewModel_Orders { get; set; }

        public class OrdersViewModel_Order
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Creator { get; set; }
            public DateTime CreationDate { get; set; }
        }
    }
}
