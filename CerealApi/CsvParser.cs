using CerealApi.Models;

/*This class is responsible for parsing the CSV file and adding the data to the database.
It will also search the "CerealPictures* directory and add a picture to the cereal class if a image with the same name exists*/

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
            /* I use a try-catch block and try to open the csv file, 
            split it into lines and then fields to extract the values and make a cereal object.*/
            try
            {
                using StreamReader reader = new StreamReader(csvPath);
                string text = await reader.ReadToEndAsync();
                string[] lines = text.Split("\n");

                int idCounter = 1;

                //I use Range expression to skip the first two lines of headers.
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
            catch (Exception ex)
            {
                Results.Problem($"An error occurred while parsing the CSV: {ex.Message}");
            }; 
        }


        /*This method will search the "CerealPictures" directory 
        for a picture with the same name as the cereal and return the path*/
        private static string GetCerealImage(string cerealName)
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