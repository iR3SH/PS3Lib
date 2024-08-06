// ************************************************* //
//    --- Copyright (c) 2015 iMCS Productions ---    //
// ************************************************* //
//              PS3Lib v4.5 By FM|T iMCSx            //
//              Edited by iR3SH                      //
//                                                   //
// Features v4.6 :                                   //
// - Support PS3M_API (Hen)                          //
//                                                   //
//                                                   //
//                                                   //
//                                                   //
//                                                   //
// Credits : Enstone, Buc-ShoTz, iMCSx, NvZ          //
//                                                   //
// ************************************************* //

using PS3Lib.Extra;
using PS3ManagerAPI;
using System.Text;

namespace PS3Lib
{
    public class HENAPI
    {
        public enum IdType
        {
            IDPS,
            PSID
        }

        public enum ConsoleType
        {
            CEX = 1,
            DEX,
            TOOL
        }

        public enum ProcessType
        {
            VSH,
            SYS_AGENT,
            CURRENTGAME
        }

        public enum RebootFlags
        {
            ShutDown = 1,
            SoftReboot,
            HardReboot
        }

        public enum LedColor
        {
            Green = 1,
            Red
        }

        public enum LedMode
        {
            Off,
            On,
            Blink
        }

        private class System
        {
            public static int connectionID = -1;

            public static uint processID = 0u;

            public static uint[] processIDs;
        }

        public class TargetInfo
        {
            public int Firmware;

            public int CCAPI;

            public int ConsoleType;

            public int TempCell;

            public int TempRSX;

            public ulong SysTable;
        }

        public class ConsoleInfo
        {
            public string Name;

            public string Ip;
        }
        private static PS3MAPI PS3M_API = new();

        private TargetInfo pInfo = new();

        public Extension Extension => new (SelectAPI.Hen);

        public HENAPI()
        {
        }

        private void CompleteInfo(ref TargetInfo Info, int fw, int ccapi, ulong sysTable, int consoleType, int tempCELL, int tempRSX)
        {
            Info.Firmware = fw;
            Info.CCAPI = ccapi;
            Info.SysTable = sysTable;
            Info.ConsoleType = consoleType;
            Info.TempCell = tempCELL;
            Info.TempRSX = tempRSX;
        }

        public bool SUCCESS(int Void)
        {
            if (Void == 0)
            {
                return true;
            }
            return false;
        }

        public bool ConnectTarget()
        {
            return PS3M_API.ConnectTarget();
        }

        public int ConnectTarget(string targetIP)
        {
            if (PS3M_API.ConnectTarget(targetIP))
            {
                return 0;
            }
            return -1;
        }

        public int GetConnectionStatus()
        {
            if (PS3M_API.IsConnected)
            {
                return 0;
            }
            return -1;
        }

        public int DisconnectTarget()
        {
            PS3M_API.DisconnectTarget();
            return 0;
        }

        public int AttachProcess()
        {
            if (PS3M_API.AttachProcess())
            {
                System.processIDs = PS3M_API.Process.Processes_Pid;
                System.processID = PS3M_API.Process.Process_Pid;
                return 0;
            }
            return -1;
        }

        public int AttachProcess(ProcessType procType)
        {
            if (PS3M_API.AttachProcess())
            {
                System.processIDs = PS3M_API.Process.Processes_Pid;
                System.processID = PS3M_API.Process.Process_Pid;
                return 0;
            }
            return -1;
        }

        public int AttachProcess(uint process)
        {
            if (PS3M_API.AttachProcess(process))
            {
                System.processID = PS3M_API.Process.Process_Pid;
                return 0;
            }
            return -1;
        }

        public int GetProcessList(out uint[] processIds)
        {
            processIds = new uint[16];
            try
            {
                processIds = PS3M_API.Process.GetPidProcesses();
                return 0;
            }
            catch
            {
                return -1;
            }
        }

        public int GetProcessName(uint processId, out string name)
        {
            name = "";
            try
            {
                name = PS3M_API.Process.GetName(processId);
                return 0;
            }
            catch
            {
                return -1;
            }
        }

        public uint GetAttachedProcess()
        {
            return System.processID;
        }

