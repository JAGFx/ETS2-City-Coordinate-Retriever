using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace jsonGenerator.Classes {
    public class AppConfig {
        public const string APP_SETTING_FILE = "appsettings";

        private IConfigurationRoot? Configuration { get; }
        public  RawConfig           Raw           { get; }
        public  OutputConfig        Out           { get; }

        // ---

        public AppConfig() {
            string? environmentName = Environment.GetEnvironmentVariable( "ENVIRONMENT" );

            // Set up configuration sources.
            IConfigurationBuilder? builder = new ConfigurationBuilder()
                                             .SetBasePath( Path.Combine( AppContext.BaseDirectory ) )
                                             .AddJsonFile( $"{APP_SETTING_FILE}.json", true, true )
                                             .AddJsonFile(
                                                          Path.Combine( AppContext.BaseDirectory,
                                                                        string.Format( "..{0}..{0}..{0}",
                                                                            Path.DirectorySeparatorChar ),
                                                                        $"{APP_SETTING_FILE}.{environmentName}.json" ),
                                                          optional: true
                                                         );

            // Load json file
            this.Configuration = builder.Build();

            // Console.WriteLine( this.Configuration[ "BasePath" ] );

            IConfigurationSection? rawData    = this.Configuration.GetSection( "RawConfig" );
            IConfigurationSection? outputData = this.Configuration.GetSection( "OutputConfig" );

            IEnumerable< KeyValuePair< string, string > >? citiesDirectory =
                rawData.GetSection( "CitiesDirectory" ).AsEnumerable();
            IEnumerable< KeyValuePair< string, string > >? companiesDirectory =
                rawData.GetSection( "CompaniesDirectory" ).AsEnumerable();
            IEnumerable< KeyValuePair< string, string > >? reportDirectory =
                rawData.GetSection( "ReportDirectory" ).AsEnumerable();

            // Setup the raw config data
            this.Raw = new RawConfig( this.Configuration[ "BasePath" ],
                                      this.Configuration[ "RawConfig:CompanyCitiesPattern" ],
                                      citiesDirectory,
                                      companiesDirectory,
                                      reportDirectory );

            // Setup the output config data
            this.Out = new OutputConfig( this.Configuration[ "BasePath" ],
                                         outputData[ "Cities" ],
                                         outputData[ "Companies" ] );
        }

        // ---

        /// <summary>
        ///     Combine and replace the folder separator with the OS one
        /// </summary>
        /// <param name="inStrs"></param>
        /// <returns></returns>
        public static string PathCombine( params string[ ] inStrs ) {
            IEnumerable< string > list     = new List< string >();
            string                finalStr = "";

            foreach ( string inStr in inStrs ) {
                var spliteds = inStr.Split( '/', '\\' );
                list     = list.Concat( spliteds );
                spliteds = null;
            }

            foreach ( string s in list ) {
                if ( String.IsNullOrWhiteSpace( s ) ) continue;

                finalStr = $"{finalStr}{Path.AltDirectorySeparatorChar}{s}";
            }

            list = null;

            finalStr.Remove( finalStr.Length - 1 );
            return finalStr;
        }
    }

    /// <summary>
    ///     Class to manage all settings about raw datas
    /// </summary>
    public class RawConfig {
        private readonly string _basePath;
        private readonly string _companyCitiesPattern;

        private readonly IEnumerable< KeyValuePair< string, string > > _citiesDirectory;
        private readonly IEnumerable< KeyValuePair< string, string > > _companiesDirectory;
        private readonly IEnumerable< KeyValuePair< string, string > > _reportDirectory;

        // ----

        public RawConfig( string                                        basePath,
                          string                                        companyCitiesPattern,
                          IEnumerable< KeyValuePair< string, string > > citiesDirectory,
                          IEnumerable< KeyValuePair< string, string > > companiesDirectory,
                          IEnumerable< KeyValuePair< string, string > > reportDirectory ) {
            this._basePath             = basePath;
            this._companyCitiesPattern = companyCitiesPattern;
            this._citiesDirectory      = citiesDirectory;
            this._companiesDirectory   = companiesDirectory;
            this._reportDirectory      = reportDirectory;
        }

        public List< string > GetCitiesDirs() {
            return this.VerifyDirectories( this._citiesDirectory );
        }

        public List< string > GetCompaniesDirs() {
            return this.VerifyDirectories( this._companiesDirectory );
        }

        public List< string > GetReportsDirs() {
            return this.VerifyDirectories( this._reportDirectory );
        }

        public Dictionary< string, string > GetCompanyCitiesFiles( ref City city ) {
            List< string >               companiesDirs = this.GetCompaniesDirs();
            Dictionary< string, string > finalList     = new Dictionary< string, string >();

            foreach ( string dir in companiesDirs ) {
                string[ ]? companiesDirectories = Directory.GetDirectories( dir );

                foreach ( string companyPath in companiesDirectories ) {
                    string company = companyPath.Replace( $"{dir}/", "" );
                    string companyCities =
                        AppConfig.PathCombine( companyPath, this._companyCitiesPattern, $"{city.gameName}.sii" );

                    if ( File.Exists( companyCities ) )
                        finalList.Add( company, companyCities );
                }
            }

            return finalList;
        }

        // ----

        /// <summary>
        ///     Add base path to all given path if it's necessary
        /// </summary>
        /// <param name="inList"></param>
        /// <returns></returns>
        private List< string > VerifyDirectories( IEnumerable< KeyValuePair< string, string > > inList ) {
            List< string > list = new List< string >();

            foreach ( var (key, value) in inList ) {
                if ( string.IsNullOrWhiteSpace( value ) || string.IsNullOrEmpty( value ) ) continue;

                string currentDir = AppConfig.PathCombine( this._basePath, value );

                if ( Directory.Exists( currentDir ) ) {
                    list.Add( currentDir );
                    continue;
                }

                currentDir = value;
                if ( Directory.Exists( currentDir ) ) {
                    list.Add( currentDir );
                } else {
                    // TODO Throw an exception or Log it
                }
            }

            return list;
        }
    }

    /// <summary>
    ///     Manage all settings about the output json file after processing
    /// </summary>
    public class OutputConfig {
        private readonly string _basePath;
        private readonly string _cities;
        private readonly string _companies;

        // ----

        public OutputConfig( string basePath, string cities, string companies ) {
            this._basePath  = basePath;
            this._cities    = cities;
            this._companies = companies;
        }

        public string GetCities() {
            return ValidatePath( this._cities );
        }

        public string GetCompanies() {
            return ValidatePath( this._companies );
        }

        // ----
        /// <summary>
        ///     Add base path on given path if it's necessary
        /// </summary>
        /// <param name="inPath"></param>
        /// <returns></returns>
        private string ValidatePath( string inPath ) {
            string currentFile = AppConfig.PathCombine( this._basePath, inPath );

            if ( !File.Exists( currentFile ) ) {
                // TODO Throw an error or add a warm
                currentFile = inPath;
            }

            return currentFile;
        }
    }
}