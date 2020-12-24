using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using jsonGenerator.Classes;
using System.Configuration;
using Newtonsoft.Json;
using Formatting = System.Xml.Formatting;

namespace jsonGenerator {
    
    class Program {
        [ DllImport( "kernel32.dll", ExactSpelling = true ) ]
        public static extern IntPtr GetConsoleWindow();

        [ DllImport( "user32.dll" ) ]
        [ return: MarshalAs( UnmanagedType.Bool ) ]
        public static extern bool SetForegroundWindow( IntPtr hWnd );

        private static void Main( string[ ] args ) {
            string outputCities    = ConfigurationManager.AppSettings[ "OutputCities" ];
            string outputCompanies = ConfigurationManager.AppSettings[ "OutputCompanies" ];

            Dictionary< string, IJsonable > companiesDictionary = BuildCompanies();
            Dictionary< string, IJsonable > citiesDictionary    = BuildCities( ref companiesDictionary );

            GenerateJson( ref companiesDictionary, outputCompanies );
            GenerateJson( ref citiesDictionary,    outputCities );
        }

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
                    var cityName = "";

                    string line;
                    while ( ( line = readCity.ReadLine() ) != null ) {

                        if ( line.Trim().StartsWith( "city_data: city." )
                             || line.Trim().StartsWith( "city_data:city." )
                             || line.Trim().StartsWith( "city_data : city." ) ) {
                            
                            cityName = line.Replace( "city_data: city.", "" )
                                           .Replace( "city_data:city.",   "" )
                                           .Replace( "city_data : city.", "" );
                            if ( cityName.Contains( "{" ) ) {
                                cityName = cityName.Remove( cityName.IndexOf( "{" ) );
                            }

                            cityName = cityName.Trim();
                            cityName.Replace( " ", "" );

                            var city = new City {
                                gameName = cityName,
                                realName = "",
                                country  = "",
                                x        = "0",
                                y        = "0",
                                z        = "0"
                            };

                            cityDictionary.Add( cityName, city );
                        }

                        if ( cityName.Length > 0 && cityDictionary.ContainsKey( cityName ) ) {
                            var city = (City) cityDictionary[ cityName ];

                            //Check real city name
                            if ( line.TrimStart().StartsWith( "city_name:" ) ) {
                                int nameIndex = line.IndexOf( "\"" );
                                string cityRealName = line.Substring( nameIndex + 1,
                                                                      line.IndexOf( "\"", nameIndex + 1 )
                                                                      - nameIndex
                                                                      - 1 );

                                city.realName = cityRealName;
                            }

                            //Check country
                            if ( line.TrimStart().StartsWith( "country:" ) ) {
                                string country =
                                    line.Trim().Replace( "country: ", "" ).Replace( "country:", "" ).Trim();


                                city.country = country;
                            }

                            UpdateCompaniesOfCity( ref companies, ref city );
                        }

                        // if ( cityName.Length    > 0
                        // && cityName.Length > 0
                        // && !cityDictionary.ContainsKey( cityName ) ) {


                        // previousX = city.z = lineContentArray[ 2 ];
                        // previousY = city.z = lineContentArray[ 3 ];
                        // previousZ = city.z = lineContentArray[ 4 ];

                        // cities.citiesList.Add ( city );

                        // var listViewItem = new ListViewItem ( city.gameName );

                        // if ( lineContentArray[ 2 ]    == previousX
                        // && lineContentArray[ 3 ] == previousY
                        // && lineContentArray[ 4 ] == previousZ ) {
                        // listViewItem.ForeColor = Color.Red;
                        // } else {
                        // listViewItem.Checked = true;
                        // }

                        // listViewItem.SubItems.Add ( city.realName );
                        // listViewItem.SubItems.Add ( city.country );
                        // listViewItem.SubItems.Add ( city.x );
                        // listViewItem.SubItems.Add ( city.y );
                        // listViewItem.SubItems.Add ( city.z );
                        // conflictSolver.listCities.Items.Add ( listViewItem );
                    }


