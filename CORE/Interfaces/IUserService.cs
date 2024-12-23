using InventorySystem.DATABASE.CodeFirst.Entities;
using InventorySystem.DTO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorySystem.CORE.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task<UserDto> GetByIdAsync(int id);
        Task AddAsync(UserDto userDto);
        Task UpdateAsync(UserDto userDto);
        Task DeleteAsync(int id);
        Task UpdateUserDetailAsync(int id, string Url);
        Task<UserDetail> GetByIdUserDetailAsync(int id);
    }
}
