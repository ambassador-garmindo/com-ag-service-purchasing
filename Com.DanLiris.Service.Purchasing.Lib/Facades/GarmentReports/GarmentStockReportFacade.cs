using Com.DanLiris.Service.Purchasing.Lib.Interfaces;
using Com.DanLiris.Service.Purchasing.Lib.Models.GarmentDeliveryOrderModel;
using Com.DanLiris.Service.Purchasing.Lib.ViewModels.GarmentReports;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using System.Data;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Com.DanLiris.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using Com.DanLiris.Service.Purchasing.Lib.Helpers;
using Newtonsoft.Json;

namespace Com.DanLiris.Service.Purchasing.Lib.Facades.GarmentReports
{
    public class GarmentStockReportFacade : IGarmentStockReportFacade
    {
        private readonly PurchasingDbContext dbContext;
        public readonly IServiceProvider serviceProvider;
        private readonly DbSet<GarmentDeliveryOrder> dbSet;

        public GarmentStockReportFacade(IServiceProvider serviceProvider, PurchasingDbContext dbContext)
        {
            this.serviceProvider = serviceProvider;
            this.dbContext = dbContext;
            this.dbSet = dbContext.Set<GarmentDeliveryOrder>();
        }

        //public IEnumerable<GarmentStockReportViewModel> GetStockQuery(string ctg, string unitcode, DateTime? datefrom, DateTime? dateto, int offset, string suppliertype, string customstype)
        //{
        //    DateTime DateFrom = datefrom == null ? new DateTime(1970, 1, 1) : (DateTime)datefrom;
        //    DateTime DateTo = dateto == null ? DateTime.Now : (DateTime)dateto;

        //    var PPAwal = (from a in dbContext.GarmentUnitReceiptNotes
        //                  join b in dbContext.GarmentUnitReceiptNoteItems on a.Id equals b.URNId
        //                  join d in dbContext.GarmentDeliveryOrderDetails on b.DODetailId equals d.Id
        //                  join f in dbContext.GarmentInternalPurchaseOrders on b.POId equals f.Id
        //                  join e in dbContext.GarmentReceiptCorrectionItems on b.Id equals e.URNItemId into RC
        //                  from ty in RC.DefaultIfEmpty()

        //                  join x in dbContext.GarmentUnitDeliveryOrderItems on b.Id equals x.URNItemId into GUDI
        //                  from xx in GUDI.DefaultIfEmpty()
        //                  join y in dbContext.GarmentUnitDeliveryOrders on xx.UnitDOId equals y.Id into GUD
        //                  from yy in GUD.DefaultIfEmpty()

        //                  join c in dbContext.GarmentUnitExpenditureNoteItems on xx.Id equals c.UnitDOItemId into UE
        //                  from ww in UE.DefaultIfEmpty()
        //                  join r in dbContext.GarmentUnitExpenditureNotes on ww.UENId equals r.Id into UEN
        //                  from dd in UEN.DefaultIfEmpty()
        //                  join epoItem in dbContext.GarmentExternalPurchaseOrderItems on b.POId equals epoItem.POId into EP
        //                  from epoItem in EP.DefaultIfEmpty()
        //                  join epo in dbContext.GarmentExternalPurchaseOrders on epoItem.GarmentEPOId equals epo.Id into EPO
        //                  from epo in EPO.DefaultIfEmpty()
        //                  where d.CodeRequirment == (String.IsNullOrWhiteSpace(ctg) ? d.CodeRequirment : ctg)
        //                  && (!String.IsNullOrWhiteSpace(suppliertype) && !String.IsNullOrWhiteSpace(customstype) ? b.CustomsCategory == suppliertype + " " + customstype
        //                  : (!String.IsNullOrWhiteSpace(suppliertype) ? b.CustomsCategory.Contains(suppliertype)
        //                  : (!String.IsNullOrWhiteSpace(customstype) ? b.CustomsCategory.Substring(b.CustomsCategory.IndexOf(' ') + 1) == customstype
        //                  : true)))
        //                  && a.IsDeleted == false && b.IsDeleted == false

        //                  // 17-02-2021 : YOKA
        //                  //&& (!String.IsNullOrWhiteSpace(suppliertype) && !String.IsNullOrWhiteSpace(customstype) ? b.CustomsCategory == suppliertype + " " + customstype
        //                  //: (!String.IsNullOrWhiteSpace(suppliertype) ? b.CustomsCategory.Contains(suppliertype)
        //                  //: (!String.IsNullOrWhiteSpace(customstype) ? b.CustomsCategory.Substring(b.CustomsCategory.IndexOf(' ')) == customstype
        //                  //: true)))
        //                  //: true))

        //                  //String.IsNullOrEmpty(a.UENNo) ? false : a.UENNo.Contains(unitcode)
        //                  //|| a.UnitCode == unitcode
        //                  //a.UENNo.Contains(unitcode) || a.UnitCode == unitcode
        //                  //a.UnitCode == unitcode || a.UENNo.Contains(unitcode)

        //                  //&& a.ReceiptDate.AddHours(offset).Date >= DateFrom.Date
        //                  && a.CreatedUtc.AddHours(offset).Date < DateFrom.Date
        //                  select new
        //                  {
        //                      ReceiptDate = a.ReceiptDate,
        //                      CodeRequirment = d.CodeRequirment,
        //                      CustomsCategory = b.CustomsCategory,
        //                      ProductCode = b.ProductCode,
        //                      ProductName = b.ProductName,
        //                      ProductRemark = b.ProductRemark,
        //                      RO = b.RONo,
        //                      Uom = b.UomUnit,
        //                      Buyer = f.BuyerCode,
        //                      PlanPo = b.POSerialNumber,
        //                      NoArticle = f.Article,
        //                      QtyReceipt = b.ReceiptQuantity,
        //                      QtyCorrection = ty.POSerialNumber == null ? 0 : ty.Quantity,
        //                      QtyExpend = ww.POSerialNumber == null ? 0 : ww.Quantity,
        //                      PriceReceipt = b.PricePerDealUnit,
        //                      PriceCorrection = ty.POSerialNumber == null ? 0 : ty.PricePerDealUnit,
        //                      PriceExpend = ww.POSerialNumber == null ? 0 : ww.PricePerDealUnit,
        //                      POId = b.POId,
        //                      URNType = a.URNType,
        //                      UnitCode = a.UnitCode,
        //                      UENNo = a.UENNo,
        //                      UnitSenderCode = dd.UnitSenderCode == null ? "-" : dd.UnitSenderCode,
        //                      UnitRequestName = dd.UnitRequestName == null ? "-" : dd.UnitRequestName,
        //                      ExpenditureTo = dd.ExpenditureTo == null ? "-" : dd.ExpenditureTo,
        //                      PaymentMethod = epo.PaymentMethod == null ? "-" : epo.PaymentMethod,
        //                      a.IsDeleted
        //                  });

        //    var CobaPP = (from a in PPAwal
        //                      //where a.ReceiptDate.AddHours(offset).Date < DateFrom.Date && a.CodeRequirment == (String.IsNullOrWhiteSpace(ctg) ? a.CodeRequirment : ctg) && a.IsDeleted == false
        //                  where (!String.IsNullOrWhiteSpace(a.UENNo) ? a.UENNo.Contains((String.IsNullOrWhiteSpace(unitcode) ? a.UnitCode : unitcode)) || a.UnitCode == (String.IsNullOrWhiteSpace(unitcode) ? a.UnitCode : unitcode) : a.UnitCode == (String.IsNullOrWhiteSpace(unitcode) ? a.UnitCode : unitcode))
        //                  //a.UENNo.Contains((String.IsNullOrWhiteSpace(unitcode) ? a.UnitCode : unitcode)) || a.UnitCode == (String.IsNullOrWhiteSpace(unitcode) ? a.UnitCode : unitcode)
        //                  select a);

        //    var PPAkhir = (from a in dbContext.GarmentUnitReceiptNotes
        //                   join b in dbContext.GarmentUnitReceiptNoteItems on a.Id equals b.URNId
        //                   join d in dbContext.GarmentDeliveryOrderDetails on b.DODetailId equals d.Id
        //                   join f in dbContext.GarmentInternalPurchaseOrders on b.POId equals f.Id
        //                   //join f in SaldoAwal on b.POId equals f.POID
        //                   join e in dbContext.GarmentReceiptCorrectionItems on b.Id equals e.URNItemId into RC
        //                   from ty in RC.DefaultIfEmpty()

        //                   join x in dbContext.GarmentUnitDeliveryOrderItems on b.Id equals x.URNItemId into GUDI
        //                   from xx in GUDI.DefaultIfEmpty()
        //                   join y in dbContext.GarmentUnitDeliveryOrders on xx.UnitDOId equals y.Id into GUD
        //                   from yy in GUD.DefaultIfEmpty()

        //                   join c in dbContext.GarmentUnitExpenditureNoteItems on xx.Id equals c.UnitDOItemId into UE
        //                   from ww in UE.DefaultIfEmpty()
        //                   join r in dbContext.GarmentUnitExpenditureNotes on ww.UENId equals r.Id into UEN
        //                   from dd in UEN.DefaultIfEmpty()
        //                   join epoItem in dbContext.GarmentExternalPurchaseOrderItems on b.POId equals epoItem.POId into EP
        //                   from epoItem in EP.DefaultIfEmpty()
        //                   join epo in dbContext.GarmentExternalPurchaseOrders on epoItem.GarmentEPOId equals epo.Id into EPO
        //                   from epo in EPO.DefaultIfEmpty()

        //                   where d.CodeRequirment == (String.IsNullOrWhiteSpace(ctg) ? d.CodeRequirment : ctg)
        //                   && (!String.IsNullOrWhiteSpace(suppliertype) && !String.IsNullOrWhiteSpace(customstype) ? b.CustomsCategory == suppliertype + " " + customstype
        //                   : (!String.IsNullOrWhiteSpace(suppliertype) ? b.CustomsCategory.Contains(suppliertype)
        //                   : (!String.IsNullOrWhiteSpace(customstype) ? b.CustomsCategory.Substring(b.CustomsCategory.IndexOf(' ') + 1) == customstype
        //                   : true)))
        //                   && a.IsDeleted == false && b.IsDeleted == false

