(() => {
    const holder = window.__PalletHolder || { Pallets: [], CurrentPalletIndex: 0 };
    let currentIndex = window.__initialIndex ?? holder.CurrentPalletIndex ?? 0;

    // DOM
    const palletSelect = document.getElementById('palletSelect');
    const materialSelect = document.getElementById('materialSelect');
    const saveBtn = document.getElementById('saveBtn');
    const scaleRange = document.getElementById('scaleRange');

    // Babylon setup
    const canvas = document.getElementById('renderCanvas');
    const engine = new BABYLON.Engine(canvas, true);
    const scene = new BABYLON.Scene(engine);

    const camera = new BABYLON.ArcRotateCamera("camera", Math.PI / 2, Math.PI / 3, 6, BABYLON.Vector3.Zero(), scene);
    camera.attachControl(canvas, true);
    const light = new BABYLON.HemisphericLight("h", new BABYLON.Vector3(1, 1, 0), scene);

    // store created objects so we can dispose everything safely
    let createdMeshes = [];
    let createdMaterials = [];

    function disposeAll() {
        // dispose meshes
        createdMeshes.forEach(m => {
            try { m.dispose(true, true); } catch (e) { /* ignore */ }
        });
        createdMeshes = [];

        // dispose materials
        createdMaterials.forEach(mat => {
            try { mat.dispose(true, true); } catch (e) { /* ignore */ }
        });
        createdMaterials = [];
    }

    // normalize server pallet payload to a well-known shape
    function normalizePallet(raw) {
        if (!raw) return null;

        // Map enum strings to numbers if needed
        const enumMap = {
            "Wood": 0, "Plastic": 1, "Metal": 2,
            "0": 0, "1": 1, "2": 2
        };

        // MaterialType might be numeric or string
        let mt = raw.MaterialType ?? raw.materialType ?? raw.materialtype;
        if (typeof mt === 'string') {
            // try parse number
            const n = parseInt(mt, 10);
            if (!Number.isNaN(n)) mt = n;
            else mt = enumMap[mt] ?? 0;
        }

        // Elements may exist in various casings — normalize each element
        const rawEls = raw.Elements ?? raw.elements ?? [];
        const elements = rawEls.map((el, idx) => ({
            Id: el.Id ?? el.id ?? idx,
            Width: el.Width ?? el.width ?? el.w ?? 0.5,
            Height: el.Height ?? el.height ?? el.h ?? 0.4,
            Depth: el.Depth ?? el.depth ?? el.d ?? 0.4,
            // optional position if present
            X: el.X ?? el.x ?? 0,
            Y: el.Y ?? el.y ?? 0,
            Z: el.Z ?? el.z ?? 0,
            // optional color
            R: el.R ?? el.r ?? 1,
            G: el.G ?? el.g ?? 1,
            B: el.B ?? el.b ?? 1
        }));

        return {
            Id: raw.Id ?? raw.id ?? 0,
            Name: raw.Name ?? raw.name ?? "",
            MaterialType: Number(mt),
            Elements: elements
        };
    }

    function createPalletVisualization(palletRaw) {
        disposeAll();
        if (!palletRaw) return;

        const pallet = normalizePallet(palletRaw);

        // draw pallet base
        const baseMat = new BABYLON.StandardMaterial(`mat_base_${pallet.Id}_${Date.now()}`, scene);
        createdMaterials.push(baseMat);
        switch (pallet.MaterialType) {
            case 0: baseMat.diffuseColor = new BABYLON.Color3(0.6, 0.4, 0.2); break; // Wood
            case 1: baseMat.diffuseColor = new BABYLON.Color3(0.1, 0.6, 0.9); break; // Plastic
            case 2: baseMat.diffuseColor = new BABYLON.Color3(0.7, 0.7, 0.7); break; // Metal
            default: baseMat.diffuseColor = new BABYLON.Color3(0.8, 0.8, 0.8);
        }

        const base = BABYLON.MeshBuilder.CreateBox(`palletBase_${pallet.Id}_${Date.now()}`, {
            width: 1.2, height: 0.2, depth: 1.0
        }, scene);
        base.position.y = 0.1;
        base.material = baseMat;
        createdMeshes.push(base);

        // draw elements (simple boxes) - position with optional coordinates if present,
        // otherwise layout in grid
        const spacingX = 0.9;
        const spacingZ = 0.7;
        for (let i = 0; i < (pallet.Elements || []).length; i++) {
            const el = pallet.Elements[i];
            const w = Number(el.Width) || 0.5;
            const h = Number(el.Height) || 0.4;
            const d = Number(el.Depth) || 0.4;

            const mesh = BABYLON.MeshBuilder.CreateBox(`el_${pallet.Id}_${el.Id}_${i}`, {
                width: w, height: h, depth: d
            }, scene);

            // prefer explicit position if provided, otherwise grid layout
            if (el.X !== 0 || el.Y !== 0 || el.Z !== 0) {
                mesh.position.x = el.X;
                mesh.position.y = el.Y;
                mesh.position.z = el.Z;
            } else {
                mesh.position.x = -spacingX / 2 + (i % 3) * (w + 0.05);
                mesh.position.z = -spacingZ / 2 + Math.floor(i / 3) * (d + 0.05);
                mesh.position.y = 0.2 + h / 2;
            }

            const mat = new BABYLON.StandardMaterial(`mat_el_${pallet.Id}_${el.Id}_${i}`, scene);
            mat.diffuseColor = new BABYLON.Color3(Number(el.R), Number(el.G), Number(el.B));
            mesh.material = mat;

            createdMaterials.push(mat);
            createdMeshes.push(mesh);
        }
    }

    engine.runRenderLoop(() => scene.render());
    window.addEventListener('resize', () => engine.resize());

    // initial render (use normalized index)
    createPalletVisualization(holder.Pallets?.[currentIndex]);

    // update UI for a given index and pallet data
    function refreshUIForIndex(idx, palletData) {
        currentIndex = idx;
        // set selects safely
        if (palletData) {
            // ensure materialSelect value matches the server shape (we use enum names)
            // if materialSelect options are strings like "Wood" we should set to that name
            // try to set by name if possible, otherwise set numeric string
            const materialOptByValue = (v) => {
                for (let i = 0; i < materialSelect.options.length; i++) {
                    if (materialSelect.options[i].value.toString() === v.toString()) return true;
                }
                return false;
            };

            // prefer matching name if server returned name in string format
            const mtPossibleNames = {
                0: "Wood", 1: "Plastic", 2: "Metal"
            };
            const normalized = normalizePallet(palletData);
            const nameCandidate = mtPossibleNames[normalized.MaterialType];
            if (materialOptByValue(nameCandidate)) {
                materialSelect.value = nameCandidate;
            } else if (materialOptByValue(normalized.MaterialType)) {
                materialSelect.value = normalized.MaterialType;
            } else {
                // fallback - set to first option
                materialSelect.selectedIndex = 0;
            }
        }

        // update palletSelect if present
        if (palletSelect) {
            palletSelect.value = idx.toString();
        }

        // render the pallet immediately with provided data (or from holder)
        const p = palletData ?? holder.Pallets?.[idx];
        createPalletVisualization(p);
    }

    palletSelect?.addEventListener('change', (e) => {
        const idx = parseInt(e.target.value, 10);
        // fetch behind a single source of truth, then refresh UI once
        fetch(`/Pallet/GetPallet?index=${idx}`)
            .then(r => {
                if (!r.ok) throw new Error('Failed to fetch pallet');
                return r.json();
            })
            .then(p => {
                // store in holder for later use
                holder.Pallets[idx] = p;
                refreshUIForIndex(idx, p);
            })
            .catch(err => {
                console.error(err);
                // fallback: try to render from local holder if available
                refreshUIForIndex(idx, holder.Pallets?.[idx]);
            });
    });

    materialSelect?.addEventListener('change', (e) => {
        // immediate client-side preview: update holder and re-render
        const val = e.target.value;
        const pallet = holder.Pallets[currentIndex];
        if (!pallet) return;

        // server may expect enum name or number; we update local pallet to reflect the selection
        // if option is text like "Wood", keep that, otherwise try parse int
        const numeric = Number(val);
        pallet.MaterialType = Number.isNaN(numeric) ? val : numeric;

        createPalletVisualization(pallet);
    });

    // TODO: make sure to adjust this when adding more fields to pallet
    saveBtn?.addEventListener('click', () => {
        const dto = {
            index: currentIndex,
            // prefer sending name if select uses names, otherwise number
            materialType: materialSelect.value
        };

        fetch('/Pallet/UpdatePallet', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(dto)
        })
            .then(r => r.json())
            .then(result => {
                if (result.success) {
                    console.log('Saved', result);
                } else {
                    console.error('Save failed', result);
                }
            })
            .catch(err => console.error(err));
    });

    // initial UI apply with the initial pallet (if any)
    const initialPallet = holder.Pallets?.[currentIndex];
    if (initialPallet) refreshUIForIndex(currentIndex, initialPallet);
})();
