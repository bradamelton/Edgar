using System;
using DBu;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EdgarC
{
    [DatabaseName("EdgarC")]
    [Table("Company")]
    public class Company : DBuClass
    {
        public string Name { get; set; }
        public Int64 SECNumber { get; set; }
        public string Symbol { get; set; }
        public object TickerId { get; set; }

        public Company() { }
    }
}