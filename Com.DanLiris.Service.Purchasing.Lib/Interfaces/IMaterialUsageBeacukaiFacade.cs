using Com.DanLiris.Service.Purchasing.Lib.ViewModels.GarmentReports;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Com.DanLiris.Service.Purchasing.Lib.Interfaces
{
    public interface IMaterialUsageBeacukaiFacade
    {
        Tuple<List<MaterialUsageBBCentralViewModel>, int> GetReportUsageBBCentral(int page, int size, string Order, DateTime? dateFrom, DateTime? dateTo, int offset);
        MemoryStream GenerateExcelUsageBBCentral (DateTime? dateFrom, DateTime? dateTo, int offset);
        Tuple<List<MutationBPCentralViewModel>, int> GetReportUsageBPCentral(int page, int size, string Order, DateTime? dateFrom, DateTime? dateTo, int offset);
        MemoryStream GenerateExcelUsageBPCentral (DateTime? dateFrom, DateTime? dateTo, int offset);
    }
}
