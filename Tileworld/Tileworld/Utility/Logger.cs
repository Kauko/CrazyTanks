/// By Teemu Kaukoranta, member of the Oulu GamedevClub Stage
/// http://www.gamedevcenter.org
/// 
/// Part of the S.o.l.u.m project
/// Licensed under WTFPL - Do What The Fuck You Want To Public License
/// It would be nice if you don't remove this comment section though


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
        int inactiveRounds = 0;
        int warningRounds = 0;
        bool fileLoggingEnabled = false;
        string fileName = "";

        public GameTime gameTime { get; set; }

        public Logger(bool logToFile){
            GameServices.GetService<KeyboardDevice>().KeyPressed += LogKeyPress;
            this.fileLoggingEnabled = logToFile;
            if(this.fileLoggingEnabled){
                this.fileName = DateTime.Now + ".txt";
                this.fileName = this.fileName.Replace(":", "");
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(this.fileName, true))
                {
                    file.WriteLine(this.fileName);
                }
            }
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

                if(this.fileLoggingEnabled){
                    logToFile("FPS: " + oldFramerate + " for " + fpsRounds + " rounds. New FPS: " + frameRate);
                }

                fpsRounds = 0;
            }else{
                fpsRounds++;
            }
            oldFramerate = frameRate;

            /* If we don't do active check as well, warnings will be logged all the time when game is inactive
             * this.inactiveRounds is added because otherwise when the game becomes active again, warnings will be logged for about a second*/
            if (gameTime.IsRunningSlowly && GameServices.GetService<Game>().IsActive && this.inactiveRounds == 0 && this.warningRounds == 0)
            {
                this.warningRounds = 60; //We don't want to spam the warnings too much
                Console.Write(getTimestamp());
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("WARNING !!! GAME RUNNING SLOW");
                Console.ResetColor();

                if (this.fileLoggingEnabled)
                {
                    logToFile("WARNING !!! GAME RUNNING SLOW");
                }
            }
            else if (!GameServices.GetService<Game>().IsActive)
            {
                this.inactiveRounds = 60;
            }
            else if (this.inactiveRounds > 0)
                this.inactiveRounds--;

            if (this.warningRounds > 0)
                this.warningRounds--;
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

            if (this.fileLoggingEnabled)
            {
                logToFile(msg);
            }
        }

        public void logMsg(String msg, ConsoleColor foreground){
            Console.Write(getTimestamp());
            Console.ResetColor();
            Console.ForegroundColor = foreground;
            Console.WriteLine(msg);
            Console.ResetColor();

            if (this.fileLoggingEnabled)
            {
                logToFile(msg);
            }
        }

        public void logMsg(String msg, ConsoleColor foreground, ConsoleColor background)
        {
            Console.Write(getTimestamp());
            Console.ResetColor();
            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;
            Console.WriteLine(msg);
            Console.ResetColor();

            if (this.fileLoggingEnabled)
            {
                logToFile(msg);
            }
        }

        void LogKeyPress(object sender,InputDeviceEventArgs<Keys, KeyboardState> e)
        {
            this.logMsg(e.Object.ToString());

            if (this.fileLoggingEnabled)
            {
                logToFile(e.Object.ToString());
            }
        }

        void logToFile(String msg){
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(this.fileName, true))
            {
                file.Write(getTimestamp());
                file.WriteLine(msg);
            }
        }

    }
}
