using DiplomServer.Application.DTOs.Common;
using DiplomServer.Application.DTOs.Orders;
using DiplomServer.Application.Interfaces;
using DiplomServer.Domain.Entities;
using DiplomServer.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DiplomServer.Application.Services
{
    public class OrdersService : IOrdersService
    {
        private readonly IOrdersRepository _repository;
        private readonly ILookupService _lookupService;
        private readonly ICurrentUserService _currentUserService;

        public OrdersService(
            IOrdersRepository repository,
            ILookupService lookupService,
            ICurrentUserService currentUserService)
        {
            _repository = repository;
            _lookupService = lookupService;
            _currentUserService = currentUserService;
        }

        public async Task<List<OrdersResponseDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync(GetUserId());

            var result = new List<OrdersResponseDto>();
            foreach (var item in entities) result.Add(await MapToResponse(item));
            return result;
        }

        public async Task<OrdersResponseDto> GetByIdAsync(uint id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return await MapToResponse(entity);
        }

        public async Task<OrdersResponseDto> CreateAsync(CreateOrdersRequestDto dto)
        {
            var order = new Orders
            {
                Id = 0,
                Number = dto.Number.Trim(),
                DateCreate = dto.DateCreate,
                StudentId = dto.StudentId,
                DisciplineId = dto.DisciplineId,
                CreatedById = GetUserId(),
                DateReceptionTeacher = dto.DateReceptionTeacher,
                DateReceptionCommission = dto.DateReceptionCommission
            };
            
            var id = await _repository.CreateAsync(order);
            var ReceptionTeacher = dto.ReceptionTeachers.Select(s => new ReceptionTeacher
            {
                Id = 0,
                TeacherId = s.TeacherId,
                OrderId = id
            });
            var ReceptionCommission = dto.ReceptionCommission.Select(s => new ReceptionCommission
            {
                Id = 0,
                TeacherId = s.TeacherId,
                OrderId = id
            });
            await _repository.CreateReceptionAsync(ReceptionTeacher, ReceptionCommission);
            return await GetByIdAsync(id);
        }

        public async Task<OrdersResponseDto> UpdateAsync(uint id, UpdateOrdersRequestDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) throw new KeyNotFoundException("Заказ не найден.");

            entity.Number = dto.Number.Trim();
            entity.DateCreate = dto.DateCreate.Value;
            entity.StudentId = dto.StudentId.Value;
            entity.DisciplineId = dto.DisciplineId.Value;
            entity.DateReceptionTeacher = dto.DateReceptionTeacher.Value;
            entity.DateReceptionCommission = dto.DateReceptionCommission.Value;

            _repository.UpdateReceptionTeachers(entity.ReceptionTeachers, dto.ReceptionTeachers);
            _repository.UpdateReceptionCommissions(entity.ReceptionCommissions, dto.ReceptionCommission);
            await _repository.UpdateAsync(entity);

            return await GetByIdAsync(id);

        }

        public async Task DeleteAsync(uint id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) throw new KeyNotFoundException("Заказ не найден.");

            await _repository.DeleteAsync(id);
        }
        private uint GetUserId()
        {
            return _currentUserService.UserId;
        }

        private async Task<OrdersResponseDto> MapToResponse(Orders entity)
        {
            var discTask = await _lookupService.GetDisciplineByIdAsync(entity.DisciplineId);

            var studentTask = await _lookupService.GetStudentByIdAsync(entity.StudentId);

            var teachersIds = entity.ReceptionTeachers.Select(s => s.TeacherId);
            var teacherTask = await _lookupService.GetTeachersDictionaryByIdAsync(teachersIds);

            var commissionIds = entity.ReceptionCommissions.Select(s => s.TeacherId);
            var commissionTask = await _lookupService.GetTeachersDictionaryByIdAsync(commissionIds);

            return new OrdersResponseDto
            {
                Id = entity.Id,
                Number = entity.Number ?? string.Empty,
                DateCreate = entity.DateCreate,
                StudentId = studentTask,
                DisciplineId = discTask,
                CreatedById = entity.CreatedById,
                DateReceptionTeacher = entity.DateReceptionTeacher,
                DateReceptionCommission = entity.DateReceptionCommission,

                ReceptionTeachers = entity.ReceptionTeachers.Select(rt => new ReceptionDto
                {
                    Id = rt.Id,
                    TeacherId = rt.TeacherId,
                    TeacherName = teacherTask[rt.TeacherId].Name
                }).ToList(),

                ReceptionCommissions = entity.ReceptionCommissions.Select(rc => new ReceptionDto
                {
                    Id = rc.Id,
                    TeacherId = rc.TeacherId,
                    TeacherName = commissionTask[rc.TeacherId].Name
                }).ToList()
            };
        }
    }
}