        //                   // 17-02-2021 : YOKA
        //                   //&& (!String.IsNullOrWhiteSpace(suppliertype) && !String.IsNullOrWhiteSpace(customstype) ? b.CustomsCategory == suppliertype + " " + customstype
        //                   //: (!String.IsNullOrWhiteSpace(suppliertype) ? b.CustomsCategory.Contains(suppliertype)
        //                   //: (!String.IsNullOrWhiteSpace(customstype) ? b.CustomsCategory.Substring(b.CustomsCategory.IndexOf(' ') + 1) == customstype
        //                   //: true)))
        //                   //: true))

        //                   //String.IsNullOrEmpty(a.UENNo) ? false : a.UENNo.Contains(unitcode)
        //                   //|| a.UnitCode == unitcode
        //                   //a.UnitCode == unitcode || a.UENNo.Contains(unitcode)
        //                   // a.UENNo.Contains(unitcode) || a.UnitCode == unitcode     /*String.IsNullOrEmpty(a.UENNo) ? true :*/ 
        //                   && a.CreatedUtc.AddHours(offset).Date >= DateFrom.Date
        //                   && a.CreatedUtc.AddHours(offset).Date <= DateTo.Date

        //                   select new
        //                   {
        //                       ReceiptDate = a.ReceiptDate,
        //                       CodeRequirment = d.CodeRequirment,
        //                       CustomsCategory = b.CustomsCategory,
        //                       ProductCode = b.ProductCode,
        //                       ProductName = b.ProductName,
        //                       ProductRemark = b.ProductRemark,
        //                       RO = b.RONo,
        //                       Uom = b.UomUnit,
        //                       Buyer = f.BuyerCode,
        //                       PlanPo = b.POSerialNumber,
        //                       NoArticle = f.Article,
        //                       QtyReceipt = b.ReceiptQuantity,
        //                       QtyCorrection = ty.POSerialNumber == null ? 0 : ty.Quantity,
        //                       QtyExpend = ww.POSerialNumber == null ? 0 : ww.Quantity,
        //                       PriceReceipt = b.PricePerDealUnit,
        //                       PriceCorrection = ty.POSerialNumber == null ? 0 : ty.PricePerDealUnit,
        //                       PriceExpend = ww.POSerialNumber == null ? 0 : ww.PricePerDealUnit,
        //                       POId = b.POId,
        //                       URNType = a.URNType,
        //                       UnitCode = a.UnitCode,
        //                       UENNo = a.UENNo,
        //                       UnitSenderCode = dd.UnitSenderCode == null ? "-" : dd.UnitSenderCode,
        //                       UnitRequestName = dd.UnitRequestName == null ? "-" : dd.UnitRequestName,
        //                       ExpenditureTo = dd.ExpenditureTo == null ? "-" : dd.ExpenditureTo,
        //                       PaymentMethod = epo.PaymentMethod == null ? "-" : epo.PaymentMethod,
        //                       a.IsDeleted
        //                   });

        //    var CobaPPAkhir = (from a in PPAkhir
        //                       where (!String.IsNullOrWhiteSpace(a.UENNo) ? a.UENNo.Contains((String.IsNullOrWhiteSpace(unitcode) ? a.UnitCode : unitcode)) || a.UnitCode == (String.IsNullOrWhiteSpace(unitcode) ? a.UnitCode : unitcode) : a.UnitCode == (String.IsNullOrWhiteSpace(unitcode) ? a.UnitCode : unitcode))
        //                       //where a.ReceiptDate.AddHours(offset).Date >= DateFrom.Date
        //                       //      && a.ReceiptDate.AddHours(offset).Date <= DateTo.Date
        //                       //      && a.CodeRequirment == (String.IsNullOrWhiteSpace(ctg) ? a.CodeRequirment : ctg)
        //                       //      && a.IsDeleted == false 
        //                       select a);

        //    var SaldoAwal = Enumerable.Empty<GarmentStockReportViewModel>().AsQueryable();
        //    var SaldoAkhir = Enumerable.Empty<GarmentStockReportViewModel>().AsQueryable();

        //    SaldoAwal = (from query in CobaPP
        //                 group query by new { query.ProductCode, query.ProductName, query.RO, query.PlanPo, query.POId, query.UnitCode, query.UnitSenderCode, query.UnitRequestName, query.CustomsCategory } into data
        //                 select new GarmentStockReportViewModel
        //                 {
        //                     ProductCode = data.Key.ProductCode,
        //                     RO = data.Key.RO,
        //                     PlanPo = data.FirstOrDefault().PlanPo,
        //                     CustomsCategory = data.FirstOrDefault().CustomsCategory,
        //                     NoArticle = data.FirstOrDefault().NoArticle,
        //                     ProductName = data.FirstOrDefault().ProductName,
        //                     ProductRemark = data.FirstOrDefault().ProductRemark,
        //                     Buyer = data.FirstOrDefault().Buyer,
        //                     BeginningBalanceQty = data.Sum(x => x.QtyReceipt) + Convert.ToDecimal(data.Sum(x => x.QtyCorrection)) - Convert.ToDecimal(data.Sum(x => x.QtyExpend)),
        //                     BeginningBalanceUom = data.FirstOrDefault().Uom,
        //                     ReceiptCorrectionQty = 0,
        //                     ReceiptQty = 0,
        //                     ReceiptUom = data.FirstOrDefault().Uom,
        //                     ExpendQty = 0,
        //                     ExpandUom = data.FirstOrDefault().Uom,
        //                     EndingBalanceQty = 0,
        //                     EndingUom = data.FirstOrDefault().Uom,
        //                     POId = data.FirstOrDefault().POId,
        //                     PaymentMethod = data.FirstOrDefault().PaymentMethod
        //                 });

        //    SaldoAkhir = (from query in CobaPPAkhir
        //                  group query by new { query.ProductCode, query.ProductName, query.RO, query.PlanPo, query.POId, query.UnitCode, query.UnitSenderCode, query.UnitRequestName, query.CustomsCategory } into data
        //                  select new GarmentStockReportViewModel
        //                  {
        //                      ProductCode = data.Key.ProductCode,
        //                      RO = data.Key.RO,
        //                      PlanPo = data.FirstOrDefault().PlanPo,
        //                      CustomsCategory = data.FirstOrDefault().CustomsCategory,
        //                      NoArticle = data.FirstOrDefault().NoArticle,
        //                      ProductName = data.FirstOrDefault().ProductName,
        //                      ProductRemark = data.FirstOrDefault().ProductRemark,
        //                      Buyer = data.FirstOrDefault().Buyer,
        //                      BeginningBalanceQty = 0,
        //                      BeginningBalanceUom = data.FirstOrDefault().Uom,
        //                      ReceiptCorrectionQty = 0,
        //                      ReceiptQty = data.Sum(x => x.QtyReceipt),
        //                      ReceiptUom = data.FirstOrDefault().Uom,
        //                      ExpendQty = data.Sum(x => x.QtyExpend),
        //                      ExpandUom = data.FirstOrDefault().Uom,
        //                      EndingBalanceQty = Convert.ToDecimal(Convert.ToDouble(data.Sum(x => x.QtyReceipt)) - data.Sum(x => x.QtyExpend)),
        //                      EndingUom = data.FirstOrDefault().Uom,
        //                      POId = data.FirstOrDefault().POId,
        //                      PaymentMethod = data.FirstOrDefault().PaymentMethod
        //                  });

        //    List<GarmentStockReportViewModel> Data1 = SaldoAwal.Concat(SaldoAkhir).ToList();

        //    var Data = (from query in Data1
        //                group query by new { query.POId, query.ProductCode, query.RO, query.CustomsCategory } into groupdata
        //                select new GarmentStockReportViewModel
        //                {
        //                    ProductCode = groupdata.FirstOrDefault().ProductCode == null ? "-" : groupdata.FirstOrDefault().ProductCode,
        //                    RO = groupdata.FirstOrDefault().RO == null ? "-" : groupdata.FirstOrDefault().RO,
        //                    PlanPo = groupdata.FirstOrDefault().PlanPo == null ? "-" : groupdata.FirstOrDefault().PlanPo,
        //                    CustomsCategory = groupdata.FirstOrDefault().CustomsCategory == null ? "-" : groupdata.FirstOrDefault().CustomsCategory,
        //                    NoArticle = groupdata.FirstOrDefault().NoArticle == null ? "-" : groupdata.FirstOrDefault().NoArticle,
        //                    ProductName = groupdata.FirstOrDefault().ProductName == null ? "-" : groupdata.FirstOrDefault().ProductName,
        //                    ProductRemark = groupdata.FirstOrDefault().ProductRemark,
        //                    Buyer = groupdata.FirstOrDefault().Buyer == null ? "-" : groupdata.FirstOrDefault().Buyer,
        //                    BeginningBalanceQty = groupdata.Sum(x => x.BeginningBalanceQty),
        //                    BeginningBalanceUom = groupdata.FirstOrDefault().BeginningBalanceUom,
        //                    ReceiptCorrectionQty = 0,
        //                    ReceiptQty = groupdata.Sum(x => x.ReceiptQty),
        //                    ReceiptUom = groupdata.FirstOrDefault().ReceiptUom,
        //                    ExpendQty = groupdata.Sum(x => x.ExpendQty),
        //                    ExpandUom = groupdata.FirstOrDefault().ExpandUom,
        //                    EndingBalanceQty = Convert.ToDecimal((groupdata.Sum(x => x.BeginningBalanceQty) + groupdata.Sum(x => x.ReceiptQty) + 0) - (Convert.ToDecimal(groupdata.Sum(x => x.ExpendQty)))),
        //                    EndingUom = groupdata.FirstOrDefault().EndingUom,
        //                    PaymentMethod = groupdata.FirstOrDefault().PaymentMethod
        //                });

