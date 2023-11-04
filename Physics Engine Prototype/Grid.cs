using System;
using System.Numerics;
using System.Linq;
using System.Reflection;

namespace Physics_Engine
{
    public struct CollisionGrid
    {
        public double cell_width { private set; get; }
        public double cell_height { private set; get; }

        public List<(Vector2, CollisionCell)> collisionCells { private set; get; } = new List<(Vector2, CollisionCell)>();

        public CollisionGrid(uint _cell_width, uint _cell_height)
        {
            cell_width = _cell_width;
            cell_height = _cell_height;

        }

        public Vector2 GetCellPosition(Vector2 position)
        {
            return new Vector2((int)(position.X / cell_width), (int)(position.Y / cell_height));

        }

        public void ClearGrid()
        {
            collisionCells.Clear();

        }

        public CollisionCell? GetCell(int pos_X, int pos_Y)
        {
            Vector2 gridPosition = new Vector2(pos_X, pos_Y);
            if (!collisionCells.Any(x => x.Item1 == gridPosition))
            {
                return null;
            }
            else
            {
                int index = collisionCells.FindIndex(x => x.Item1 == gridPosition);
                return collisionCells[index].Item2;

            }

        }



        public void AddColliderToCell(int x, int y, CircleCollider _circleCollider)
        {
            Vector2 gridPosition = new Vector2(x, y);
            if (!collisionCells.Any(x => x.Item1 == gridPosition))
            {
                CollisionCell cell = new CollisionCell(x, y);
                cell.AddCollider(_circleCollider);
                collisionCells.Add((gridPosition, cell));
            }
            else
            {
                int index = collisionCells.FindIndex(x => x.Item1 == gridPosition);
                collisionCells[index].Item2.AddCollider(_circleCollider);

            }

        }

        public void AddColliderToCell(int x, int y, BoxCollider _boxCollider)
        {
            Vector2 gridPosition = new Vector2(x, y);
            if (!collisionCells.Any(x => x.Item1 == gridPosition))
            {
                CollisionCell cell = new CollisionCell(x, y);
                cell.AddCollider(_boxCollider);
                collisionCells.Add((gridPosition, cell));
            }
            else
            {
                int index = collisionCells.FindIndex(x => x.Item1 == gridPosition);
                collisionCells[index].Item2.AddCollider(_boxCollider);

            }
        }

    }

    public struct CollisionCell
    {
        public double x { private set; get; }
        public double y { private set; get; }

        public List<CircleCollider> circleColliders = new List<CircleCollider>();
        public List<BoxCollider> boxColliders = new List<BoxCollider>();

        public CollisionCell(int _x, int _y)
        {
            x = _x;
            y = _y;
        }

        public void AddCollider(CircleCollider _circleCollider)
        {
            circleColliders.Add(_circleCollider);

        }

        public void AddCollider(BoxCollider _boxCollider)
        {
            boxColliders.Add(_boxCollider);

        }

        public List<CircleCollider> GetCircleColliders()
        {
            return circleColliders;

        }

        public List<BoxCollider> GetBoxColliders()
        {
            return boxColliders;

        }


    }


}

