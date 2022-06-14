using moja_druzyna.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace moja_druzyna.ViewModels
{
    public class OrderFormViewModel
    {
        public OrderFormViewModel()
        {
            OrderForm_Order OrderForm_Data = new OrderForm_Order();
        }

        public OrderForm_Order CreateOrderFromInput(List<String> data)
        {
            return (new OrderForm_Order() { Number = data[0], Date = data[1], Place = data[2] });
        }

        public OrderForm_Order OrderForm_Data { get; set; }

        public class OrderForm_Order
        {
            public string Number { get; set; }
            public string Date { get; set; }
            public string Place { get; set; }
        }
    }
}
