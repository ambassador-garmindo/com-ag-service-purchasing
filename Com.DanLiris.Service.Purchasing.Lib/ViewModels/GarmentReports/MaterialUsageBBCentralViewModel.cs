using System;
using System.Collections.Generic;
using System.Text;

namespace Com.DanLiris.Service.Purchasing.Lib.ViewModels.GarmentReports
{
    public class MaterialUsageBBCentralViewModel
    {
        public string UENNo { get; set; }
        public DateTimeOffset UENDate { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string UnitQtyName { get; set; }
        public string ExpenditureType { get; set; }
        public double ExpenditureProcessQty { get; set; }
        public double ExpenditureSubconQty { get; set; }
        public string SubconProvider { get; set; }
    }
}
