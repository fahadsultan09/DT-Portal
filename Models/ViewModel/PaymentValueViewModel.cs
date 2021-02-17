using System.ComponentModel.DataAnnotations;

namespace Models.ViewModel
{
    public class PaymentValueViewModel
    {
        [Display(Name = "Unapproved Order Values")]
        public double TotalUnapprovedOrderValues { get; set; }
        [Display(Name = "Approved Order Values")]
        public double TotalApprovedOrderValues { get; set; }
        [Display(Name = "Pending Order Values")]
        public double PendingOrderValues { get; set; }
        [Display(Name = "Current Balance")]
        public double CurrentBalance { get; set; }
        [Display(Name = "UnConfirmed Payment")]
        public double UnConfirmedPayment { get; set; }
        [Display(Name = "Net Payable")]
        public double NetPayable { get; set; }

        public double SAMITotalUnapprovedOrderValues { get; set; }
        public double SAMITotalApprovedOrderValues { get; set; }
        public double SAMIPendingOrderValues { get; set; }
        public double SAMICurrentBalance { get; set; }
        public double SAMIUnConfirmedPayment { get; set; }
        public double SAMINetPayable { get; set; }

        public double HealthTekTotalUnapprovedOrderValues { get; set; }
        public double HealthTekApprovedTotalOrderValues { get; set; }
        public double HealthTekPendingOrderValues { get; set; }
        public double HealthTekCurrentBalance { get; set; }
        public double HealthTekUnConfirmedPayment { get; set; }
        public double HealthTekNetPayable { get; set; }

        public double PhytekTotalUnapprovedOrderValues { get; set; }
        public double PhytekApprovedTotalOrderValues { get; set; }
        public double PhytekPendingOrderValues { get; set; }
        public double PhytekCurrentBalance { get; set; }
        public double PhytekUnConfirmedPayment { get; set; }
        public double PhytekNetPayable { get; set; }
    }
}