        public int SetMemory(uint offset, byte[] buffer)
        {
            try
            {
                PS3M_API.Process.Memory.Set(GetAttachedProcess(), offset, buffer);
                return 0;
            }
            catch
            {
                return -1;
            }
        }

        public int SetMemory(ulong offset, byte[] buffer)
        {
            try
            {
                PS3M_API.Process.Memory.Set(GetAttachedProcess(), offset, buffer);
                return 0;
            }
            catch
            {
                return -1;
            }
        }

        public int SetMemory(ulong offset, string hexadecimal, EndianType Type = EndianType.BigEndian)
        {
            byte[] array = StringToByteArray(hexadecimal);
            if (Type == EndianType.LittleEndian)
            {
                Array.Reverse(array);
            }
            try
            {
                PS3M_API.Process.Memory.Set(GetAttachedProcess(), offset, array);
                return 0;
            }
            catch
            {
                return -1;
            }
        }

        public int GetMemory(uint offset, byte[] buffer)
        {
            try
            {
                PS3M_API.Process.Memory.Get(GetAttachedProcess(), offset, buffer);
                return 0;
            }
            catch
            {
                return -1;
            }
        }

        public int GetMemory(ulong offset, byte[] buffer)
        {
            try
            {
                PS3M_API.Process.Memory.Get(GetAttachedProcess(), offset, buffer);
                return 0;
            }
            catch
            {
                return -1;
            }
        }

        public byte[] GetBytes(uint offset, uint length)
        {
            byte[] array = new byte[length];
            PS3M_API.Process.Memory.Get(GetAttachedProcess(), offset, array);
            return array;
        }

        public byte[] GetBytes(ulong offset, uint length)
        {
            byte[] array = new byte[length];
            PS3M_API.Process.Memory.Get(GetAttachedProcess(), offset, array);
            return array;
        }

        public int Notify(NotifyIcon icon, string message)
        {
            try
            {
                PS3M_API.PS3.Notify(message);
                return 0;
            }
            catch
            {
                return -1;
            }
        }

        public int Notify(int icon, string message)
        {
            try
            {
                PS3M_API.PS3.Notify(message);
                return 0;
            }
            catch
            {
                return -1;
            }
        }

        public int ShutDown(RebootFlags flag)
        {
            switch (flag)
            {
                case RebootFlags.ShutDown:
                    try
                    {
                        PS3M_API.PS3.Power(PS3MAPI.PS3_CMD.PowerFlags.ShutDown);
                        return 0;
                    }
                    catch
                    {
                        return -1;
                    }
                case RebootFlags.SoftReboot:
                    try
                    {
                        PS3M_API.PS3.Power(PS3MAPI.PS3_CMD.PowerFlags.SoftReboot);
                        return 0;
                    }
                    catch
                    {
                        return -1;
                    }
                case RebootFlags.HardReboot:
                    try
                    {
                        PS3M_API.PS3.Power(PS3MAPI.PS3_CMD.PowerFlags.HardReboot);
                        return 0;
                    }
                    catch
                    {
                        return -1;
                    }
                default:
                    return -1;
            }
        }

        public int RingBuzzer(BuzzerMode flag)
        {
            switch (flag)
            {
                case BuzzerMode.Single:
                    try
                    {
                        PS3M_API.PS3.RingBuzzer(PS3MAPI.PS3_CMD.BuzzerMode.Single);
                        return 0;
                    }
                    catch
                    {
                        return -1;
                    }
                case BuzzerMode.Double:
                    try
                    {
                        PS3M_API.PS3.RingBuzzer(PS3MAPI.PS3_CMD.BuzzerMode.Double);
                        return 0;
                    }
                    catch
                    {
                        return -1;
                    }
                case BuzzerMode.Continuous:
                    try
                    {
                        PS3M_API.PS3.RingBuzzer(PS3MAPI.PS3_CMD.BuzzerMode.Triple);
                        return 0;
                    }
                    catch
                    {
                        return -1;
                    }
                default:
                    return -1;
            }
        }

