using System;
using System.ComponentModel.DataAnnotations;

namespace pre_registration.Models.DataBaseModel
{
    public class Order
    {
        public int id { get; set; }
        [Display(Name = "Коментарий")]
        public string Comment { get; set; }
        public DateTime OrderDate { get; set; }
        public int ClientId { get; set; }
        public int CuponDateId { get; set; }

        public CuponDate CuponDate { get; set; }
        public Client Client { get; set; }
    }
}
