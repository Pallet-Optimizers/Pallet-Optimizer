using System.Threading.Tasks;
using Pallet_Optimizer.Models;

namespace Pallet_Optimizer.Data
{
    // Updated to match actual usage in controllers and repositories.
    public interface IPalletRepository
    {
        Task<PalletHolder> GetHolderAsync();
        Task UpdateHolderAsync(PalletHolder holder);

        Task<Pallet?> GetPalletAsync(string palletId);

        Task AddPalletAsync(Pallet pallet);
        Task UpdatePalletAsync(string id, Pallet pallet);
        Task DeletePalletAsync(string id);
    }
}
