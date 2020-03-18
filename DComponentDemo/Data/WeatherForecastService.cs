using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DComponentDemo.Data
{
    public class WeatherForecastService
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public Task<WeatherForecast[]> GetForecastAsync(DateTime startDate)
        {
            var rng = new Random();
            var result = new List<WeatherForecast>();
            var parents = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Id=index.ToString(),
                Date = startDate.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            });
            result.AddRange(parents);
            foreach (var parent in parents)
            {
                var childs = Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    ParentId=parent.Id,
                    Date = startDate.AddDays(index),
                    TemperatureC = rng.Next(-20, 55),
                    Summary = Summaries[rng.Next(Summaries.Length)]
                });
                result.AddRange(childs);
            }
            return Task.FromResult(result.ToArray());
        }
    }
}
