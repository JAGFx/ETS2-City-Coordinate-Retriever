using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using jsonGenerator.Classes;
using Newtonsoft.Json;

namespace jsonGenerator {
    class Program {
        private const string PATTERN_CITY_NAME         = @"city_data\s*:\s*city\.(\w+)";
        private const string PATTERN_CITY_REAL_NAME    = "\\s*\\t*city_name\\s*:\\s*\\\"([-\\w\\s]+)\\\"";
        private const string PATTERN_CITY_COUNTRY      = "\\s*\\t*country\\s*:\\s*([-\\w\\s]+)";
        private const string PATTERN_COMPANY_NAME      = @"company_permanent\s*:\s*company\.permanent\.(\w+)";
        private const string PATTERN_COMPANY_REAL_NAME = "\\s*\\t*name\\s*:\\s*\\\"([-\\w\\s]+)\\\"";
        private const string FILE_TARGET_SUI = "*.sui";
        private const string FILE_TARGET_TXT = "*.txt";

        [ DllImport( "kernel32.dll", ExactSpelling = true ) ]
        public static extern IntPtr GetConsoleWindow();

        [ DllImport( "user32.dll" ) ]
        [ return: MarshalAs( UnmanagedType.Bool ) ]
        public static extern bool SetForegroundWindow( IntPtr hWnd );

        private static readonly AppConfig Config = new AppConfig();

        private static void Main( string[ ] args ) {
            string outputCompanies = Config.Out.GetCompanies();
            string outputCities    = Config.Out.GetCities();

            Dictionary< string, IJsonable > companiesDictionary = BuildCompanies();
            Dictionary< string, IJsonable > citiesDictionary    = BuildCities( ref companiesDictionary );

            GenerateJson( ref companiesDictionary, outputCompanies );
            GenerateJson( ref citiesDictionary,    outputCities );
        }

        /// <summary>
        ///     Generate cities datas
        /// </summary>
        /// <param name="companies"></param>
        /// <returns></returns>
        private static Dictionary< string, IJsonable > BuildCities( ref Dictionary< string, IJsonable > companies ) {
            List< string >? cityDirs     = Config.Raw.GetCitiesDirs();
            List< string >? reportsDirs  = Config.Raw.GetReportsDirs();
            List< string >? citiesFiles  = GetRecursiveFileList( cityDirs,    FILE_TARGET_SUI );
            List< string >? reportsFiles = GetRecursiveFileList( reportsDirs, FILE_TARGET_TXT );

            var cityDictionary = new Dictionary< string, IJsonable >();

            // ---- Parse SCS files
            foreach ( string citiesFile in citiesFiles ) {
                var readCity = new StreamReader( citiesFile );
                try {
                    string? cityName = null;
                    string  line;

                    while ( ( line = readCity.ReadLine() ) != null ) {
                        string? currentCityName = MatchAndGetValue( PATTERN_CITY_NAME, line.Trim() );

                        if ( !string.IsNullOrEmpty( currentCityName )
                             && !cityDictionary.ContainsKey( currentCityName ) ) {
                            var city = new City {
                                gameName = currentCityName,
                                realName = "",
                                country  = "",
                                x        = "0",
                                y        = "0",
                                z        = "0"
                            };

                            cityDictionary.Add( currentCityName, city );
                            cityName = currentCityName;
                        }

                        // Console.WriteLine( cityName);

                        if ( !string.IsNullOrEmpty( cityName ) && cityDictionary.ContainsKey( cityName ) ) {
                            var city = (City) cityDictionary[ cityName ];

                            //Check real city name
                            string? cityRealName = MatchAndGetValue( PATTERN_CITY_REAL_NAME, line.Trim() );
                            if ( !string.IsNullOrEmpty( cityRealName ) )
                                city.realName = cityRealName;

                            //Check country
                            string? country = MatchAndGetValue( PATTERN_CITY_COUNTRY, line.Trim() );
                            if ( !string.IsNullOrEmpty( country ) )
                                city.country = country;

                            UpdateCompaniesOfCity( ref companies, ref city );
                        }
                    }
                } catch ( Exception ex ) {
                    Console.Write( ex.ToString() );
                }
            }


            // ---- Parse report file

            foreach ( string reportsFile in reportsFiles ) {
                var       read        = new StreamReader( reportsFile );
                string    output      = read.ReadToEnd();
                string[ ] outputArray = output.Split( new string[ ] { Environment.NewLine }, StringSplitOptions.None );

                read.Close();

                foreach ( string line in outputArray ) {
                    try {
                        string    lineContent      = line.Replace( " ", "" );
                        string[ ] lineContentArray = lineContent.Split( ";" );

                        if ( string.IsNullOrWhiteSpace( lineContent ) || lineContentArray.Length < 4 ) {
                            Console.WriteLine( $"Line ignored: {lineContent} | File: {reportsFile}" );
                            continue;
                        }

                        string cityName = lineContentArray[ 0 ];
                        string cityPosX = lineContentArray[ 2 ];
                        string cityPosY = lineContentArray[ 3 ];
                        string cityPosZ = lineContentArray[ 4 ];

                        var city = (City?) ( cityDictionary.ContainsKey( cityName )
                                                 ? cityDictionary[ cityName ]
                                                 : null );

                        if ( city != null ) {
                            city.x = cityPosX;
                            city.y = cityPosY;
                            city.z = cityPosZ;
                        }
                    } catch ( Exception ex ) {
                        Console.WriteLine( ex.ToString() );
                    }
                }
            }

            Console.WriteLine( "[Cities] Read: "
                               + citiesFiles.Count()
                               + " | In list: "
                               + cityDictionary.Count );

            return cityDictionary;
        }

