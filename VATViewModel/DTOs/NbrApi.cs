using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;

namespace VATViewModel.DTOs
{
    public class NbrApi
    {
        private string URI = "http://175.29.140.41:5677/IvasWebService.asmx?op=Action";
        private string URISecondary = "http://175.29.140.41:5677/IvasWebService.asmx?op=Action";

        private Dictionary<string, string> ResponseCodes = new Dictionary<string, string>();
        private Dictionary<string, string> NoteMapping = new Dictionary<string, string>();
        private Dictionary<string, string> NoteUrlMap = new Dictionary<string, string>();


        public NbrApi()
        {
            #region Response Codes

            ResponseCodes.Add("300", "Connect to database failt");
            ResponseCodes.Add("301", "There's no data for the query");
            ResponseCodes.Add("302", "Oracle error while query data");
            ResponseCodes.Add("000", "Success");
            ResponseCodes.Add("100", "Invalid request format");
            ResponseCodes.Add("200", "Invalid username/pasword");
            ResponseCodes.Add("203", "Dont have permision to call API");
            ResponseCodes.Add("304", "Oracle error while saving data");

            #endregion

            #region Note Map

            NoteMapping.Add("1", "SL01");
            NoteMapping.Add("2", "SL02");
            NoteMapping.Add("3", "SL03");
            NoteMapping.Add("4", "SL04");
            NoteMapping.Add("5", "SL05");
            NoteMapping.Add("6", "SL06");
            NoteMapping.Add("7", "SL07");
            NoteMapping.Add("8", "SL08");
            NoteMapping.Add("10", "SL10");
            NoteMapping.Add("11", "SL11");
            NoteMapping.Add("12", "SL12");
            NoteMapping.Add("13", "SL13");
            NoteMapping.Add("14", "SL14");
            NoteMapping.Add("15", "SL15");
            NoteMapping.Add("16", "SL16");
            NoteMapping.Add("17", "SL17");
            NoteMapping.Add("18", "SL18");
            NoteMapping.Add("19", "SL20");
            NoteMapping.Add("20", "SL21");
            NoteMapping.Add("21", "SL22");
            NoteMapping.Add("22", "SL23");

            NoteMapping.Add("24", "SL25");
            NoteMapping.Add("29", "SL30");

            NoteMapping.Add("30", "SL31");

            NoteMapping.Add("58", "SL53");
            NoteMapping.Add("59", "SL54");
            NoteMapping.Add("60", "SL58");
            NoteMapping.Add("61", "SL59");
            NoteMapping.Add("62", "SL60");
            NoteMapping.Add("63", "SL61");
            NoteMapping.Add("64", "SL62");

            NoteMapping.Add("26", "SL126");
            NoteMapping.Add("31", "SL131");

            NoteMapping.Add("27", "SL127");
            NoteMapping.Add("32", "SL132");



            #endregion

            #region Note API Map

            NoteUrlMap.Add("SL01", "get_goodsService");
            NoteUrlMap.Add("SL02", "get_goodsService");
            NoteUrlMap.Add("SL03", "get_goodsService");
            NoteUrlMap.Add("SL04", "get_goodsService");
            NoteUrlMap.Add("SL05", "get_goodsService");
            NoteUrlMap.Add("SL07", "get_goodsService");
            NoteUrlMap.Add("SL08", "get_goodsService");
            NoteUrlMap.Add("SL10", "get_goodsService");
            NoteUrlMap.Add("SL11", "get_goodsService");
            NoteUrlMap.Add("SL12", "get_goodsService");
            NoteUrlMap.Add("SL13", "get_goodsService");
            NoteUrlMap.Add("SL16", "get_goodsService");
            NoteUrlMap.Add("SL17", "get_goodsService");
            NoteUrlMap.Add("SL21", "get_goodsService");
            NoteUrlMap.Add("SL22", "get_goodsService");
            NoteUrlMap.Add("SL23", "get_goodsService");
            NoteUrlMap.Add("SL06", "get_goodsService_01");
            NoteUrlMap.Add("SL18", "get_goodsService_01");
            NoteUrlMap.Add("SL14", "get_goodsService_02");
            NoteUrlMap.Add("SL15", "get_goodsService_02");
            NoteUrlMap.Add("SL20", "get_goodsService_03");

            #endregion


        }

