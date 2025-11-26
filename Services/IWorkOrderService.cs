using Microsoft.AspNetCore.Http;
using wsahRecieveDelivary.DTOs;

namespace wsahRecieveDelivary.Services
{
    public interface IWorkOrderService
    {
        Task<WorkOrderResponseDto> CreateAsync(WorkOrderDto dto, int userId);
        Task<WorkOrderResponseDto> UpdateAsync(int id, WorkOrderDto dto, int userId);
        Task<bool> DeleteAsync(int id);
        Task<WorkOrderResponseDto?> GetByIdAsync(int id);
        Task<WorkOrderResponseDto?> GetByWorkOrderNoAsync(string workOrderNo);
        Task<List<WorkOrderResponseDto>> GetAllAsync();
        Task<WorkOrderBulkUploadResponseDto> BulkUploadFromExcelAsync(IFormFile file, int userId);

        // ✅ ADD THIS LINE
        Task<PaginatedResponseDto<WorkOrderResponseDto>> GetPaginatedAsync(PaginationRequestDto request);
    }
}