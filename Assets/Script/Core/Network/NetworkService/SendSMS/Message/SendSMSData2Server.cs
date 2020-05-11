using UnityEngine;
using System.Collections;

public class SendSMSData2Server 
{
    public string internationalTelephoneCode;
    public string phoneNumber;
    public string templateID;
    public string[] parameters;

    public SendSMSData2Server()
    {

    }

    public SendSMSData2Server(string internationalTelephoneCode, string phoneNumber, string templateID, string[] parameters)
    {
        this.internationalTelephoneCode = internationalTelephoneCode;
        this.phoneNumber = phoneNumber;
        this.templateID = templateID;
        this.parameters = parameters;
    }
}
