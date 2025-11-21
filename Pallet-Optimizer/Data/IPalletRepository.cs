using Pallet_Optimizer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pallet_Optimizer.Data
{
    public interface IPalletRepository
    {
        Task<PalletHolder> GetHolderAsync();
        Task<Pallet> GetPalletAsync(int index);
        Task UpdatePalletAsync(int index, Pallet updated);
        Task AddPalletAsync(Pallet pallet);
       // Task SaveChangesAsync(); 
    }
}
