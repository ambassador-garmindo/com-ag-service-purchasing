using Com.DanLiris.Service.Purchasing.Lib.Helpers;
using Com.DanLiris.Service.Purchasing.Lib.Interfaces;
using Com.DanLiris.Service.Purchasing.Lib.Models.GarmentDeliveryOrderModel;
using Com.DanLiris.Service.Purchasing.Lib.ViewModels.GarmentReports;
using Com.DanLiris.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Com.DanLiris.Service.Purchasing.Lib.Facades.GarmentReports
{
    public class MaterialUsageBeacukaiFacade : IMaterialUsageBeacukaiFacade
    {
        private readonly PurchasingDbContext dbContext;
        public readonly IServiceProvider serviceProvider;
        private readonly DbSet<GarmentDeliveryOrder> dbSet;

        public MaterialUsageBeacukaiFacade(IServiceProvider serviceProvider, PurchasingDbContext dbContext)
        {
            this.serviceProvider = serviceProvider;
            this.dbContext = dbContext;
            this.dbSet = dbContext.Set<GarmentDeliveryOrder>();
        }

        #region BB
        public Tuple<List<MaterialUsageBBCentralViewModel>, int> GetReportUsageBBCentral(int page, int size, string Order, DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            List<MaterialUsageBBCentralViewModel> Query = GetCentralItemUsageBBReport(dateFrom, dateTo, offset).ToList();

            Pageable<MaterialUsageBBCentralViewModel> pageable = new Pageable<MaterialUsageBBCentralViewModel>(Query, page - 1, size);
            List<MaterialUsageBBCentralViewModel> Data = pageable.Data.ToList<MaterialUsageBBCentralViewModel>();
            int TotalData = pageable.TotalCount;
            return Tuple.Create(Data, TotalData);
        }

        private List<GarmentCategoryViewModel> GetProductCodes(int page, int size, string order, string filter)
        {
            IHttpClientService httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));
            var garmentSupplierUri = APIEndpoint.Core + $"master/garment-categories";
            string queryUri = "?page=" + page + "&size=" + size + "&order=" + order + "&filter=" + filter;
            string uri = garmentSupplierUri + queryUri;
            var response = httpClient.GetAsync($"{APIEndpoint.Core}/master/garment-categories?page={page}&size={size}").Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
                List<GarmentCategoryViewModel> viewModel = JsonConvert.DeserializeObject<List<GarmentCategoryViewModel>>(result.GetValueOrDefault("data").ToString()); ;
                return viewModel ;
            }
            else
            {
                return null;
            }
        }

        public List<MaterialUsageBBCentralViewModel> GetCentralItemUsageBBReport(DateTime? datefrom, DateTime? dateto, int offset)
        {
            DateTime DateFrom = datefrom == null ? new DateTime(1970, 1, 1) : (DateTime)datefrom;
            DateTime DateTo = dateto == null ? DateTime.Now : (DateTime)dateto;

            var pengeluaran = new[] { "PROSES", "SUBCON" };
            var unit = new[] { "AG2" };
            var storage = new[] { "GUDANG BAHAN BAKU" };
            var customs = new[] { "IMPORT FASILITAS", "LOKAL FASILITAS" };

            var categories = GetProductCodes(1, int.MaxValue, "{}", "{}");
            var categories1 = categories.Where(x => x.CodeRequirement == "BB").Select(x => x.Name).ToArray();
           
            var Expenditure = (from a in (from aa in dbContext.GarmentUnitExpenditureNotes
                                               where aa.CreatedUtc.Date <= DateTo.Date
                                               && aa.CreatedUtc.Date >= DateFrom.Date
                                               && aa.UId == null && aa.IsDeleted == false && pengeluaran.Contains(aa.ExpenditureType) && unit.Contains(aa.UnitSenderCode) && storage.Contains(aa.StorageName)
                                               select aa)
                                    join b in (from bb in dbContext.GarmentUnitExpenditureNoteItems where categories1.Contains(bb.ProductName) && bb.IsDeleted == false select bb) on a.Id equals b.UENId
                                    join f in dbContext.GarmentUnitReceiptNoteItems on b.URNItemId equals f.Id where customs.Contains(f.CustomsCategory)
                                    join c in dbContext.GarmentExternalPurchaseOrderItems.IgnoreQueryFilters() on f.EPOItemId equals c.Id
                                    join d in dbContext.GarmentExternalPurchaseOrders.IgnoreQueryFilters() on c.GarmentEPOId equals d.Id

                                    select new MaterialUsageBBCentralViewModel
                                    {
                                          UENNo = a.UENNo,
                                          UENDate = a.ExpenditureDate,
                                          ItemCode = b.ProductCode,
                                          ItemName = b.ProductName,
                                          UnitQtyName = b.UomUnit,
                                          ExpenditureType = a.ExpenditureType,
                                          ExpenditureProcessQty = (a.ExpenditureType == "PROSES" ? b.Quantity : 0),
                                          ExpenditureSubconQty = (a.ExpenditureType == "SUBCON" ? b.Quantity : 0),
                                          SubconProvider = (a.ExpenditureType == "SUBCON" ? "SUBCON" : "-")
                                    }).ToList();            

            return Expenditure;
        }

        public MemoryStream GenerateExcelUsageBBCentral(DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            var data = GetCentralItemUsageBBReport(dateFrom, dateTo, offset);
            var Query = data.OrderByDescending(x => x.ItemCode).ThenBy(x => x.ItemName).ToList();
            //Query = Query.OrderBy(b => b.ItemCode).ToList();
            DataTable result = new DataTable();

            var headers = new string[] { "No", "Bukti Pengeluaran", "Bukti Pengeluaran1" , "Kode Barang", "Nama Barang", "Satuan", "Jumlah", "Jumlah1", "Penerima Subkontrak" };
            var subheaders = new string[] { "Nomor", "Tanggal", "Digunakan", "Disubkontrakkan"};

            for (int i = 0; i < 6; i++)
            {
                result.Columns.Add(new DataColumn() { ColumnName = headers[i], DataType = typeof(String) });
            }

            result.Columns.Add(new DataColumn() { ColumnName = headers[6], DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = headers[7], DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = headers[8], DataType = typeof(String) });

            var index = 1;
            double ProcessQtyTotal = 0;
            double SubconQtyTotal = 0;
            foreach (var item in Query)
            {
                ProcessQtyTotal += item.ExpenditureProcessQty;
                SubconQtyTotal += item.ExpenditureSubconQty;

                result.Rows.Add(index++, item.UENNo, item.UENDate.ToString("dd MMM yyyy", new CultureInfo("id-ID")), item.ItemCode, item.ItemName, item.UnitQtyName, Convert.ToDouble(item.ExpenditureProcessQty), Convert.ToDouble(item.ExpenditureSubconQty), item.SubconProvider);
            }

            ExcelPackage package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("Data");

            var col = (char)('A' + result.Columns.Count);
            string tglawal = new DateTimeOffset(dateFrom.Value).ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
            string tglakhir = new DateTimeOffset(dateTo.Value).ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));

            sheet.Cells[$"A1:{col}1"].Value = string.Format("LAPORAN PEMAKAIAN BAHAN BAKU");
            sheet.Cells[$"A1:{col}1"].Merge = true;
            sheet.Cells[$"A1:{col}1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            sheet.Cells[$"A1:{col}1"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            sheet.Cells[$"A1:{col}1"].Style.Font.Bold = true;

            sheet.Cells[$"A2:{col}2"].Value = string.Format("Periode {0} - {1}", tglawal, tglakhir);
            sheet.Cells[$"A2:{col}2"].Merge = true;
            sheet.Cells[$"A2:{col}2"].Style.Font.Bold = true;
            sheet.Cells[$"A2:{col}2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            sheet.Cells[$"A2:{col}2"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

            sheet.Cells["A7"].LoadFromDataTable(result, false, OfficeOpenXml.Table.TableStyles.Light16);

            sheet.Cells["A5"].Value = headers[0];
            sheet.Cells["A5:A6"].Merge = true;

            sheet.Cells["B5"].Value = headers[1];
            sheet.Cells["B5:C5"].Merge = true;

            sheet.Cells["B6"].Value = subheaders[0];
            sheet.Cells["C6"].Value = subheaders[1];

            sheet.Cells["D5"].Value = headers[3];
            sheet.Cells["D5:D6"].Merge = true;

            sheet.Cells["E5"].Value = headers[4];
            sheet.Cells["E5:E6"].Merge = true;

            sheet.Cells["F5"].Value = headers[5];
            sheet.Cells["F5:F6"].Merge = true;

            sheet.Cells["G5"].Value = headers[6];
            sheet.Cells["G5:H5"].Merge = true;

            sheet.Cells["G6"].Value = subheaders[2];
            sheet.Cells["H6"].Value = subheaders[3];

            sheet.Cells["I5"].Value = headers[8];
            sheet.Cells["I5:I6"].Merge = true;

            sheet.Cells["A5:I6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells["A5:I6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Cells["A5:I6"].Style.Font.Bold = true;

            var widths = new int[] { 10, 20, 20, 15, 15, 15, 20, 20, 20};

            foreach (var i in Enumerable.Range(0, headers.Length))
            {
                sheet.Column(i + 1).Width = widths[i];
            }

            var a = Query.Count();

            sheet.Cells[$"A{8 + a}"].Value = "T O T A L  . . . . . . . . . . . . . . .";
            sheet.Cells[$"A{8 + a}:F{8 + a}"].Merge = true;
            sheet.Cells[$"A{8 + a}:F{8 + a}"].Style.Font.Bold = true;
            sheet.Cells[$"A{8 + a}:F{8 + a}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells[$"A{8 + a}:F{8 + a}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Cells[$"G{8 + a}"].Value = ProcessQtyTotal;
            sheet.Cells[$"H{8 + a}"].Value = SubconQtyTotal;

            MemoryStream stream = new MemoryStream();
            package.SaveAs(stream);
            return stream;
        }
        #endregion 

        #region BP
        public Tuple<List<MutationBPCentralViewModel>, int> GetReportUsageBPCentral(int page, int size, string Order, DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            //var Query = GetStockQuery(tipebarang, unitcode, dateFrom, dateTo, offset);
            //Query = Query.OrderByDescending(x => x.SupplierName).ThenBy(x => x.Dono);
            List<MutationBPCentralViewModel> Query = GetCentralItemUsageBPReport(dateFrom, dateTo, offset).ToList();
            //Query = Query.OrderBy(x => x.ItemCode).ToList();

            Pageable<MutationBPCentralViewModel> pageable = new Pageable<MutationBPCentralViewModel>(Query, page - 1, size);
            List<MutationBPCentralViewModel> Data = pageable.Data.ToList<MutationBPCentralViewModel>();
            int TotalData = pageable.TotalCount;
            //int TotalData = Data.Count();
            return Tuple.Create(Data, TotalData);
        }

        public List<MutationBPCentralViewModel> GetCentralItemUsageBPReport(DateTime? datefrom, DateTime? dateto, int offset)
        {
            DateTime DateFrom = datefrom == null ? new DateTime(1970, 1, 1) : (DateTime)datefrom;
            DateTime DateTo = dateto == null ? DateTime.Now : (DateTime)dateto;

            var pengeluaran = new[] { "PROSES", "SAMPLE", "EXTERNAL" };
            var pemasukan = new[] { "PROSES", "PEMBELIAN" };

            var categories = GetProductCodes(1, int.MaxValue, "{}", "{}");
            var coderequirement = new[] { "BP", "BE" };
            var categories1 = categories.Where(x => coderequirement.Contains(x.CodeRequirement)).Select(x => x.Name).ToArray();

            List<MutationBPCentralViewModelTemp> saldoawalreceipt = new List<MutationBPCentralViewModelTemp>();
            List<MutationBPCentralViewModelTemp> saldoawalexpenditure = new List<MutationBPCentralViewModelTemp>();
            List<MutationBPCentralViewModelTemp> saldoawalreceiptcorrection = new List<MutationBPCentralViewModelTemp>();

            #region Balance
            //var lastdate = dbContext.BalanceStocks.OrderByDescending(x => x.CreateDate).Select(x => x.CreateDate).FirstOrDefault();

            //var BalanceStock = (from a in (from aa in dbContext.BalanceStocks where aa.CreateDate == lastdate select aa)
            //                    join b in (from bb in dbContext.GarmentExternalPurchaseOrderItems.IgnoreQueryFilters() where categories1.Contains(bb.ProductName) select bb) on (long)a.EPOItemId equals b.Id
            //                    join c in dbContext.GarmentExternalPurchaseOrders.IgnoreQueryFilters() on b.GarmentEPOId equals c.Id
                
            //                    select new MutationBPCentralViewModelTemp
            //                    {
            //                        ItemCode = b.ProductCode,
            //                        BeginQty = (double)a.CloseStock,
            //                        ExpenditureQty = 0,
            //                        ItemName = b.ProductName,
            //                        ReceiptQty = 0,
            //                        SupplierType = c.SupplierImport,
            //                        UnitQtyName = b.DealUomUnit
            //                    }
            //                    );

            var ReceiptBalance = (from a in (from aa in dbContext.GarmentUnitReceiptNotes where aa.CreatedUtc.Date < DateFrom.Date
                                             && aa.UId == null && aa.IsDeleted == false && pemasukan.Contains(aa.URNType) select aa)
                                  join b in (from bb in dbContext.GarmentUnitReceiptNoteItems where categories1.Contains(bb.ProductName) && bb.IsDeleted == false select bb) on a.Id equals b.URNId
                                  join c in dbContext.GarmentExternalPurchaseOrderItems.IgnoreQueryFilters() on b.EPOItemId equals c.Id
                                  join d in dbContext.GarmentExternalPurchaseOrders.IgnoreQueryFilters() on c.GarmentEPOId equals d.Id
  
                                  select new MutationBPCentralViewModelTemp
                                  {
                                      ItemCode = b.ProductCode,
                                      BeginQty = (double)(b.ReceiptQuantity * b.Conversion),
                                      ExpenditureQty = 0,
                                      ItemName = b.ProductName,
                                      ReceiptQty = 0,
                                      SupplierType = d.SupplierImport,
                                      UnitQtyName = b.SmallUomUnit
                                  }
                                  );



            var ReceiptBalanceLocal = (from a in (from aa in dbContext.GarmentUnitReceiptNotes
                                                   where aa.LastModifiedUtc.Date < DateFrom.Date
                                                    && aa.UId != null && aa.IsDeleted == false && pemasukan.Contains(aa.URNType)
                                                  select aa)
                                       join b in (from bb in dbContext.GarmentUnitReceiptNoteItems where categories1.Contains(bb.ProductName) && bb.IsDeleted == false select bb) on a.Id equals b.URNId
                                       join c in dbContext.GarmentExternalPurchaseOrderItems.IgnoreQueryFilters() on b.EPOItemId equals c.Id
                                       join d in dbContext.GarmentExternalPurchaseOrders.IgnoreQueryFilters() on c.GarmentEPOId equals d.Id

                                       select new MutationBPCentralViewModelTemp
                                       {
                                           ItemCode = b.ProductCode,
                                           BeginQty = (double)(b.ReceiptQuantity * b.Conversion),
                                           ExpenditureQty = 0,
                                           ItemName = b.ProductName,
                                           ReceiptQty = 0,
                                           SupplierType = d.SupplierImport,
                                           UnitQtyName = b.SmallUomUnit
                                       }
                                       );


            var ExpenditureBalance = (from a in (from aa in dbContext.GarmentUnitExpenditureNotes
                                                 where aa.CreatedUtc.Date < DateFrom.Date && aa.UId == null && aa.IsDeleted == false && pengeluaran.Contains(aa.ExpenditureType) select aa)
                                      join b in (from bb in dbContext.GarmentUnitExpenditureNoteItems where categories1.Contains(bb.ProductName) && bb.IsDeleted == false select bb) on a.Id equals b.UENId
                                      //join b in dbContext.GarmentUnitExpenditureNoteItems on a.Id equals b.UENId
                                      //join g in dbContext.GarmentUnitDeliveryOrderItems on b.UnitDOItemId equals g.Id
                                      join f in (from ff in dbContext.GarmentUnitReceiptNoteItems where categories1.Contains(ff.ProductName) select ff) on b.URNItemId equals f.Id
                                      join c in dbContext.GarmentExternalPurchaseOrderItems.IgnoreQueryFilters() on f.EPOItemId equals c.Id
                                      join d in dbContext.GarmentExternalPurchaseOrders.IgnoreQueryFilters() on c.GarmentEPOId equals d.Id
    
                                      select new MutationBPCentralViewModelTemp
                                      {
                                          ItemCode = b.ProductCode,
                                          BeginQty = (double)(b.Quantity),
                                          ExpenditureQty = 0,
                                          ItemName = b.ProductName,
                                          ReceiptQty = 0,
                                          SupplierType = d.SupplierImport,
                                          UnitQtyName = b.UomUnit
                                      }
                                      );

            var ExpenditureBalanceLocal = (from a in (from aa in dbContext.GarmentUnitExpenditureNotes
                                                      where aa.LastModifiedUtc.Date < DateFrom.Date && aa.UId != null && aa.IsDeleted == false && pengeluaran.Contains(aa.ExpenditureType)
                                                      select aa)
                                           join b in (from bb in dbContext.GarmentUnitExpenditureNoteItems where categories1.Contains(bb.ProductName) && bb.IsDeleted == false select bb) on a.Id equals b.UENId
                                           //join g in dbContext.GarmentUnitDeliveryOrderItems on b.UnitDOItemId equals g.Id
                                           join f in (from ff in dbContext.GarmentUnitReceiptNoteItems where categories1.Contains(ff.ProductName) select ff) on b.URNItemId equals f.Id
                                           join c in dbContext.GarmentExternalPurchaseOrderItems.IgnoreQueryFilters() on b.EPOItemId equals c.Id
                                           join d in dbContext.GarmentExternalPurchaseOrders.IgnoreQueryFilters() on c.GarmentEPOId equals d.Id

                                           select new MutationBPCentralViewModelTemp
                                           {
                                               ItemCode = b.ProductCode,
                                               BeginQty = (double)(b.Quantity),
                                               ExpenditureQty = 0,
                                               ItemName = b.ProductName,
                                               ReceiptQty = 0,
                                               SupplierType = d.SupplierImport,
                                               UnitQtyName = b.UomUnit
                                           }
                                           );

            #endregion
            #region filtered
            var Receipt = (from a in (from aa in dbContext.GarmentUnitReceiptNotes
                                      where aa.CreatedUtc.Date >= DateFrom.Date && aa.CreatedUtc.Date <= DateTo.Date
                                        && aa.UId == null && aa.IsDeleted == false && pemasukan.Contains(aa.URNType)
                                      select aa)
                           join b in (from bb in dbContext.GarmentUnitReceiptNoteItems where categories1.Contains(bb.ProductName) && bb.IsDeleted == false select bb) on a.Id equals b.URNId
                           join c in dbContext.GarmentExternalPurchaseOrderItems.IgnoreQueryFilters() on b.EPOItemId equals c.Id
                           join d in dbContext.GarmentExternalPurchaseOrders.IgnoreQueryFilters() on c.GarmentEPOId equals d.Id

                           select new MutationBPCentralViewModelTemp
                           {
                               ItemCode = b.ProductCode,
                               BeginQty = 0,
                               ExpenditureQty = 0,
                               ItemName = b.ProductName,
                               ReceiptQty = (double)(b.ReceiptQuantity * b.Conversion),
                               SupplierType = d.SupplierImport,
                               UnitQtyName = b.SmallUomUnit
                           }
                           );

            var ReceiptLocal = (from a in (from aa in dbContext.GarmentUnitReceiptNotes
                                           where aa.LastModifiedUtc.Date >= DateFrom.Date && aa.LastModifiedUtc.Date <= DateTo.Date
                                            && aa.UId != null && aa.IsDeleted == false && pemasukan.Contains(aa.URNType)
                                           select aa)
                                join b in (from bb in dbContext.GarmentUnitReceiptNoteItems where categories1.Contains(bb.ProductName) && bb.IsDeleted == false select bb) on a.Id equals b.URNId
                                join c in dbContext.GarmentExternalPurchaseOrderItems.IgnoreQueryFilters() on b.EPOItemId equals c.Id
                                join d in dbContext.GarmentExternalPurchaseOrders.IgnoreQueryFilters() on c.GarmentEPOId equals d.Id

                                select new MutationBPCentralViewModelTemp
                                {
                                    ItemCode = b.ProductCode,
                                    BeginQty = 0,
                                    ExpenditureQty = 0,
                                    ItemName = b.ProductName,
                                    ReceiptQty = (double)(b.ReceiptQuantity * b.Conversion),
                                    SupplierType = d.SupplierImport,
                                    UnitQtyName = b.SmallUomUnit
                                }
                                );

            var Expenditure = (from a in (from aa in dbContext.GarmentUnitExpenditureNotes
                                          where aa.CreatedUtc.Date >= DateFrom.Date &&
                                            aa.CreatedUtc.Date <= DateTo.Date && aa.UId == null && aa.IsDeleted == false && pengeluaran.Contains(aa.ExpenditureType)
                                          select aa)
                               join b in (from bb in dbContext.GarmentUnitExpenditureNoteItems where categories1.Contains(bb.ProductName) && bb.IsDeleted == false select bb) on a.Id equals b.UENId
                               //join g in dbContext.GarmentUnitDeliveryOrderItems on b.UnitDOItemId equals g.Id
                               join f in (from ff in dbContext.GarmentUnitReceiptNoteItems select ff) on b.URNItemId equals f.Id
                               join c in dbContext.GarmentExternalPurchaseOrderItems.IgnoreQueryFilters() on f.EPOItemId equals c.Id
                               join d in dbContext.GarmentExternalPurchaseOrders.IgnoreQueryFilters() on c.GarmentEPOId equals d.Id
                               //where b.ProductCode == "NLB001"
                               select new MutationBPCentralViewModelTemp
                               {
                                   ItemCode = b.ProductCode,
                                   BeginQty = 0,
                                   ExpenditureQty = (double)(b.Quantity),
                                   ItemName = b.ProductName,
                                   ReceiptQty = 0,
                                   SupplierType = d.SupplierImport,
                                   UnitQtyName = b.UomUnit
                               }
                               );

            var ExpenditureLocal = (from a in (from aa in dbContext.GarmentUnitExpenditureNotes
                                               where aa.LastModifiedUtc.Date >= DateFrom.Date &&
                                                 aa.LastModifiedUtc.Date <= DateTo.Date && aa.UId != null && aa.IsDeleted == false && pengeluaran.Contains(aa.ExpenditureType)
                                               select aa)
                                    join b in (from bb in dbContext.GarmentUnitExpenditureNoteItems where categories1.Contains(bb.ProductName) && bb.IsDeleted == false select bb) on a.Id equals b.UENId
                                    //join g in dbContext.GarmentUnitDeliveryOrderItems on b.UnitDOItemId equals g.Id
                                    join f in (from ff in dbContext.GarmentUnitReceiptNoteItems select ff) on b.URNItemId equals f.Id
                                    join c in dbContext.GarmentExternalPurchaseOrderItems.IgnoreQueryFilters() on f.EPOItemId equals c.Id
                                    join d in dbContext.GarmentExternalPurchaseOrders.IgnoreQueryFilters() on c.GarmentEPOId equals d.Id
                                    //where b.ProductCode == "NLB001"
                                    select new MutationBPCentralViewModelTemp
                                    {
                                        ItemCode = b.ProductCode,
                                        BeginQty = 0,
                                        ExpenditureQty = (double)(b.Quantity),
                                        ItemName = b.ProductName,
                                        ReceiptQty = 0,
                                        SupplierType = d.SupplierImport,
                                        UnitQtyName = b.UomUnit
                                    }
                                    );

            //var ReceiptCorrection = (from a in (from aa in dbContext.GarmentReceiptCorrections where aa.CreatedUtc.Date >= DateFrom.Date
            //                                     && aa.CreatedUtc.Date <= DateTo.Date && aa.UId == null && aa.IsDeleted == false
            //                                    select aa)
            //                         join b in (from bb in dbContext.GarmentReceiptCorrectionItems where categories1.Contains(bb.ProductName) && bb.IsDeleted == false select bb) on a.Id equals b.CorrectionId
            //                         join c in dbContext.GarmentExternalPurchaseOrderItems.IgnoreQueryFilters() on b.EPOItemId equals c.Id
            //                         join d in dbContext.GarmentExternalPurchaseOrders.IgnoreQueryFilters() on c.GarmentEPOId equals d.Id
                             
            //                         select new MutationBPCentralViewModelTemp
            //                         {
            //                             ItemCode = b.ProductCode,
            //                             BeginQty = 0,
            //                             ExpenditureQty = b.SmallQuantity < 0 ? b.SmallQuantity * -1 : 0,
            //                             ItemName = b.ProductName,
            //                             ReceiptQty = b.SmallQuantity > 0 ? b.SmallQuantity : 0,
            //                             SupplierType = d.SupplierImport,
            //                             UnitQtyName = b.UomUnit
            //                         }
            //                         );

            #endregion

            var data = ReceiptBalance.Union(ReceiptBalanceLocal).Union(ExpenditureBalance).Union(ExpenditureBalanceLocal).
                Union(Receipt).Union(ReceiptLocal).Union(Expenditure).Union(ExpenditureLocal).
                //Union(ReceiptCorrection).
                AsEnumerable();

            var mutationgroup = data.GroupBy(x => new { x.ItemCode, x.ItemName, x.SupplierType, x.UnitQtyName }, (key, group) => new MutationBPCentralViewModelTemp
            {
                //AdjustmentQty = Math.Round(group.Sum(x => x.AdjustmentQty), 2),
                BeginQty = Math.Round(group.Sum(x => x.BeginQty), 2),
                ExpenditureQty = Math.Round(group.Sum(x => x.ExpenditureQty), 2),
                ItemCode = key.ItemCode,
                ItemName = key.ItemName,
                //LastQty = Math.Round(group.Sum(x => x.BeginQty) + group.Sum(x => x.ReceiptQty) - group.Sum(x => x.ExpenditureQty) + group.Sum(x => x.AdjustmentQty) + group.Sum(x => x.OpnameQty), 2),
                ReceiptQty = Math.Round(group.Sum(x => x.ReceiptQty), 2),
                SupplierType = key.SupplierType,
                UnitQtyName = key.UnitQtyName,
                //OpnameQty = Math.Round(group.Sum(x => x.OpnameQty), 2),
               // Diff = Math.Round(group.Sum(x => x.Diff), 2)
            });

            List<MutationBPCentralViewModel> mutations = new List<MutationBPCentralViewModel>();

            foreach(var item in mutationgroup)
            {
                MutationBPCentralViewModel mutation = new MutationBPCentralViewModel()
                {
                    AdjustmentQty = 0,
                    BeginQty = item.BeginQty,
                    ExpenditureQty = item.ExpenditureQty,
                    ItemCode = item.ItemCode,
                    ItemName = item.ItemName,
                    LastQty = (item.BeginQty + item.ReceiptQty) - (item.ExpenditureQty + 0 + 0),
                    ReceiptQty = item.ReceiptQty,
                    SupplierType = item.SupplierType == true ? "IMPORT" : "LOKAL",
                    UnitQtyName = item.UnitQtyName,
                    OpnameQty = 0,
                    Diff = 0
                };

                mutations.Add(mutation);
            }


            mutations = mutations.OrderBy(x => x.ItemCode).ToList();

            var mm = new MutationBPCentralViewModel();

            mm.AdjustmentQty = Math.Round(mutations.Sum(x => x.AdjustmentQty), 2);
            mm.BeginQty = Math.Round(mutations.Sum(x => x.BeginQty), 2);
            mm.ExpenditureQty = Math.Round(mutations.Sum(x => x.ExpenditureQty), 2);
            mm.ItemCode = "";
            mm.ItemName = "";
            mm.LastQty = Math.Round(mutations.Sum(x => x.LastQty), 2);
            mm.ReceiptQty = Math.Round(mutations.Sum(x => x.ReceiptQty), 2);
            mm.SupplierType = "";
            mm.UnitQtyName = "";
            mm.OpnameQty = 0;
            mm.Diff = 0;

            mutations.Add(new MutationBPCentralViewModel
            {
                AdjustmentQty = mm.AdjustmentQty,
                BeginQty = mm.BeginQty,
                ExpenditureQty = mm.ExpenditureQty,
                ItemCode = mm.ItemCode,
                ItemName = mm.ItemName,
                LastQty = mm.LastQty,
                ReceiptQty = mm.ReceiptQty,
                SupplierType = mm.SupplierType,
                UnitQtyName = mm.UnitQtyName,
                OpnameQty = mm.OpnameQty,
                Diff = mm.Diff
            });

            return mutations;



        }

        public MemoryStream GenerateExcelUsageBPCentral(DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            var Query = GetCentralItemUsageBPReport(dateFrom, dateTo, offset);
            //Query = Query.OrderBy(b => b.ItemCode).ToList();
            DataTable result = new DataTable();
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tipe", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Saldo Awal", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Pemasukan", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Pengeluaran", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Penyesuaian", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Saldo Akhir", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Stock Opname", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Selisih", DataType = typeof(Double) });
            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", "", "", "", "", "", "", ""); // to allow column name to be generated properly for empty data as template
            else
                foreach (var item in Query)
                {
                    result.Rows.Add((item.ItemCode), item.ItemName, item.SupplierType, item.UnitQtyName, item.BeginQty, item.ReceiptQty, item.ExpenditureQty, item.AdjustmentQty, item.LastQty, item.OpnameQty, item.Diff);
                }

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") }, true);

        }

        #endregion


    }

}
