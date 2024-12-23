using AutoMapper;
using InventorySystem.CORE.Interfaces;
using InventorySystem.DATABASE.CodeFirst.Entities;
using InventorySystem.DATABASE.Repositories;
using InventorySystem.DTO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorySystem.CORE.Services
{
    public class UserService : IUserService
    {
        private readonly UserRepository _userRepository;
        private readonly UserDetailRepository _userDetailRepository;
        private readonly IMapper _mapper;

        public UserService(UserRepository userRepository, IMapper mapper, UserDetailRepository userDetailRepository)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _userDetailRepository = userDetailRepository;
        }

        public async Task AddAsync(UserDto userDto)
        {
            var userExists = await _userRepository.FindFirstAsync(w => w.Name == userDto.Name);
            if (userExists != null)
            {
                MessageBox.Show("User already exists.");
                return;
            }
            var user = _mapper.Map<User>(userDto);
            await _userRepository.AddAsync(user);
        }

        public async Task DeleteAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user != null)
            {
                await _userRepository.DeleteAsync(user);
            }
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<UserDto> GetByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user == null ? null : _mapper.Map<UserDto>(user);
        }

        public async Task UpdateAsync(UserDto userDto)
        {
            var user = await _userRepository.GetByIdAsync(userDto.UserID);
            if (user == null) return;

            _mapper.Map(userDto, user);
            await _userRepository.UpdateAsync(user);
        }
        public async Task UpdateUserDetailAsync(int id, string Url)
        {
            var userDetail = await _userDetailRepository.GetByIdAsync(id);
            if (userDetail != null) return;
            userDetail = new UserDetail
            {
                UserId = id,
                ProfileImageUrl = Url
            };

            await _userDetailRepository.AddAsync(userDetail);
        }
        public async Task<UserDetail> GetByIdUserDetailAsync(int id)
        {
            var userDetail = await _userDetailRepository.FindFirstAsync(d=>d.UserId == id);
            if(userDetail == null) return null;
            return userDetail;
        }
    }
}
