using CerealApi.Models;

namespace CerealApi.CsvParser
{

    public interface ICsvParser
    {
        Task ParseCsvAsync(CerealDb db, string csvPath);
    }

    public class CsvParser : ICsvParser
    {
        public async Task ParseCsvAsync(CerealDb db, string csvPath)
        {
        // Read the lines from the CSV file, skipping the 2 header lines.
            try
            {
                using StreamReader reader = new StreamReader(csvPath);
                string text = await reader.ReadToEndAsync();
                string[] lines = text.Split("\n");

                int idCounter = 1;

                foreach (string line in lines[2..^1])
                {
            
                    var parts = line.Split(";");
                    var cereal = new Cereal
                    {
                        Id = idCounter++,
                        Name = parts.ElementAt(0),
                        Mfr = parts.ElementAt(1),
                        Type = parts.ElementAt(2),
                        Calories = int.Parse(parts.ElementAt(3)),
                        Protein = int.Parse(parts.ElementAt(4)),
                        Fat = int.Parse(parts.ElementAt(5)),
                        Sodium = int.Parse(parts.ElementAt(6)),
                        Fiber = float.Parse(parts.ElementAt(7)),
                        Carbo = float.Parse(parts.ElementAt(8)),
                        Sugar = int.Parse(parts.ElementAt(9)),
                        Potass = int.Parse(parts.ElementAt(10)),
                        Vitamins = int.Parse(parts.ElementAt(11)),
                        Shelf = int.Parse(parts.ElementAt(12)),
                        Weight = float.Parse(parts.ElementAt(13)),
                        Cups = float.Parse(parts.ElementAt(14)),
                        Rating = float.Parse(parts.ElementAt(15)),
                        Image = GetCerealImage(parts.ElementAt(0))
                    };

                    await db.Cereals.AddAsync(cereal);
                }

                await db.SaveChangesAsync(); 
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error reading CSV file: {e.Message}");
                throw;
            }; 
        }

        private string GetCerealImage(string cerealName)
        {   
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                cerealName = cerealName.Replace(c, '_');
            }
            
            string pathPng = Path.Combine("CerealPictures",$"{cerealName}.png");
            string pathJpg = Path.Combine("CerealPictures", $"{cerealName}.jpg");
            if (File.Exists(pathPng))
            {
                return pathPng;
            }
            else if (File.Exists(pathJpg))
            {
                return pathJpg;
            }
            else
            {
                return "No picture found";
            }
        }       
    }
}