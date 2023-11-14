using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace TodoList
{
    /*
     * Hjälpmetoder som ligger lite utanför programklassen. Därför ligger de 
     * inte i "Partial"-filen. Dock hårfint.
    */
    internal class ToDoUtils
    {
        internal static List<ToDoProject> ParseToDoInfo(string jsonFile)
        {
            List<ToDoProject>? list = null;
            try
            {
                string strToParse = File.ReadAllText(jsonFile);

                var options = new JsonSerializerOptions();
                options.Converters.Add(new JsonStringEnumConverter());

                list = JsonSerializer.Deserialize<List<ToDoProject>>(strToParse, options);
            }
            catch (JsonException je)
            {
                //Fel på datan
                throw new ToDoDataIsCorupptedException("Data is corrupted", je);
            }
            catch (FileNotFoundException fe)
            {
                throw new ToDoDataIsCorupptedException("Data file is missing", fe);
            }
            catch 
            {
                throw;
            }
            return (list ?? new List<ToDoProject>());
        }

        internal static void SerializeListToFile(List<ToDoProject> list, string dataSource)
        {
            //skriver ut enumtexten
            var options = new JsonSerializerOptions();
            options.Converters.Add(new JsonStringEnumConverter());
            options.WriteIndented = true;

            try
            {
                using (var stream = File.Create(dataSource))
                {
                    JsonSerializer.Serialize<List<ToDoProject>>(stream, list, options);
                }
            }
            catch (JsonException je)
            {
                throw new ToDoDataIsCorupptedException("Something went wrong", je);
            }
            catch 
            {
                throw;
            }
        }


        internal static Dictionary<string, string> GetDataSource(string dataDirectory)
        {
            Dictionary<string, string> files = new Dictionary<string, string>();
            try
            {
                string[] fileArray = Directory.GetFiles(dataDirectory);
                for (int i = 0; i < fileArray.Length; i++)
                {
                    files.Add((i + 1).ToString(), fileArray[i].Substring(fileArray[i].LastIndexOf(@"\") + 1));
                }
            }
            catch 
            {
                throw;
            }
            return files;
        }

        internal class ToDoDataIsCorupptedException : Exception
        {
            ToDoDataIsCorupptedException()
            {
            }

            public ToDoDataIsCorupptedException(string message)
                : base(message)
            {
            }

            public ToDoDataIsCorupptedException(string message, Exception inner)
                : base(message, inner)
            {
            }
        }

        internal static string EditString(string str)
        {
            int start = Console.CursorLeft;

            Console.Write(str);
            //int pos = Console.CursorLeft + start;

            ConsoleKeyInfo info;
            List<char> chars = new List<char>();
            if (!string.IsNullOrEmpty(str))
            {
                chars.AddRange(str.ToCharArray());
            }
            int pos = chars.Count + start;
            int currPostion = Console.CursorLeft - start;

            while (true)
            {
                info = Console.ReadKey(true);

                if (info.Key == ConsoleKey.Delete) 
                {
                    if (Console.CursorLeft < pos)
                    {
                        int removeAt = Console.CursorLeft - start;
                        chars.RemoveAt(removeAt);
                        int tmp = Console.CursorLeft;
                        Console.Write(chars.ToArray(), removeAt, chars.Count - removeAt);

                        Console.Write(' ');
                        Console.CursorLeft = tmp;
                    }
                }
                else if (info.Key == ConsoleKey.Backspace)
                {
                    if (Console.CursorLeft > start)
                    {
                        int removeAt = Console.CursorLeft - 1 - start;
                        chars.RemoveAt(removeAt);
                        Console.CursorLeft--;
                        int tmp = Console.CursorLeft;
                        Console.Write(chars.ToArray(), removeAt, chars.Count - removeAt);

                        Console.Write(' ');
                        Console.CursorLeft = tmp;
                    }
                }
                else if (info.Key == ConsoleKey.LeftArrow)
                {
                    if (Console.CursorLeft <= start)
                        continue;

                    Console.CursorLeft--;
                }
                else if (info.Key == ConsoleKey.RightArrow) 
                {
                    if (Console.CursorLeft <= (start + chars.Count) - 1)
                        Console.CursorLeft++;
                }
                else if (info.Key == ConsoleKey.Spacebar)
                {
                    if (Console.CursorLeft < pos)
                    {
                        int insertAt = Console.CursorLeft - start;
                        chars.Insert(insertAt, ' ');

                        int tmp = Console.CursorLeft;
                        Console.Write(chars.ToArray(), insertAt, chars.Count - insertAt);

                        Console.CursorLeft = ++tmp;
                    }
                    else
                    {
                        chars.Add(' ');
                        Console.Write(' ');
                    }
                }
                else if (info.Key == ConsoleKey.Enter)
                {
                    Console.Write(Environment.NewLine);
                    break;
                }
                else 
                {                   
                    if (Console.CursorLeft < pos)
                    {
                        int insertAt = Console.CursorLeft - start;
                        chars.Insert(insertAt, info.KeyChar);

                        int tmp = Console.CursorLeft;
                        Console.Write(chars.ToArray(), insertAt, chars.Count - insertAt);
                        Console.CursorLeft = ++tmp;
                    }
                    else
                    {
                        chars.Add(info.KeyChar);
                        Console.Write(info.KeyChar);
                    }
                }                
                pos = chars.Count + start;
            }
            return new string(chars.ToArray());
        }
    }
}
