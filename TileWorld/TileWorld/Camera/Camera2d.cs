using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Tileworld.Utility;
/*  Thanks to David Amador and others
 *  http://www.david-amador.com/2009/10/xna-camera-2d-with-zoom-and-rotation/
 */
namespace Tileworld.Camera
{
    public class Camera2d
    {
        protected float _zoom; // Camera Zoom
        public Matrix _transform; // Matrix Transform
        public Vector2 _pos; // Camera Position
        protected float _rotation; // Camera Rotation

        private MouseState oldMouse;

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

        public Matrix get_transformation()
        {
            _transform =       // Thanks to o KB o for this solution
                Matrix.Identity *
                Matrix.CreateTranslation(new Vector3(-_pos.X, -_pos.Y, 0)) *
                                         Matrix.CreateRotationZ(Rotation) *
                                         Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                                         Matrix.CreateTranslation(new Vector3(GameServices.GetService<GraphicsDevice>().Viewport.Width * 0.5f, GameServices.GetService<GraphicsDevice>().Viewport.Height * 0.5f, 0));
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

        public void updateCamera(KeyboardState keyboard, MouseState mouse)
        {

            //oldMouse = (oldMouse == null) ? mouse : oldMouse;

            //Keyboard camera movement
            if (keyboard.IsKeyDown(Keys.A))
                GameServices.GetService<Camera2d>().MoveLeft(C.camKeyboardScrollSpeed * 2 / GameServices.GetService<Camera2d>().Zoom);

            if (keyboard.IsKeyDown(Keys.D))
                GameServices.GetService<Camera2d>().MoveRight(C.camKeyboardScrollSpeed * 2 / GameServices.GetService<Camera2d>().Zoom);

            if (keyboard.IsKeyDown(Keys.W))
                GameServices.GetService<Camera2d>().MoveUp(C.camKeyboardScrollSpeed * 2 / GameServices.GetService<Camera2d>().Zoom);

            if (keyboard.IsKeyDown(Keys.S))
                GameServices.GetService<Camera2d>().MoveDown(C.camKeyboardScrollSpeed * 2 / GameServices.GetService<Camera2d>().Zoom);

            //Keyboard zoom
            if (keyboard.IsKeyDown(Keys.R))
                if (GameServices.GetService<Camera2d>().Zoom > C.camZoomSpeedThreshold)
                {
                    GameServices.GetService<Camera2d>().Zoom += C.camKeyboardCloseZoomSpeed;
                }
                //zoom for far away
                else
                {
                    GameServices.GetService<Camera2d>().Zoom += C.camKeyboardFarZoomSpeed;
                }

            if (keyboard.IsKeyDown(Keys.F))
                if (GameServices.GetService<Camera2d>().Zoom > C.camZoomSpeedThreshold)
                {
                    GameServices.GetService<Camera2d>().Zoom -= C.camKeyboardCloseZoomSpeed;
                }
                //zoom for far away
                else
                {
                    GameServices.GetService<Camera2d>().Zoom -= C.camKeyboardFarZoomSpeed;
                }

            //Keyboard camera rotation
            if (keyboard.IsKeyDown(Keys.Q))
                GameServices.GetService<Camera2d>().Rotation -= C.camKeyboardRotationSpeed;

            if (keyboard.IsKeyDown(Keys.E))
                GameServices.GetService<Camera2d>().Rotation += C.camKeyboardRotationSpeed;

            //Mouse camera movement
            if (mouse.Y < C.camMouseScrollBorderWidth && mouse.Y > 0)
                GameServices.GetService<Camera2d>().MoveUp(C.camKeyboardScrollSpeed);

            if (mouse.Y > GameServices.GetService<GraphicsDevice>().Viewport.Height - C.camMouseScrollBorderWidth && mouse.Y < GameServices.GetService<GraphicsDevice>().Viewport.Height)
                GameServices.GetService<Camera2d>().MoveDown(C.camKeyboardScrollSpeed);

            if (mouse.X < C.camMouseScrollBorderWidth && mouse.X > 0)
                GameServices.GetService<Camera2d>().MoveLeft(C.camKeyboardScrollSpeed);

            if (mouse.X > GameServices.GetService<GraphicsDevice>().Viewport.Width - C.camMouseScrollBorderWidth && mouse.X < GameServices.GetService<GraphicsDevice>().Viewport.Width)
                GameServices.GetService<Camera2d>().MoveRight(C.camKeyboardScrollSpeed);

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                if (Mouse.GetState().X - oldMouse.X > 0)
                    GameServices.GetService<Camera2d>().MoveLeft((Mouse.GetState().X - oldMouse.X) * (C.camMouseDragSpeed * 2 / GameServices.GetService<Camera2d>().Zoom));
                else if (Mouse.GetState().X - oldMouse.X < 0)
                    GameServices.GetService<Camera2d>().MoveRight(-((Mouse.GetState().X - oldMouse.X) * (C.camMouseDragSpeed * 2 / GameServices.GetService<Camera2d>().Zoom))); //NOTE: If you want to change how mousedrag works,
                //It's probably better to change C.camDragSpeed than touch these method calls

                if (Mouse.GetState().Y - oldMouse.Y > 0)
                    GameServices.GetService<Camera2d>().MoveUp((Mouse.GetState().Y - oldMouse.Y) * (C.camMouseDragSpeed * 2 / GameServices.GetService<Camera2d>().Zoom));
                else if (Mouse.GetState().Y - oldMouse.Y < 0)
                    GameServices.GetService<Camera2d>().MoveDown(-((Mouse.GetState().Y - oldMouse.Y) * (C.camMouseDragSpeed * 2 / GameServices.GetService<Camera2d>().Zoom))); //Drag is slower if we are zoomed in

            }

            if (Mouse.GetState().ScrollWheelValue != oldMouse.ScrollWheelValue)
            {
                //Zoom for close distance
                if (GameServices.GetService<Camera2d>().Zoom > C.camZoomSpeedThreshold)
                {
                    GameServices.GetService<Camera2d>().Zoom += (Mouse.GetState().ScrollWheelValue - oldMouse.ScrollWheelValue) / 120.0f / C.camMouseCloseZoomSpeed;
                }
                //zoom for far away
                else
                {
                    GameServices.GetService<Camera2d>().Zoom *= 1 + (Mouse.GetState().ScrollWheelValue - oldMouse.ScrollWheelValue) / 120.0f / C.camMouseFarZoomSpeed; //120.0f is some weird magic number..
                }
            }
            if (Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                GameServices.GetService<Camera2d>().Rotation += ((Mouse.GetState().X - oldMouse.X) / C.camMouseRotateSpeed);
                GameServices.GetService<Camera2d>().Rotation += ((Mouse.GetState().Y - oldMouse.Y) / C.camMouseRotateSpeed);
            }

            oldMouse = mouse;
        }

    }
}
