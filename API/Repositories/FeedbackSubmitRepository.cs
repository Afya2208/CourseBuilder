using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Models.Entities;

namespace API.Repositories
{
    public class FeedbackSubmitRepository(CoursesDbContext context) :BaseRepository<FeedbackSubmit>(context)
    {
        public void RainbowHello()
        {
            var colors = (ConsoleColor[])Enum.GetValues(typeof(ConsoleColor));
            for (int i = 0; i < colors.Length; i++)
            {
                Console.ForegroundColor = colors[i];
                Console.WriteLine("Hello world!");
            }
        }
    }
}