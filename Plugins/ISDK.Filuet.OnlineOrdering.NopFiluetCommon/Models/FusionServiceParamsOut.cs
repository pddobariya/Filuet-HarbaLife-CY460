using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models.FiluetCommon;
using System.Collections.Generic;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models
{
    public class FusionServiceParamsOut : FusionServiceParamsModel
    {
        public List<OrderItemModel> OrderItems { get; set; }

        public FusionServiceParamsOut(FusionServiceParamsModel serviceParams,List<OrderItemModel> orderItems)
        {
            #region Properties

            Address = serviceParams.Address;
            City = serviceParams.City;
            CountryCode = serviceParams.CountryCode;
            CustomerName = serviceParams.CustomerName;
            DistributorId = serviceParams.DistributorId;
            DsType = serviceParams.DsType;
            FreightCode = serviceParams.FreightCode;
            InvoiceWithShipment = serviceParams.InvoiceWithShipment;
            OrderItems = orderItems;
            OrderMonth = serviceParams.OrderMonth;
            OrderNumber = serviceParams.OrderNumber;
            OrderType = serviceParams.OrderType;
            OrderTypeCode = serviceParams.OrderTypeCode;
            OrderTypeId = serviceParams.OrderTypeId;
            PhoneNumber = serviceParams.PhoneNumber;
            PostalCode = serviceParams.PostalCode;
            PostamatId = serviceParams.PostamatId;
            ProcessingLocation = serviceParams.ProcessingLocation;
            TimeSlot = serviceParams.TimeSlot;
            VolumePoints = serviceParams.VolumePoints;
            WarehouseCode = serviceParams.WarehouseCode;

            #endregion
        }
    }
}
