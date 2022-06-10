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
            OrderForm_Data = new List<OrderForm_Order>();
        }

        public void AddScout(Scout scout)
        {
            OrderForm_Data.Add(new OrderForm_Order() { Id = scout.Pesel, Label = string.Format("{0} {1}\t({2})", scout.Name, scout.Surname, scout.Pesel), Prop1 = "", Prop2 = "" });
        }

        public List<OrderForm_Order> OrderForm_Data { get; set; }

        public class OrderForm_Order
        {
            public string Id { get; set; }
            public string Label { get; set; }
            public string Prop1 { get; set; }
            public string Prop2 { get; set; }
        }
    }
}