        /// <summary>
        ///     Generate companies datas 
        /// </summary>
        /// <returns></returns>
        private static Dictionary< string, IJsonable > BuildCompanies() {
            List< string >? companyDirs         = Config.Raw.GetCompaniesDirs();
            List< string >  files               = GetRecursiveFileList( companyDirs, FILE_TARGET_SUI );
            var             companiesDictionary = new Dictionary< string, IJsonable >();

            foreach ( string file in files ) {
                var readCompany = new StreamReader( file );
                try {
                    var companyGameName = "";
                    var companyRealName = "";

                    string line;
                    while ( ( line = readCompany.ReadLine() ) != null ) {
                        string? currentCompanyGameName = MatchAndGetValue( PATTERN_COMPANY_NAME, line.Trim() );

                        if ( !string.IsNullOrEmpty( currentCompanyGameName ) )
                            companyGameName = currentCompanyGameName;

                        else {
                            //Check real city name
                            string? currentCompanyRealName = MatchAndGetValue( PATTERN_COMPANY_REAL_NAME, line.Trim() );

                            if ( !string.IsNullOrEmpty( currentCompanyRealName ) )
                                companyRealName = currentCompanyRealName;
                        }

                        // Console.WriteLine( currentCompanyGameName );

                        if ( !string.IsNullOrEmpty( companyGameName )
                             && !string.IsNullOrEmpty( companyRealName )
                             && !companiesDictionary.ContainsKey( companyGameName ) ) {
                            var company = new Company() {
                                gameName = companyGameName,
                                realName = companyRealName,
                            };

                            companiesDictionary.Add( companyGameName, company );
                        }
                    }
                } catch ( Exception ex ) {
                    Console.Write( ex.ToString() );
                }
            }

            Console.WriteLine( "[Companies] Read: "
                               + files.Count()
                               + " | In list: "
                               + companiesDictionary.Count );

            return companiesDictionary;
        }

        /// <summary>
        ///     Update ce cities object with each comapny
        /// </summary>
        /// <param name="companies"></param>
        /// <param name="city"></param>
        private static void UpdateCompaniesOfCity( ref Dictionary< string, IJsonable > companies, ref City city ) {
            Dictionary< string, string > companyCities = Config.Raw.GetCompanyCitiesFiles( ref city );

            foreach ( KeyValuePair< string, string > companyCity in companyCities ) {
                // In each dir of company if a list of cities exist ...
                if ( companies.ContainsKey( companyCity.Key ) ) {
                    // ... add it in the city companies list
                    city.addCompany( (Company) companies[ companyCity.Key ] );
                }
            }
        }

        // ---

        /// <summary>
        ///     Convert an object to json and store in a file
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="output"></param>
        private static void GenerateJson( ref Dictionary< string, IJsonable > dictionary, string output ) {
            // string outputFile     = ConfigurationManager.AppSettings[ "OutputFile" ];
            string json  = JsonConvert.SerializeObject( dictionary, (Formatting) System.Xml.Formatting.Indented );
            var    write = new StreamWriter( output );

            write.Write( json );
            write.Close();
        }

        /// <summary>
        ///     Get a value with RegEx pattern from a string
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        private static string? MatchAndGetValue( string pattern, string input ) {
            var r = new Regex( pattern, RegexOptions.IgnoreCase );

            if ( !r.IsMatch( input ) ) return null;

            Match m = r.Match( input );

            return m.Groups[ 1 ].Value;
        }

        /// <summary>
        ///     Retrieve all files matched with the given target
        /// </summary>
        /// <param name="inDirs"></param>
        /// <param name="targetedFile"></param>
        /// <returns></returns>
        private static List< string > GetRecursiveFileList( List< string > inDirs, string targetedFile ) {
            List< string > files = new List< string >();

            foreach ( string dir in inDirs ) {
                Console.WriteLine( $"File: {dir} | Target: {targetedFile}" );

                if ( File.Exists( dir ) )
                    files.Add( dir );

                if ( Directory.Exists( dir ) ) {
                    DirectoryInfo folder      = new DirectoryInfo( dir );
                    FileInfo[ ]   folderFiles = folder.GetFiles( targetedFile, SearchOption.AllDirectories );

                    foreach ( FileInfo file in folderFiles )
                        files.Add( file.FullName );
                }
            }

            return files;
        }
    }
}