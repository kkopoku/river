using River.API.Models;
using River.API.DTOs;
using River.API.DTOs.Transfer;

namespace River.API.Services;


public interface ITransferService{

    Task<ApiResponse<Transfer>> CreateTransferAsync(CreateTransferDto createTransferDto);
    Task<ApiResponse<ReverseTransferResultDto>> ReverseTransferAsync(ReverseTransferDto reverseTransferDto);
    Task<ApiResponse<List<Transfer>>> GetAllTransfersAsync(int page, int pageSize);
}