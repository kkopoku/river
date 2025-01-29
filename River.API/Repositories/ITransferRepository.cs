using River.API.Models;

namespace River.API.Repositories;

public interface ITransferRepository {

    Task<Transfer> CreateTransferAsync(Transfer transfer);
    Task<List<Transfer>> GetAllTransfersAsync(int page, int pageSize);

}