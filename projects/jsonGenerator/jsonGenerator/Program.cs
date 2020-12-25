﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using jsonGenerator.Classes;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace jsonGenerator {
    class Program {
        private const string PATTERN_CITY_NAME         = @"city_data\s*:\s*city\.(\w+)";
        private const string PATTERN_CITY_REAL_NAME    = "\\s*\\t*city_name\\s*:\\s*\\\"([-\\w\\s]+)\\\"";
        private const string PATTERN_CITY_COUNTRY      = "\\s*\\t*country\\s*:\\s*([-\\w\\s]+)";
        private const string PATTERN_COMPANY_NAME      = @"company_permanent\s*:\s*company\.permanent\.(\w+)";
        private const string PATTERN_COMPANY_REAL_NAME = "\\s*\\t*name\\s*:\\s*\\\"([-\\w\\s]+)\\\"";
        private const string APP_SETTING_FILE          = "appsettings.json";

        [ DllImport( "kernel32.dll", ExactSpelling = true ) ]
        public static extern IntPtr GetConsoleWindow();

        [ DllImport( "user32.dll" ) ]
        [ return: MarshalAs( UnmanagedType.Bool ) ]
        public static extern bool SetForegroundWindow( IntPtr hWnd );

        public static IConfigurationRoot? Configuration;

        private static void Main( string[ ] args ) {
            SetupConfiguration();

            string outputCities    = ConfigurationManager.AppSettings[ "OutputCities" ];
            string outputCompanies = ConfigurationManager.AppSettings[ "OutputCompanies" ];

            // Dictionary< string, IJsonable > companiesDictionary = BuildCompanies();
            // Dictionary< string, IJsonable > citiesDictionary    = BuildCities( ref companiesDictionary );

            // GenerateJson( ref companiesDictionary, outputCompanies );
            // GenerateJson( ref citiesDictionary,    outputCities );
        }

        /// <summary>
        ///     Generate cities datas
        /// </summary>
        /// <param name="companies"></param>
        /// <returns></returns>
        private static Dictionary< string, IJsonable > BuildCities( ref Dictionary< string, IJsonable > companies ) {
            string cityDirectory = ConfigurationManager.AppSettings[ "CityDirectory" ];
            string inputFile     = ConfigurationManager.AppSettings[ "InputFile" ];

            var cityDictionary = new Dictionary< string, IJsonable >();

            // ---- Parse SCS files
            string[ ] citiesFiles    = Directory.GetFiles( cityDirectory );
            int       numberOfCities = citiesFiles.Length;

            for ( var i = 0; i < numberOfCities; i++ ) {
                var readCity = new StreamReader( citiesFiles[ i ] );
                try {
                    string? cityName = null;
                    string  line;

                    while ( ( line = readCity.ReadLine() ) != null ) {
                        string? currentCityName = MatchAndGetValue( PATTERN_CITY_NAME, line.Trim() );

                        if ( !string.IsNullOrEmpty( currentCityName ) ) {
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
            var       read        = new StreamReader( inputFile );
            string    output      = read.ReadToEnd();
            string[ ] outputArray = output.Split( new string[ ] { Environment.NewLine }, StringSplitOptions.None );

            read.Close();

            foreach ( string line in outputArray ) {
                try {
                    string    lineContent      = line.Replace( " ", "" );
                    string[ ] lineContentArray = lineContent.Split( new char[ ] { ';' } );

                    if ( lineContent.Length == 0 ) continue;

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

            Console.WriteLine( "[Cities] Read: "
                               + numberOfCities
                               + " | In list: "
                               + cityDictionary.Count );

            return cityDictionary;
        }

        /// <summary>
        ///     Generate companies datas 
        /// </summary>
        /// <returns></returns>
        private static Dictionary< string, IJsonable > BuildCompanies() {
            string companyDirectory = ConfigurationManager.AppSettings[ "CompanyDirectory" ];

            //Set two dictionaries, so we can later retrieve the real city name and country
            var companiesDictionary = new Dictionary< string, IJsonable >();

            // var conflictSolver = new ConflictSolver();

            //Find all files
            string[ ] files             = Directory.GetFiles( companyDirectory );
            int       numberOfCompanies = files.Length;

            // var companies = new Companies();

            for ( var i = 0; i < numberOfCompanies; i++ ) {
                var readCompany = new StreamReader( files[ i ] );
                try {
                    var companyGameName = "";
                    var companyRealName = "";

                    string line;
                    while ( ( line = readCompany.ReadLine() ) != null ) {
                        // Console.WriteLine( line.Trim() );

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
                               + numberOfCompanies
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
            string    companiesInput       = ConfigurationManager.AppSettings[ "CompanyDirectory" ];
            string[ ] companiesDirectories = Directory.GetDirectories( companiesInput );

            foreach ( string companyPath in companiesDirectories ) {
                // In each dir of company...
                string companyInput  = ConfigurationManager.AppSettings[ "CompanyDirectory" ];
                string companyCities = ConfigurationManager.AppSettings[ "CompanyCitiesDirectory" ];

                string company = companyPath.Replace( companyInput + "\\", "" );
                companyCities = companyCities.Replace( "__company_name__", company );

                string companyCityPath = companyCities + "\\" + city.gameName + ".sii";

                // ... if a list of cities exist ...
                if ( Directory.Exists( companyCities )
                     && File.Exists( companyCityPath )
                     && companies.ContainsKey( company ) ) {
                    // ... add it in the city companies list
                    city.addCompany( (Company) companies[ company ] );
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
        ///     Load the settings file
        /// </summary>
        private static void SetupConfiguration() {
            // Set up configuration sources.
            IConfigurationBuilder? builder = new ConfigurationBuilder()
                                             .SetBasePath( Path.Combine( AppContext.BaseDirectory ) )
                                             .AddJsonFile( APP_SETTING_FILE, optional: true );

            Configuration = builder.Build();
        }
    }
}