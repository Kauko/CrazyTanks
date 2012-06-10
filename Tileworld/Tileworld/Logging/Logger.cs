using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Tileworld.Logging
{
    public class Logger
    {
        float oldFramerate = 0.0f;
        int fpsRounds = 0;

        public GameTime gameTime { get; set; }

        public void logFPS()
        {
            float frameRate = 1 / (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (frameRate - oldFramerate > 3.0f || frameRate - oldFramerate < -3.0f)
            {
                Console.Write(getTimestamp());
                Console.WriteLine("FPS: " + oldFramerate +" for " + fpsRounds + " rounds. New FPS: "+frameRate );
                fpsRounds = 0;
            }else{
                fpsRounds++;
            }
            oldFramerate = frameRate;
        }

        private string getTimestamp()
        {
            return "[" + gameTime.TotalGameTime.ToString("c") +"]: ";
        }

        public void logMsg(String msg){
            Console.Write(getTimestamp());
            Console.WriteLine(msg);
        }


    }
}
