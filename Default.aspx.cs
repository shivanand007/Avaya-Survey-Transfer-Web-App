﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.UI;
using System.Xml;

public partial class _Default : Page
{



    protected void Page_Load(object sender, EventArgs e)
    {
        string contactId = Request.QueryString["contactId"]; // taking contact Id from url

        // generate sso token from sessionservice.logintoCct
        string SsoRequest = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:ses=\"http://www.nortel.com/soa/oi/cct/types/SessionService\" xmlns:typ=\"http://www.nortel.com/soa/oi/cct/types\">\n" +
            "   <soapenv:Header/>\n" +
            "   <soapenv:Body>\n" +
            "      <ses:LogInToCCTServerRequest>\n" +
            "         <ses:authenticationLevel>\n" +
            "            <typ:username>its-shatha-al</typ:username>\n" +
            "            <typ:password>R@inbow1234!@</typ:password>\n" +
            "            <typ:domain>Arabbank.plc</typ:domain>\n" +
            "         </ses:authenticationLevel>\n" +
            "      </ses:LogInToCCTServerRequest>\n" +
            "   </soapenv:Body>\n" +
            "</soapenv:Envelope>";

        string url = "http://jo00-ccenterha:9080/SOAOICCT/services/SessionService";                             //needs to be updated
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.ContentType = "text/xml;charset=\"UTF-8\"";
        request.Accept = "text/xml";
        request.Method = "POST";
        request.UserAgent = "its-shatha-al";
        byte[] encod1 = Encoding.UTF8.GetBytes(SsoRequest);
        request.ContentLength = encod1.Length;
        //  XmlDocument SOAPReqBody = new XmlDocument();
        Stream stream = request.GetRequestStream();
        stream.Write(encod1, 0, encod1.Length);
        stream.Close();
        WebResponse Serviceres = request.GetResponse();
        StreamReader rd = new StreamReader(Serviceres.GetResponseStream());
        string SsoResponse = rd.ReadToEnd();

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(SsoResponse);

        XmlNodeList name = xmlDoc.GetElementsByTagName("token");

        string SsoToken = name[0].InnerText;

        //*******************************************************************************************************************************************************************************************************************************************//
        // Invoking ContactService.getConnections
        string getConnectionsRequest = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:con=\"http://www.nortel.com/soa/oi/cct/types/ContactService\" xmlns:typ=\"http://www.nortel.com/soa/oi/cct/types\">\n" +
                            "   <soapenv:Header/>\n" +
                            "   <soapenv:Body>\n" +
                            "      <con:GetConnectionsRequest>\n" +
                            "         <con:ssoToken>\n" +
                            "            <!--Optional:-->\n" +
                            "            <typ:token>" + SsoToken + "</typ:token>\n" +
                            "         </con:ssoToken>\n" +
                            "         <con:contact>\n" +
                            "            <typ:externalContactId>" + contactId + "</typ:externalContactId>\n" +
                            "            <typ:provider>\n" +
                            "               <typ:providerName>Passive</typ:providerName>\n" +
                            "            </typ:provider>\n" +
                            "            <typ:contactTypes>\n" +
                            "               <typ:item>\n" +
                            "                  <typ:type></typ:type>\n" +
                            "               </typ:item>\n" +
                            "            </typ:contactTypes>\n" +
                            "         </con:contact>\n" +
                            "      </con:GetConnectionsRequest>\n" +
                            "   </soapenv:Body>\n" +
                            "</soapenv:Envelope>";

        string getConnectionsUrl = "http://jo00-ccenterha:9080/SOAOICCT/services/ContactService";                               //needs to be updated
        HttpWebRequest request1 = (HttpWebRequest)WebRequest.Create(getConnectionsUrl);
        request1.ContentType = "text/xml;charset=\"UTF-8\"";
        request1.Accept = "text/xml";
        request1.Method = "POST";
        byte[] encod2 = Encoding.UTF8.GetBytes(getConnectionsRequest);
        request1.ContentLength = encod2.Length;
        XmlDocument SOAPReqBody1 = new XmlDocument();
        Stream stream1 = request1.GetRequestStream();
        stream1.Write(encod2, 0, encod2.Length);
        stream1.Close();
        WebResponse Serviceres1 = request1.GetResponse();
        StreamReader rd1 = new StreamReader(Serviceres1.GetResponseStream());

        string getConnectionsResponse = rd1.ReadToEnd();        // response of getConnectionsResponse 
                                                                //writting stream result on console    
        Console.WriteLine(getConnectionsResponse);
        Console.ReadLine();

        XmlDocument xmlDoc1 = new XmlDocument();
        xmlDoc1.LoadXml(getConnectionsResponse);

        XmlNodeList ConnectionIDobj = xmlDoc1.GetElementsByTagName("connectionId");

        List<string> ConnectionIDlist = new List<string>();

        int count = ConnectionIDobj.Count;
        Trace.Warn("The list count is :" + count);
        if (count == 1)
        {
            Session["ConnectionID"] = ConnectionIDobj[0].InnerText;
            ConnectionIDlist.Add(Session["ConnectionID"].ToString());

        }
        if (count == 2)
        {
            Session["ConnectionID"] = ConnectionIDobj[0].InnerText;
            Session["ConnectionID1"] = ConnectionIDobj[1].InnerText;
            ConnectionIDlist.Add(Session["ConnectionID"].ToString());
            ConnectionIDlist.Add(Session["ConnectionID1"].ToString());

            //Session["ConnectionID2"] = "7eb25961-3905-4374-9b5c-b30558d13fe5";
        }
        else
        {
            Session["ConnectionID"] = ConnectionIDobj[0].InnerText;
            Session["ConnectionID1"] = ConnectionIDobj[1].InnerText;
            Session["ConnectionID2"] = ConnectionIDobj[2].InnerText;
            ConnectionIDlist.Add(Session["ConnectionID"].ToString());
            ConnectionIDlist.Add(Session["ConnectionID1"].ToString());
            ConnectionIDlist.Add(Session["ConnectionID2"].ToString());
        }

        ConnectionIDlist.ForEach(Trace.Warn); // list of connections

        //*******************************************************************************************************************************************************************************************

        //INVOKING getTerminalConnections THREE TIMES TILL WE GET SOME RESPONSE OUTPUT TerminalConnectionID 

        foreach (string connectionIDres in ConnectionIDlist)
        {



            string getTerminalConnectionsRequest = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:con=\"http://www.nortel.com/soa/oi/cct/types/ConnectionService\" xmlns:typ=\"http://www.nortel.com/soa/oi/cct/types\">\n" +
                        "   <soapenv:Header/>\n" +
                        "   <soapenv:Body>\n" +
                        "      <con:GetTerminalConnectionsRequest>\n" +
                        "         <con:ssoToken>\n" +
                        "            <!--Optional:-->\n" +
                        "            <typ:token>" + SsoToken + "</typ:token>\n" +
                        "         </con:ssoToken>\n" +
                        "         <con:connection>\n" +
                        "            <typ:connectionId>" + connectionIDres + "</typ:connectionId>\n" +
                        "         </con:connection>\n" +
                        "      </con:GetTerminalConnectionsRequest>\n" +
                        "   </soapenv:Body>\n" +
                        "</soapenv:Envelope>";

            string getTerminalConnectionsurl = "http://jo00-ccenterha:9080/SOAOICCT/services/ConnectionService";                                                     // this URL needs to be updated 

            HttpWebRequest request2 = (HttpWebRequest)WebRequest.Create(getTerminalConnectionsurl);
            request2.ContentType = "text/xml;charset=\"UTF-8\"";
            request2.Accept = "text/xml";
            request2.Method = "POST";

            byte[] encod3 = Encoding.UTF8.GetBytes(getTerminalConnectionsRequest);
            request2.ContentLength = encod3.Length;
            XmlDocument SOAPReqBody2 = new XmlDocument();
            Stream stream2 = request2.GetRequestStream();
            stream2.Write(encod3, 0, encod3.Length);
            stream2.Close();

            WebResponse Serviceres2 = request2.GetResponse();
            StreamReader rd2 = new StreamReader(Serviceres2.GetResponseStream());

            string getTerminalConnectionResponse = rd2.ReadToEnd();

            if (getTerminalConnectionResponse.Contains("ns2:terminalConnections"))
            {


                XmlDocument xmlDoc2 = new XmlDocument();
                xmlDoc2.LoadXml(getTerminalConnectionResponse);



                XmlNodeList AgentTerminalIDobj = xmlDoc2.GetElementsByTagName("terminalConnectionId");  //UPDATE BY RESPONSE OF GETTERMINAL CONNECTION

                Session["TerminalConnectionID"] = AgentTerminalIDobj[0].InnerText;

                Trace.Warn("Agent Terminal  : " + Session["TerminalConnectionID"]);

            }

        }

        //***************************************************************************************************************************************************************************

        //  INVOKING FINAL WEB SERVICE TO GET CURRENT HANDLING TERMINAL BY AgentTerminalConnection.GetTerminal


        string GetAgentTerminalRequest = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:agen=\"http://www.nortel.com/soa/oi/cct/types/AgentTerminalConnectionService\" xmlns:typ=\"http://www.nortel.com/soa/oi/cct/types\">\n" +
                "   <soapenv:Header/>\n" +
                "   <soapenv:Body>\n" +
                "      <agen:GetTerminalRequest>\n" +
                "         <agen:ssoToken>\n" +
                "            <!--Optional:-->\n" +
                "            <typ:token>" + SsoToken + "</typ:token>\n" +
                "         </agen:ssoToken>\n" +
                "         <agen:terminalConnection>\n" +
                "            <typ:terminalConnectionId>" + Session["TerminalConnectionID"] + "</typ:terminalConnectionId>\n" +
                "         </agen:terminalConnection>\n" +
                "      </agen:GetTerminalRequest>\n" +
                "   </soapenv:Body>\n" +
                "</soapenv:Envelope>";



        string GetAgentTerminalurl = "http://jo00-ccenterha:9080/SOAOICCT/services/AgentTerminalConnectionService";         /// update the url 

        HttpWebRequest request3 = (HttpWebRequest)WebRequest.Create(GetAgentTerminalurl);
        request3.ContentType = "text/xml;charset=\"UTF-8\"";
        request3.Accept = "text/xml";
        request3.Method = "POST";

        byte[] encod4 = Encoding.UTF8.GetBytes(GetAgentTerminalRequest);
        request3.ContentLength = encod4.Length;
        XmlDocument SOAPReqBody3 = new XmlDocument();
        Stream stream3 = request3.GetRequestStream();
        stream3.Write(encod4, 0, encod4.Length);
        stream3.Close();
        WebResponse Serviceres3 = request3.GetResponse();
        StreamReader rd3 = new StreamReader(Serviceres3.GetResponseStream());

        string GetTerminalResponse = rd3.ReadToEnd();


        XmlDocument xmlDoc3 = new XmlDocument();
        xmlDoc3.LoadXml(GetTerminalResponse);

        XmlNodeList GetTerminalNameIDobj = xmlDoc3.GetElementsByTagName("terminalName");  //UPDATE BY RESPONSE OF GetTerminalResponse CONNECTION
        XmlNodeList GetTerminalTypeIDobj = xmlDoc3.GetElementsByTagName("terminalType");

        string TerminalName = GetTerminalNameIDobj[0].InnerText;
        string TerminalType = GetTerminalTypeIDobj[0].InnerText;



        Trace.Warn("Agent Terminal Name : " + TerminalName);
        Trace.Warn("Agent Terminal Type : " + TerminalType);

//**************************************************IMPLEMENTING HASH TABLE FOR SUVRVEY skillset MAPPING****************************************************************


        Dictionary<string, string> SurveyMapping = new Dictionary<string, string>();

        SurveyMapping.Add("AE_ELT_AR", "sip:49005@arabbank.plc");
        SurveyMapping.Add("AE_ELT_EN", "sip:49012@arabbank.plc");
        SurveyMapping.Add("JO_ELT_AR", "sip:49002@arabbank.plc");
        SurveyMapping.Add("JO_ELT_EN", "sip:49004@arabbank.plc");
        SurveyMapping.Add("BH_ELT_AR", "sip:49006@arabbank.plc");
        SurveyMapping.Add("BH_ELT_EN", "sip:49013@arabbank.plc");
        SurveyMapping.Add("LB_ELT_AR", "sip:44000@arabbank.plc");
        SurveyMapping.Add("LB_ELT_EN", "sip:49024@arabbank.plc");
        SurveyMapping.Add("PS_ELT_AR", "sip:49025@arabbank.plc");
        SurveyMapping.Add("PS_ELT_EN", "sip:49007@arabbank.plc");
        SurveyMapping.Add("QA_ELT_AR", "sip:49011@arabbank.plc");
        SurveyMapping.Add("QA_ELT_EN", "sip:49010@arabbank.plc");
        SurveyMapping.Add("JO_PRM_AR", "sip:49003@arabbank.plc");
        SurveyMapping.Add("JO_PRM_EN", "sip:49015@arabbank.plc");
        SurveyMapping.Add("PS_PRM_AR", "sip:49019@arabbank.plc");
        SurveyMapping.Add("PS_PRM_EN", "sip:49018@arabbank.plc");
        SurveyMapping.Add("JO_SBB_AR", "sip:49014@arabbank.plc");
        SurveyMapping.Add("JO_SBB_EN", "sip:49017@arabbank.plc");
        SurveyMapping.Add("PS_SBB_AR", "sip:49009@arabbank.plc");
        SurveyMapping.Add("PS_SBB_EN", "sip:49008@arabbank.plc");

        var SkilsetInhashMap = SurveyMapping.Keys.ToList();
        string skillset = Request.QueryString["skillset"]; // taking skillset from url 

        foreach (string k in SkilsetInhashMap)
        {
            if (k == skillset)
            {
                Session["DestinationAddress"] = SurveyMapping[k];
                Trace.Warn("" + k + "  : " + SurveyMapping[k] + "");
                Transfersurveyname.Text = k;
                
                DropDownList1.Text = k;

            }
            else
            {
                Session["DestinationAddress"] = "49004"; //default skillset 
            }

        }


    }

