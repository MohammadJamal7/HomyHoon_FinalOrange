using HomyHoon.Models;
using System.Numerics;

namespace HomyHoon_Ecommerce.ViewModels
{
    public class OwnerVM
    {
        public List<Apartment> OwnerAparts { get; set; }
        public double TotalReveniew {  get; set; }
        public int TotalApart { get; set; }
        public int TotalBookings { get; set; }
        public User User { get; set; }
    }
}
