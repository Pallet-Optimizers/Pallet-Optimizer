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

        // New methods for package plan management
        Task<List<PackagePlanViewModel>> GetAllPackagePlansAsync();
        Task<PalletHolder> GetPackagePlanByIdAsync(string id);
        Task<string> CreatePackagePlanAsync(string name);
        Task<bool> DeletePackagePlanAsync(string id);
        Task<bool> UpdatePackagePlanAsync(string id, PalletHolder holder);
    }
}