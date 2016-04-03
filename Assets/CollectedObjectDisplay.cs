using UnityEngine;

namespace Klonamari
{
    public class CollectedObjectDisplay : MonoBehaviour
    {
        public MeshRenderer meshRenderer;
        public MeshFilter meshFilter;

        public void Init(Mesh mesh, Material mat, Vector3 scale)
        {
            meshFilter.mesh = mesh;
            meshRenderer.material = mat;
            transform.localScale = scale; //TODO: adjust position, I guess? based on scale, maybe
        }

        public void Clear()
        {
            meshFilter.mesh = null;
            meshRenderer.material = null;
        }
    }
}
