//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WalletService
{
    using System;
    using System.Collections.Generic;
    
    public partial class TransactionType
    {
        public TransactionType()
        {
            this.WalletTransactions = new HashSet<WalletTransaction>();
        }
    
        public int Id { get; set; }
        public string Type { get; set; }
    
        public virtual ICollection<WalletTransaction> WalletTransactions { get; set; }
    }
}