        public int SetConsoleLed(LedColor color, LedMode mode)
        {
            try
            {
                if (color == LedColor.Red && mode == LedMode.Off)
                {
                    PS3M_API.PS3.Led(PS3MAPI.PS3_CMD.LedColor.Red, PS3MAPI.PS3_CMD.LedMode.Off);
                }
                else if (color == LedColor.Red && mode == LedMode.On)
                {
                    PS3M_API.PS3.Led(PS3MAPI.PS3_CMD.LedColor.Red, PS3MAPI.PS3_CMD.LedMode.On);
                }
                else if (color == LedColor.Red && mode == LedMode.Blink)
                {
                    PS3M_API.PS3.Led(PS3MAPI.PS3_CMD.LedColor.Red, PS3MAPI.PS3_CMD.LedMode.BlinkFast);
                }
                else if (color == LedColor.Green && mode == LedMode.Off)
                {
                    PS3M_API.PS3.Led(PS3MAPI.PS3_CMD.LedColor.Green, PS3MAPI.PS3_CMD.LedMode.Off);
                }
                else if (color == LedColor.Green && mode == LedMode.On)
                {
                    PS3M_API.PS3.Led(PS3MAPI.PS3_CMD.LedColor.Green, PS3MAPI.PS3_CMD.LedMode.On);
                }
                else if (color == LedColor.Green && mode == LedMode.Blink)
                {
                    PS3M_API.PS3.Led(PS3MAPI.PS3_CMD.LedColor.Green, PS3MAPI.PS3_CMD.LedMode.BlinkFast);
                }
                return 1;
            }
            catch
            {
                return -1;
            }
        }

        private int GetTargetInfo()
        {
            try
            {
                uint[] array = new uint[2];
                int num = 0;
                int num2 = 0;
                int consoleType = 0;
                ulong sysTable = 0uL;
                num = Convert.ToInt32(PS3M_API.PS3.GetFirmwareVersion());
                num2 = Convert.ToInt32(PS3M_API.Core.GetVersion());
                string firmwareType = PS3M_API.PS3.GetFirmwareType();
                if (firmwareType.Contains("CEX"))
                {
                    consoleType = 1;
                }
                else if (firmwareType.Contains("DEX"))
                {
                    consoleType = 2;
                }
                else if (firmwareType.Contains("TOOL"))
                {
                    consoleType = 3;
                }
                PS3M_API.PS3.GetTemperature(out array[0], out array[1]);
                CompleteInfo(ref pInfo, num, num2, sysTable, consoleType, Convert.ToInt32(array[0]), Convert.ToInt32(array[1]));
                return 0;
            }
            catch
            {
                return -1;
            }
        }

        public int GetTargetInfo(out TargetInfo Info)
        {
            Info = new TargetInfo();
            try
            {
                uint[] array = new uint[2];
                int num = 0;
                int num2 = 0;
                int consoleType = 0;
                ulong sysTable = 0uL;
                num = Convert.ToInt32(PS3M_API.PS3.GetFirmwareVersion());
                num2 = Convert.ToInt32(PS3M_API.Core.GetVersion());
                string firmwareType = PS3M_API.PS3.GetFirmwareType();
                if (firmwareType.Contains("CEX"))
                {
                    consoleType = 1;
                }
                else if (firmwareType.Contains("DEX"))
                {
                    consoleType = 2;
                }
                else if (firmwareType.Contains("TOOL"))
                {
                    consoleType = 3;
                }
                PS3M_API.PS3.GetTemperature(out array[0], out array[1]);
                CompleteInfo(ref Info, num, num2, sysTable, consoleType, Convert.ToInt32(array[0]), Convert.ToInt32(array[1]));
                CompleteInfo(ref pInfo, num, num2, sysTable, consoleType, Convert.ToInt32(array[0]), Convert.ToInt32(array[1]));
                return 0;
            }
            catch
            {
                return -1;
            }
        }

        public string GetFirmwareVersion()
        {
            return PS3M_API.PS3.GetFirmwareVersion_Str();
        }

        public string GetTemperatureCELL()
        {
            if (pInfo.TempCell == 0)
            {
                GetTargetInfo(out pInfo);
            }
            return pInfo.TempCell + " C";
        }

        public string GetTemperatureRSX()
        {
            if (pInfo.TempRSX == 0)
            {
                GetTargetInfo(out pInfo);
            }
            return pInfo.TempRSX + " C";
        }

