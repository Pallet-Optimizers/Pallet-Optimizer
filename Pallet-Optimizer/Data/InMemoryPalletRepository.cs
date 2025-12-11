using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pallet_Optimizer.Data;
using Pallet_Optimizer.Models;

public class InMemoryPalletRepository : IPalletRepository
{
    private readonly PalletHolder _holder;
    private readonly List<PackagePlanViewModel> _plans = new();

    public InMemoryPalletRepository()
    {
        var p1 = new Pallet { Name = "Pallet A", Width = 1.2, Length = 0.8, MaxHeight = 1.8, MaxWeightKg = 800 };
        var p2 = new Pallet { Name = "Pallet B", Width = 1.2, Length = 0.8, MaxHeight = 1.8, MaxWeightKg = 800 };

        _holder = new PalletHolder
        {
            Pallets = new List<Pallet> { p1, p2 },
            CurrentPalletIndex = 0
        };
    }

    public Task<PalletHolder> GetHolderAsync() => Task.FromResult(_holder);

    public Task<Pallet?> GetPalletAsync(string palletId)
    {
        var p = _holder.Pallets.FirstOrDefault(x => x.Id == palletId);
        return Task.FromResult<Pallet?>(p);
    }

    public Task AddPalletAsync(Pallet pallet)
    {
        if (pallet == null) throw new ArgumentNullException(nameof(pallet), "Pallet payload was null.");
        _holder.Pallets.Add(pallet);
        return Task.CompletedTask;
    }

    public Task UpdatePalletAsync(string id, Pallet pallet)
    {
        if (pallet == null) throw new ArgumentNullException(nameof(pallet), "Pallet payload was null.");
        var idx = _holder.Pallets.FindIndex(p => p.Id == id);
        if (idx >= 0) _holder.Pallets[idx] = pallet;
        return Task.CompletedTask;
    }

    public Task DeletePalletAsync(string id)
    {
        _holder.Pallets.RemoveAll(p => p.Id == id);
        return Task.CompletedTask;
    }

    public Task UpdateHolderAsync(PalletHolder holder)
    {
        if (holder == null) throw new ArgumentNullException(nameof(holder), "Holder payload was null.");
        if (holder.Pallets == null) throw new ArgumentException("Holder.Pallets collection was null.", nameof(holder));

        _holder.Pallets = holder.Pallets;
        _holder.CurrentPalletIndex = holder.CurrentPalletIndex;
        return Task.CompletedTask;
    }

    // Package plan operations
    public Task<List<PackagePlanViewModel>> GetAllPackagePlansAsync()
    {
        var list = _plans.OrderByDescending(p => p.CreatedDate).ToList();
        return Task.FromResult(list);
    }

    public Task<string> CreatePackagePlanAsync(string planName)
    {
        if (string.IsNullOrWhiteSpace(planName))
            throw new ArgumentException("Plan name must be provided.", nameof(planName));

        var id = Guid.NewGuid().ToString("N");
        var now = DateTime.UtcNow;

        _plans.Add(new PackagePlanViewModel
        {
            Id = id,
            Name = planName,
            CreatedDate = now,
            LastModified = now,
            PalletCount = 0,
            TotalElements = 0,
            TotalWeight = 0
        });

        return Task.FromResult(id);
    }

    public Task<bool> DeletePackagePlanAsync(string id)
    {
        var removed = _plans.RemoveAll(p => p.Id == id) > 0;
        return Task.FromResult(removed);
    }
}