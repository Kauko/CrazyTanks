using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Solum.Utility;
using Solum.Input;
using Microsoft.Xna.Framework.Input;

namespace Solum.Logging
{
    public class Logger
    {
        float oldFramerate = 0.0f;
        int fpsRounds = 0;

        public GameTime gameTime { get; set; }

        public Logger(){
            GameServices.GetService<KeyboardDevice>().KeyPressed += LogKeyPress;
        }

        public void logFPS()
        {
            float frameRate = 1 / (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (frameRate - oldFramerate > 3.0f || frameRate - oldFramerate < -3.0f)
            {
                
                Console.Write(getTimestamp());
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("FPS: " + oldFramerate +" for " + fpsRounds + " rounds. New FPS: "+frameRate );
                Console.ResetColor();
                fpsRounds = 0;
            }else{
                fpsRounds++;
            }
            oldFramerate = frameRate;

            if (gameTime.IsRunningSlowly)
            {             
                Console.Write(getTimestamp());
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("WARNING !!! GAME RUNNING SLOW");
                Console.ResetColor();
            }
        }

        private string getTimestamp()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            return "[" + gameTime.TotalGameTime.ToString("c") +"]: ";
        }

        public void logMsg(String msg)
        {
            Console.Write(getTimestamp());
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(msg);
            Console.ResetColor();
        }

        public void logMsg(String msg, ConsoleColor foreground){
            Console.Write(getTimestamp());
            Console.ResetColor();
            Console.ForegroundColor = foreground;
            Console.WriteLine(msg);
            Console.ResetColor();
        }

        public void logMsg(String msg, ConsoleColor foreground, ConsoleColor background)
        {
            Console.Write(getTimestamp());
            Console.ResetColor();
            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;
            Console.WriteLine(msg);
            Console.ResetColor();
        }

        void LogKeyPress(object sender,InputDeviceEventArgs<Keys, KeyboardState> e)
        {
            this.logMsg(e.Object.ToString());
        }

    }
}
