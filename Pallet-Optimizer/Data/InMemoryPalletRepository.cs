using Pallet_Optimizer.Data;
using Pallet_Optimizer.Models;

public class InMemoryPalletRepository : IPalletRepository
{
    private readonly PalletHolder _holder;

    public InMemoryPalletRepository()
    {
        // seed with 2 standard pallets
        var p1 = new Pallet { Name = "Pallet A", Width = 1.2, Length = 0.8, MaxHeight = 1.8, MaxWeight = 800 };
        var p2 = new Pallet { Name = "Pallet B", Width = 1.2, Length = 0.8, MaxHeight = 1.8, MaxWeight = 800 };

        _holder = new PalletHolder
        {
            Pallets = new List<Pallet> { p1, p2 },
            CurrentPalletIndex = 0
        };
    }

    public Task AddPalletAsync(Pallet pallet)
    {
        _holder.Pallets.Add(pallet);
        return Task.CompletedTask;
    }

    public Task DeletePalletAsync(string id)
    {
        _holder.Pallets.RemoveAll(p => p.Id == id);
        return Task.CompletedTask;
    }

    public Task<Pallet> GetPalletAsync(string id)
    {
        var p = _holder.Pallets.FirstOrDefault(x => x.Id == id);
        return Task.FromResult(p);
    }

    public Task<PalletHolder> GetHolderAsync() => Task.FromResult(_holder);

    public Task UpdateHolderAsync(PalletHolder holder)
    {
        // for in-memory repo we just replace lists
        _holder.Pallets = holder.Pallets;
        _holder.CurrentPalletIndex = holder.CurrentPalletIndex;
        return Task.CompletedTask;
    }

    public Task UpdatePalletAsync(string id, Pallet pallet)
    {
        var idx = _holder.Pallets.FindIndex(p => p.Id == id);
        if (idx >= 0) _holder.Pallets[idx] = pallet;
        return Task.CompletedTask;
    }
}