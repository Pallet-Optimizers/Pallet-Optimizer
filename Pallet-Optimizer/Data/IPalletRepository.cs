using System.Threading.Tasks;
using Pallet_Optimizer.Models;

namespace Pallet_Optimizer.Data
{
    public interface IPalletRepository
    {
        Task<PalletHolder> GetHolderAsync();
        Task UpdateHolderAsync(PalletHolder holder);

        Task<Pallet> GetPalletAsync(string palletId);
        Task UpdatePalletAsync(Pallet pallet);
    }
}