        public List<Item> GetGoods_Service(string hsCode, string slNo, string periodKey)
        {
            try
            {
                string noteNo = slNo;
                slNo = NoteMapping[slNo];

                #region Prepare Param XML

                List<Parameter> parameters = new List<Parameter>();

                parameters.Add(new Parameter() { param_name = "GOSERV_CODE", value = hsCode });
                parameters.Add(new Parameter() { param_name = "PERIOD_KEY", value = periodKey });
                parameters.Add(new Parameter() { param_name = "SERIAL", value = "%" + slNo + "%" });

                BaseAPiParam param = new BaseAPiParam();
                List<List> lists = new List<List>();
                lists.Add(new List() { parameter = parameters });

                param.auth = new Auth();
                param.body = new Body() { api = NoteUrlMap[slNo], method = "get", list = lists };

                XmlDocument xmlDocument = GetMasterApiXML();
                XmlNamespaceManager manager = new XmlNamespaceManager(xmlDocument.NameTable);

                manager.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
                manager.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");
                manager.AddNamespace("xsd", "http://www.w3.org/2001/XMLSchema");
                manager.AddNamespace("tem", "http://tempuri.org/");


                XmlNode json = xmlDocument.SelectSingleNode("//tem:json", manager);

                json.InnerText = JsonConvert.SerializeObject(param);

                #endregion

                string result = PostData(URI, xmlDocument.InnerXml);

                BaseAPIResult apiResult = GetApiResult(result);

                if (!apiResult.response.Succeed)
                {
                    throw new Exception(ResponseCodes[apiResult.response.message] + " Code: '" + hsCode + "' Note No: " +
                                        noteNo);
                }

                return apiResult.response.items;

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private BaseAPIResult GetApiResult(string result)
        {
            XmlDocument xmlDocument = new XmlDocument();
            XmlNode json;
            xmlDocument.LoadXml(result);

            XmlNamespaceManager manager = new XmlNamespaceManager(xmlDocument.NameTable);

            manager.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
            manager.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            manager.AddNamespace("xsd", "http://www.w3.org/2001/XMLSchema");
            manager.AddNamespace("tem", "http://tempuri.org/");


            json = xmlDocument.SelectSingleNode("//tem:ActionResult", manager);

            BaseAPIResult apiResult = JsonConvert.DeserializeObject<BaseAPIResult>(json.InnerText);
            return apiResult;
        }


        public List<Item> Get_Banks()
        {
            try
            {
                XmlDocument xmlDocument = GetBankXML();

                string responseMessage = PostData(URI, xmlDocument.InnerXml);

                BaseAPIResult result = GetApiResult(responseMessage);

                return result.response.items;

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<Item> Get_Cushouse()
        {
            try
            {
                XmlDocument xmlDocument = GetCustomHouseXML();

                string responseMessage = PostData(URI, xmlDocument.InnerXml);

                BaseAPIResult result = GetApiResult(responseMessage);

                return result.response.items;

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<Item> Get_APIStatus(string mesgId)
        {
            try
            {

                #region Prepare Param XML
                XmlDocument xmlDocument = GetStatusXML();

                List<Parameter> parameters = new List<Parameter>();

                parameters.Add(new Parameter() { param_name = "MSGID", value = mesgId });

                BaseAPiParam param = new BaseAPiParam();
                List<List> lists = new List<List>();
                lists.Add(new List() { parameter = parameters });

                param.auth = new Auth();
                param.body = new Body() { api = "get_msgids", method = "get", list = lists };

                XmlNamespaceManager manager = new XmlNamespaceManager(xmlDocument.NameTable);

                manager.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
                manager.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");
                manager.AddNamespace("xsd", "http://www.w3.org/2001/XMLSchema");
                manager.AddNamespace("tem", "http://tempuri.org/");


                XmlNode json = xmlDocument.SelectSingleNode("//tem:json", manager);

                json.InnerText = JsonConvert.SerializeObject(param);

                #endregion


                string responseMessage = PostData(URISecondary, xmlDocument.InnerXml);

                BaseAPIResult result = GetApiResult(responseMessage);

                return result.response.items;

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private string PostData(string url, string payLoad)
        {
            try
            {
                WebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";

                byte[] byteArray = Encoding.UTF8.GetBytes(payLoad);
                request.ContentLength = byteArray.Length;

                request.ContentType = "text/xml;charset=UTF-8";

                NetworkCredential creds = GetCredentials();
                request.Credentials = creds;

                Stream datastream = request.GetRequestStream();
                datastream.Write(byteArray, 0, byteArray.Length);
                datastream.Close();

                WebResponse response = request.GetResponse();
                datastream = response.GetResponseStream();

                StreamReader reader = new StreamReader(datastream);

                string responseMessage = reader.ReadToEnd();

                reader.Close();

                return responseMessage;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private NetworkCredential GetCredentials()
        {
            // read from config file
            return new NetworkCredential("user01", "Ivas@2020");
        }


        public string GetSLNo(string noteNo)
        {
            if (!NoteMapping.ContainsKey(noteNo))
            {
                return "";
            }

            return NoteMapping[noteNo];
        }

        private XmlDocument GetMasterApiXML()
        {
            XmlDocument xmlDocument = new XmlDocument();

            string xml = @"<soap:Envelope xmlns:soap=""http://www.w3.org/2003/05/soap-envelope"" xmlns:tem=""http://tempuri.org/"">
                <soap:Header/>
                <soap:Body>
                <tem:Action>
                <!--Optional:-->
                <tem:json>
                
            </tem:json>
                </tem:Action>
                </soap:Body>
                </soap:Envelope>
                ";

            xmlDocument.LoadXml(xml);

            return xmlDocument;
        }

        private XmlDocument GetBankXML()
        {
            XmlDocument xmlDocument = new XmlDocument();

            string xml = @"<soap:Envelope xmlns:soap=""http://www.w3.org/2003/05/soap-envelope"" xmlns:tem=""http://tempuri.org/"">
                <soap:Header/>
                <soap:Body>
                <tem:Action>
                <!--Optional:-->
                <tem:json>{
                ""auth"": {
                    ""username"":""nbr"",
                    ""password"":""nbr@return""
                },
                ""body"": {
                    ""api"":""get_bank"",
                    ""method"":""get"",
                    ""list"": [

                        ]
                }     
            } 
            </tem:json>
                </tem:Action>
                </soap:Body>
                </soap:Envelope>";

            xmlDocument.LoadXml(xml);

            return xmlDocument;
        }

        private XmlDocument GetCustomHouseXML()
        {
            XmlDocument xmlDocument = new XmlDocument();

            string xml = @"<soap:Envelope xmlns:soap=""http://www.w3.org/2003/05/soap-envelope"" xmlns:tem=""http://tempuri.org/"">
                <soap:Header/>
                <soap:Body>
                <tem:Action>
                <!--Optional:-->
                <tem:json>{
                ""auth"": {
                    ""username"":""nbr"",
                    ""password"":""nbr@return""
                },
                ""body"": {
                    ""api"":""get_cushouse"",
                    ""method"":""get"",
                    ""list"": [

                        ]
                }     
            } 
            </tem:json>
                </tem:Action>
                </soap:Body>
                </soap:Envelope>";

            xmlDocument.LoadXml(xml);

            return xmlDocument;
        }

        private XmlDocument GetStatusXML()
        {
            XmlDocument xmlDocument = new XmlDocument();

            string xml = @"<soap:Envelope xmlns:soap=""http://www.w3.org/2003/05/soap-envelope"" xmlns:tem=""http://tempuri.org/"">
<soap:Header/>
<soap:Body>
<tem:Action>
<!--Optional:-->
<tem:json>{


</tem:json>
</tem:Action>
</soap:Body>
</soap:Envelope>";

            xmlDocument.LoadXml(xml);

            return xmlDocument;
        }
    }
}
