using System.Collections.Generic;
using System.Threading.Tasks;
using Pallet_Optimizer.Models;

namespace Pallet_Optimizer.Data
{
    public interface IPalletRepository
    {
        Task<PalletHolder> GetHolderAsync();
        Task UpdateHolderAsync(PalletHolder holder);

        Task<Pallet?> GetPalletAsync(string palletId);

        Task AddPalletAsync(Pallet pallet);
        Task UpdatePalletAsync(string id, Pallet pallet);
        Task DeletePalletAsync(string id);

        // Package plan operations used by DashboardController
        Task<List<PackagePlanViewModel>> GetAllPackagePlansAsync();
        Task<string> CreatePackagePlanAsync(string planName);
        Task<bool> DeletePackagePlanAsync(string id);
    }
}