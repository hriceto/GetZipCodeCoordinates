using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;
using System.Data.SqlClient;
using System.Configuration;
using System.Threading;

namespace GetZipCodeCoordinates
{
    class Program
    {
        static void Main(string[] args)
        {
            Program p = new Program();
            List<GeoCodeObject> geoCodes = new List<GeoCodeObject>();

            for (int i = 46001; i < 47997; i++)
            {
                GeoCodeObject geoCode = p.GetZipCodeGeoCode(i, "Indiana");

                if (geoCode != null)
                {
                    if (geoCode.Type == "postal_code")
                    {
                        geoCodes.Add(geoCode);
                    }
                }
                Thread.Sleep(300);
            }

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["SQLConnection"].ToString()))
            {
                conn.Open();
                foreach (GeoCodeObject geoCode in geoCodes)
                {
                    p.InsertUpdateZipCode(geoCode, conn);
                }
            }
        }

        protected void InsertUpdateZipCode(GeoCodeObject geoCode, SqlConnection conn)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = conn;
                command.CommandText = "ZipCodeInsertUpdate";
                
                command.Parameters.Add(GetStringParameter("@Type", geoCode.Type));
                command.Parameters.Add(GetStringParameter("@FullAddress", geoCode.FullAddress));
                command.Parameters.Add(GetIntegerParameter("@Zip", geoCode.ZipCode));
                command.Parameters.Add(GetStringParameter("@City", geoCode.City));
                command.Parameters.Add(GetStringParameter("@County", geoCode.County));
                command.Parameters.Add(GetStringParameter("@StateLong", geoCode.StateLong));
                command.Parameters.Add(GetStringParameter("@StateShort", geoCode.StateShort));
                command.Parameters.Add(GetDecimalParameter("@Lattitude", geoCode.Lattitude));
                command.Parameters.Add(GetDecimalParameter("@Longitude", geoCode.Longitude));
                command.Parameters.Add(GetStringParameter("@LocationType", geoCode.LocationType));
                command.Parameters.Add(GetDecimalParameter("@ViewportSouthWestLattitude", geoCode.ViewportSouthWestLattitude));
                command.Parameters.Add(GetDecimalParameter("@ViewportSouthWestLongitude", geoCode.ViewportSouthWestLongitude));
                command.Parameters.Add(GetDecimalParameter("@ViewportNorthEastLattitude", geoCode.ViewportNorthEastLattitude));
                command.Parameters.Add(GetDecimalParameter("@ViewportNorthEastLongitude", geoCode.ViewportNorthEastLongitude));
                command.Parameters.Add(GetDecimalParameter("@BoundsSouthWestLattitude", geoCode.BoundsSouthWestLattitude));
                command.Parameters.Add(GetDecimalParameter("@BoundsSouthWestLongitude", geoCode.BoundsSouthWestLongitude));
                command.Parameters.Add(GetDecimalParameter("@BoundsNorthEastLattitude", geoCode.BoundsNorthEastLattitude));
                command.Parameters.Add(GetDecimalParameter("@BoundsNorthEastLongitude", geoCode.BoundsNorthEastLongitude));

                command.CommandType = System.Data.CommandType.StoredProcedure;
                int i = command.ExecuteNonQuery();
            }
        }

        private SqlParameter GetStringParameter(string name, string value)
        {
            if (value == null)
            {
                value = "";
            }
            SqlParameter param = new SqlParameter(name, System.Data.SqlDbType.NVarChar);
            param.Value = value;
            return param;
        }

        private SqlParameter GetDecimalParameter(string name, Double value)
        {
            SqlParameter param = new SqlParameter(name, System.Data.SqlDbType.Decimal);
            param.Value = value;
            param.Precision = 10;
            param.Size = 18;
            return param;
        }

        private SqlParameter GetIntegerParameter(string name, int value)
        {
            SqlParameter param = new SqlParameter(name, System.Data.SqlDbType.Int);
            param.Value = value;
            return param;
        }

        protected GeoCodeObject GetZipCodeGeoCode(int ZipCode, string state)
        {
            GeoCodeObject geoCode = null;
            WebRequest request = WebRequest.Create("http://maps.googleapis.com/maps/api/geocode/xml?address=" + ZipCode.ToString() + "," + state + ",United States&sensor=false");
            request.Method = "GET";
            WebResponse response = null;
            StreamReader reader = null;
            string content = null;
            try
            {
                response = request.GetResponse();
                Stream stream = response.GetResponseStream();
                reader = new StreamReader(stream);
                content = reader.ReadToEnd();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (reader != null) reader.Close();
                if (response != null) response.Close();
            }

            if (String.IsNullOrEmpty(content))
            {
                return null;
            }

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(content);
            XmlNode nodeStatus = doc.SelectSingleNode("GeocodeResponse/status");
            if (nodeStatus.InnerText == "OK")
            {
                XmlNode nodeResult = doc.SelectSingleNode("GeocodeResponse/result");
                if (nodeResult != null)
                {
                    geoCode = new GeoCodeObject();
                    geoCode.Type = GetXMLString(nodeResult, "type");
                    geoCode.FullAddress = GetXMLString(nodeResult, "formatted_address");

                    XmlNodeList nodeListAddressComponents = nodeResult.SelectNodes("address_component");
                    foreach (XmlNode nodeAddressComponent in nodeListAddressComponents)
                    {
                        XmlNode nodeAddressComponentType = nodeAddressComponent.SelectSingleNode("type");
                        XmlNode nodeAddressComponentLongName = nodeAddressComponent.SelectSingleNode("long_name");
                        XmlNode nodeAddressComponentShortName = nodeAddressComponent.SelectSingleNode("short_name");
                        if (nodeAddressComponentType != null &&
                            nodeAddressComponentLongName != null &&
                            nodeAddressComponentShortName != null)
                        {
                            switch (nodeAddressComponentType.InnerText)
                            {
                                case "postal_code": 
                                    int zip = 0; 
                                    Int32.TryParse(nodeAddressComponentLongName.InnerText,  out zip); 
                                    geoCode.ZipCode = zip; 
                                    break;
                                case "locality": geoCode.City = nodeAddressComponentLongName.InnerText; break;
                                case "administrative_area_level_2": geoCode.County = nodeAddressComponentLongName.InnerText; break;
                                case "administrative_area_level_1":
                                    geoCode.StateLong = nodeAddressComponentLongName.InnerText;
                                    geoCode.StateShort = nodeAddressComponentShortName.InnerText;
                                    break;
                            }
                        }
                    }

                    XmlNode nodeGeometry = nodeResult.SelectSingleNode("geometry");
                    if (nodeGeometry != null)
                    {
                        geoCode.Lattitude = GetXMLDouble(nodeGeometry, "location/lat");
                        geoCode.Longitude = GetXMLDouble(nodeGeometry, "location/lng");
                        geoCode.LocationType = GetXMLString(nodeGeometry, "location_type");
                        geoCode.ViewportSouthWestLattitude = GetXMLDouble(nodeGeometry, "viewport/southwest/lat");
                        geoCode.ViewportSouthWestLongitude = GetXMLDouble(nodeGeometry, "viewport/southwest/lng");
                        geoCode.ViewportNorthEastLattitude = GetXMLDouble(nodeGeometry, "viewport/northeast/lat");
                        geoCode.ViewportNorthEastLongitude = GetXMLDouble(nodeGeometry, "viewport/northeast/lng");

                        geoCode.BoundsSouthWestLattitude = GetXMLDouble(nodeGeometry, "bounds/southwest/lat");
                        geoCode.BoundsSouthWestLongitude = GetXMLDouble(nodeGeometry, "bounds/southwest/lng");
                        geoCode.BoundsNorthEastLattitude = GetXMLDouble(nodeGeometry, "bounds/northeast/lat");
                        geoCode.BoundsNorthEastLongitude = GetXMLDouble(nodeGeometry, "bounds/northeast/lng");

                    }
                }
            }

            return geoCode;
        }

        public string GetXMLString(XmlNode node, string path)
        {
            string result = "";

            if (node != null)
            {
                XmlNode childNode = node.SelectSingleNode(path);
                if (childNode != null)
                {
                    result = childNode.InnerText;
                }
            }

            return result;
        }

        public double GetXMLDouble(XmlNode node, string path)
        {
            double result = 0;

            if (node != null)
            {
                XmlNode childNode = node.SelectSingleNode(path);
                if (childNode != null)
                {
                    if (!Double.TryParse(childNode.InnerText, out result))
                    {
                        result = 0;
                    }
                }
            }

            return result;
        }
    }
}