        public string GetFirmwareType()
        {
            return PS3M_API.PS3.GetFirmwareType();
        }

        public void ClearTargetInfo()
        {
            pInfo = new TargetInfo();
        }

        public int SetConsoleID(string consoleID)
        {
            if (consoleID.Length < 32)
            {
                MessageBox.Show("Invalid ConsoleID", "Error PS3M_API", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return -1;
            }
            try
            {
                string iDPS = "";
                if (consoleID.Length > 32)
                {
                    iDPS = consoleID.Substring(0, 32);
                }
                PS3M_API.PS3.SetIDPS(iDPS);
                return 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error PS3M_API", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return -1;
            }
        }

        public int SetConsoleID(byte[] consoleID)
        {
            string text = ByteArrayToString(consoleID);
            if (text.Length < 32)
            {
                MessageBox.Show("Invalid ConsoleID", "Error PS3M_API", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return -1;
            }
            try
            {
                if (text.Length > 32)
                {
                    text = text.Substring(0, 32);
                }
                PS3M_API.PS3.SetIDPS(text);
                return 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error PS3M_API", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return -1;
            }
        }

        public int SetPSID(string PSID)
        {
            if (PSID.Length < 32)
            {
                MessageBox.Show("Invalid ConsoleID", "Error PS3M_API", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return -1;
            }
            try
            {
                string pSID = "";
                if (PSID.Length > 32)
                {
                    pSID = PSID.Substring(0, 32);
                }
                PS3M_API.PS3.SetPSID(pSID);
                return 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error PS3M_API", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return -1;
            }
        }

        public int SetPSID(byte[] PSID)
        {
            string text = ByteArrayToString(PSID);
            if (text.Length < 32)
            {
                MessageBox.Show("Invalid ConsoleID", "Error PS3M_API", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return -1;
            }
            try
            {
                if (text.Length > 32)
                {
                    text = text.Substring(0, 32);
                }
                PS3M_API.PS3.SetPSID(text);
                return 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error PS3M_API", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return -1;
            }
        }

        public int SetBootConsoleID(string consoleID, IdType Type = IdType.IDPS)
        {
            MessageBox.Show("SetBootConsoleID: Unsuported By PS3M_API", "Error.", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            return 0;
        }

        public int SetBootConsoleID(byte[] consoleID, IdType Type = IdType.IDPS)
        {
            MessageBox.Show("SetBootConsoleID: Unsuported By PS3M_API", "Error.", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            return 0;
        }

        public int ResetBootConsoleID(IdType Type = IdType.IDPS)
        {
            MessageBox.Show("ResetBootConsoleID: Unsuported By PS3M_API", "Error.", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            return 0;
        }

        public int GetDllVersion()
        {
            return 260;
        }

        public List<ConsoleInfo> GetConsoleList()
        {
            List<ConsoleInfo> list = new List<ConsoleInfo>();
            ConsoleInfo consoleInfo = new ConsoleInfo();
            consoleInfo.Ip = "127.0.0.1";
            consoleInfo.Name = "PS3MAPI";
            list.Add(consoleInfo);
            return list;
        }

        internal static string ByteArrayToString(byte[] bytes)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder(bytes.Length * 2);
                foreach (byte b in bytes)
                {
                    stringBuilder.AppendFormat("{0:x2}", b);
                }
                return stringBuilder.ToString();
            }
            catch
            {
                throw new ArgumentException("Value not possible.", "HEX String");
            }
        }

        internal static byte[] StringToByteArray(string hex)
        {
            string replace = hex.Replace("0x", "");
            string Stringz = replace.Insert(replace.Length - 1, "0");
            int length = replace.Length;
            bool flag = length % 2 == 0;
            try
            {
                if (flag)
                {
                    return (from x in Enumerable.Range(0, replace.Length)
                            where x % 2 == 0
                            select Convert.ToByte(replace.Substring(x, 2), 16)).ToArray();
                }
                return (from x in Enumerable.Range(0, replace.Length)
                        where x % 2 == 0
                        select Convert.ToByte(Stringz.Substring(x, 2), 16)).ToArray();
            }
            catch
            {
                throw new ArgumentException("Value not possible.", "Byte Array");
            }
        }
    }
}
