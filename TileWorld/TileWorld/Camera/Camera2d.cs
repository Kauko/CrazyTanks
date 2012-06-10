using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TileWorld.Camera
{
    public class Camera2d
    {
        protected float _zoom; // Camera Zoom
        public Matrix _transform; // Matrix Transform
        public Vector2 _pos; // Camera Position
        protected float _rotation; // Camera Rotation

        public Camera2d()
        {
            _zoom = 1.0f;
            _rotation = 0.0f;
            _pos = Vector2.Zero;
        }

        // Sets and gets zoom
        public float Zoom
        {
            get { return _zoom; }
            set { _zoom = value; if (_zoom < 0.1f) _zoom = 0.1f; } // Negative zoom will flip image
        }

        public float Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }

        // Auxiliary function to move the camera
        public void Move(Vector2 amount)
        {
            _pos += amount;
        }

        public void MoveDown(float amount)
        {
            _pos += new Vector2((float)(Math.Cos(-_rotation + MathHelper.PiOver2) * amount), (float)(Math.Sin(-_rotation + MathHelper.PiOver2) * amount));
        }

        public void MoveUp(float amount)
        {
            _pos += new Vector2((float)(Math.Cos(-_rotation - MathHelper.PiOver2) * amount), (float)(Math.Sin(-_rotation - MathHelper.PiOver2) * amount));
        }

        public void MoveLeft(float amount)
        {
            _pos += new Vector2((float)(Math.Cos(-_rotation + MathHelper.Pi) * amount), (float)(Math.Sin(-_rotation + MathHelper.Pi) * amount));
        }

        public void MoveRight(float amount)
        {
            _pos += new Vector2((float)(Math.Cos(-_rotation) * amount), (float)(Math.Sin(-_rotation) * amount));
        }

        // Get set position
        public Vector2 Pos
        {
            get { return _pos; }
            set { _pos = value; }
        }

        public Matrix get_transformation(GraphicsDevice graphicsDevice)
        {
            _transform =       // Thanks to o KB o for this solution
                Matrix.Identity *
                Matrix.CreateTranslation(new Vector3(-_pos.X, -_pos.Y, 0)) *
                                         Matrix.CreateRotationZ(Rotation) *
                                         Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                                         Matrix.CreateTranslation(new Vector3(graphicsDevice.Viewport.Width * 0.5f, graphicsDevice.Viewport.Height * 0.5f, 0));
            return _transform;
        }

        public Vector2 ToWorldLocation(Vector2 position)
        {
            return Vector2.Transform(position, Matrix.Invert(_transform));
        }

        public Vector2 ToLocalLocation(Vector2 position)
        {
            return Vector2.Transform(position, _transform);
        }

    }
}
