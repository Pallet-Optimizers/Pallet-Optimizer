using Pallet_Optimizer.Models;
using System.Threading.Tasks;

namespace Pallet_Optimizer.Data
{
    public interface IPalletRepository
    {
        Task<PalletHolder> GetHolderAsync();
        Task<Pallet?> GetPalletAsync(string index);
        Task UpdatePalletAsync(string index, Pallet updated);
        Task AddPalletAsync(Pallet pallet);
        Task DeletePalletAsync(string index);
    }
}
