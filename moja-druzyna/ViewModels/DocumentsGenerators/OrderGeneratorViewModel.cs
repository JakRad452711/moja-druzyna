using System;

namespace moja_druzyna.ViewModels.DocumentsGenerators
{
    public class OrderGeneratorViewModel
    {
        public string OrderNumber { get; set; }
        public string Location { get; set; }
        public DateTime? CreationDate { get; set; }
    }
}
