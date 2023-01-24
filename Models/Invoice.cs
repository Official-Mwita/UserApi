namespace UserApi.Models
{
    public class Invoice
    {
        public string InvoiceID { get; set; }

        public DateTime DateCreated { get; set; }

        public decimal TotalBillable { get; set; }

        public decimal TotalTaxable { get; set; }

        public string ServerdBy { get; set; }

    }
}
