using Pallet_Optimizer.Data;
using Pallet_Optimizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pallet_Optimizer.Algorithms
{
    // helper classes inside algorithm
    internal class LayerState
    {
        public double Z { get; set; }         // base Z for layer
        public double CurrentX { get; set; } = 0;
        public double CurrentY { get; set; } = 0;
        public double RowDepth { get; set; } = 0;
    }

    public static class GreedyPalletOptimizer
    {
        // Main entry: pack list of elements into pallets (uses existing pallet templates from holder if provided)
        public static PalletHolder PackAll(PalletHolder holder, PackingSettings settings)
        {
            // collect all elements, detach them from pallets
            var allElements = holder.GetAllElements().Select(CloneElementWithoutPlacement).ToList();

            // empty target pallets list
            var outputPallets = new List<Pallet>();

            // pick template pallet if any else default
            Pallet template = holder.Pallets.FirstOrDefault() ?? new Pallet();

            // Place elements requiring alone pallets first
            var alone = allElements.Where(e => e.MustBeAlone && settings.RespectMustBeAlone).ToList();
            foreach (var el in alone)
            {
                var p = ClonePalletTemplate(template);
                PlaceOnEmptyPalletSingle(p, el);
                outputPallets.Add(p);
            }

            // remaining
            var remaining = allElements.Except(alone).OrderByDescending(e => e.Width * e.Depth * e.Height).ToList();

            foreach (var el in remaining)
            {
                bool placed = false;

                // try place on existing pallets
                foreach (var pallet in outputPallets)
                {
                    if (TryPlaceOnPallet(pallet, el, settings))
                    {
                        placed = true;
                        break;
                    }
                }

                if (!placed)
                {
                    // try existing holder pallets as templates (if any left unused, append)
                    // create new pallet from template
                    var newP = ClonePalletTemplate(template);
                    if (!TryPlaceOnPallet(newP, el, settings))
                    {
                        // if a single element is larger than pallet (even with overhang) -> mark it, but place anyway at 0,0
                        el.X = 0; el.Y = 0; el.Z = 0;
                        newP.Elements.Add(el);
                    }
                    outputPallets.Add(newP);
                }
            }

            // finalize holder
            var outHolder = new PalletHolder { Pallets = outputPallets.ToList() };
            return outHolder;
        }

        private static Element CloneElementWithoutPlacement(Element e)
        {
            return new Element
            {
                Id = e.Id,
                Name = e.Name,
                Width = e.Width,
                Depth = e.Depth,
                Height = e.Height,
                WeightKg = e.WeightKg,
                CanRotate = e.CanRotate,
                MustBeAlone = e.MustBeAlone
            };
        }

        private static Pallet ClonePalletTemplate(Pallet t)
        {
            return new Pallet
            {
                Id = Guid.NewGuid().ToString(),
                Name = t.Name,
                MaterialType = t.MaterialType,
                Width = t.Width,
                Length = t.Length,
                MaxHeight = t.MaxHeight,
                MaxWeight = t.MaxWeight,
                Elements = new List<Element>()
            };
        }

        private static void PlaceOnEmptyPalletSingle(Pallet p, Element el)
        {
            el.X = 0; el.Y = 0; el.Z = 0;
            el.Rotated = false;
            el.PalletId = p.Id;
            p.Elements.Add(el);
        }

        private static bool TryPlaceOnPallet(Pallet pallet, Element el, PackingSettings settings)
        {
            // quick weight / height checks
            if (pallet.CurrentWeightKg + el.WeightKg > pallet.MaxWeight) return false;
            if (el.Height + pallet.CurrentHeight > Math.Min(pallet.MaxHeight, settings.MaxPalletHeightAbsolute)) return false;

            // build layers (list of Z base positions)
            var layers = BuildLayerStates(pallet);

            // try existing layers
            foreach (var layer in layers)
            {
                if (TryPlaceInLayer(pallet, layer, el, settings))
                {
                    el.PalletId = pallet.Id;
                    return true;
                }
            }

            // create new top layer
            double newZ = layers.Count == 0 ? 0.0 : layers.Max(l => l.Z + l.RowDepth);
            var layerState = new LayerState { Z = newZ, CurrentX = 0, CurrentY = 0, RowDepth = 0 };

            if (TryPlaceInLayer(pallet, layerState, el, settings, createLayer: true))
            {
                el.PalletId = pallet.Id;
                return true;
            }

            return false;
        }

        private static List<LayerState> BuildLayerStates(Pallet pallet)
        {
            var groups = pallet.Elements.GroupBy(e => Math.Round(e.Z, 6)).OrderBy(g => g.Key);
            var list = new List<LayerState>();
            foreach (var g in groups)
            {
                var z = g.Key;
                var depth = g.Max(e => e.Depth);
                var currentX = g.Max(e => e.X + e.Width);
                var currentY = g.Max(e => e.Y + e.Depth);
                list.Add(new LayerState { Z = z, CurrentX = currentX, CurrentY = currentY, RowDepth = depth });
            }
            return list;
        }

        private static bool TryPlaceInLayer(Pallet pallet, LayerState layer, Element el, PackingSettings settings, bool createLayer = false)
        {
            // try orientations
            var orientations = settings.AllowRotation && el.CanRotate
                ? new[] { false, true } // false = no rotation, true = rotated (swap width/depth)
                : new[] { false };

            double pad = settings.Padding;
            double palletW = pallet.Width + settings.MaxOverhangX;
            double palletL = pallet.Length + settings.MaxOverhangY;
            double layerZ = layer.Z;

            foreach (var rotated in orientations)
            {
                double w = rotated ? el.Depth : el.Width;
                double d = rotated ? el.Width : el.Depth;

                // quick height check
                if (layerZ + el.Height > pallet.MaxHeight + 1e-9) continue;
                if (layerZ + el.Height > settings.MaxPalletHeightAbsolute) continue;

                // attempt row-fill starting at layer.CurrentX, layer.CurrentY
                double x = layer.CurrentX;
                double y = 0; // we try place in same row first; if row full we push y down
                double rowDepth = layer.RowDepth;

                // Collect occupied rectangles at this Z
                var occupied = pallet.Elements.Where(e => Math.Abs(e.Z - layerZ) < 1e-6).ToList();

                // simple greedy scan: try grid anchors derived from occupied
                var anchors = new List<(double X, double Y)>();
                anchors.Add((layer.CurrentX, layer.CurrentY));
                anchors.Add((0.0, 0.0));
                foreach (var occ in occupied)
                {
                    anchors.Add((occ.X + occ.Width + pad, occ.Y));
                    anchors.Add((occ.X, occ.Y + occ.Depth + pad));
                    anchors.Add((occ.X + occ.Width + pad, occ.Y + occ.Depth + pad));
                }

                // order anchors by proximity
                anchors = anchors.Distinct().OrderBy(a => a.X * a.X + a.Y * a.Y).ToList();

                foreach (var a in anchors)
                {
                    x = a.X;
                    y = a.Y;

                    // bounds check
                    if (x + w > palletW + 1e-9) continue;
                    if (y + d > palletL + 1e-9) continue;

                    // check collisions with occupied
                    bool collides = false;
                    foreach (var occ in occupied)
                    {
                        double ow = occ.Width, od = occ.Depth;
                        double ox = occ.X, oy = occ.Y;

                        bool overlapX = x < ox + ow - 1e-9 && x + w > ox + 1e-9;
                        bool overlapY = y < oy + od - 1e-9 && y + d > oy + 1e-9;
                        if (overlapX && overlapY) { collides = true; break; }
                    }

                    if (collides) continue;

                    // good placement
                    el.X = (float)Math.Max(0.0, x);
                    el.Y = (float)Math.Max(0.0, y);
                    el.Z = (float)layerZ;
                    el.Rotated = rotated;

                    // update layer state row depth and currentX
                    layer.RowDepth = Math.Max(layer.RowDepth, d);
                    layer.CurrentX = el.X + w + pad;
                    layer.CurrentY = Math.Max(layer.CurrentY, el.Y + d + pad);

                    // apply to pallet
                    pallet.Elements.Add(el);
                    return true;
                }
            }

            // nothing fit
            return false;
        }
    }
}
