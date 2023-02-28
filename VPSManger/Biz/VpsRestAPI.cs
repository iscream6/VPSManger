using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using VPSManger.Common;
using VPSManger.DTO;

namespace VPSManger.Biz
{
    public sealed class VpsRestAPI
    {
        public List<ServerDTO> GetServerList()
        {
            var targetURL = "https://api.goodbyeblock.com/api/serverlists";

            List<ServerDTO> serverList = new List<ServerDTO>();

            try
            {
                string sServersJson = RestController.Instance.RequestHttpsGET(targetURL);

                var jArray = JArray.Parse(sServersJson);

                foreach (var item in jArray.Children())
                {
                    ServerDTO s = item.ToObject<ServerDTO>();
                    serverList.Add(s);
                }

                //ID 순 정렬
                serverList.Sort((x, y) => int.Parse(x.Id).CompareTo(int.Parse(y.Id)));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return serverList;
        }

        public bool SaveServer(ServerDTO server)
        {
            bool bResult = false;
            string targetUrl = "https://api.goodbyeblock.com/api/serverlists";

            if (server.IsDelete)
            {
                bResult = RestController.Instance.RequestHttpsDelete(targetUrl + "/" + server.Id);

            }
            else if (server.IsNew)
            {
                bResult = RestController.Instance.RequestHttpsPOST<ServerDTO>(targetUrl, server);
            }
            else if (server.IsModify)
            {
                bResult = RestController.Instance.RequestHttpsDelete(targetUrl + "/" + server.Id);
                if(bResult)
                {
                    bResult = RestController.Instance.RequestHttpsPOST<ServerDTO>(targetUrl, server);
                }
            }

            return bResult;
        }
    }
}
