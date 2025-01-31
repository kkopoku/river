using River.API.Models;
using River.API.DTOs;

namespace River.API.Services;


public interface ITransferService{

    Task<ApiResponse<TransferDataDto>> CreateTransferAsync(CreateTransferDto createTransferDto);
    Task<ApiResponse<List<Transfer>>> GetAllTransfersAsync(int page, int pageSize);
}