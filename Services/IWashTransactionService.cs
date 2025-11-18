using wsahRecieveDelivary.DTOs;

namespace wsahRecieveDelivary.Services
{
    public interface IWashTransactionService
    {
        // Create
        Task<WashTransactionResponseDto> CreateReceiveAsync(CreateWashTransactionDto dto, int userId);
        Task<WashTransactionResponseDto> CreateDeliveryAsync(CreateWashTransactionDto dto, int userId);

        // Read
        Task<WashTransactionResponseDto?> GetByIdAsync(int id);
        Task<List<WashTransactionResponseDto>> GetAllAsync();
        Task<List<WashTransactionResponseDto>> GetByWorkOrderAsync(int workOrderId);
        Task<List<WashTransactionResponseDto>> GetByStageAsync(int processStageId);  // ✅ CHANGED
        Task<List<WashTransactionResponseDto>> GetByFilterAsync(WashTransactionFilterDto filter);

        // Update/Delete
        Task<WashTransactionResponseDto> UpdateAsync(int id, CreateWashTransactionDto dto, int userId);
        Task<bool> DeleteAsync(int id);

        // Balance & Status
        Task<List<ProcessBalanceDto>> GetBalancesByWorkOrderAsync(int workOrderId);
        Task<WorkOrderWashStatusDto?> GetWashStatusAsync(int workOrderId);
        Task<List<WorkOrderWashStatusDto>> GetAllWashStatusesAsync();

        // Reports
        Task<List<ProcessStageSummaryDto>> GetStageSummaryAsync();
        Task<List<WashTransactionResponseDto>> GetReceivesByStageAsync(int processStageId, DateTime? startDate = null, DateTime? endDate = null);  // ✅ CHANGED
        Task<List<WashTransactionResponseDto>> GetDeliveriesByStageAsync(int processStageId, DateTime? startDate = null, DateTime? endDate = null);  // ✅ CHANGED
    }
}