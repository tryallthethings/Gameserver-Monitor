using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace GameServerInfo
{
    class Samp : GameServerInfo.Protocol
    {
        byte[] iQuery = new byte[10];

        public Samp(string host, int port)
            : base(host, port)
        {
            base._protocol = GameProtocol.Samp;
            iQuery = new byte[] { (byte)'S', (byte)'A', (byte)'M', (byte)'P', 0x21, 0x21, 0x21, 0x21, 0x00, 0x00, (byte)'i' };
            Connect(host, port);
        }

        public override void GetServerInfo()
        {
            base.GetServerInfo();
            if (!IsOnline) { return; }
            Query(Encoding.Default.GetString(iQuery));
            _params["passworded"]=(Response[11]==0 ? true : false).ToString();

            byte[] numPlayers = new byte[2];
            Array.Copy(Response,12,numPlayers,0,2);
            _params["numplayers"] = System.BitConverter.ToInt16(numPlayers,0).ToString();

            byte[] maxPlayers = new byte[2];
            Array.Copy(Response, 14, maxPlayers, 0, 2);
            _params["maxplayers"] = System.BitConverter.ToInt16(maxPlayers, 0).ToString();

            byte[] hostNameLength = new byte[4];
            Array.Copy(Response, 16, hostNameLength, 0, 4);
            int hostNameLengthInt = System.BitConverter.ToInt32(hostNameLength, 0);

            byte[] hostName = new byte[hostNameLengthInt];
            Array.Copy(Response, 20, hostName, 0, hostNameLengthInt);
            _params["hostname"] = Encoding.Default.GetString(hostName);
            _params["mapname"] = "San Andreas";
        }
    }
}
