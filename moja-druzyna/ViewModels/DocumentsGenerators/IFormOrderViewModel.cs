using moja_druzyna.Lib.Order;
using System.Collections.Generic;

namespace moja_druzyna.ViewModels.DocumentsGenerators
{
    public interface IFormOrderViewModel
    {
        public string GetScoutId();
        public void AddElement(string scoutId, string pesel, string name, string surname);
        public List<IOrderElement> GetList();
    }
}