//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AppsSocket.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class core_chat
    {
        public int chatId { get; set; }
        public string msgfrom { get; set; }
        public string msgto { get; set; }
        public string msg { get; set; }
        public string createdby { get; set; }
        public Nullable<System.DateTime> createddate { get; set; }
        public Nullable<int> isread { get; set; }
    }
}