    //**************************************************************************************************************************************************************************************************

    protected void Button1_Click(object sender, EventArgs e)
    {

        string contactId = Request.QueryString["contactId"]; // taking contact Id from url

        // generate sso token from sessionservice.logintoCct
        string SsoRequest = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:ses=\"http://www.nortel.com/soa/oi/cct/types/SessionService\" xmlns:typ=\"http://www.nortel.com/soa/oi/cct/types\">\n" +
            "   <soapenv:Header/>\n" +
            "   <soapenv:Body>\n" +
            "      <ses:LogInToCCTServerRequest>\n" +
            "         <ses:authenticationLevel>\n" +
            "            <typ:username>acraadmin</typ:username>\n" +
            "            <typ:password>z!Gd5bh*7</typ:password>\n" +
            "            <typ:domain>Arabbank.plc</typ:domain>\n" +
            "         </ses:authenticationLevel>\n" +
            "      </ses:LogInToCCTServerRequest>\n" +
            "   </soapenv:Body>\n" +
            "</soapenv:Envelope>";

        string url = "http://jo00-ccenterha:9080/SOAOICCT/services/SessionService";                             //needs to be updated
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.ContentType = "text/xml;charset=\"UTF-8\"";
        request.Accept = "text/xml";
        request.Method = "POST";
        request.UserAgent = "acraadmin";
        byte[] encod1 = Encoding.UTF8.GetBytes(SsoRequest);
        request.ContentLength = encod1.Length;
        //  XmlDocument SOAPReqBody = new XmlDocument();
        Stream stream = request.GetRequestStream();
        stream.Write(encod1, 0, encod1.Length);
        stream.Close();
        WebResponse Serviceres = request.GetResponse();
        StreamReader rd = new StreamReader(Serviceres.GetResponseStream());
        string SsoResponse = rd.ReadToEnd();
        //writting stream result on console    
        Console.WriteLine(SsoResponse);
        Console.ReadLine();
        Trace.Warn("SSO TOKEN LOGIN TO CCT SERVER VIA SHATHA" + SsoResponse);
        Trace.Warn("REQUEST TO LOGIN SERVICE" + SsoRequest);

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(SsoResponse);

        XmlNodeList name = xmlDoc.GetElementsByTagName("token");

        string SsoToken = name[0].InnerText;
        Trace.Warn("THE TOKEN FROM WEB IS : " + SsoToken);

        //*******************************************************************************************************************************************************************************************************************************************//
        // Invoking ContactService.getConnections
        string getConnectionsRequest = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:con=\"http://www.nortel.com/soa/oi/cct/types/ContactService\" xmlns:typ=\"http://www.nortel.com/soa/oi/cct/types\">\n" +
                            "   <soapenv:Header/>\n" +
                            "   <soapenv:Body>\n" +
                            "      <con:GetConnectionsRequest>\n" +
                            "         <con:ssoToken>\n" +
                            "            <!--Optional:-->\n" +
                            "            <typ:token>" + SsoToken + "</typ:token>\n" +
                            "         </con:ssoToken>\n" +
                            "         <con:contact>\n" +
                            "            <typ:externalContactId>" + contactId + "</typ:externalContactId>\n" +
                            "            <typ:provider>\n" +
                            "               <typ:providerName>Passive</typ:providerName>\n" +
                            "            </typ:provider>\n" +
                            "            <typ:contactTypes>\n" +
                            "               <typ:item>\n" +
                            "                  <typ:type></typ:type>\n" +
                            "               </typ:item>\n" +
                            "            </typ:contactTypes>\n" +
                            "         </con:contact>\n" +
                            "      </con:GetConnectionsRequest>\n" +
                            "   </soapenv:Body>\n" +
                            "</soapenv:Envelope>";
        Trace.Write("Request sent to to api : - " + getConnectionsRequest);
        string getConnectionsUrl = "http://jo00-ccenterha:9080/SOAOICCT/services/ContactService";                               //needs to be updated
        HttpWebRequest request1 = (HttpWebRequest)WebRequest.Create(getConnectionsUrl);
        request1.ContentType = "text/xml;charset=\"UTF-8\"";
        request1.Accept = "text/xml";
        request1.Method = "POST";
        byte[] encod2 = Encoding.UTF8.GetBytes(getConnectionsRequest);
        request1.ContentLength = encod2.Length;
        XmlDocument SOAPReqBody1 = new XmlDocument();
        Stream stream1 = request1.GetRequestStream();
        stream1.Write(encod2, 0, encod2.Length);
        stream1.Close();
        WebResponse Serviceres1 = request1.GetResponse();
        StreamReader rd1 = new StreamReader(Serviceres1.GetResponseStream());

        string getConnectionsResponse = rd1.ReadToEnd();        // response of getConnectionsResponse 
                                                                //writting stream result on console    
        Console.WriteLine(getConnectionsResponse);
        Console.ReadLine();
        Trace.Write("RESPONSE of Get Connection : " + getConnectionsResponse);

        XmlDocument xmlDoc1 = new XmlDocument();
        xmlDoc1.LoadXml(getConnectionsResponse);
        Trace.Write("***************************RESPONSE XML LOADED *******8 : - ");

        XmlNodeList ConnectionIDobj = xmlDoc1.GetElementsByTagName("connectionId");
        Trace.Write("***************************SToRED COONECTION ONE  ************");

        List<string> ConnectionIDlist = new List<string>();

        int count = ConnectionIDobj.Count;
        Trace.Warn("The list count is :" + count);
        if (count == 1)
        {
            Session["ConnectionID"] = ConnectionIDobj[0].InnerText;
            ConnectionIDlist.Add(Session["ConnectionID"].ToString());
            
        }
        if (count == 2)
        {
            Session["ConnectionID"] = ConnectionIDobj[0].InnerText;
            Session["ConnectionID1"] = ConnectionIDobj[1].InnerText;
            ConnectionIDlist.Add(Session["ConnectionID"].ToString());
            ConnectionIDlist.Add(Session["ConnectionID1"].ToString());

        }
        else
        {
            Session["ConnectionID"] = ConnectionIDobj[0].InnerText;
            Session["ConnectionID1"] = ConnectionIDobj[1].InnerText;
            Session["ConnectionID2"] = ConnectionIDobj[2].InnerText;
            ConnectionIDlist.Add(Session["ConnectionID"].ToString());
            ConnectionIDlist.Add(Session["ConnectionID1"].ToString());
            ConnectionIDlist.Add(Session["ConnectionID2"].ToString());
        }

        Trace.Write("BELOW IS LIST OF CONEECTION ID GOT FROM WEB SERVICE RESPONSE : - ");
        ConnectionIDlist.ForEach(Trace.Warn); // list of connections

        Trace.Write("EXCUTING FOR LOOP TO TRY ALL THREE VALUES FROM LIST IN TO GET TERMINALCONNECTIONID : - ");

//*******************************************************************************************************************************************************************************************

   //INVOKING getTerminalConnections THREE TIMES TILL WE GET SOME RESPONSE OUTPUT TerminalConnectionID 

        foreach (string connectionIDres in ConnectionIDlist)
        {

            Trace.Write("Loop starts for connectionids: | ");

            string getTerminalConnectionsRequest = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:con=\"http://www.nortel.com/soa/oi/cct/types/ConnectionService\" xmlns:typ=\"http://www.nortel.com/soa/oi/cct/types\">\n" +
                        "   <soapenv:Header/>\n" +
                        "   <soapenv:Body>\n" +
                        "      <con:GetTerminalConnectionsRequest>\n" +
                        "         <con:ssoToken>\n" +
                        "            <!--Optional:-->\n" +
                        "            <typ:token>" + SsoToken + "</typ:token>\n" +
                        "         </con:ssoToken>\n" +
                        "         <con:connection>\n" +
                        "            <typ:connectionId>" + connectionIDres + "</typ:connectionId>\n" +
                        "         </con:connection>\n" +
                        "      </con:GetTerminalConnectionsRequest>\n" +
                        "   </soapenv:Body>\n" +
                        "</soapenv:Envelope>";


            Trace.Write("Request for terminal connection id : " + getTerminalConnectionsRequest);

            string getTerminalConnectionsurl = "http://jo00-ccenterha:9080/SOAOICCT/services/ConnectionService";                                                     // this URL needs to be updated 
            Trace.Write("URL FOR CONNECTION SERVICE : | " + getTerminalConnectionsurl);
            HttpWebRequest request2 = (HttpWebRequest)WebRequest.Create(getTerminalConnectionsurl);
            request2.ContentType = "text/xml;charset=\"UTF-8\"";
            request2.Accept = "text/xml";
            request2.Method = "POST";

            byte[] encod3 = Encoding.UTF8.GetBytes(getTerminalConnectionsRequest);
            request2.ContentLength = encod3.Length;
            XmlDocument SOAPReqBody2 = new XmlDocument();
            Stream stream2 = request2.GetRequestStream();
            stream2.Write(encod3, 0, encod3.Length);
            stream2.Close();


            WebResponse Serviceres2 = request2.GetResponse();
            StreamReader rd2 = new StreamReader(Serviceres2.GetResponseStream());

            string getTerminalConnectionResponse = rd2.ReadToEnd();

            if (getTerminalConnectionResponse.Contains("ns2:terminalConnections"))
            {
                Trace.Write("WEB SERVICE SUCCESS Get Terminal Connection RESPONSE IS : " + getTerminalConnectionResponse);


                XmlDocument xmlDoc2 = new XmlDocument();
                xmlDoc2.LoadXml(getTerminalConnectionResponse);

                Trace.Write("response loaded");

                XmlNodeList AgentTerminalIDobj = xmlDoc2.GetElementsByTagName("terminalConnectionId");  //UPDATE BY RESPONSE OF GETTERMINAL CONNECTION

                Session["TerminalConnectionID"] = AgentTerminalIDobj[0].InnerText;

                Trace.Warn("Agent Terminal  : " + Session["TerminalConnectionID"]);

            }
            else
            {
                Trace.Write("*********CHECKING FOR NEXT CONNECTION ID *****");
            }

        }

//***************************************************************************************************************************************************************************

//  INVOKING FINAL WEB SERVICE TO GET CURRENT HANDLING TERMINAL BY AgentTerminalConnection.GetTerminal


        string GetAgentTerminalRequest = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:agen=\"http://www.nortel.com/soa/oi/cct/types/AgentTerminalConnectionService\" xmlns:typ=\"http://www.nortel.com/soa/oi/cct/types\">\n" +
                "   <soapenv:Header/>\n" +
                "   <soapenv:Body>\n" +
                "      <agen:GetTerminalRequest>\n" +
                "         <agen:ssoToken>\n" +
                "            <!--Optional:-->\n" +
                "            <typ:token>" + SsoToken + "</typ:token>\n" +
                "         </agen:ssoToken>\n" +
                "         <agen:terminalConnection>\n" +
                "            <typ:terminalConnectionId>" + Session["TerminalConnectionID"] + "</typ:terminalConnectionId>\n" +
                "         </agen:terminalConnection>\n" +
                "      </agen:GetTerminalRequest>\n" +
                "   </soapenv:Body>\n" +
                "</soapenv:Envelope>";

        Trace.Write("the request for agent erminal id is :" + GetAgentTerminalRequest);

        string GetAgentTerminalurl = "http://jo00-ccenterha:9080/SOAOICCT/services/AgentTerminalConnectionService";         /// update the url 

        HttpWebRequest request3 = (HttpWebRequest)WebRequest.Create(GetAgentTerminalurl);
        request3.ContentType = "text/xml;charset=\"UTF-8\"";
        request3.Accept = "text/xml";
        request3.Method = "POST";

        byte[] encod4 = Encoding.UTF8.GetBytes(GetAgentTerminalRequest);
        request3.ContentLength = encod4.Length;
        XmlDocument SOAPReqBody3 = new XmlDocument();
        Stream stream3 = request3.GetRequestStream();
        stream3.Write(encod4, 0, encod4.Length);
        stream3.Close();
        WebResponse Serviceres3 = request3.GetResponse();
        StreamReader rd3 = new StreamReader(Serviceres3.GetResponseStream());

        string GetTerminalResponse = rd3.ReadToEnd();
        Trace.Write("the response of web service is  : " + GetTerminalResponse);

        XmlDocument xmlDoc3 = new XmlDocument();
        xmlDoc3.LoadXml(GetTerminalResponse);

        XmlNodeList GetTerminalNameIDobj = xmlDoc3.GetElementsByTagName("terminalName");  //UPDATE BY RESPONSE OF GetTerminalResponse CONNECTION
        XmlNodeList GetTerminalTypeIDobj = xmlDoc3.GetElementsByTagName("terminalType");

        string TerminalName = GetTerminalNameIDobj[0].InnerText;
        string TerminalType = GetTerminalTypeIDobj[0].InnerText;

        Trace.Warn("Agent Terminal Response  : " + GetTerminalResponse);

        Trace.Warn("Agent Terminal Name : " + TerminalName);
        Trace.Warn("Agent Terminal Type : " + TerminalType);


//****************************************IMPLEMENTING HASH TABLE FOR SUVRVEY MAPPING********************************************************************************************



        Dictionary<string, string> SurveyMapping = new Dictionary<string, string>();

        SurveyMapping.Add("AE_ELT_AR", "sip:49005@arabbank.plc");
        SurveyMapping.Add("AE_ELT_EN", "sip:49012@arabbank.plc");
        SurveyMapping.Add("JO_ELT_AR", "sip:49002@arabbank.plc");
        SurveyMapping.Add("JO_ELT_EN", "sip:49004@arabbank.plc");
        SurveyMapping.Add("BH_ELT_AR", "sip:49006@arabbank.plc");
        SurveyMapping.Add("BH_ELT_EN", "sip:49013@arabbank.plc");
        SurveyMapping.Add("LB_ELT_AR", "sip:44000@arabbank.plc");
        SurveyMapping.Add("LB_ELT_EN", "sip:49024@arabbank.plc");
        SurveyMapping.Add("PS_ELT_AR", "sip:49025@arabbank.plc");
        SurveyMapping.Add("PS_ELT_EN", "sip:49007@arabbank.plc");
        SurveyMapping.Add("QA_ELT_AR", "sip:49011@arabbank.plc");
        SurveyMapping.Add("QA_ELT_EN", "sip:49010@arabbank.plc");
        SurveyMapping.Add("JO_PRM_AR", "sip:49003@arabbank.plc");
        SurveyMapping.Add("JO_PRM_EN", "sip:49015@arabbank.plc");
        SurveyMapping.Add("PS_PRM_AR", "sip:49019@arabbank.plc");
        SurveyMapping.Add("PS_PRM_EN", "sip:49018@arabbank.plc");
        SurveyMapping.Add("JO_SBB_AR", "sip:49014@arabbank.plc");
        SurveyMapping.Add("JO_SBB_EN", "sip:49017@arabbank.plc");
        SurveyMapping.Add("PS_SBB_AR", "sip:49009@arabbank.plc");
        SurveyMapping.Add("PS_SBB_EN", "sip:49008@arabbank.plc");

   

        var SkilsetInhashMap = SurveyMapping.Keys.ToList();
        Trace.Warn("******values stored in list wiht key values******");

        string skillset = Request.QueryString["skillset"]; // taking skillset from url 

        foreach (string k in SkilsetInhashMap)
        {
            if (k == skillset)
            {
                Session["DestinationAddress"] = SurveyMapping[k];
                Trace.Warn("" + k + "  : " + SurveyMapping[k] + "");
                // TextBox1.Text = k;
                
                DropDownList1.Text = k;

            }
            //else
            //{
            //    Session["DestinationAddress"] = "49004"; //default skillset 
            //}

        }

        //**************************************************IMPLEMENTING HASH TABLE FOR SUVERY NAMES MAPPING***********************************************************************************


        string AgentSurveyselect = DropDownList1.SelectedValue;
        Trace.Warn("agent selected value : " + AgentSurveyselect); //agent is selecting survey name here 

        if (AgentSurveyselect != string.Empty|| AgentSurveyselect != null)
        {

        
            Dictionary<string, string> SurveyNamesMapping = new Dictionary<string, string>();

            SurveyNamesMapping.Add("Elite_AE_AR", "sip:49005@arabbank.plc");
            SurveyNamesMapping.Add("Elite_AE_EN", "sip:49012@arabbank.plc");
            SurveyNamesMapping.Add("Elite_AR", "sip:49002@arabbank.plc");
            SurveyNamesMapping.Add("Elite_EN", "sip:49004@arabbank.plc");
            SurveyNamesMapping.Add("Elite_BH_AR", "sip:49006@arabbank.plc");
            SurveyNamesMapping.Add("Elite_BH_EN", "sip:49013@arabbank.plc");
            SurveyNamesMapping.Add("Elite_LB_AR", "sip:44000@arabbank.plc");
            SurveyNamesMapping.Add("Elite_LB_EN", "sip:49024@arabbank.plc");
            SurveyNamesMapping.Add("Elite_PS_AR", "sip:49025@arabbank.plc");
            SurveyNamesMapping.Add("Elite_PS_EN", "sip:49007@arabbank.plc");
            SurveyNamesMapping.Add("Elite_QA_AR", "sip:49011@arabbank.plc");
            SurveyNamesMapping.Add("Elite_QA_EN", "sip:49010@arabbank.plc");
            SurveyNamesMapping.Add("PRM_JO_AR", "sip:49003@arabbank.plc");
            SurveyNamesMapping.Add("PRM_JO_EN", "sip:49015@arabbank.plc");
            SurveyNamesMapping.Add("PRM_PS_AR", "sip:49019@arabbank.plc");
            SurveyNamesMapping.Add("PRM_PS_EN", "sip:49018@arabbank.plc");
            SurveyNamesMapping.Add("shabab_JO_AR", "sip:49014@arabbank.plc");
            SurveyNamesMapping.Add("shabab_JO_EN", "sip:49017@arabbank.plc");
            SurveyNamesMapping.Add("shabab_PS_AR", "sip:49009@arabbank.plc");
            SurveyNamesMapping.Add("shabab_PS_EN", "sip:49008@arabbank.plc");

            var NamesMapWithDes = SurveyNamesMapping.Keys.ToList();
            Trace.Write("**********************sURVEY NAMES LOADED IN LIST********");






            foreach (string Des in NamesMapWithDes)
            {
                if (Des == AgentSurveyselect)
                {
                    Session["DestinationAddress"] = SurveyNamesMapping[Des];
                    Trace.Warn("" + Des + "  : " + SurveyNamesMapping[Des] + "");
                    Trace.Warn("%%%%%%%%%%%%  UPDATED VALUE FOR SURVEY %%%%%% ");

                    DropDownList1.Text = Des;
                }

            }
           
        }
        Trace.Write("***********UPDATED AGENT VALUE IS :***" + Session["DestinationAddress"]);






        //INVOKING FIRST TRANSFER WEB SERVICE BY GETTING AGENT INPUT OF SKILLSET



        string InitaiteTransferRequest = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:ses=\"http://www.nortel.com/soa/oi/cct/types/SessionService\" xmlns:typ=\"http://www.nortel.com/soa/oi/cct/types\">\n" +
                        "   <soapenv:Header/>\n" +
                        "   <soapenv:Body>\n" +
                        "      <ses:InitiateSupervisedTransferRequest>\n" +
                        "         <ses:ssoToken>\n" +
                        "            <!--Optional:-->\n" +
                        "            <typ:token>" + SsoToken + "</typ:token>\n" +
                        "         </ses:ssoToken>\n" +
                        "         <ses:terminal>            \n" +
                        "            <typ:terminalName>" + TerminalName + "</typ:terminalName>\n" +
                        "            <typ:terminalType>" + TerminalType + "</typ:terminalType>\n" +
                        "            <typ:provider>\n" +
                        "               <typ:providerName>Passive</typ:providerName>\n" +
                        "            </typ:provider>  \n" +
                        "         </ses:terminal>\n" +
                        "         <ses:contact>\n" +
                        "            <typ:externalContactId>" + contactId + "</typ:externalContactId>\n" +
                        "            <typ:provider>\n" +
                        "               <typ:providerName>Passive</typ:providerName>\n" +
                        "            </typ:provider>\n" +
                        "            <typ:contactTypes> \n" +
                        "            </typ:contactTypes>\n" +
                        "         </ses:contact>\n" +
                        "         <ses:address>\n" +
                        "            <typ:addressName>" + Session["DestinationAddress"] + "</typ:addressName>\n" +
                        "            <typ:addressType>EXTERNAL</typ:addressType>\n" +
                        "            <typ:provider>\n" +
                        "               <typ:providerName>Passive</typ:providerName>\n" +
                        "            </typ:provider>           \n" +
                        "         </ses:address>\n" +
                        "      </ses:InitiateSupervisedTransferRequest>\n" +
                        "   </soapenv:Body>\n" +
                        "</soapenv:Envelope>";

        string InitaiteTransferRequesturl = "http://jo00-ccenterha:9080/SOAOICCT/services/SessionService";

        HttpWebRequest request4 = (HttpWebRequest)WebRequest.Create(InitaiteTransferRequesturl);
        request4.ContentType = "text/xml;charset=\"UTF-8\"";
        request4.Accept = "text/xml";
        request4.Method = "POST";

        byte[] encod5 = Encoding.UTF8.GetBytes(InitaiteTransferRequest);
        request4.ContentLength = encod5.Length;
        XmlDocument SOAPReqBody4 = new XmlDocument();
        Stream stream4 = request4.GetRequestStream();
        stream4.Write(encod5, 0, encod5.Length);
        stream4.Close();
        WebResponse Serviceres4 = request4.GetResponse();
        StreamReader rd4 = new StreamReader(Serviceres4.GetResponseStream());

        string InitaiteTransferResponse = rd4.ReadToEnd();

        if (InitaiteTransferResponse.Contains("contactId"))
        {
            Trace.Write("INITATE TRANSFER SUCCESSFULL  :  " + InitaiteTransferResponse);
        }
        else
        {
            Trace.Write("INITATE TRANSFER FAILED CCT DOWN :  " + InitaiteTransferResponse);
        }


        XmlDocument xmlDoc4 = new XmlDocument();
        xmlDoc4.LoadXml(InitaiteTransferResponse);

        XmlNodeList Iitiateresponseobj = xmlDoc4.GetElementsByTagName("externalContactId");

        string ConsultedContactID = Iitiateresponseobj[0].InnerText;

        Trace.Write("THE EXTERNAL CONTACT ID RECIVED FOR NEW CALL   :  " + ConsultedContactID);


        ////***************************************************************************************************************************************************8

        // INVOKING FINAL TRANSFER WEB SERVICE  COMPLETE TRASFER WIHT NEW CONTACTID

        string CompleteTransferRequest = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:ses=\"http://www.nortel.com/soa/oi/cct/types/SessionService\" xmlns:typ=\"http://www.nortel.com/soa/oi/cct/types\">\n" +
            "   <soapenv:Header/>\n" +
            "   <soapenv:Body>\n" +
            "      <ses:CompleteSupervisedTransferRequest>\n" +
            "         <ses:ssoToken>\n" +
            "            <!--Optional:-->\n" +
            "            <typ:token>" + SsoToken + "</typ:token>\n" +
            "         </ses:ssoToken>\n" +
            "         <ses:terminal>\n" +
            "            <typ:terminalName>" + TerminalName + "</typ:terminalName>\n" +
            "            <typ:terminalType>" + TerminalType + "</typ:terminalType>\n" +
            "         </ses:terminal>\n" +
            "         <ses:contact>\n" +
            "            \n" +
            "            <typ:externalContactId>" + contactId + "</typ:externalContactId>\n" +
            "            \n" +
            "            <typ:provider>\n" +
            "               <typ:providerName>Passive</typ:providerName>\n" +
            "            </typ:provider>\n" +
            "            <typ:contactTypes>\n" +
            " \n" +
            "            </typ:contactTypes>\n" +
            "         </ses:contact>\n" +
            "         <ses:consultedContact>\n" +
            "           \n" +
            "            <typ:externalContactId>" + ConsultedContactID + "</typ:externalContactId>\n" +
            "           \n" +
            "            <typ:provider>\n" +
            "               <typ:providerName>Passive</typ:providerName>\n" +
            "            </typ:provider>\n" +
            "         </ses:consultedContact>\n" +
            "      </ses:CompleteSupervisedTransferRequest>\n" +
            "   </soapenv:Body>\n" +
            "</soapenv:Envelope>";
        Trace.Write("the request for complete transfer is : " + CompleteTransferRequest);


        string CompleteTransferRequesturl = "http://jo00-ccenterha:9080/SOAOICCT/services/SessionService";

        HttpWebRequest request5 = (HttpWebRequest)WebRequest.Create(CompleteTransferRequesturl);
        request5.ContentType = "text/xml;charset=\"UTF-8\"";
        request5.Accept = "text/xml";
        request5.Method = "POST";

        byte[] encod6 = Encoding.UTF8.GetBytes(CompleteTransferRequest);
        request5.ContentLength = encod6.Length;
        XmlDocument SOAPReqBody5 = new XmlDocument();
        Stream stream5 = request5.GetRequestStream();
        stream5.Write(encod6, 0, encod6.Length);
        stream5.Close();
        WebResponse Serviceres5 = request5.GetResponse();
        StreamReader rd5 = new StreamReader(Serviceres5.GetResponseStream());

        string CompleteTransferResponse = rd5.ReadToEnd();


        Trace.Write("CALL TRANSFERED SUCESSFULLY: " + CompleteTransferResponse);
        string script = "alert('CALL TRANSFERED SUCCESSFULLY');";
        ClientScript.RegisterClientScriptBlock(this.GetType(), "Info", script, true);


    }
}