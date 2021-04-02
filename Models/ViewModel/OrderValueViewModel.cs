using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.ViewModel
{
    public class OrderValueViewModel
    {
        [Display(Name = "0% Supplies")]
        public double Supplies0 { get; set; }
        [Display(Name = "1% Supplies")]
        public double Supplies1 { get; set; }
        [Display(Name = "4% Supplies")]
        public double Supplies4 { get; set; }
        [Display(Name = "This Order Value")]
        public double TotalOrderValues { get; set; }
        [Display(Name = "Approved Order Values")]
        public double PendingOrderValues { get; set; }
        [Display(Name = "Current Balance")]
        public double CurrentBalance { get; set; }
        [Display(Name = "Unapproved Payments")]
        public double UnConfirmedPayment { get; set; }
        [Display(Name = "Net Payable")]
        public double NetPayable { get; set; }
        [Display(Name = "Unapproved Order Values")]
        public double TotalUnapprovedOrderValues { get; set; }

        public double SAMISupplies0 { get; set; }
        public double SAMISupplies1 { get; set; }
        public double SAMISupplies4 { get; set; }
        public double SAMITotalOrderValues { get; set; }
        public double SAMIPendingOrderValues { get; set; }
        public double SAMICurrentBalance { get; set; }
        public double SAMIUnConfirmedPayment { get; set; }
        public double SAMINetPayable { get; set; }
        public double SAMITotalUnapprovedOrderValues { get; set; }

        public double HealthTekSupplies0 { get; set; }
        public double HealthTekSupplies1 { get; set; }
        public double HealthTekSupplies4 { get; set; }
        public double HealthTekTotalOrderValues { get; set; }
        public double HealthTekPendingOrderValues { get; set; }
        public double HealthTekCurrentBalance { get; set; }
        public double HealthTekUnConfirmedPayment { get; set; }
        public double HealthTekNetPayable { get; set; }
        public double HealthTekTotalUnapprovedOrderValues { get; set; }

        public double PhytekSupplies0 { get; set; }
        public double PhytekSupplies1 { get; set; }
        public double PhytekSupplies4 { get; set; }
        public double PhytekTotalOrderValues { get; set; }
        public double PhytekPendingOrderValues { get; set; }
        public double PhytekCurrentBalance { get; set; }
        public double PhytekUnConfirmedPayment { get; set; }
        public double PhytekNetPayable { get; set; }
        public double PhytekTotalUnapprovedOrderValues { get; set; }
    }
}
