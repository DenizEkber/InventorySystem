using AutoMapper;
using InventorySystem.CORE.Interfaces;
using InventorySystem.DATABASE.CodeFirst.Entities;
using InventorySystem.DATABASE.Repositories;
using InventorySystem.DTO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace InventorySystem.CORE.Services
{
    public class ProductService : IProductService
    {
        private readonly IMapper _mapper;
        private readonly ProductRepository _productRepository;

        public ProductService(ProductRepository repository, IMapper mapper)
        {
            _productRepository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            //return (await _repository.GetAllAsync()).ToList();
            var product = await _productRepository.GetAllAsync();
            var mappedProduct = _mapper.Map<IEnumerable<ProductDto>>(product);
            return mappedProduct;
        }
        public async Task<ProductDto> GetByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return null;
            }
            var mappedProduct = _mapper.Map<ProductDto>(product);
            return mappedProduct;
        }
        public async Task<int> AddAsync(ProductDto productDto)
        {
            //await _repository.AddAsync(supplier);
            var productExists = await _productRepository.FindFirstAsync(w => w.Name == productDto.Name);
            if (productExists != null)
            {
                MessageBox.Show("SupplierExsists ");
            }
            var product = _mapper.Map<Product>(productDto);
            await _productRepository.AddAsync(product);
            return product.Id;
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product != null)
            {
                await _productRepository.DeleteAsync(product);
            }
        }
        public async Task UpdateAsync(ProductDto productDto)
        {
            //await _warehouseRepository.UpdateAsync(warehouse);
            var product = await _productRepository.GetByIdAsync(productDto.Id);
            if (product == null)
            {

            }

            _mapper.Map(productDto, product);
            await _productRepository.UpdateAsync(product);

        }
        public async Task<ProductDto> FindFirstAsync(Expression<Func<Product, bool>> predicate)
        {
            var product = await _productRepository.FindFirstAsync(predicate);
            if (product == null)
            {

            }
            var productDto = _mapper.Map<ProductDto>(product);
            return productDto;
        }

        public async Task<int> GetProductCountForTodayAsync()
        {
            return await _productRepository.GetProductTransactionCountForTodayAsync(TransactionType.TransferIn); // "Entry" yeni eklenen ürünleri ifade ediyor
        }

    }
}
