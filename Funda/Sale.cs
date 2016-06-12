//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Funda
{
    using System;
    using System.Collections.Generic;
    
    public partial class Sale : IFundaRecord
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Address { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<int> LivingArea { get; set; }
        public Nullable<int> RoomCount { get; set; }
        public string PostCode { get; set; }
        public Nullable<int> TotalArea { get; set; }
        public Nullable<System.DateTime> DateAdded { get; set; }
        public Nullable<System.DateTime> DateRemoved { get; set; }
        public Nullable<System.DateTime> DateLastProcessed { get; set; }
    }
}
