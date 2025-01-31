using River.API.Models;
using River.API.DTOs.Transfer;

namespace River.API.Repositories;

public interface ITransferRepository {

    Task<Transfer> CreateTransferAsync(Transfer transfer);
    Task<List<Transfer>> GetAllTransfersAsync(int page, int pageSize);
    Task<Transfer> FindTransferByIdAsync(string id);
    Task<Transfer?> UpdateTransferAsync(UpdateTransferDto updateTransfer);

}