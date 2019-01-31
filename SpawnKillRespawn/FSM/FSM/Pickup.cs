using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FSM
{
    class Pickup : GameObject
    {
        public enum Type
        {
            ammo,
            health,
            shield
        }
        public Type thisType;

        public Pickup(Type type, Vector2 position)
        {
            thisType = type;
            this.position = position;
            switch (thisType)
            {
                case Type.ammo:
                    this.texture = AssetsManager.ammo;
                    break;
                case Type.health:
                    this.texture = AssetsManager.health;
                    break;
                case Type.shield:
                    this.texture = AssetsManager.shield;
                    break;
            }
        }
        public override void Update(GameTime gameTime)
        {
            
        }
    }
}
