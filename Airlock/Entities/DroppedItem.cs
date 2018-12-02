using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Airlock.Render;
using Microsoft.Xna.Framework;
using NetCode;

namespace Airlock.Entities
{
    [EnumerateSynchEntity]
    public class DroppedItem : Entity
    {
        public DroppedItem()
        {
        }

        public DroppedItem(Vector2 position)
        {
            Position = position;
            UpdateNetMotion(NetTime.Now());
            Radius = 10;
        }

        public override void Render(Camera camera)
        {
            if (MouseOver)
            {
                Drawing.DrawString(camera.Batch, "Item", camera.Map(Position), Color.White);
            }
            Drawing.DrawSquare(camera.Batch, camera.Map(Position), camera.Scale * new Vector2(Radius * 2), 0, Color.Yellow);
        }
    }
}
