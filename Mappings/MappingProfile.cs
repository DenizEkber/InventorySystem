using InventorySystem.DATABASE.CodeFirst.Entities;
using AutoMapper;
using InventorySystem.DTO.Models;
using InventorySystem.CORE.Services;

namespace InventorySystem.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //Normal Dto
            CreateMap<Warehouse, WarehouseDto>().ReverseMap();
            CreateMap<Supplier, SupplierDto>().ReverseMap();
            CreateMap<Stock, StockDto>().ReverseMap();
            CreateMap<CurrentStock, StockDto>().ReverseMap();
            CreateMap<Stock, CurrentStock>().ReverseMap();
            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<StockTransaction, StockTransactionDto>().ReverseMap();
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<CurrentStock, InevntryDataGridViewDto>().ReverseMap();
            CreateMap<CurrentStock, SupplierInventoryDataGridViewDto>().ReverseMap();


        }
    }
}
