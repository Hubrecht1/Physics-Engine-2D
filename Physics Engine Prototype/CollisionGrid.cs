using System;
using System.Numerics;

namespace Physics_Engine
{
    public struct CollisionGrid
    {
        Vector2 TopLeftCell;
        Vector2 BottomRightCell;
        public float cellSize;
        public uint xCells;
        public uint yCells;

        public List<List<CollisionCell>> collisionCells = new List<List<CollisionCell>>();

        public CollisionGrid(Vector2 _TopLeftCell, Vector2 _BottomRightCell, float _CellSize)
        {
            TopLeftCell = _TopLeftCell;
            BottomRightCell = _BottomRightCell;
            cellSize = _CellSize;

            xCells = (uint)((BottomRightCell.X - TopLeftCell.X) / cellSize);
            yCells = (uint)((BottomRightCell.Y - TopLeftCell.Y) / cellSize);


            for (int row = 0; row <= xCells; row++)
            {
                List<CollisionCell> rowList = new List<CollisionCell>();
                for (int col = 0; col <= yCells; col++)
                {
                    // Initialize each cell in the row
                    rowList.Add(new CollisionCell());

                }
                collisionCells.Add(rowList);
            }

        }

        public Vector2 GetCellCoordinate(Vector2 position)
        {
            int x = (int)((position.X - TopLeftCell.X) / cellSize);
            int y = (int)((position.Y - TopLeftCell.Y) / cellSize);

            return new Vector2(x, y);
        }

        public void AddColliderToCell(BoxCollider boxCollider, out int x, out int y)
        {
            Vector2 cellCoor = GetCellCoordinate(boxCollider.rigidBody.entityTransform.position);
            x = (int)cellCoor.X;
            y = (int)cellCoor.Y;
            collisionCells[x][y].boxColliders.Add(boxCollider);


        }

        public void AddColliderToCell(CircleCollider circleCollider, out int x, out int y)
        {
            Vector2 cellCoor = GetCellCoordinate(circleCollider.rigidBody.entityTransform.position);
            x = (int)cellCoor.X;
            y = (int)cellCoor.Y;
            collisionCells[x][y].circleColliders.Add(circleCollider);

        }

        public List<BoxCollider> GetBoxCollidersFromCell(int x, int y)
        {
            return collisionCells[x][y].boxColliders;

        }

        public List<CircleCollider> GetCircleCollidersFromCell(int x, int y)
        {
            return collisionCells[x][y].circleColliders;

        }

    }


    public struct CollisionCell
    {
        public List<BoxCollider> boxColliders;
        public List<CircleCollider> circleColliders;

        public CollisionCell()
        {
            boxColliders = new List<BoxCollider>();
            circleColliders = new List<CircleCollider>();


        }
    }
}

