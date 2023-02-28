using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPSManger.Common;

namespace VPSManger.DTO
{
    public sealed class ServerDTO
    {
        public string Id { get; set; }
        public string State { get; set; }
        public string UserCount { get; set; }
        public string RegionCode { get; set; }
        public string RegionName { get; set; }
        public string ServerScript { get; set; }

        public bool IsNew { get; private set; }
        public bool IsModify { get; private set; }
        public bool IsDelete { get; private set; }

        public ServerDTO()
        {
            IsNew = false;
            IsModify = false;
            IsDelete = false;
        }

        public static ServerDTO NewDTO()
        {
            ServerDTO newDTO = new ServerDTO();
            newDTO.IsNew = true;
            newDTO.IsModify = false;
            newDTO.State = "3";
            newDTO.UserCount = "0";
            newDTO.RegionCode = "0";
            newDTO.RegionName = "ToKyo";
            newDTO.ServerScript = "";

            return newDTO;
        }

        public void ChangeMode(Status status)
        {
            if(status == Status.Modify) 
            {
                IsModify = true;
            }
            else if(status == Status.Delete)
            {
                IsDelete = true;
            }
        }

        public ServerDTO Copy()
        {
            ServerDTO copyDTO = new ServerDTO();
            copyDTO.Id = Id;
            copyDTO.State = State;
            copyDTO.UserCount = UserCount;
            copyDTO.RegionCode = RegionCode;
            copyDTO.RegionName = RegionName;
            copyDTO.ServerScript = ServerScript;
            copyDTO.IsNew = IsNew;
            copyDTO.IsModify = false;
            
            return copyDTO;
        }
    }
}
