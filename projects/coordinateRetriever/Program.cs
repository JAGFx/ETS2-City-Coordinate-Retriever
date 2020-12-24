using System;
using System.Configuration;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using InputManager;

namespace ets2_city_coordinate_retriever
{

    class Program
    {
        private const string CityCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        [DllImport("kernel32.dll", ExactSpelling = true)]
        public static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        static void Main()
        {

            bool debugMode = bool.Parse(ConfigurationManager.AppSettings["DebugMode"]);

            Console.WriteLine(@"After pressing {ENTER} you will have 10 seconds to switch to ETS2 / ATS for coordinate capture. 

Once the operation is complete, all coordinates will be stored in:

%USERPROFILE%\Documents\{ETS2/ATS Folder}\bugs.txt" + "\n");

            // Wait until user presses {ENTER}
            Console.ReadLine();

            const int numberOfSeconds = 10;
            for (int i = numberOfSeconds; i >= 1; i--)
            {
                Console.Write(i + " ");
                Thread.Sleep(1000);
            }

            string cityDirectory = ConfigurationManager.AppSettings["CityDirectory"];
            double sleepMultiplier = double.Parse(ConfigurationManager.AppSettings["SleepMultiplier"]);

            string[] files = Directory.GetFiles(cityDirectory);
            int numberOfCities = files.Length;

            if (debugMode)
            {
                Console.WriteLine($"Processing {numberOfCities} cities");
            }

            for (int i = 0; i < numberOfCities; i++)
            {
                //string cityName = Path.GetFileNameWithoutExtension(files[i]);
                StreamReader read = new StreamReader(files[i]);
                string output = read.ReadToEnd();
                string[] outputArray = output.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                read.Close();
                try
                {
                    foreach (var line in outputArray)
                    {
                        if (line.Trim().StartsWith("city_data: city.") || line.Trim().StartsWith("city_data:city.") || line.Trim().StartsWith("city_data : city."))
                        {
                            //int nameIndex = line.IndexOf("\"");
                            //string cityName = line.Substring(nameIndex + 1,
                            //    line.IndexOf("\"", nameIndex + 1) - nameIndex - 1);
                            string cityName = line.Replace("city_data: city.", "").Replace("city_data:city.", "").Replace("city_data : city.", "");
                            if (cityName.Contains("{"))
                            {
                                cityName = cityName.Remove(cityName.IndexOf("{"));
                            }
                            cityName = cityName.Trim();
                            if (debugMode)
                            {
                                Console.WriteLine($"Processing city {cityName} ({i + 1} / {numberOfCities})");
                            }
                            int keyPressTime = (int) (50*sleepMultiplier);

                            // Need to be on US keyboard layout !
                            Keyboard.KeyPress(Keys.Oemtilde);
                            Thread.Sleep((int)(500 * sleepMultiplier));
                            Keyboard.KeyPress(Keys.Back);
                            Thread.Sleep((int)(150 * sleepMultiplier));
                            Keyboard.KeyPress(Keys.G);
                            Thread.Sleep(keyPressTime);
                            Keyboard.KeyPress(Keys.O);
                            Thread.Sleep(keyPressTime);
                            Keyboard.KeyPress(Keys.T);
                            Thread.Sleep(keyPressTime);
                            Keyboard.KeyPress(Keys.O);
                            Thread.Sleep(keyPressTime);
                            Keyboard.KeyPress(Keys.Space);
                            Thread.Sleep(keyPressTime);
                            foreach (char index in cityName)
                            {
                                if (index.ToString() == "_")
                                {
                                    Keyboard.KeyDown(Keys.ShiftKey);
                                    Thread.Sleep(keyPressTime);
                                    Keyboard.KeyPress(Keys.OemMinus);
                                    Thread.Sleep(keyPressTime);
                                    Keyboard.KeyUp(Keys.ShiftKey);
                                    Thread.Sleep(keyPressTime);
                                }
                                else if (CityCharacters.Contains(index.ToString().ToUpper()))
                                {
                                    Keyboard.KeyPress(
                                        (Keys) Enum.Parse(typeof (Keys), index.ToString().ToUpper()));
                                    Thread.Sleep(keyPressTime);
                                }
                            }
                            Thread.Sleep((int)(100 * sleepMultiplier));
                            Keyboard.KeyDown(Keys.Enter);
                            Thread.Sleep((int)(100 * sleepMultiplier));
                            Keyboard.KeyUp(Keys.Enter);
                            Thread.Sleep((int)(3000 * sleepMultiplier));
                            Keyboard.KeyPress(Keys.Oemtilde);
                            Thread.Sleep((int)(100 * sleepMultiplier));
                            Keyboard.KeyPress(Keys.F11);
                            Thread.Sleep((int)(100 * sleepMultiplier));
                            foreach (char index in cityName)
                            {
                                if (index.ToString() == "_")
                                {
                                    Keyboard.KeyDown(Keys.ShiftKey);
                                    Thread.Sleep(keyPressTime);
                                    Keyboard.KeyPress(Keys.OemMinus);
                                    Thread.Sleep(keyPressTime);
                                    Keyboard.KeyUp(Keys.ShiftKey);
                                    Thread.Sleep(keyPressTime);
                                }
                                else if (CityCharacters.Contains(index.ToString().ToUpper()))
                                {
                                    Keyboard.KeyPress(
                                        (Keys)Enum.Parse(typeof(Keys), index.ToString().ToUpper()));
                                    Thread.Sleep(keyPressTime);
                                }
                            }
                            Keyboard.KeyPress(Keys.Enter);
                            Thread.Sleep((int)(100 * sleepMultiplier));
                            if (!debugMode)
                            {
                                DrawProgressBar(i + 1, numberOfCities, new decimal(.5)*Console.WindowWidth, '=', '-');
                            }
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    for (int a = 0; a <= 20; a++)
                    {
                        Keyboard.KeyPress(Keys.Back);
                    }
                    Thread.Sleep((int)(1000 * sleepMultiplier));
                }
            }


            // Complete
            SetForegroundWindow(GetConsoleWindow());
            Console.WriteLine("Complete! Press {ENTER} to exit.");
            Console.ReadLine();
        }

        private static void DrawProgressBar(int complete, int maxVal, decimal barSize, char completeProgressChar, char incompleteProgressChar)
        {
            Console.CursorVisible = false;
            int left = Console.CursorLeft;
            decimal perc = (decimal)complete / maxVal;
            int chars = (int)Math.Floor(perc / (1 / barSize));
            string p1 = string.Empty, p2 = string.Empty;

            for (int i = 0; i < chars; i++) p1 += completeProgressChar;
            for (int i = 0; i < barSize - chars; i++) p2 += incompleteProgressChar;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write('[');
            Console.Write(p1);
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write(p2);
            Console.Write("]");

            Console.ResetColor();
            string percentage = (perc * 100).ToString("N2");
            Console.Write(" {0}%", percentage);

            if (percentage != "100.00")
            {
                Console.CursorLeft = left;
            }
            else {
                Console.WriteLine();
            }
        }
    }
}