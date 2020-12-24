using System.Collections.Generic;

namespace jsonGenerator.Classes {
    public class City : IJsonable {
        public string gameName = "";
        public string realName = "";

        public string country = "";

        public Dictionary< string, Company > companies = new Dictionary< string, Company >();

        public string x = "";
        public string y = "";
        public string z = "";

        public void addCompany( Company company ) {
            if ( !this.companies.ContainsKey( company.gameName ) )
                this.companies.Add( company.gameName, company );
        }
    }
}
