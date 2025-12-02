using UnityEngine;
using System.Collections.Generic;

public static class MeshCombinerUtil
{
    public static void CombineVisualMeshes(GameObject parentObj)
    {
        MeshFilter[] meshFilters = parentObj.GetComponentsInChildren<MeshFilter>();

        List<CombineInstance> combine = new List<CombineInstance>();
        Material savedMaterial = null;

        // 1. NAJPIERW zbierzemy wszystkie dane zanim coś zniszczymy
        foreach (MeshFilter mf in meshFilters)
        {
            // Pomijamy obiekty logiki (Tile)
            if (mf.GetComponent<Tile>() != null)
                continue;

            MeshRenderer renderer = mf.GetComponent<MeshRenderer>();
            if (renderer == null) 
                continue;

            // Zapamiętaj materiał zanim go zniszczymy
            if (savedMaterial == null)
                savedMaterial = renderer.sharedMaterial;

            // Dodawaj mesh do combine listy
            CombineInstance ci = new CombineInstance();
            ci.mesh = mf.sharedMesh;
            ci.transform = mf.transform.localToWorldMatrix;

            combine.Add(ci);
        }

        // Jeśli nie ma co łączyć — wyjdź
        if (combine.Count == 0)
        {
            Debug.LogWarning("MeshCombiner: No meshes found to merge.");
            return;
        }

        // 2. TWORZYMY POŁĄCZONY MESH
        Mesh combinedMesh = new Mesh();
        combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        combinedMesh.CombineMeshes(combine.ToArray(), true, true);

        // 3. DODAJEMY renderer + filter DO PARENT
        MeshFilter parentFilter = parentObj.AddComponent<MeshFilter>();
        MeshRenderer parentRenderer = parentObj.AddComponent<MeshRenderer>();

        parentFilter.sharedMesh = combinedMesh;

        if (savedMaterial != null)
            parentRenderer.sharedMaterial = savedMaterial;
        else
            Debug.LogError("MeshCombiner: No material found!");

        // 4. NA KOŃCU dopiero usuwamy mesh filtery i renderery z dzieci
        foreach (MeshFilter mf in meshFilters)
        {
            if (mf.GetComponent<Tile>() != null) continue;

            MeshRenderer renderer = mf.GetComponent<MeshRenderer>();
            if (renderer != null) Object.Destroy(renderer);
            Object.Destroy(mf);
        }

        Debug.Log("Mesh combining complete ✔ (safe mode)");
    }
}