        //    return Data.AsQueryable();
        //}


        //FUTURE
        //Need Adjustment
        //
        public List<GarmentStockReportViewModel> GetStockQuery(string ctg, string unitcode, DateTime? datefrom, DateTime? dateto, int offset, string suppliertype, string customstype)
        {
            DateTime DateFrom = datefrom == null ? new DateTime(1970, 1, 1) : (DateTime)datefrom;
            DateTime DateTo = dateto == null ? DateTime.Now : (DateTime)dateto;

            var categories = GetProductCodes(1, int.MaxValue, "{}", "{}");

            var categories1 = ctg == "BB" ? categories.Where(x => x.CodeRequirement == "BB").Select(x => x.Name).ToArray() : ctg == "BP" ? categories.Where(x => x.CodeRequirement == "BP").Select(x => x.Name).ToArray() : ctg == "BE" ? categories.Where(x => x.CodeRequirement == "BE").Select(x => x.Name).ToArray() : categories.Select(x => x.Name).ToArray();

            List<GarmentStockReportViewModel> penerimaan = new List<GarmentStockReportViewModel>();
            List<GarmentStockReportViewModel> pengeluaran = new List<GarmentStockReportViewModel>();
            List<GarmentStockReportViewModel> koreksi = new List<GarmentStockReportViewModel>();
            List<GarmentStockReportViewModel> penerimaanSA = new List<GarmentStockReportViewModel>();
            List<GarmentStockReportViewModel> pengeluaranSA = new List<GarmentStockReportViewModel>();
            List<GarmentStockReportViewModel> koreksiSA = new List<GarmentStockReportViewModel>();

            #region SaldoAwal
            var IdSATerima = (from a in dbContext.GarmentUnitReceiptNotes
                              join b in dbContext.GarmentUnitReceiptNoteItems on a.Id equals b.URNId
                              where categories1.Contains(b.ProductName)
                              && a.IsDeleted == false && b.IsDeleted == false
                              && a.CreatedUtc.AddHours(offset).Date < DateFrom.Date
                              && a.UnitCode == (string.IsNullOrWhiteSpace(unitcode) ? a.UnitCode : unitcode)
                              && (!String.IsNullOrWhiteSpace(suppliertype) && !String.IsNullOrWhiteSpace(customstype) ? b.CustomsCategory == suppliertype + " " + customstype
                              : (!String.IsNullOrWhiteSpace(suppliertype) ? b.CustomsCategory.Contains(suppliertype)
                              : (!String.IsNullOrWhiteSpace(customstype) ? b.CustomsCategory.Substring(b.CustomsCategory.IndexOf(' ') + 1) == customstype
                              : true)))
                              select new
                              {
                                  UrnId = a.Id,
                                  UrnItemId = b.Id,
                                  CustomCategory = b.CustomsCategory,
                                  a.UnitCode
                              }).ToList().Distinct();
            var sapenerimaanunitreceiptnoteids = IdSATerima.Select(x => x.UrnId).ToList();
            var sapenerimaanunitreceiptnotes = dbContext.GarmentUnitReceiptNotes.Where(x => sapenerimaanunitreceiptnoteids.Contains(x.Id)).Select(s => new { s.ReceiptDate, s.URNType, s.UnitCode, s.UENNo, s.Id }).ToList();
            var sapenerimaanunitreceiptnoteItemIds = IdSATerima.Select(x => x.UrnItemId).ToList();
            var sapenerimaanuntreceiptnoteItems = dbContext.GarmentUnitReceiptNoteItems.Where(x => sapenerimaanunitreceiptnoteItemIds.Contains(x.Id)).Select(s => new { s.ProductCode, s.ProductName, s.RONo, s.SmallUomUnit, s.POSerialNumber, s.CustomsCategory, s.ReceiptQuantity, s.DOCurrencyRate, s.PricePerDealUnit, s.Id, s.SmallQuantity, s.Conversion, s.ProductRemark, s.EPOItemId }).ToList();
            //var sapenerimaandeliveryorderdetailIds = IdSATerima.Select(x => x.DoDetailId).ToList();
            //var sapenerimaandeliveryorderdetails = dbContext.GarmentDeliveryOrderDetails.Where(x => sapenerimaandeliveryorderdetailIds.Contains(x.Id)).Select(s => new { s.CodeRequirment, s.Id, s.DOQuantity }).ToList();
            var sapenerimaanExternalPurchaseOrderItemIds = sapenerimaanuntreceiptnoteItems.Select(x => x.EPOItemId).ToList();
            var sapenerimaanExternalPurchaseOrderItems = dbContext.GarmentExternalPurchaseOrderItems.IgnoreQueryFilters().Where(x => sapenerimaanExternalPurchaseOrderItemIds.Contains(x.Id)).Select(s => new { s.GarmentEPOId, s.Id, s.PO_SerialNumber }).ToList();
            var sapenerimaanExternalPurchaseOrderIds = sapenerimaanExternalPurchaseOrderItems.Select(x => x.GarmentEPOId).ToList();
            var sapenerimaanExternalPurchaseOrders = dbContext.GarmentExternalPurchaseOrders.IgnoreQueryFilters().Where(x => sapenerimaanExternalPurchaseOrderIds.Contains(x.Id)).Select(s => new { s.Id, s.PaymentMethod }).ToList();
            var sapenerimaanpurchaserequestros = sapenerimaanuntreceiptnoteItems.Select(x => x.RONo).ToList();
            var sapenerimaanpurchaserequests = dbContext.GarmentPurchaseRequests.Where(x => sapenerimaanpurchaserequestros.Contains(x.RONo)).Select(x => new { x.BuyerCode, x.Article, x.RONo }).ToList();
            //var sapenerimaanintrenalpurchaseorders = dbContext.GarmentInternalPurchaseOrders.Where(x => sapenerimaanintrenalpurchaseorderIds.Contains(x.Id)).Select(s => new { s.BuyerCode, s.Article, s.Id }).ToList();
            foreach (var item in IdSATerima)
            {
                var sapenerimaanunitreceiptnote = sapenerimaanunitreceiptnotes.FirstOrDefault(x => x.Id == item.UrnId);
                var sapenerimaanuntreceiptnoteItem = sapenerimaanuntreceiptnoteItems.FirstOrDefault(x => x.Id == item.UrnItemId);
                //var sapenerimaandeliveryorderdetail = sapenerimaandeliveryorderdetails.FirstOrDefault(x => x.Id == item.DoDetailId);
                var sapenerimaanExternalPurchaseOrderitem = sapenerimaanExternalPurchaseOrderItems.FirstOrDefault(x => x.Id == sapenerimaanuntreceiptnoteItem.EPOItemId);
                var sapenerimaanExternalPurchaseOrder = sapenerimaanExternalPurchaseOrders.FirstOrDefault(x => x.Id == sapenerimaanExternalPurchaseOrderitem.GarmentEPOId);
                var sapenerimaanpurchaserequest = sapenerimaanpurchaserequests.FirstOrDefault(x => x.RONo == sapenerimaanuntreceiptnoteItem.RONo);

                penerimaanSA.Add(new GarmentStockReportViewModel
                {
                    BeginningBalanceQty = 0,
                    BeginningBalanceUom = sapenerimaanuntreceiptnoteItem.SmallUomUnit,
                    Buyer = sapenerimaanpurchaserequest == null ? "" : sapenerimaanpurchaserequest.BuyerCode,
                    EndingBalanceQty = 0,
                    EndingUom = sapenerimaanuntreceiptnoteItem.SmallUomUnit,
                    ExpandUom = sapenerimaanuntreceiptnoteItem.SmallUomUnit,
                    ExpendQty = 0,
                    NoArticle = sapenerimaanpurchaserequest == null ? "" : sapenerimaanpurchaserequest.Article,
                    PaymentMethod = sapenerimaanExternalPurchaseOrder == null ? "" : sapenerimaanExternalPurchaseOrder.PaymentMethod,
                    PlanPo = sapenerimaanuntreceiptnoteItem.POSerialNumber,
                    CustomsCategory = sapenerimaanuntreceiptnoteItem.CustomsCategory,
                    //POId = sapenerimaanintrenalpurchaseorder.Id,
                    ProductCode = sapenerimaanuntreceiptnoteItem.ProductCode,
                    ProductName = sapenerimaanuntreceiptnoteItem.ProductName,
                    ProductRemark = sapenerimaanuntreceiptnoteItem.ProductRemark,
                    ReceiptCorrectionQty = 0,
                    ReceiptQty = (decimal)sapenerimaanuntreceiptnoteItem.ReceiptQuantity * sapenerimaanuntreceiptnoteItem.Conversion,
                    ReceiptUom = sapenerimaanuntreceiptnoteItem.SmallUomUnit,
                    RO = sapenerimaanuntreceiptnoteItem.RONo,
                    UnitCode = "",
                    UnitSenderCode = "",
                    UnitRequestCode = ""
                });
            }

            var IdSAPengeluaran = (from a in dbContext.GarmentUnitExpenditureNotes
                                   join b in dbContext.GarmentUnitExpenditureNoteItems on a.Id equals b.UENId
                                   where categories1.Contains(b.ProductName)
                                   && a.IsDeleted == false && b.IsDeleted == false
                                   && a.CreatedUtc.AddHours(offset).Date < DateFrom.Date
                                   && a.UnitSenderCode == (string.IsNullOrWhiteSpace(unitcode) ? a.UnitSenderCode : unitcode)
                                   && (!String.IsNullOrWhiteSpace(suppliertype) && !String.IsNullOrWhiteSpace(customstype) ? b.CustomsCategory == suppliertype + " " + customstype
                                   : (!String.IsNullOrWhiteSpace(suppliertype) ? b.CustomsCategory.Contains(suppliertype)
                                   : (!String.IsNullOrWhiteSpace(customstype) ? b.CustomsCategory.Substring(b.CustomsCategory.IndexOf(' ') + 1) == customstype
                                   : true)))
                                   select new
                                   {
                                       UENId = a.Id,
                                       UENItemsId = b.Id,
                                       CustomsCategory = b.CustomsCategory
                                   }).ToList().Distinct();

            var sapengeluaranUnitExpenditureNoteItemIds = IdSAPengeluaran.Select(x => x.UENItemsId).ToList();
            var sapengeluaranUnitExpenditureNoteItems = dbContext.GarmentUnitExpenditureNoteItems.Where(x => sapengeluaranUnitExpenditureNoteItemIds.Contains(x.Id)).Select(s => new { s.UnitDOItemId, s.Quantity, s.PricePerDealUnit, s.Id, s.ProductCode, s.ProductName, s.RONo, s.POSerialNumber, s.CustomsCategory, s.UomUnit, s.DOCurrencyRate, s.ProductRemark, s.URNItemId, s.EPOItemId }).ToList();
            var sapengeluaranUnitExpenditureNoteIds = IdSAPengeluaran.Select(x => x.UENId).ToList();
            var sapengeluaranUnitExpenditureNotes = dbContext.GarmentUnitExpenditureNotes.Where(x => sapengeluaranUnitExpenditureNoteIds.Contains(x.Id)).Select(s => new { s.UnitSenderCode, s.UnitRequestName, s.ExpenditureTo, s.Id, s.UENNo, s.ExpenditureType }).ToList();
            var sapengeluaranunitdeliveryorderItemIds = sapengeluaranUnitExpenditureNoteItems.Select(x => x.UnitDOItemId);
            var sapengeluaranunitdeliveryorderitems = dbContext.GarmentUnitDeliveryOrderItems.Where(x => sapengeluaranunitdeliveryorderItemIds.Contains(x.Id)).Select(x => new { x.URNItemId, x.Id }).ToList();
            var sapengeluaranunitreceiptnoteitemslIds = sapengeluaranunitdeliveryorderitems.Select(x => x.URNItemId).ToList();
            var sapengeluaranunitreceiptnoteitems = dbContext.GarmentUnitReceiptNoteItems.IgnoreQueryFilters().Where(x => sapengeluaranunitreceiptnoteitemslIds.Contains(x.Id)).Select(x => new { x.EPOItemId, x.Id, x.CustomsCategory }).ToList();
            //var sapengeluarandeliveryorderdetails = dbContext.GarmentDeliveryOrderDetails.Where(x => sapengeluarandeliveryorderdetailIds.Contains(x.Id)).Select(s => new { s.CodeRequirment, s.Id, s.DOQuantity }).ToList();

            var sapengeluaranpurchaserequestros = sapengeluaranUnitExpenditureNoteItems.Select(x => x.RONo).ToList();
            var sapengeluaranpurchaserequests = dbContext.GarmentPurchaseRequests.Where(x => sapengeluaranpurchaserequestros.Contains(x.RONo)).Select(x => new { x.BuyerCode, x.RONo, x.Article }).ToList();
            var sapengeluaranexternalpurchaseorderitemros = sapengeluaranUnitExpenditureNoteItems.Select(x => x.RONo).ToList();
            var sapengeluaranexternalpurchaseorderitems = dbContext.GarmentExternalPurchaseOrderItems.IgnoreQueryFilters().Where(x => sapengeluaranexternalpurchaseorderitemros.Contains(x.RONo)).Select(s => new { s.GarmentEPOId, s.Id, s.RONo }).ToList();
            var sapengeluaranexternalpurchaseorderIds = sapengeluaranexternalpurchaseorderitems.Select(x => x.GarmentEPOId).ToList();
            var sapengeluaranexternalpurchaseorders = dbContext.GarmentExternalPurchaseOrders.IgnoreQueryFilters().Where(x => sapengeluaranexternalpurchaseorderIds.Contains(x.Id)).Select(x => new { x.PaymentMethod, x.Id }).ToList();

            foreach (var item in IdSAPengeluaran)
            {
                var sapengeluaranUnitExpenditureNoteItem = sapengeluaranUnitExpenditureNoteItems.FirstOrDefault(x => x.Id == item.UENItemsId);
                var sapengeluaranUnitExpenditureNote = sapengeluaranUnitExpenditureNotes.FirstOrDefault(x => x.Id == item.UENId);
                var sapengeluaranunitdeliveryorderitem = sapengeluaranunitdeliveryorderitems.FirstOrDefault(x => x.Id == sapengeluaranUnitExpenditureNoteItem.UnitDOItemId);
                var sapengeluaranunitreceiptnoteitem = sapengeluaranunitreceiptnoteitems.FirstOrDefault(x => x.Id == sapengeluaranunitdeliveryorderitem.URNItemId);
                var sapengeluaranexternalpurchaseorderitem = sapengeluaranexternalpurchaseorderitems.FirstOrDefault(x => x.RONo == sapengeluaranUnitExpenditureNoteItem.RONo);
                var sapengeluaranexternalpurchaseorder = sapengeluaranexternalpurchaseorders.FirstOrDefault(x => x.Id == sapengeluaranexternalpurchaseorderitem.GarmentEPOId);
                var sapengeluaranpurchaserequest = sapengeluaranpurchaserequests.FirstOrDefault(x => x.RONo == sapengeluaranUnitExpenditureNoteItem.RONo);

                pengeluaranSA.Add(new GarmentStockReportViewModel
                {
                    BeginningBalanceQty = 0,
                    BeginningBalanceUom = sapengeluaranUnitExpenditureNoteItem.UomUnit,
                    Buyer = sapengeluaranpurchaserequest == null ? "" : sapengeluaranpurchaserequest.BuyerCode,
                    EndingBalanceQty = 0,
                    EndingUom = sapengeluaranUnitExpenditureNoteItem.UomUnit,
                    ExpandUom = sapengeluaranUnitExpenditureNoteItem.UomUnit,
                    ExpendQty = sapengeluaranUnitExpenditureNoteItem.Quantity,
                    NoArticle = sapengeluaranpurchaserequest == null ? "" : sapengeluaranpurchaserequest.Article,
                    PaymentMethod = sapengeluaranexternalpurchaseorder == null ? "" : sapengeluaranexternalpurchaseorder.PaymentMethod,
                    PlanPo = sapengeluaranUnitExpenditureNoteItem.POSerialNumber,
                    CustomsCategory = sapengeluaranUnitExpenditureNoteItem.CustomsCategory,
                    ProductCode = sapengeluaranUnitExpenditureNoteItem.ProductCode,
                    ProductName = sapengeluaranUnitExpenditureNoteItem.ProductName,
                    ProductRemark = sapengeluaranUnitExpenditureNoteItem.ProductRemark,
                    ReceiptCorrectionQty = 0,
                    ReceiptQty = 0,
                    ReceiptUom = sapengeluaranUnitExpenditureNoteItem.UomUnit,
                    RO = sapengeluaranUnitExpenditureNoteItem.RONo,
                    UnitCode = "",
                    UnitSenderCode = "",
                    UnitRequestCode = ""
                });
            }

            var IdSAKoreksi = (from a in dbContext.GarmentUnitReceiptNotes
                               join b in dbContext.GarmentUnitReceiptNoteItems on a.Id equals b.URNId
                               join c in dbContext.GarmentReceiptCorrectionItems on b.Id equals c.URNItemId
                               where categories1.Contains(b.ProductName)
                               && a.IsDeleted == false && b.IsDeleted == false
                               && a.CreatedUtc.AddHours(offset).Date < DateFrom.Date
                               && a.UnitCode == (string.IsNullOrWhiteSpace(unitcode) ? a.UnitCode : unitcode)
                               && (!String.IsNullOrWhiteSpace(suppliertype) && !String.IsNullOrWhiteSpace(customstype) ? b.CustomsCategory == suppliertype + " " + customstype
                               : (!String.IsNullOrWhiteSpace(suppliertype) ? b.CustomsCategory.Contains(suppliertype)
                               : (!String.IsNullOrWhiteSpace(customstype) ? b.CustomsCategory.Substring(b.CustomsCategory.IndexOf(' ') + 1) == customstype
                               : true)))
                               select new
                               {
                                   UrnId = a.Id,
                                   UrnItemId = b.Id,
                                   RCorrItemId = c.Id,
                                   CustomsCategory = b.CustomsCategory
                               }).ToList().Distinct();

            var sakoreksiunitreceiptnoteids = IdSAKoreksi.Select(x => x.UrnId).ToList();
            var sakoreksiunitreceiptnotes = dbContext.GarmentUnitReceiptNotes.Where(x => sakoreksiunitreceiptnoteids.Contains(x.Id)).Select(s => new { s.ReceiptDate, s.URNType, s.UnitCode, s.UENNo, s.Id }).ToList();
            var sakoreksiunitreceiptnoteItemIds = IdSATerima.Select(x => x.UrnItemId).ToList();
            var sakoreksiuntreceiptnoteItems = dbContext.GarmentUnitReceiptNoteItems.Where(x => sakoreksiunitreceiptnoteItemIds.Contains(x.Id)).Select(s => new { s.ProductCode, s.ProductName, s.RONo, s.SmallUomUnit, s.POSerialNumber, s.CustomsCategory, s.ReceiptQuantity, s.DOCurrencyRate, s.PricePerDealUnit, s.Id, s.SmallQuantity, s.Conversion, s.ProductRemark, s.EPOItemId }).ToList();
            //var sapenerimaandeliveryorderdetailIds = IdSATerima.Select(x => x.DoDetailId).ToList();
            //var sapenerimaandeliveryorderdetails = dbContext.GarmentDeliveryOrderDetails.Where(x => sapenerimaandeliveryorderdetailIds.Contains(x.Id)).Select(s => new { s.CodeRequirment, s.Id, s.DOQuantity }).ToList();
            var sakoreksiExternalPurchaseOrderItemIds = sakoreksiuntreceiptnoteItems.Select(x => x.EPOItemId).ToList();
            var sapkoreksiExternalPurchaseOrderItems = dbContext.GarmentExternalPurchaseOrderItems.IgnoreQueryFilters().Where(x => sakoreksiExternalPurchaseOrderItemIds.Contains(x.Id)).Select(s => new { s.GarmentEPOId, s.Id, s.PO_SerialNumber }).ToList();
            var sakoreksiExternalPurchaseOrderIds = sapkoreksiExternalPurchaseOrderItems.Select(x => x.GarmentEPOId).ToList();
            var sakoreksiExternalPurchaseOrders = dbContext.GarmentExternalPurchaseOrders.IgnoreQueryFilters().Where(x => sakoreksiExternalPurchaseOrderIds.Contains(x.Id)).Select(s => new { s.Id, s.PaymentMethod }).ToList();
            var sakoreksipurchaserequestros = sakoreksiuntreceiptnoteItems.Select(x => x.RONo).ToList();
            var sakoreksipurchaserequests = dbContext.GarmentPurchaseRequests.Where(x => sakoreksipurchaserequestros.Contains(x.RONo)).Select(x => new { x.BuyerCode, x.Article, x.RONo }).ToList();
            var sakoreksireceiptcorritemids = IdSAKoreksi.Select(x => x.RCorrItemId).ToList();
            var sakoreksireceiptcorritems = dbContext.GarmentReceiptCorrectionItems.Where(x => sakoreksireceiptcorritemids.Contains(x.Id)).Select(x => new { x.Id, x.SmallQuantity, x.Conversion, x.Quantity, x.PricePerDealUnit });
            //var sapenerimaanintrenalpurchaseorders = dbContext.GarmentInternalPurchaseOrders.Where(x => sapenerimaanintrenalpurchaseorderIds.Contains(x.Id)).Select(s => new { s.BuyerCode, s.Article, s.Id }).ToList();

            foreach (var item in IdSAKoreksi)
            {
                var sakoreksiunitreceiptnote = sakoreksiunitreceiptnotes.FirstOrDefault(x => x.Id == item.UrnId);
                var sakoreksiuntreceiptnoteItem = sakoreksiuntreceiptnoteItems.FirstOrDefault(x => x.Id == item.UrnItemId);
                //var sapenerimaandeliveryorderdetail = sapenerimaandeliveryorderdetails.FirstOrDefault(x => x.Id == item.DoDetailId);
                var sakoreksiExternalPurchaseOrderitem = sapkoreksiExternalPurchaseOrderItems.FirstOrDefault(x => x.Id == sakoreksiuntreceiptnoteItem.EPOItemId);
                var sakoreksiExternalPurchaseOrder = sakoreksiExternalPurchaseOrders.FirstOrDefault(x => x.Id == sakoreksiExternalPurchaseOrderitem.GarmentEPOId);
                var sakoreksipurchaserequest = sakoreksipurchaserequests.FirstOrDefault(x => x.RONo == sakoreksiuntreceiptnoteItem.RONo);
                var sakoreksireceiptcorritem = sakoreksireceiptcorritems.FirstOrDefault(x => x.Id == item.RCorrItemId);

                koreksiSA.Add(new GarmentStockReportViewModel
                {
                    BeginningBalanceQty = 0,
                    BeginningBalanceUom = sakoreksiuntreceiptnoteItem.SmallUomUnit,
                    Buyer = sakoreksipurchaserequest == null ? "" : sakoreksipurchaserequest.BuyerCode,
                    EndingBalanceQty = 0,
                    EndingUom = sakoreksiuntreceiptnoteItem.SmallUomUnit,
                    ExpandUom = sakoreksiuntreceiptnoteItem.SmallUomUnit,
                    ExpendQty = 0,
                    NoArticle = sakoreksipurchaserequest == null ? "" : sakoreksipurchaserequest.Article,
                    PaymentMethod = sakoreksiExternalPurchaseOrder == null ? "" : sakoreksiExternalPurchaseOrder.PaymentMethod,
                    PlanPo = sakoreksiuntreceiptnoteItem.POSerialNumber,
                    CustomsCategory = sakoreksiuntreceiptnoteItem.CustomsCategory,
                    //POId = sapenerimaanintrenalpurchaseorder.Id,
                    ProductCode = sakoreksiuntreceiptnoteItem.ProductCode,
                    ProductName = sakoreksiuntreceiptnoteItem.ProductName,
                    ProductRemark = sakoreksiuntreceiptnoteItem.ProductRemark,
                    ReceiptCorrectionQty = 0,
                    ReceiptQty = sakoreksireceiptcorritem == null ? 0 : (decimal)sakoreksireceiptcorritem.SmallQuantity,
                    ReceiptUom = sakoreksiuntreceiptnoteItem.SmallUomUnit,
                    RO = sakoreksiuntreceiptnoteItem.RONo,
                    UnitCode = "",
                    UnitSenderCode = "",
                    UnitRequestCode = ""
                });
            }

            var SAwal = penerimaanSA.Concat(pengeluaranSA).Concat(koreksiSA).ToList();

            var SaldoAwal = (from a in SAwal
                             group a by new { a.ProductCode, a.PlanPo, a.CustomsCategory } into data
                             select new GarmentStockReportViewModel
                             {
                                 BeginningBalanceQty = Math.Round(data.Sum(x => x.BeginningBalanceQty) + data.Sum(x => x.ReceiptQty) + data.Sum(x => x.ReceiptCorrectionQty) - (decimal)data.Sum(x => x.ExpendQty), 2),
                                 BeginningBalanceUom = data.FirstOrDefault().BeginningBalanceUom,
                                 Buyer = data.FirstOrDefault().Buyer,
                                 EndingBalanceQty = 0,
                                 EndingUom = data.FirstOrDefault().EndingUom,
                                 ExpandUom = data.FirstOrDefault().ExpandUom,
                                 ExpendQty = 0,
                                 NoArticle = data.FirstOrDefault().NoArticle,
                                 PaymentMethod = data.FirstOrDefault().PaymentMethod,
                                 PlanPo = data.FirstOrDefault().PlanPo,
                                 CustomsCategory = data.FirstOrDefault().CustomsCategory,
                                 POId = data.FirstOrDefault().POId,
                                 ProductCode = data.FirstOrDefault().ProductCode,
                                 ProductName = data.FirstOrDefault().ProductName,
                                 ProductRemark = data.FirstOrDefault().ProductRemark,
                                 ReceiptCorrectionQty = 0,
                                 ReceiptQty = 0,
                                 ReceiptUom = data.FirstOrDefault().ReceiptUom,
                                 RO = data.FirstOrDefault().RO,
                                 UnitCode = data.FirstOrDefault().UnitCode,
                                 UnitSenderCode = data.FirstOrDefault().UnitSenderCode,
                                 UnitRequestCode = data.FirstOrDefault().UnitRequestCode
                             }).ToList();
            #endregion
            #region Now
            var IdTerima = (from a in dbContext.GarmentUnitReceiptNotes
                            join b in dbContext.GarmentUnitReceiptNoteItems on a.Id equals b.URNId
                            where categories1.Contains(b.ProductName)
                            && a.IsDeleted == false && b.IsDeleted == false
                            && a.CreatedUtc.AddHours(offset).Date >= DateFrom.Date
                            && a.CreatedUtc.AddHours(offset).Date <= DateTo.Date
                            && a.UnitCode == (string.IsNullOrWhiteSpace(unitcode) ? a.UnitCode : unitcode)
                            && (!String.IsNullOrWhiteSpace(suppliertype) && !String.IsNullOrWhiteSpace(customstype) ? b.CustomsCategory == suppliertype + " " + customstype
                            : (!String.IsNullOrWhiteSpace(suppliertype) ? b.CustomsCategory.Contains(suppliertype)
                            : (!String.IsNullOrWhiteSpace(customstype) ? b.CustomsCategory.Substring(b.CustomsCategory.IndexOf(' ') + 1) == customstype
                            : true)))
                            select new
                            {
                                UrnId = a.Id,
                                UrnItemId = b.Id,
                                //UENNo = dd == null ? "-" : dd.UENNo,
                                CustomCategory = b.CustomsCategory,
                                a.UnitCode
                            }).ToList().Distinct();

            var penerimaanunitreceiptnoteids = IdTerima.Select(x => x.UrnId).ToList();
            var penerimaanunitreceiptnotes = dbContext.GarmentUnitReceiptNotes.Where(x => penerimaanunitreceiptnoteids.Contains(x.Id)).Select(s => new { s.ReceiptDate, s.URNType, s.UnitCode, s.UENNo, s.Id }).ToList();
            var penerimaanunitreceiptnoteItemIds = IdTerima.Select(x => x.UrnItemId).ToList();
            var penerimaanuntreceiptnoteItems = dbContext.GarmentUnitReceiptNoteItems.Where(x => penerimaanunitreceiptnoteItemIds.Contains(x.Id)).Select(s => new { s.EPOItemId, s.ProductCode, s.ProductName, s.RONo, s.SmallUomUnit, s.POSerialNumber, s.CustomsCategory, s.ReceiptQuantity, s.DOCurrencyRate, s.PricePerDealUnit, s.Id, s.SmallQuantity, s.Conversion, s.ProductRemark }).ToList();
            var penerimaanExternalPurchaseOrderItemIds = penerimaanuntreceiptnoteItems.Select(x => x.EPOItemId).ToList();
            var penerimaanExternalPurchaseOrderItems = dbContext.GarmentExternalPurchaseOrderItems.IgnoreQueryFilters().Where(x => penerimaanExternalPurchaseOrderItemIds.Contains(x.Id)).Select(s => new { s.GarmentEPOId, s.Id, s.PO_SerialNumber }).ToList();
            var penerimaanExternalPurchaseOrderIds = penerimaanExternalPurchaseOrderItems.Select(x => x.GarmentEPOId).ToList();
            var penerimaanExternalPurchaseOrders = dbContext.GarmentExternalPurchaseOrders.IgnoreQueryFilters().Where(x => penerimaanExternalPurchaseOrderIds.Contains(x.Id)).Select(s => new { s.Id, s.PaymentMethod }).ToList();
            var penerimaanpurchaserequestros = penerimaanuntreceiptnoteItems.Select(x => x.RONo).ToList();
            var penerimaanpurchaserequests = dbContext.GarmentPurchaseRequests.Where(x => penerimaanpurchaserequestros.Contains(x.RONo)).Select(x => new { x.BuyerCode, x.Article, x.RONo }).ToList();

            foreach (var item in IdTerima)
            {
                var penerimaanunitreceiptnote = penerimaanunitreceiptnotes.FirstOrDefault(x => x.Id == item.UrnId);
                var penerimaanuntreceiptnoteItem = penerimaanuntreceiptnoteItems.FirstOrDefault(x => x.Id == item.UrnItemId);
                var penerimaanExternalPurchaseOrderitem = penerimaanExternalPurchaseOrderItems.FirstOrDefault(x => x.Id == penerimaanuntreceiptnoteItem.EPOItemId);
                var penerimaanExternalPurchaseOrder = penerimaanExternalPurchaseOrders.FirstOrDefault(x => x.Id == penerimaanExternalPurchaseOrderitem.GarmentEPOId);
                var penerimaanpurchaserequest = penerimaanpurchaserequests.FirstOrDefault(x => x.RONo == penerimaanuntreceiptnoteItem.RONo);

                penerimaan.Add(new GarmentStockReportViewModel
                {
                    BeginningBalanceQty = 0,
                    BeginningBalanceUom = penerimaanuntreceiptnoteItem.SmallUomUnit,
                    Buyer = penerimaanpurchaserequest == null ? "" : penerimaanpurchaserequest.BuyerCode,
                    EndingBalanceQty = 0,
                    EndingUom = penerimaanuntreceiptnoteItem.SmallUomUnit,
                    ExpandUom = penerimaanuntreceiptnoteItem.SmallUomUnit,
                    ExpendQty = 0,
                    NoArticle = penerimaanpurchaserequest == null ? "" : penerimaanpurchaserequest.Article,
                    PaymentMethod = penerimaanExternalPurchaseOrder == null ? "" : penerimaanExternalPurchaseOrder.PaymentMethod,
                    PlanPo = penerimaanuntreceiptnoteItem.POSerialNumber,
                    CustomsCategory = penerimaanuntreceiptnoteItem.CustomsCategory,
                    ProductCode = penerimaanuntreceiptnoteItem.ProductCode,
                    ProductName = penerimaanuntreceiptnoteItem.ProductName,
                    ProductRemark = penerimaanuntreceiptnoteItem.ProductRemark,
                    ReceiptCorrectionQty = 0,
                    ReceiptQty = (decimal)penerimaanuntreceiptnoteItem.ReceiptQuantity * penerimaanuntreceiptnoteItem.Conversion,
                    ReceiptUom = penerimaanuntreceiptnoteItem.SmallUomUnit,
                    RO = penerimaanuntreceiptnoteItem.RONo,
                    UnitCode = "",
                    UnitSenderCode = "",
                    UnitRequestCode = ""
                });
            }

            var IdPengeluaran = (from a in dbContext.GarmentUnitExpenditureNotes
                                 join b in dbContext.GarmentUnitExpenditureNoteItems on a.Id equals b.UENId
                                 where categories1.Contains(b.ProductName)
                                 && a.CreatedUtc.Date >= DateFrom.Date
                                 && a.CreatedUtc.Date <= DateTo.Date
                                 && a.UnitSenderCode == (string.IsNullOrWhiteSpace(unitcode) ? a.UnitSenderCode : unitcode)
                                 && (!String.IsNullOrWhiteSpace(suppliertype) && !String.IsNullOrWhiteSpace(customstype) ? b.CustomsCategory == suppliertype + " " + customstype
                                 : (!String.IsNullOrWhiteSpace(suppliertype) ? b.CustomsCategory.Contains(suppliertype)
                                 : (!String.IsNullOrWhiteSpace(customstype) ? b.CustomsCategory.Substring(b.CustomsCategory.IndexOf(' ') + 1) == customstype
                                 : true)))
                                 && a.IsDeleted == false && b.IsDeleted == false
                                 select new
                                 {
                                     UENId = a.Id,
                                     UENItemsId = b.Id,
                                     CustomsCategory = b.CustomsCategory,
                                 }).ToList().Distinct();

            var pengeluaranUnitExpenditureNoteItemIds = IdPengeluaran.Select(x => x.UENItemsId).ToList();
            var pengeluaranUnitExpenditureNoteItems = dbContext.GarmentUnitExpenditureNoteItems.Where(x => pengeluaranUnitExpenditureNoteItemIds.Contains(x.Id)).Select(s => new { s.UnitDOItemId, s.Quantity, s.PricePerDealUnit, s.Id, s.ProductCode, s.ProductName, s.RONo, s.POSerialNumber, s.CustomsCategory, s.UomUnit, s.DOCurrencyRate, s.URNItemId, s.ProductRemark }).ToList();
            var pengeluaranUnitExpenditureNoteIds = IdPengeluaran.Select(x => x.UENId).ToList();
            var pengeluaranUnitExpenditureNotes = dbContext.GarmentUnitExpenditureNotes.Where(x => pengeluaranUnitExpenditureNoteIds.Contains(x.Id)).Select(s => new { s.UnitSenderCode, s.UnitRequestName, s.ExpenditureTo, s.Id, s.UENNo }).ToList();
            var pengeluaranunitdeliveryorderItemIds = pengeluaranUnitExpenditureNoteItems.Select(x => x.UnitDOItemId);
            var pengeluaranunitdeliveryorderitems = dbContext.GarmentUnitDeliveryOrderItems.Where(x => pengeluaranunitdeliveryorderItemIds.Contains(x.Id)).Select(x => new { x.URNItemId, x.Id }).ToList();
            var pengeluaranunitreceiptnoteitemslIds = pengeluaranunitdeliveryorderitems.Select(x => x.URNItemId).ToList();
            var pengeluaranunitreceiptnoteitems = dbContext.GarmentUnitReceiptNoteItems.IgnoreQueryFilters().Where(x => pengeluaranunitreceiptnoteitemslIds.Contains(x.Id)).Select(x => new { x.EPOItemId, x.Id, x.CustomsCategory }).ToList();
            //var sapengeluarandeliveryorderdetails = dbContext.GarmentDeliveryOrderDetails.Where(x => sapengeluarandeliveryorderdetailIds.Contains(x.Id)).Select(s => new { s.CodeRequirment, s.Id, s.DOQuantity }).ToList();
            var pengeluaranexternalpurchaseorderitemIds = pengeluaranunitreceiptnoteitems.Select(x => x.EPOItemId).ToList();
            var pengeluaranexternalpurchaseorderitems = dbContext.GarmentExternalPurchaseOrderItems.IgnoreQueryFilters().Where(x => pengeluaranexternalpurchaseorderitemIds.Contains(x.Id)).Select(s => new { s.GarmentEPOId, s.Id }).ToList();
            var pengeluaranexternalpurchaseorderIds = pengeluaranexternalpurchaseorderitems.Select(x => x.GarmentEPOId).ToList();
            var pengeluaranexternalpurchaseorders = dbContext.GarmentExternalPurchaseOrders.IgnoreQueryFilters().Where(x => pengeluaranexternalpurchaseorderIds.Contains(x.Id)).Select(x => new { x.PaymentMethod, x.Id }).ToList();
            var pengeluaranpurchaserequestros = pengeluaranUnitExpenditureNoteItems.Select(x => x.RONo).ToList();
            var pengeluaranpurchaserequests = dbContext.GarmentPurchaseRequests.Where(x => pengeluaranpurchaserequestros.Contains(x.RONo)).Select(x => new { x.BuyerCode, x.RONo, x.Article }).ToList();

            foreach (var item in IdPengeluaran)
            {
                var pengeluaranUnitExpenditureNoteItem = pengeluaranUnitExpenditureNoteItems.FirstOrDefault(x => x.Id == item.UENItemsId);
                var pengeluaranUnitExpenditureNote = pengeluaranUnitExpenditureNotes.FirstOrDefault(x => x.Id == item.UENId);
                var pengeluaranunitdeliveryorderitem = pengeluaranunitdeliveryorderitems.FirstOrDefault(x => x.Id == pengeluaranUnitExpenditureNoteItem.UnitDOItemId);
                var pengeluaranunitreceiptnoteitem = pengeluaranunitreceiptnoteitems.FirstOrDefault(x => x.Id == pengeluaranunitdeliveryorderitem.URNItemId);
                var pengeluaranexternalpurchaseorderitem = pengeluaranexternalpurchaseorderitems.FirstOrDefault(x => x.Id == pengeluaranunitreceiptnoteitem.EPOItemId);
                var pengeluaranexternalpurchaseorder = pengeluaranexternalpurchaseorders.FirstOrDefault(x => x.Id == pengeluaranexternalpurchaseorderitem.GarmentEPOId);
                var pengeluaranpurchaserequest = pengeluaranpurchaserequests.FirstOrDefault(x => x.RONo == pengeluaranUnitExpenditureNoteItem.RONo);

                pengeluaran.Add(new GarmentStockReportViewModel
                {
                    BeginningBalanceQty = 0,
                    BeginningBalanceUom = pengeluaranUnitExpenditureNoteItem.UomUnit,
                    Buyer = pengeluaranpurchaserequest == null ? "" : pengeluaranpurchaserequest.BuyerCode,
                    EndingBalanceQty = 0,
                    EndingUom = pengeluaranUnitExpenditureNoteItem.UomUnit,
                    ExpandUom = pengeluaranUnitExpenditureNoteItem.UomUnit,
                    ExpendQty = pengeluaranUnitExpenditureNoteItem.Quantity,
                    NoArticle = pengeluaranpurchaserequest == null ? "" : pengeluaranpurchaserequest.Article,
                    PaymentMethod = pengeluaranexternalpurchaseorder == null ? "" : pengeluaranexternalpurchaseorder.PaymentMethod,
                    PlanPo = pengeluaranUnitExpenditureNoteItem.POSerialNumber,
                    CustomsCategory = pengeluaranUnitExpenditureNoteItem.CustomsCategory,
                    ProductCode = pengeluaranUnitExpenditureNoteItem.ProductCode,
                    ProductName = pengeluaranUnitExpenditureNoteItem.ProductName,
                    ProductRemark = pengeluaranUnitExpenditureNoteItem.ProductRemark,
                    ReceiptCorrectionQty = 0,
                    ReceiptQty = 0,
                    ReceiptUom = pengeluaranUnitExpenditureNoteItem.UomUnit,
                    RO = pengeluaranUnitExpenditureNoteItem.RONo,
                    UnitCode = "",
                    UnitSenderCode = "",
                    UnitRequestCode = ""
                });

            }

            var IdKoreksi = (from a in dbContext.GarmentUnitReceiptNotes
                             join b in dbContext.GarmentUnitReceiptNoteItems on a.Id equals b.URNId
                             join c in dbContext.GarmentReceiptCorrectionItems on b.Id equals c.URNItemId
                             where categories1.Contains(b.ProductName)
                             && a.IsDeleted == false && b.IsDeleted == false
                             && a.CreatedUtc.AddHours(offset).Date >= DateFrom.Date
                             && a.CreatedUtc.AddHours(offset).Date <= DateTo.Date
                             && a.UnitCode == (string.IsNullOrWhiteSpace(unitcode) ? a.UnitCode : unitcode)
                             && (!String.IsNullOrWhiteSpace(suppliertype) && !String.IsNullOrWhiteSpace(customstype) ? b.CustomsCategory == suppliertype + " " + customstype
                             : (!String.IsNullOrWhiteSpace(suppliertype) ? b.CustomsCategory.Contains(suppliertype)
                             : (!String.IsNullOrWhiteSpace(customstype) ? b.CustomsCategory.Substring(b.CustomsCategory.IndexOf(' ') + 1) == customstype
                             : true)))
                             select new
                             {
                                 UrnId = a.Id,
                                 UrnItemId = b.Id,
                                 CustomsCategory = b.CustomsCategory,
                                 RCorrItemId = c.Id
                             }).ToList().Distinct();

            var koreksiunitreceiptnoteids = IdKoreksi.Select(x => x.UrnId).ToList();
            var koreksiunitreceiptnotes = dbContext.GarmentUnitReceiptNotes.Where(x => koreksiunitreceiptnoteids.Contains(x.Id)).Select(s => new { s.ReceiptDate, s.URNType, s.UnitCode, s.UENNo, s.Id }).ToList();
            var koreksiunitreceiptnoteItemIds = IdKoreksi.Select(x => x.UrnItemId).ToList();
            var koreksiuntreceiptnoteItems = dbContext.GarmentUnitReceiptNoteItems.Where(x => koreksiunitreceiptnoteItemIds.Contains(x.Id)).Select(s => new { s.ProductCode, s.ProductName, s.RONo, s.SmallUomUnit, s.POSerialNumber,s.CustomsCategory, s.ReceiptQuantity, s.DOCurrencyRate, s.PricePerDealUnit, s.Id, s.SmallQuantity, s.Conversion, s.ProductRemark, s.EPOItemId }).ToList();
            //var sapenerimaandeliveryorderdetailIds = IdSATerima.Select(x => x.DoDetailId).ToList();
            //var sapenerimaandeliveryorderdetails = dbContext.GarmentDeliveryOrderDetails.Where(x => sapenerimaandeliveryorderdetailIds.Contains(x.Id)).Select(s => new { s.CodeRequirment, s.Id, s.DOQuantity }).ToList();
            var koreksiExternalPurchaseOrderItemIds = koreksiuntreceiptnoteItems.Select(x => x.EPOItemId).ToList();
            var koreksiExternalPurchaseOrderItems = dbContext.GarmentExternalPurchaseOrderItems.IgnoreQueryFilters().Where(x => koreksiExternalPurchaseOrderItemIds.Contains(x.Id)).Select(s => new { s.GarmentEPOId, s.Id, s.PO_SerialNumber }).ToList();
            var koreksiExternalPurchaseOrderIds = koreksiExternalPurchaseOrderItems.Select(x => x.GarmentEPOId).ToList();
            var koreksiExternalPurchaseOrders = dbContext.GarmentExternalPurchaseOrders.IgnoreQueryFilters().Where(x => koreksiExternalPurchaseOrderIds.Contains(x.Id)).Select(s => new { s.Id, s.PaymentMethod }).ToList();
            var koreksipurchaserequestros = koreksiuntreceiptnoteItems.Select(x => x.RONo).ToList();
            var koreksipurchaserequests = dbContext.GarmentPurchaseRequests.Where(x => koreksipurchaserequestros.Contains(x.RONo)).Select(x => new { x.BuyerCode, x.Article, x.RONo }).ToList();
            var koreksireceiptcorritemids = IdKoreksi.Select(x => x.RCorrItemId).ToList();
            var koreksireceiptcorritems = dbContext.GarmentReceiptCorrectionItems.Where(x => koreksireceiptcorritemids.Contains(x.Id)).Select(x => new { x.Id, x.SmallQuantity, x.Conversion, x.Quantity, x.PricePerDealUnit });
            //var sapenerimaanintrenalpurchaseorders = dbContext.GarmentInternalPurchaseOrders.Where(x => sapenerimaanintrenalpurchaseorderIds.Contains(x.Id)).Select(s => new { s.BuyerCode, s.Article, s.Id }).ToList();

            foreach (var item in IdKoreksi)
            {
                var koreksiunitreceiptnote = koreksiunitreceiptnotes.FirstOrDefault(x => x.Id == item.UrnId);
                var koreksiuntreceiptnoteItem = koreksiuntreceiptnoteItems.FirstOrDefault(x => x.Id == item.UrnItemId);
                //var sapenerimaandeliveryorderdetail = sapenerimaandeliveryorderdetails.FirstOrDefault(x => x.Id == item.DoDetailId);
                var koreksiExternalPurchaseOrderitem = koreksiExternalPurchaseOrderItems.FirstOrDefault(x => x.Id == koreksiuntreceiptnoteItem.EPOItemId);
                var koreksiExternalPurchaseOrder = koreksiExternalPurchaseOrders.FirstOrDefault(x => x.Id == koreksiExternalPurchaseOrderitem.GarmentEPOId);
                var koreksipurchaserequest = koreksipurchaserequests.FirstOrDefault(x => x.RONo == koreksiuntreceiptnoteItem.RONo);
                var koreksireceiptcorritem = koreksireceiptcorritems.FirstOrDefault(x => x.Id == item.RCorrItemId);

                koreksiSA.Add(new GarmentStockReportViewModel
                {
                    BeginningBalanceQty = 0,
                    BeginningBalanceUom = koreksiuntreceiptnoteItem.SmallUomUnit,
                    Buyer = koreksipurchaserequest == null ? "" : koreksipurchaserequest.BuyerCode,
                    EndingBalanceQty = 0,
                    EndingUom = koreksiuntreceiptnoteItem.SmallUomUnit,
                    ExpandUom = koreksiuntreceiptnoteItem.SmallUomUnit,
                    ExpendQty = 0,
                    NoArticle = koreksipurchaserequest == null ? "" : koreksipurchaserequest.Article,
                    PaymentMethod = koreksiExternalPurchaseOrder == null ? "" : koreksiExternalPurchaseOrder.PaymentMethod,
                    PlanPo = koreksiuntreceiptnoteItem.POSerialNumber,
                    CustomsCategory = koreksiuntreceiptnoteItem.CustomsCategory,
                    ProductCode = koreksiuntreceiptnoteItem.ProductCode,
                    ProductName = koreksiuntreceiptnoteItem.ProductName,
                    ProductRemark = koreksiuntreceiptnoteItem.ProductRemark,
                    ReceiptCorrectionQty = 0,
                    ReceiptQty = koreksireceiptcorritem == null ? 0 : (decimal)koreksireceiptcorritem.SmallQuantity,
                    ReceiptUom = koreksiuntreceiptnoteItem.SmallUomUnit,
                    RO = koreksiuntreceiptnoteItem.RONo,
                    UnitCode = "",
                    UnitSenderCode = "",
                    UnitRequestCode = ""
                });
            }

            var SAkhir = penerimaan.Concat(pengeluaran).Concat(koreksi).ToList();
            var SaldoAkhir = (from a in SAkhir
                              group a by new { a.PlanPo, a.ProductCode, a.CustomsCategory } into data
                              select new GarmentStockReportViewModel
                              {
                                  BeginningBalanceQty = Math.Round(data.Sum(x => x.BeginningBalanceQty), 2),
                                  BeginningBalanceUom = data.FirstOrDefault().BeginningBalanceUom,
                                  Buyer = data.FirstOrDefault().Buyer,
                                  EndingBalanceQty = Math.Round(data.Sum(x => x.EndingBalanceQty), 2),
                                  EndingUom = data.FirstOrDefault().EndingUom,
                                  ExpandUom = data.FirstOrDefault().ExpandUom,
                                  ExpendQty = Math.Round(data.Sum(x => x.ExpendQty), 2),
                                  NoArticle = data.FirstOrDefault().NoArticle,
                                  PaymentMethod = data.FirstOrDefault().PaymentMethod,
                                  PlanPo = data.FirstOrDefault().PlanPo,
                                  CustomsCategory = data.FirstOrDefault().CustomsCategory,
                                  POId = data.FirstOrDefault().POId,
                                  ProductCode = data.FirstOrDefault().ProductCode,
                                  ProductName = data.FirstOrDefault().ProductName,
                                  ProductRemark = data.FirstOrDefault().ProductRemark,
                                  ReceiptCorrectionQty = Math.Round(data.Sum(x => x.ReceiptCorrectionQty), 2),
                                  ReceiptQty = Math.Round(data.Sum(x => x.ReceiptQty), 2),
                                  ReceiptUom = data.FirstOrDefault().ReceiptUom,
                                  RO = data.FirstOrDefault().RO,
                                  UnitCode = data.FirstOrDefault().UnitCode,
                                  UnitSenderCode = data.FirstOrDefault().UnitSenderCode,
                                  UnitRequestCode = data.FirstOrDefault().UnitRequestCode
                              }).ToList();
            #endregion

            var Summaries = SaldoAwal.Concat(SaldoAkhir).ToList();
            var stock = (from a in Summaries
                         group a by new { a.PlanPo, a.ProductCode, a.CustomsCategory } into data
                         select new GarmentStockReportViewModel
                         {
                             BeginningBalanceQty = Math.Round(data.Sum(x => x.BeginningBalanceQty), 2),
                             BeginningBalanceUom = data.FirstOrDefault().BeginningBalanceUom,
                             Buyer = data.FirstOrDefault().Buyer,
                             EndingBalanceQty = Math.Round(data.Sum(x => x.BeginningBalanceQty) + data.Sum(x => x.ReceiptQty) + data.Sum(x => x.ReceiptCorrectionQty) - (decimal)data.Sum(x => x.ExpendQty), 2),
                             EndingUom = data.FirstOrDefault().EndingUom,
                             ExpandUom = data.FirstOrDefault().ExpandUom,
                             ExpendQty = Math.Round(data.Sum(x => x.ExpendQty), 2),
                             NoArticle = data.FirstOrDefault().NoArticle,
                             PaymentMethod = data.FirstOrDefault().PaymentMethod,
                             PlanPo = data.FirstOrDefault().PlanPo,
                             CustomsCategory = data.FirstOrDefault().CustomsCategory,
                             POId = data.FirstOrDefault().POId,
                             ProductCode = data.FirstOrDefault().ProductCode,
                             ProductName = data.FirstOrDefault().ProductName,
                             ProductRemark = data.FirstOrDefault().ProductRemark,
                             ReceiptCorrectionQty = Math.Round(data.Sum(x => x.ReceiptCorrectionQty), 2),
                             ReceiptQty = Math.Round(data.Sum(x => x.ReceiptQty), 2),
                             ReceiptUom = data.FirstOrDefault().ReceiptUom,
                             RO = data.FirstOrDefault().RO,
                             UnitCode = data.FirstOrDefault().UnitCode,
                             UnitSenderCode = data.FirstOrDefault().UnitSenderCode,
                             UnitRequestCode = data.FirstOrDefault().UnitRequestCode
                         }).ToList();


            foreach (var i in stock)
            {
                i.BeginningBalanceQty = i.BeginningBalanceQty > 0 ? i.BeginningBalanceQty : 0;
                i.EndingBalanceQty = i.EndingBalanceQty > 0 ? i.EndingBalanceQty : 0;
            }

            return stock.Where(x => (x.ProductCode != "EMB001") && (x.ProductCode != "WSH001") && (x.ProductCode != "PRC001") && (x.ProductCode != "APL001") && (x.ProductCode != "QLT001") && (x.ProductCode != "SMT001") && (x.ProductCode != "GMT001") && (x.ProductCode != "PRN001") && (x.ProductCode != "SMP001")).ToList(); ;
        }

