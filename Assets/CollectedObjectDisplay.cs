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

            //make the mesh fit in front of our camera. only normalizing larger objects so that the first little boxes appear smaller when collected.
            if (scale.magnitude > 1.0f)
            {
                scale = scale.normalized;
            }
            transform.localScale = scale;
        }

        public void Clear()
        {
            meshFilter.mesh = null;
            meshRenderer.material = null;
        }
    }
}
