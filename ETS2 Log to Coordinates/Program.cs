using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace ETS2_Log_to_Coordinates {
    class Program {
        [DllImport("kernel32.dll", ExactSpelling = true)]
        public static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        static void Main(string[] args) {
            //Set two dictionaries, so we can later retrieve the real city name and country
            Dictionary<string, string> cityConversionTable = new Dictionary<string, string>();
            Dictionary<string, string> cityCountryTable    = new Dictionary<string, string>();

            ConflictSolver conflictSolver = new ConflictSolver();

            //Set city directory (def\city)
            string cityDirectory = ConfigurationManager.AppSettings["CityDirectory"];

            //Find all files
            string[] files          = Directory.GetFiles(cityDirectory);
            int      numberOfCities = files.Length;

            for (int i = 0; i < numberOfCities; i++) {
                StreamReader readCity = new StreamReader(files[i]);
                string       line;
                //string output = read.ReadToEnd();
                //string[] outputArray = output.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                //read.Close();
                try {
                    string cityName     = "";
                    string cityRealName = "";
                    string country      = "";
                    while ((line = readCity.ReadLine()) != null) {
                        if (line.Trim().StartsWith("city_data: city.") || line.Trim().StartsWith("city_data:city.") ||
                            line.Trim().StartsWith("city_data : city.")) {
                            //int nameIndex = line.IndexOf("\"");
                            //string cityName = line.Substring(nameIndex + 1,
                            //    line.IndexOf("\"", nameIndex + 1) - nameIndex - 1);
                            cityName = line.Replace("city_data: city.", "").Replace("city_data:city.", "")
                                           .Replace("city_data : city.", "");
                            if (cityName.Contains("{")) {
                                cityName = cityName.Remove(cityName.IndexOf("{"));
                            }

                            cityName = cityName.Trim();
                            cityName.Replace(" ", "");
                        }

                        //Check real city name
                        if (line.TrimStart().StartsWith("city_name:")) {
                            int nameIndex = line.IndexOf("\"");
                            cityRealName = line.Substring(nameIndex                         + 1,
                                                          line.IndexOf("\"", nameIndex + 1) - nameIndex - 1);
                        }

                        //Check country
                        if (line.TrimStart().StartsWith("country:")) {
                            country = line.Trim().Replace("country: ", "").Replace("country:", "").Trim();
                        }
                    }

                    //Add them to the dictionaries
                    cityConversionTable.Add(cityName, cityRealName);
                    cityCountryTable.Add(cityName, country);
                    Console.WriteLine(cityName + " - " + cityRealName);
                }
                catch (Exception ex) {
                    Console.Write(ex.ToString());
                }
            }

            //Read log file
            string       inputFile   = ConfigurationManager.AppSettings["InputFile"];
            StreamReader read        = new StreamReader(inputFile);
            string       output      = read.ReadToEnd();
            string[]     outputArray = output.Split(new string[] {Environment.NewLine}, StringSplitOptions.None);
            read.Close();

            //Initiate Cities object
            Cities cities       = new Cities();
            Cities citiesExport = new Cities();

            //Set previous location for double (non-existent) locations check
            string previousX = "";
            string previousY = "";
            string previousZ = "";

            foreach (string line in outputArray) {
                try {
                    string   lineContent      = line.Replace(" ", "");
                    string[] lineContentArray = lineContent.Split(new char[] {';'});

                    City city = new City();
                    city.gameName = lineContentArray[0];
                    city.realName = cityConversionTable[lineContentArray[0]];
                    city.country  = cityCountryTable[lineContentArray[0]];
                    city.x        = lineContentArray[2];
                    city.y        = lineContentArray[3];
                    city.z        = lineContentArray[4];
                    cities.citiesList.Add(city);

                    ListViewItem listViewItem = new ListViewItem(city.gameName);

                    if (lineContentArray[2] == previousX && lineContentArray[3] == previousY &&
                        lineContentArray[4] == previousZ) {
                        listViewItem.ForeColor = Color.Red;
                    }
                    else {
                        listViewItem.Checked = true;
                    }

                    listViewItem.SubItems.Add(city.realName);
                    listViewItem.SubItems.Add(city.country);
                    listViewItem.SubItems.Add(city.x);
                    listViewItem.SubItems.Add(city.y);
                    listViewItem.SubItems.Add(city.z);
                    conflictSolver.listCities.Items.Add(listViewItem);

                    previousX = city.z = lineContentArray[2];
                    previousY = city.z = lineContentArray[3];
                    previousZ = city.z = lineContentArray[4];
                }
                catch (Exception ex) {
                    Console.WriteLine(ex.ToString());
                }
            }

            citiesExport.citiesList = cities.citiesList;

            if (conflictSolver.ShowDialog() == DialogResult.OK) {
                foreach (City city in cities.citiesList.ToList()) {
                    if (conflictSolver.uncheckedCities.Contains(city.gameName)) {
                        Console.WriteLine(city.gameName);
                        citiesExport.citiesList.Remove(city);
                    }
                }

                string jsonCitiesList = Newtonsoft.Json.JsonConvert.SerializeObject(citiesExport, Formatting.Indented);
                Console.Write(jsonCitiesList);
                string       outputFile = ConfigurationManager.AppSettings["OutputFile"];
                StreamWriter write      = new StreamWriter(outputFile);
                write.Write(jsonCitiesList);
                write.Close();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("-- Done, have fun :-)");
                Console.ReadLine();
            }
            else {
                Console.WriteLine("Aborted");
                Console.ReadLine();
            }
        }
    }

    class Cities {
        public List<City> citiesList = new List<City>();
    }

    class City {
        public string gameName = "";
        public string realName = "";

        public string country = "";

        public string x = "";
        public string y = "";
        public string z = "";
    }
}