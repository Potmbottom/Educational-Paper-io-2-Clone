using PaperClone.Domain;
using UnityEngine;
using UniRx;
using VContainer;
using Clipper2Lib;
using System.Collections.Generic;

namespace PaperClone.Presentation
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class TerritoryView : MonoBehaviour
    {
        private PlayerModel _model;
        private MeshFilter _meshFilter;
        private bool _initialized = false;
        private const int CP = 4;

        [Inject]
        public void Construct(PlayerModel model)
        {
            _model = model;
        }

        private void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
        }

        private void Start()
        {
            if (_model != null && !_initialized)
            {
                BindModel();
            }
        }

        public void Initialize(PlayerModel model)
        {
            _model = model;
            if (!_initialized)
            {
                BindModel();
            }
        }

        private void BindModel()
        {
            _initialized = true;
            
            var renderer = GetComponent<MeshRenderer>();
            var mat = renderer.material;
            mat.color = _model.PlayerColor;
            
            transform.position = new Vector3(0, -0.01f, 0);

            _model.OwnedTerritory
                .Subscribe(UpdateMesh)
                .AddTo(this);
        }

        private void UpdateMesh(PathsD paths)
        {
            if (paths == null || paths.Count == 0)
            {
                if (_meshFilter.mesh != null) _meshFilter.mesh.Clear();
                return;
            }
            
            PathsD solution;
            var result = Clipper.Triangulate(paths, CP, out solution);

            if (result != TriangulateResult.success || solution == null || solution.Count == 0)
                return;

            var vertices = new List<Vector3>();
            var triangles = new List<int>();
            var vertIndex = 0;

            foreach (var tri in solution)
            {
                if (tri.Count < 3) continue;

                var a = new Vector3((float)tri[0].x, 0, (float)tri[0].y);
                var b = new Vector3((float)tri[1].x, 0, (float)tri[1].y);
                var c = new Vector3((float)tri[2].x, 0, (float)tri[2].y);

                vertices.Add(a);
                vertices.Add(b);
                vertices.Add(c);

                var cross = Vector3.Cross(b - a, c - a);
                if (cross.y >= 0)
                {
                    triangles.Add(vertIndex);
                    triangles.Add(vertIndex + 1);
                    triangles.Add(vertIndex + 2);
                }
                else
                {
                    triangles.Add(vertIndex);
                    triangles.Add(vertIndex + 2);
                    triangles.Add(vertIndex + 1);
                }
                vertIndex += 3;
            }

            var mesh = new Mesh();
            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            _meshFilter.mesh = mesh;
        }
    }
}