                    //Add them to the dictionaries
                    // cityConversionTable.Add ( cityName, cityRealName );
                    // cityCountryTable.Add ( cityName, country );
                    // Console.WriteLine( cityName + " - " + cityRealName );
                } catch ( Exception ex ) {
                    Console.Write( ex.ToString() );
                }
            }


            // ---- Parse report file
            var       read        = new StreamReader( inputFile );
            string    output      = read.ReadToEnd();
            string[ ] outputArray = output.Split( new string[ ] { Environment.NewLine }, StringSplitOptions.None );
            read.Close();

            //Set previous location for double (non-existent) locations check
            // var previousX      = "";
            // var previousY      = "";
            // var previousZ      = "";
            // var conflictSolver = new ConflictSolver();

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

                        // var listViewItem = new ListViewItem( city.gameName );

                        // if ( cityPosX    == previousX
                             // && cityPosY == previousY
                             // && cityPosZ == previousZ ) {
                            // listViewItem.ForeColor = Color.Red;
                        // } else {
                            // listViewItem.Checked = true;
                        // }

                        // listViewItem.SubItems.Add( city.realName );
                        // listViewItem.SubItems.Add( city.country );
                        // listViewItem.SubItems.Add( city.x );
                        // listViewItem.SubItems.Add( city.y );
                        // listViewItem.SubItems.Add( city.z );

                        // conflictSolver.listCities.Items.Add( listViewItem );

                        // previousX = cityPosX;
                        // previousY = cityPosY;
                        // previousZ = cityPosZ;
                    }
                } catch ( Exception ex ) {
                    Console.WriteLine( ex.ToString() );
                }
            }

            Console.WriteLine( "Read: "
                               + numberOfCities
                               + " | In list: "
                               + cityDictionary.Count );

            return cityDictionary;

            // citiesExport.citiesList = cities.citiesList;
            //
            // string jsonCitiesList = JsonConvert.SerializeObject ( citiesExport, Formatting.Indented );
            // if ( conflictSolver.ShowDialog() == DialogResult.OK ) {
            //     foreach ( City city in cities.citiesList.ToList()
            //                                  .Where ( city => conflictSolver.uncheckedCities
            //                                                                 .Contains ( city.gameName ) )
            //     ) {
            //         Console.WriteLine ( city.gameName );
            //         citiesExport.citiesList.Remove ( city );
            //     }
            //
            //     Console.Write ( jsonCitiesList );
            //
            //     var write = new StreamWriter ( outputFile );
            //     write.Write ( jsonCitiesList );
            //     write.Close();
            //     Console.WriteLine();
            //     Console.WriteLine();
            //     Console.WriteLine ( "-- Done, have fun :-)" );
            //     Console.ReadLine();
            // } else {
            //     Console.WriteLine ( "Aborted" );
            //     Console.ReadLine();
            // }
        }

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

                        if ( line.Trim().StartsWith( "company_permanent: company.permanent." )
                             || line.Trim().StartsWith( "company_permanent:company.permanent." )
                             || line.Trim().StartsWith( "company_permanent : company.permanent." ) ) {
                            companyGameName = line.Replace( "company_permanent: company.permanent.", "" )
                                                  .Replace( "company_permanent:company.permanent.",   "" )
                                                  .Replace( "company_permanent : company.permanent.", "" );

                            if ( companyGameName.Contains( "{" ) )
                                companyGameName = companyGameName.Remove( companyGameName.IndexOf( "{" ) );

                            companyGameName = companyGameName.Trim();
                            companyGameName.Replace( " ", "" );
                        } else {
                            //Check real city name
                            if ( line.TrimStart().StartsWith( "name:" ) ) {
                                int nameIndex = line.IndexOf( "\"" );
                                companyRealName = line.Substring( nameIndex + 1,
                                                                  line.IndexOf( "\"", nameIndex + 1 )
                                                                  - nameIndex
                                                                  - 1 );
                            }
                        }

                        if ( companyGameName.Length    > 0
                             && companyRealName.Length > 0
                             && !companiesDictionary.ContainsKey( companyGameName ) ) {
                            var company = new Company() {
                                gameName = companyGameName,
                                realName = companyRealName,
                            };

                            companiesDictionary.Add( companyGameName, company );
                            // Console.WriteLine( companyGameName + " - " + companyRealName );
                        }
                    }
                } catch ( Exception ex ) {
                    Console.Write( ex.ToString() );
                }
            }

            Console.WriteLine( "Read: "
                               + numberOfCompanies
                               + " | In list: "
                               + companiesDictionary.Count );

            return companiesDictionary;
        }

        private static void GenerateJson( ref Dictionary< string, IJsonable > dictionary, string output ) {
            // string outputFile     = ConfigurationManager.AppSettings[ "OutputFile" ];
            string json  = JsonConvert.SerializeObject( dictionary, (Newtonsoft.Json.Formatting) Formatting.Indented );
            var    write = new StreamWriter( output );

            write.Write( json );
            write.Close();
        }

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

                if ( Directory.Exists( companyCities ) && File.Exists( companyCityPath ) ) {
                    // ... if a list of cities exist
                    city.addCompany( (Company) companies[ company ] );
                }
            }
        }
    }
}