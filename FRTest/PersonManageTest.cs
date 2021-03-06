﻿using FRTest.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FRTest
{
    public class PersonManageTest
    {
        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_STDXMLConfig(int iUserID, ref CHCNetSDK.NET_DVR_XML_CONFIG_INPUT lpInputParam, ref CHCNetSDK.NET_DVR_XML_CONFIG_OUTPUT lpOutputParam);

        public string SearchPerson(string IpAddress, string UserName, string Password, ushort Port)
        {

            CHCNetSDK.NET_DVR_USER_LOGIN_INFO nET_DVR_USER_LOGIN_INFO = new CHCNetSDK.NET_DVR_USER_LOGIN_INFO();

            nET_DVR_USER_LOGIN_INFO.sDeviceAddress = IpAddress;
            nET_DVR_USER_LOGIN_INFO.wPort = Port;
            nET_DVR_USER_LOGIN_INFO.sUserName = UserName;
            nET_DVR_USER_LOGIN_INFO.sPassword = Password;
            nET_DVR_USER_LOGIN_INFO.bUseAsynLogin = false;

            CHCNetSDK.NET_DVR_DEVICEINFO_V40 nET_DVR_DEVICEINFO_V40 = new CHCNetSDK.NET_DVR_DEVICEINFO_V40();
            nET_DVR_DEVICEINFO_V40.struDeviceV30.sSerialNumber = new byte[CHCNetSDK.SERIALNO_LEN];

            int userID = CHCNetSDK.NET_DVR_Login_V40(ref nET_DVR_USER_LOGIN_INFO, ref nET_DVR_DEVICEINFO_V40);

            string url = "POST /ISAPI/AccessControl/UserInfo/Search?format=json";

            CHCNetSDK.NET_DVR_XML_CONFIG_INPUT nET_DVR_XML_CONFIG_INPUT = new CHCNetSDK.NET_DVR_XML_CONFIG_INPUT();

            Int32 nInSize = Marshal.SizeOf(nET_DVR_XML_CONFIG_INPUT);
            nET_DVR_XML_CONFIG_INPUT.dwSize = (uint)nInSize;
            nET_DVR_XML_CONFIG_INPUT.lpRequestUrl = Marshal.StringToHGlobalAnsi(url);
            nET_DVR_XML_CONFIG_INPUT.dwRequestUrlLen = (uint)url.Length;

            var userInfoSearchCond = new { searchID = "0", searchResultPosition = 0, maxResults = 50 };
            var json = new { UserInfoSearchCond = userInfoSearchCond };
            var strJson = JsonConvert.SerializeObject(json);

            nET_DVR_XML_CONFIG_INPUT.lpInBuffer = Marshal.StringToHGlobalAnsi(strJson);
            nET_DVR_XML_CONFIG_INPUT.dwInBufferSize = (uint)strJson.Length;

            // reserve space for return data
            CHCNetSDK.NET_DVR_XML_CONFIG_OUTPUT nET_DVR_XML_CONFIG_OUTPUT = new CHCNetSDK.NET_DVR_XML_CONFIG_OUTPUT();
            nET_DVR_XML_CONFIG_OUTPUT.dwSize = (uint)Marshal.SizeOf(nET_DVR_XML_CONFIG_INPUT);
            nET_DVR_XML_CONFIG_OUTPUT.lpOutBuffer = Marshal.AllocHGlobal(3 * 1024 * 1024);
            nET_DVR_XML_CONFIG_OUTPUT.dwOutBufferSize = 3 * 1024 * 1024;
            nET_DVR_XML_CONFIG_OUTPUT.lpStatusBuffer = Marshal.AllocHGlobal(4096 * 4);
            nET_DVR_XML_CONFIG_OUTPUT.dwStatusSize = 4096 * 4;

            if (!NET_DVR_STDXMLConfig(userID, ref nET_DVR_XML_CONFIG_INPUT, ref nET_DVR_XML_CONFIG_OUTPUT))
            {
                uint getLastError = CHCNetSDK.NET_DVR_GetLastError();
                string errorStr = "NET_DVR_STDXMLConfig failed :" + getLastError;
                Console.WriteLine(errorStr);
            }

            string result = Marshal.PtrToStringAnsi(nET_DVR_XML_CONFIG_OUTPUT.lpOutBuffer);

            Console.WriteLine(result);


            Marshal.FreeHGlobal(nET_DVR_XML_CONFIG_INPUT.lpRequestUrl);
            Marshal.FreeHGlobal(nET_DVR_XML_CONFIG_OUTPUT.lpOutBuffer);
            Marshal.FreeHGlobal(nET_DVR_XML_CONFIG_OUTPUT.lpStatusBuffer);
            
            return result;
        }

        public string AddPerson(string IpAddress, string UserName, string Password, ushort Port, dynamic userInfo)
        {
            CHCNetSDK.NET_DVR_USER_LOGIN_INFO nET_DVR_USER_LOGIN_INFO = new CHCNetSDK.NET_DVR_USER_LOGIN_INFO();

            nET_DVR_USER_LOGIN_INFO.sDeviceAddress = IpAddress;
            nET_DVR_USER_LOGIN_INFO.wPort = Port;
            nET_DVR_USER_LOGIN_INFO.sUserName = UserName;
            nET_DVR_USER_LOGIN_INFO.sPassword = Password;
            nET_DVR_USER_LOGIN_INFO.bUseAsynLogin = false;

            CHCNetSDK.NET_DVR_DEVICEINFO_V40 nET_DVR_DEVICEINFO_V40 = new CHCNetSDK.NET_DVR_DEVICEINFO_V40();
            nET_DVR_DEVICEINFO_V40.struDeviceV30.sSerialNumber = new byte[CHCNetSDK.SERIALNO_LEN];

            int userID = CHCNetSDK.NET_DVR_Login_V40(ref nET_DVR_USER_LOGIN_INFO, ref nET_DVR_DEVICEINFO_V40);

            string url = "PUT /ISAPI/AccessControl/UserInfo/SetUp?format=json";

            CHCNetSDK.NET_DVR_XML_CONFIG_INPUT nET_DVR_XML_CONFIG_INPUT = new CHCNetSDK.NET_DVR_XML_CONFIG_INPUT();
            Int32 nInSize = Marshal.SizeOf(nET_DVR_XML_CONFIG_INPUT);
            nET_DVR_XML_CONFIG_INPUT.dwSize = (uint)Marshal.SizeOf(nET_DVR_XML_CONFIG_INPUT);
            nET_DVR_XML_CONFIG_INPUT.lpRequestUrl = Marshal.StringToHGlobalAnsi(url);
            nET_DVR_XML_CONFIG_INPUT.dwRequestUrlLen = (uint)url.Length;

            var json = new { UserInfo = userInfo };
            var strJson = JsonConvert.SerializeObject(json);

            nET_DVR_XML_CONFIG_INPUT.lpInBuffer = Marshal.StringToHGlobalAnsi(strJson);
            nET_DVR_XML_CONFIG_INPUT.dwInBufferSize = (uint)strJson.Length;

            CHCNetSDK.NET_DVR_XML_CONFIG_OUTPUT nET_DVR_XML_CONFIG_OUTPUT = new CHCNetSDK.NET_DVR_XML_CONFIG_OUTPUT();
            nET_DVR_XML_CONFIG_OUTPUT.dwSize = (uint)Marshal.SizeOf(nET_DVR_XML_CONFIG_INPUT);
            nET_DVR_XML_CONFIG_OUTPUT.lpOutBuffer = Marshal.AllocHGlobal(3 * 1024 * 1024);
            nET_DVR_XML_CONFIG_OUTPUT.dwOutBufferSize = 3 * 1024 * 1024;
            nET_DVR_XML_CONFIG_OUTPUT.lpStatusBuffer = Marshal.AllocHGlobal(4096 * 4);
            nET_DVR_XML_CONFIG_OUTPUT.dwStatusSize = 4096 * 4;

            if (!NET_DVR_STDXMLConfig(userID, ref nET_DVR_XML_CONFIG_INPUT, ref nET_DVR_XML_CONFIG_OUTPUT))
            {
                uint getLastError = CHCNetSDK.NET_DVR_GetLastError();
                string errorStr = "NET_DVR_STDXMLConfig failed :" + getLastError;
                Console.WriteLine(errorStr);
            }


            string result = Marshal.PtrToStringAnsi(nET_DVR_XML_CONFIG_OUTPUT.lpOutBuffer);
            string outXML = Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(result));
            Console.WriteLine(outXML);
            string outStatus = Marshal.PtrToStringAnsi(nET_DVR_XML_CONFIG_OUTPUT.lpStatusBuffer);
            Console.WriteLine("Status Output:\n" + outStatus);
            

            Marshal.FreeHGlobal(nET_DVR_XML_CONFIG_INPUT.lpRequestUrl);
            Marshal.FreeHGlobal(nET_DVR_XML_CONFIG_OUTPUT.lpOutBuffer);
            Marshal.FreeHGlobal(nET_DVR_XML_CONFIG_OUTPUT.lpStatusBuffer);
            return "ok";
        }
        public string EditPerson(string IpAddress, string UserName, string Password, ushort Port, dynamic userInfo)
        {
            CHCNetSDK.NET_DVR_USER_LOGIN_INFO nET_DVR_USER_LOGIN_INFO = new CHCNetSDK.NET_DVR_USER_LOGIN_INFO();

            nET_DVR_USER_LOGIN_INFO.sDeviceAddress = IpAddress;
            nET_DVR_USER_LOGIN_INFO.wPort = Port;
            nET_DVR_USER_LOGIN_INFO.sUserName = UserName;
            nET_DVR_USER_LOGIN_INFO.sPassword = Password;
            nET_DVR_USER_LOGIN_INFO.bUseAsynLogin = false;

            CHCNetSDK.NET_DVR_DEVICEINFO_V40 nET_DVR_DEVICEINFO_V40 = new CHCNetSDK.NET_DVR_DEVICEINFO_V40();
            nET_DVR_DEVICEINFO_V40.struDeviceV30.sSerialNumber = new byte[CHCNetSDK.SERIALNO_LEN];

            int userID = CHCNetSDK.NET_DVR_Login_V40(ref nET_DVR_USER_LOGIN_INFO, ref nET_DVR_DEVICEINFO_V40);

            string url = "PUT /ISAPI/AccessControl/UserInfo/SetUp?format=json";

            CHCNetSDK.NET_DVR_XML_CONFIG_INPUT nET_DVR_XML_CONFIG_INPUT = new CHCNetSDK.NET_DVR_XML_CONFIG_INPUT();
            Int32 nInSize = Marshal.SizeOf(nET_DVR_XML_CONFIG_INPUT);
            nET_DVR_XML_CONFIG_INPUT.dwSize = (uint)Marshal.SizeOf(nET_DVR_XML_CONFIG_INPUT);
            nET_DVR_XML_CONFIG_INPUT.lpRequestUrl = Marshal.StringToHGlobalAnsi(url);
            nET_DVR_XML_CONFIG_INPUT.dwRequestUrlLen = (uint)url.Length;

            var json = new { UserInfo = userInfo };
            var strJson = JsonConvert.SerializeObject(json);

            nET_DVR_XML_CONFIG_INPUT.lpInBuffer = Marshal.StringToHGlobalAnsi(strJson);
            nET_DVR_XML_CONFIG_INPUT.dwInBufferSize = (uint)strJson.Length;

            CHCNetSDK.NET_DVR_XML_CONFIG_OUTPUT nET_DVR_XML_CONFIG_OUTPUT = new CHCNetSDK.NET_DVR_XML_CONFIG_OUTPUT();
            nET_DVR_XML_CONFIG_OUTPUT.dwSize = (uint)Marshal.SizeOf(nET_DVR_XML_CONFIG_INPUT);
            nET_DVR_XML_CONFIG_OUTPUT.lpOutBuffer = Marshal.AllocHGlobal(3 * 1024 * 1024);
            nET_DVR_XML_CONFIG_OUTPUT.dwOutBufferSize = 3 * 1024 * 1024;
            nET_DVR_XML_CONFIG_OUTPUT.lpStatusBuffer = Marshal.AllocHGlobal(4096 * 4);
            nET_DVR_XML_CONFIG_OUTPUT.dwStatusSize = 4096 * 4;

            if (!NET_DVR_STDXMLConfig(userID, ref nET_DVR_XML_CONFIG_INPUT, ref nET_DVR_XML_CONFIG_OUTPUT))
            {
                uint getLastError = CHCNetSDK.NET_DVR_GetLastError();
                string errorStr = "NET_DVR_STDXMLConfig failed :" + getLastError;
                Console.WriteLine(errorStr);
            }


            string result = Marshal.PtrToStringAnsi(nET_DVR_XML_CONFIG_OUTPUT.lpOutBuffer);
            string outXML = Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(result));
            Console.WriteLine(outXML);
            string outStatus = Marshal.PtrToStringAnsi(nET_DVR_XML_CONFIG_OUTPUT.lpStatusBuffer);
            Console.WriteLine("Status Output:\n" + outStatus);


            Marshal.FreeHGlobal(nET_DVR_XML_CONFIG_INPUT.lpRequestUrl);
            Marshal.FreeHGlobal(nET_DVR_XML_CONFIG_OUTPUT.lpOutBuffer);
            Marshal.FreeHGlobal(nET_DVR_XML_CONFIG_OUTPUT.lpStatusBuffer);
            return "ok";
        }
    }
}
