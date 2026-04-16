#if MAP
using UnityEngine;

namespace Core.Map
{
    [ExecuteInEditMode]
    public abstract class GridPainter : MonoBehaviour
    {
        [SerializeField] protected int mapLength = 40;
        [SerializeField] protected int mapWidth = 40;
        [SerializeField] protected float gridSize = 1f;
        private BoxCollider box;

        public int MapWidth
        {
            get { return mapWidth; }
            set { mapWidth = value; }
        }

        public int MapLength
        {
            get { return mapLength; }
            set { mapLength = value; }
        }

        public float GridSize
        {
            get { return gridSize; }
            set { gridSize = value; }
        }

        public int RowCount
        {
            get { return Mathf.FloorToInt(mapLength / gridSize); }
        }

        public int ColumnCount
        {
            get { return Mathf.FloorToInt(mapWidth / gridSize); }
        }

        private void Awake()
        {
            BuildGrid();
        }

        public void BuildGrid()
        {
            if (gridSize <= 0 || mapWidth <= 0 || mapLength <= 0)
            {
                return;
            }

            OnBuildGrids();
            UpdatePosition();
            UpdateBoxSize();
        }

        protected void UpdateBoxSize()
        {
            if (null != box)
            {
                box.center = new Vector3(mapWidth * 0.5f, 0, -mapLength * 0.5f);
                box.size = new Vector3(mapWidth, 0.05f, mapLength);
            }
        }

        protected abstract void OnBuildGrids();


        public abstract void UpdatePosition();

        public abstract GridData GenerateGridsData();

        public virtual void SetGridsConfig(string jsonText)
        {
        }

        public void SetBoxActive(bool enable)
        {
            if (null == box)
            {
                box = gameObject.AddComponent<BoxCollider>();
                box.isTrigger = true;
                UpdateBoxSize();
            }

            box.enabled = enable;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Vector3 center = transform.position + new Vector3(mapWidth * 0.5f, 0, -mapLength * 0.5f);
            Gizmos.DrawWireCube(center, new Vector3(mapWidth, 0.05f, mapLength));

            Vector3 size = new Vector3(gridSize - 0.05f, 0, gridSize - 0.05f);
            if (null != box && box.enabled)
            {
                OnDrawGrids(size);
            }
        }

        protected abstract void OnDrawGrids(Vector3 size);
       
    }
}
#endif