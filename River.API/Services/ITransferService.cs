using River.API.Models;
using River.API.DTOs;
using River.API.DTOs.Transfer;

namespace River.API.Services;


public interface ITransferService{

    Task<ApiResponse<string>> CreateTransferAsync(CreateTransferDto createTransferDto);
    Task<ApiResponse<string>> ReverseTransferAsync(ReverseTransferDto reverseTransferDto);
    Task<ApiResponse<List<Transfer>>> GetAllTransfersAsync(int page, int pageSize);
    Task<ApiResponse<Transfer>> GetTransferAsync(string id);
    Task<ApiResponse<SimulateResponseDto>> SimulateTransferAsync(CreateTransferDto simulateTransferDto);
}