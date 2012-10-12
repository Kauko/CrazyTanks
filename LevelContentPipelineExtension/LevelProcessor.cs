using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

using TInput = System.String;
using TOutput = LevelLibrary.Level;

namespace LevelContentPipelineExtension
{
    [ContentProcessor(DisplayName = "Level Processor")]
    public class LevelProcessor : ContentProcessor<TInput, TOutput>
    {
        public override TOutput Process(TInput input, ContentProcessorContext context)
        {
            string[] lines = input.Split(new char[] { '\n' });
            int rows = Convert.ToInt32(lines[0]);
            int columns = Convert.ToInt32(lines[1]);

            int[,] levelData = new int[rows, columns];
            for (int row = 0; row < rows; row++)
            {
                string[] values = lines[row + 2].Split(new char[] { ' ' });
                for (int column = 0; column < columns; column++)
                {
                    levelData[row, column] = Convert.ToInt32(values[column]);
                }
            }

            return new LevelLibrary.Level(levelData);
        }
    }
}