        public Tuple<List<GarmentStockReportViewModel>, int> GetStockReport(int offset, string unitcode, string tipebarang, int page, int size, string Order, DateTime? dateFrom, DateTime? dateTo, string suppliertype, string customstype)
        {
            //var Query = GetStockQuery(tipebarang, unitcode, dateFrom, dateTo, offset);
            //Query = Query.OrderByDescending(x => x.SupplierName).ThenBy(x => x.Dono);
            List<GarmentStockReportViewModel> Data = GetStockQuery(tipebarang, unitcode, dateFrom, dateTo, offset, suppliertype, customstype).ToList();
            Data = Data.OrderByDescending(x => x.ProductCode).ThenBy(x => x.ProductName).ToList();
            //int TotalData = Data.Count();
            return Tuple.Create(Data, Data.Count());
        }

        public MemoryStream GenerateExcelStockReport(string ctg, string unitcode, DateTime? datefrom, DateTime? dateto, int offset, string suppliertype, string customstype)
        {
            var data = GetStockQuery(ctg, unitcode, datefrom, dateto, offset, suppliertype, customstype);
            var Query = data.OrderByDescending(x => x.ProductCode).ThenBy(x => x.ProductName).ToList();
            DataTable result = new DataTable();
            var headers = new string[] { "No", "Kode Barang", "No RO", "Plan PO", "Jenis Pembelian", "Artikel", "Nama Barang", "Keterangan Barang", "Buyer", "Saldo Awal", "Saldo Awal2", "Penerimaan", "Penerimaan1", "Penerimaan2", "Pengeluaran", "Pengeluaran1", "Saldo Akhir", "Saldo Akhir1", "Asal" }; 
            var subheaders = new string[] { "Jumlah", "Sat", "Jumlah", "Koreksi", "Sat", "Jumlah", "Sat", "Jumlah", "Sat" };
            for (int i = 0; i < headers.Length; i++)
            {
                result.Columns.Add(new DataColumn() { ColumnName = headers[i], DataType = typeof(string) });
            }
            var index = 1;
            foreach (var item in Query)
            {

                //result.Rows.Add(index++, item.ProductCode, item.RO, item.PlanPo, item.NoArticle, item.ProductName, item.Information, item.Buyer,

                //    item.BeginningBalanceQty, item.BeginningBalanceUom, item.ReceiptQty, item.ReceiptCorrectionQty, item.ReceiptUom,
                //    NumberFormat(item.ExpendQty),
                //    item.ExpandUom, item.EndingBalanceQty, item.EndingUom, item.From);


                result.Rows.Add(index++, item.ProductCode, item.RO, item.PlanPo, item.CustomsCategory, item.NoArticle, item.ProductName, item.ProductRemark, item.Buyer,

                    Convert.ToDouble(item.BeginningBalanceQty), item.BeginningBalanceUom, Convert.ToDouble(item.ReceiptQty), Convert.ToDouble(item.ReceiptCorrectionQty), item.ReceiptUom,
                    item.ExpendQty,
                    item.ExpandUom, Convert.ToDouble(item.EndingBalanceQty), item.EndingUom,
                    item.PaymentMethod == "FREE FROM BUYER" || item.PaymentMethod == "CMT" || item.PaymentMethod == "CMT/IMPORT" ? "BY" : "BL");

            }

            ExcelPackage package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("Data");

            sheet.Cells["A3"].LoadFromDataTable(result, false, OfficeOpenXml.Table.TableStyles.Light16);
            sheet.Cells["J1"].Value = headers[8];
            sheet.Cells["J1:K1"].Merge = true;

            sheet.Cells["L1"].Value = headers[10];
            sheet.Cells["L1:N1"].Merge = true;
            sheet.Cells["O1"].Value = headers[13];
            sheet.Cells["O1:P1"].Merge = true;
            sheet.Cells["Q1"].Value = headers[15];
            sheet.Cells["Q1:R1"].Merge = true;

            foreach (var i in Enumerable.Range(0, 8))
            {
                var col = (char)('A' + i);
                sheet.Cells[$"{col}1"].Value = headers[i];
                sheet.Cells[$"{col}1:{col}2"].Merge = true;
            }

            for (var i = 0; i < 9; i++)
            {
                var col = (char)('J' + i);
                sheet.Cells[$"{col}2"].Value = subheaders[i];

            }

            foreach (var i in Enumerable.Range(0, 1))
            {
                var col = (char)('S' + i);
                sheet.Cells[$"{col}1"].Value = headers[i + 18];
                sheet.Cells[$"{col}1:{col}2"].Merge = true;
            }

            sheet.Cells["A1:S2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells["A1:S2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Cells["A1:S2"].Style.Font.Bold = true;
            var widths = new int[] {10, 15, 15, 20, 20 , 20, 15, 20, 15, 10, 10, 10, 10, 10, 10, 10, 10, 10, 15 };
            foreach (var i in Enumerable.Range(0, headers.Length))
            {
                sheet.Column(i + 1).Width = widths[i];
            }

            MemoryStream stream = new MemoryStream();
            package.SaveAs(stream);
            return stream;


        }

        String NumberFormat(double? numb)
        {

            var number = string.Format("{0:0,0.00}", numb);

            return number;
        }

        private List<GarmentCategoryViewModel> GetProductCodes(int page, int size, string order, string filter)
        {
            IHttpClientService httpClient = (IHttpClientService)this.serviceProvider.GetService(typeof(IHttpClientService));
            if (httpClient != null)
            {
                var garmentSupplierUri = APIEndpoint.Core + $"master/garment-categories";
                string queryUri = "?page=" + page + "&size=" + size + "&order=" + order + "&filter=" + filter;
                string uri = garmentSupplierUri + queryUri;
                var response = httpClient.GetAsync($"{uri}").Result.Content.ReadAsStringAsync();
                Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Result);
                List<GarmentCategoryViewModel> viewModel = JsonConvert.DeserializeObject<List<GarmentCategoryViewModel>>(result.GetValueOrDefault("data").ToString());
                return viewModel;
            }
            else
            {
                List<GarmentCategoryViewModel> viewModel = null;
                return viewModel;
            }
        }


    }
}
