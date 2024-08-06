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
using PS3Lib.Properties;
using System.Globalization;
using System.Reflection;

namespace PS3Lib
{
    public enum Lang
    {
        Null,
        French,
        English,
        German
    }
    public enum BuzzerMode
    {
        Continuous,
        Single,
        Double
    }
    public enum SelectAPI
    {
        ControlConsole,
        TargetManager,
        Hen
    }
    public enum NotifyIcon
    {
        INFO,
        CAUTION,
        FRIEND,
        SLIDER,
        WRONGWAY,
        DIALOG,
        DIALOGSHADOW,
        TEXT,
        POINTER,
        GRAB,
        HAND,
        PEN,
        FINGER,
        ARROW,
        ARROWRIGHT,
        PROGRESS,
        TROPHY1,
        TROPHY2,
        TROPHY3,
        TROPHY4
    }

    public class PS3API
    {
        private static string targetName = string.Empty;
        private static string targetIp = string.Empty;
        public PS3API(SelectAPI API = SelectAPI.TargetManager)
        {
            SetAPI.API = API;
            MakeInstanceAPI(API);
        }

        public void setTargetName(string value)
        {
            targetName = value;
        }

        private void MakeInstanceAPI(SelectAPI API)
        {
            if (API == SelectAPI.TargetManager)
                if (Common.TmApi == null)
                    Common.TmApi = new TMAPI();
            if (API == SelectAPI.ControlConsole)
                if (Common.CcApi == null)
                    Common.CcApi = new CCAPI();
            if (API == SelectAPI.Hen)
                if (Common.HenApi == null)
                    Common.HenApi = new HENAPI();
        }

        private class SetLang
        {
            public static Lang defaultLang = Lang.Null;
        }

        private class SetAPI
        {
            public static SelectAPI API;
        }

        private class Common
        {
            public static CCAPI CcApi;
            public static TMAPI TmApi;
            public static HENAPI HenApi;
        }

        /// <summary>Force a language for the console list popup.</summary>
        public void SetFormLang(Lang Language)
        {
            SetLang.defaultLang = Language;
        }

       /// <summary>init again the connection if you use a Thread or a Timer.</summary>
        public void InitTarget()
        {
            if (SetAPI.API == SelectAPI.TargetManager)
                Common.TmApi.InitComms();
        }

        /// <summary>Connect your console with selected API.</summary>
        public bool ConnectTarget(int target = 0)
        {
            // We'll check again if the instance has been done, else you'll got an exception error.
            MakeInstanceAPI(GetCurrentAPI());

            bool result = false;
            if (SetAPI.API == SelectAPI.TargetManager)
                result = Common.TmApi.ConnectTarget(target);
            else if (SetAPI.API == SelectAPI.ControlConsole)
                result = new ConsoleList(this).Show();
            else if (SetAPI.API == SelectAPI.Hen)
                result = Common.HenApi.ConnectTarget();
            return result;
        }

        /// <summary>Connect your console with CCAPI.</summary>
        public bool ConnectTarget(string ip)
        {
            // We'll check again if the instance has been done.
            MakeInstanceAPI(GetCurrentAPI());
            if (GetCurrentAPI() == SelectAPI.ControlConsole)
            {
                if (Common.CcApi.SUCCESS(Common.CcApi.ConnectTarget(ip)))
                {
                    targetIp = ip;
                    return true;
                }
                else return false;
            }
            else if(GetCurrentAPI() == SelectAPI.Hen)
            {
                if (Common.HenApi.SUCCESS(Common.HenApi.ConnectTarget(ip)))
                {
                    targetIp = ip;
                    return true;
                }
                else return false;
            }
            return false;
        }

        /// <summary>Disconnect Target with selected API.</summary>
        public void DisconnectTarget()
        {
            if (SetAPI.API == SelectAPI.TargetManager)
                Common.TmApi.DisconnectTarget();
            else if (SetAPI.API == SelectAPI.ControlConsole)
                Common.CcApi.DisconnectTarget();
            else if (SetAPI.API == SelectAPI.Hen)
                Common.HenApi.DisconnectTarget();
        }

        /// <summary>Attach the current process (current Game) with selected API.</summary>
        public bool AttachProcess()
        {
            // We'll check again if the instance has been done.
            MakeInstanceAPI(GetCurrentAPI());

            bool AttachResult = false;
            if (SetAPI.API == SelectAPI.TargetManager)
                AttachResult = Common.TmApi.AttachProcess();
            else if (SetAPI.API == SelectAPI.ControlConsole)
                AttachResult = Common.CcApi.SUCCESS(Common.CcApi.AttachProcess());
            else if (SetAPI.API == SelectAPI.Hen)
                AttachResult = Common.HenApi.SUCCESS(Common.HenApi.AttachProcess());
            return AttachResult;
        }

        public string GetConsoleName()
        {
            if (SetAPI.API == SelectAPI.TargetManager)
                return Common.TmApi.SCE.GetTargetName();
            else if(SetAPI.API == SelectAPI.ControlConsole)
            {
                if (targetName != string.Empty)
                    return targetName;

                if (targetIp != string.Empty)
                {
                    List<CCAPI.ConsoleInfo> Data = [];
                    Data = Common.CcApi.GetConsoleList();
                    if (Data.Count > 0)
                    {
                        for (int i = 0; i < Data.Count; i++)
                            if (Data[i].Ip == targetIp)
                                return Data[i].Name;
                    }
                }
                return targetIp;
            }
            else if(SetAPI.API == SelectAPI.Hen)
            {
                if (targetName != string.Empty)
                    return targetName;

                if (targetIp != string.Empty)
                {
                    List<HENAPI.ConsoleInfo> Data = [];
                    Data = Common.HenApi.GetConsoleList();
                    if (Data.Count > 0)
                    {
                        for (int i = 0; i < Data.Count; i++)
                            if (Data[i].Ip == targetIp)
                                return Data[i].Name;
                    }
                }
                return targetIp;
            }
            return "";
        }

        /// <summary>Set memory to offset with selected API.</summary>
        public void SetMemory(uint offset, byte[] buffer)
        {
            if (SetAPI.API == SelectAPI.TargetManager)
                Common.TmApi.SetMemory(offset, buffer);
            else if (SetAPI.API == SelectAPI.ControlConsole)
                Common.CcApi.SetMemory(offset, buffer);
            else if (SetAPI.API == SelectAPI.Hen)
                Common.HenApi.SetMemory(offset, buffer);
        }

        /// <summary>Get memory from offset using the Selected API.</summary>
        public void GetMemory(uint offset, byte[] buffer)
        {
            if (SetAPI.API == SelectAPI.TargetManager)
                Common.TmApi.GetMemory(offset, buffer);
            else if (SetAPI.API == SelectAPI.ControlConsole)
                Common.CcApi.GetMemory(offset, buffer);
            else if (SetAPI.API == SelectAPI.Hen)
                Common.HenApi.GetMemory(offset, buffer);
        }

        /// <summary>Get memory from offset with a length using the Selected API.</summary>
        public byte[] GetBytes(uint offset, uint length)
        {
            byte[] buffer = new byte[length];
            if (SetAPI.API == SelectAPI.TargetManager)
                buffer = Common.TmApi.GetBytes(offset, length);
            else if (SetAPI.API == SelectAPI.ControlConsole)
                buffer = Common.CcApi.GetBytes(offset, length);
            else if (SetAPI.API == SelectAPI.Hen)
                buffer = Common.HenApi.GetBytes(offset, length);
            return buffer;
        }

        /// <summary>Change current API.</summary>
        public void ChangeAPI(SelectAPI API)
        {
            SetAPI.API = API;
            MakeInstanceAPI(GetCurrentAPI());
        }

        /// <summary>Return selected API.</summary>
        public SelectAPI GetCurrentAPI()
        {
            return SetAPI.API;
        }
        public int GetTemperatureCell()
        {
            int temp = 0;
            if(SetAPI.API == SelectAPI.TargetManager)
            {
                temp = 0;
            }
            else if(SetAPI.API == SelectAPI.ControlConsole)
            {
                temp = Convert.ToInt32(Common.CcApi.GetTemperatureCELL().Replace(" C", ""));
            }
            else if(SetAPI.API == SelectAPI.Hen)
            {
                temp = Convert.ToInt32(Common.HenApi.GetTemperatureCELL().Replace(" C", ""));
            }
            return temp;
        }
        public int GetTemperatureRSX()
        {
            int temp = 0;
            if (SetAPI.API == SelectAPI.TargetManager)
            {
                temp = 0;
            }
            else if (SetAPI.API == SelectAPI.ControlConsole)
            {
                temp = Convert.ToInt32(Common.CcApi.GetTemperatureRSX().Replace(" C", ""));
            }
            else if (SetAPI.API == SelectAPI.Hen)
            {
                temp = Convert.ToInt32(Common.HenApi.GetTemperatureRSX().Replace(" C", ""));
            }
            return temp;
        }

        public void Notify(NotifyIcon icon, string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                if(SetAPI.API == SelectAPI.ControlConsole)
                {
                    Common.CcApi.Notify(icon, message);
                }
                else if(SetAPI.API == SelectAPI.Hen)
                {
                    Common.HenApi.Notify(icon, message);
                }
            }
        }
        public void RingBuzzer(BuzzerMode mode)
        {
            if (SetAPI.API == SelectAPI.ControlConsole)
            {
                Common.CcApi.RingBuzzer(mode);
            }
            else if (SetAPI.API == SelectAPI.Hen)
            {
                Common.HenApi.RingBuzzer(mode);
            }
        }
        /// <summary>Return selected API into string format.</summary>
        public string GetCurrentAPIName()
        {
            string output = String.Empty;
            if (SetAPI.API == SelectAPI.TargetManager)
                output = Enum.GetName(typeof(SelectAPI), SelectAPI.TargetManager).Replace("Manager"," Manager");
            else if(SetAPI.API == SelectAPI.ControlConsole) 
                output = Enum.GetName(typeof(SelectAPI), SelectAPI.ControlConsole).Replace("Console", " Console");
            else if(SetAPI.API == SelectAPI.Hen)
                output = Enum.GetName(typeof(SelectAPI), SelectAPI.Hen).Replace("Hen", " Hen");
            return output;
        }
        public void SetConsoleId(string id)
        {
            if (SetAPI.API == SelectAPI.ControlConsole)
            {
                Common.CcApi.SetConsoleID(id);
            }
            else if (SetAPI.API == SelectAPI.Hen)
            {
                Common.HenApi.SetConsoleID(id);
            }
        }

        /// <summary>This will find the dll ps3tmapi_net.dll for TMAPI.</summary>
        public Assembly PS3TMAPI_NET()
        {
            return Common.TmApi.PS3TMAPI_NET();
        }

        /// <summary>Use the extension class with your selected API.</summary>
        public Extension Extension
        {
            get { return new Extension(SetAPI.API); }
        }

        /// <summary>Access to all TMAPI functions.</summary>
        public TMAPI TMAPI
        {
            get { return new TMAPI(); }
        }

        /// <summary>Access to all CCAPI functions.</summary>
        public CCAPI CCAPI
        {
            get { return new CCAPI(); }
        }

        public HENAPI HENAPI
        {
            get { return new HENAPI(); }
        }
        public class ConsoleList
        {
            private PS3API Api;
            private List<CCAPI.ConsoleInfo> data;
            private List<HENAPI.ConsoleInfo> datahen;

            public ConsoleList(PS3API f)
            {
                Api = f;
                if (f.GetCurrentAPI() == SelectAPI.ControlConsole)
                {
                    data = Api.CCAPI.GetConsoleList();
                }
                if(f.GetCurrentAPI() == SelectAPI.Hen)
                {
                    datahen = Api.HENAPI.GetConsoleList();
                }
            }

            /// <summary>Return the systeme language, if it's others all text will be in english.</summary>
            private Lang getSysLanguage()
            {
                if (SetLang.defaultLang == Lang.Null)
                {
                    if (CultureInfo.CurrentCulture.ThreeLetterWindowsLanguageName.StartsWith("FRA"))
                        return Lang.French;
                    else if (CultureInfo.CurrentCulture.ThreeLetterWindowsLanguageName.StartsWith("GER"))
                        return Lang.German;
                    return Lang.English;
                }
                else return SetLang.defaultLang;
            }

            private string strTraduction(string keyword)
            {
                Lang lang = getSysLanguage();
                if (lang == Lang.French)
                {
                    switch (keyword)
                    {
                        case "btnConnect": return "Connexion";
                        case "btnRefresh": return "Rafraîchir";
                        case "errorSelect": return "Vous devez d'abord sélectionner une console.";
                        case "errorSelectTitle": return "Sélectionnez une console.";
                        case "selectGrid": return "Sélectionnez une console dans la grille.";
                        case "selectedLbl": return "Sélection :";
                        case "formTitle": return "Choisissez une console...";
                        case "noConsole": return "Aucune console disponible, démarrez CCAPI Manager (v2.60+) et ajoutez une nouvelle console.";
                        case "noConsoleTitle": return "Aucune console disponible.";
                    }
                }
                else if(lang == Lang.German)
                {
                    switch (keyword)
                    {
                        case "btnConnect": return "Verbinde";
                        case "btnRefresh": return "Wiederholen";
                        case "errorSelect": return "Du musst zuerst eine Konsole auswählen.";
                        case "errorSelectTitle": return "Wähle eine Konsole.";
                        case "selectGrid": return "Wähle eine Konsole innerhalb dieses Gitters.";
                        case "selectedLbl": return "Ausgewählt :";
                        case "formTitle": return "Wähle eine Konsole...";
                        case "noConsole": return "Keine Konsolen verfügbar - starte CCAPI Manager (v2.60+) und füge eine neue Konsole hinzu.";
                        case "noConsoleTitle": return "Keine Konsolen verfügbar.";
                    }
                }
                else
                {
                    switch (keyword)
                    {
                        case "btnConnect": return "Connection";
                        case "btnRefresh": return "Refresh";
                        case "errorSelect": return "You need to select a console first.";
                        case "errorSelectTitle": return "Select a console.";
                        case "selectGrid": return "Select a console within this grid.";
                        case "selectedLbl": return "Selected :";
                        case "formTitle": return "Select a console...";
                        case "noConsole": return "None consoles available, run CCAPI Manager (v2.60+) and add a new console.";
                        case "noConsoleTitle": return "None console available.";
                    }
                }
                return "?";
            }

            public bool Show()
            {
                bool Result = false;
                int tNum = -1;

                // Instance of widgets
                Label lblInfo = new();
                Button btnConnect = new();
                Button btnRefresh = new();
                ListViewGroup listViewGroup = new("Consoles", HorizontalAlignment.Left);
                ListView listView = new();
                Form formList = new();

                // Create our button connect
                btnConnect.Location = new Point(12, 254);
                btnConnect.Name = "btnConnect";
                btnConnect.Size = new Size(198, 23);
                btnConnect.TabIndex = 1;
                btnConnect.Text = strTraduction("btnConnect");
                btnConnect.UseVisualStyleBackColor = true;
                btnConnect.Enabled = false;
                btnConnect.Click += (sender, e) =>
                {
                    if(tNum > -1)
                    {
                        Result = false;
                        if (data != null)
                        {
                            if (data.Count > 0)
                            {
                                if (Api.ConnectTarget(data[tNum].Ip))
                                {
                                    Api.setTargetName(data[tNum].Name);
                                    Result = true;
                                }
                            }
                        }
                        else if(datahen != null)
                        {
                            if(datahen.Count > 0) 
                            {
                                if (Api.ConnectTarget(datahen[tNum].Ip))
                                {
                                    Api.setTargetName(datahen[tNum].Name);
                                    Result = true;
                                }
                            }
                        }
                        formList.Close();
                    }
                    else
                        MessageBox.Show(strTraduction("errorSelect"), strTraduction("errorSelectTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                };

                // Create our button refresh
                btnRefresh.Location = new Point(216, 254);
                btnRefresh.Name = "btnRefresh";
                btnRefresh.Size = new Size(86, 23);
                btnRefresh.TabIndex = 1;
                btnRefresh.Text = strTraduction("btnRefresh");
                btnRefresh.UseVisualStyleBackColor = true;
                btnRefresh.Click += (sender, e) =>
                {
                    tNum = -1;
                    listView.Clear();
                    lblInfo.Text = strTraduction("selectGrid");
                    btnConnect.Enabled = false;
                    if (Api.CCAPI != null)
                    {
                        data = Api.CCAPI.GetConsoleList();
                    }
                    if (Api.HENAPI != null)
                    {
                        datahen = Api.HENAPI.GetConsoleList();
                    }
                    if (data != null)
                    {
                        if (data.Count > 0)
                        {
                            int sizeD = data.Count();
                            for (int i = 0; i < sizeD; i++)
                            {
                                ListViewItem item = new(" " + data[i].Name + " - " + data[i].Ip)
                                {
                                    ImageIndex = 0
                                };
                                listView.Items.Add(item);
                            }
                        }
                    }
                    if (datahen != null)
                    {
                        if (datahen.Count > 0)
                        {
                            int sizeE = datahen.Count();
                            for (int i = 0; i < sizeE; i++)
                            {
                                ListViewItem item = new(" " + datahen[i].Name + " - " + datahen[i].Ip)
                                {
                                    ImageIndex = 0
                                };
                                listView.Items.Add(item);
                            }
                        }
                    }
                };

                // Create our list view
                listView.Font = new Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                listViewGroup.Header = "Consoles";
                listViewGroup.Name = "consoleGroup";
                listView.Groups.AddRange([listViewGroup]);
                listView.HideSelection = false;
                listView.Location = new Point(12, 12);
                listView.MultiSelect = false;
                listView.Name = "ConsoleList";
                listView.ShowGroups = false;
                listView.Size = new Size(290, 215);
                listView.TabIndex = 0;
                listView.UseCompatibleStateImageBehavior = false;
                listView.View = View.List;
                listView.ItemSelectionChanged += (sender, e) =>
                {
                    tNum = e.ItemIndex;
                    btnConnect.Enabled = true;
                    string Name = "?";
                    string Ip = "?";
                    if (data != null)
                    {
                        if (data.Count - 1 >= tNum)
                        {
                            if (data[tNum].Name.Length > 18)
                                Name = data[tNum].Name.Substring(0, 17) + "...";
                            else Name = data[tNum].Name;
                            if (data[tNum].Ip.Length > 16)
                                Ip = data[tNum].Name.Substring(0, 16) + "...";
                            else Ip = data[tNum].Ip;
                        }
                    }
                    if (datahen != null)
                    {
                        if (datahen.Count - 1 >= tNum)
                        {
                            if (datahen[tNum].Name.Length > 18)
                                Name = datahen[tNum].Name.Substring(0, 17) + "...";
                            else Name = datahen[tNum].Name;
                            if (datahen[tNum].Ip.Length > 16)
                                Ip = datahen[tNum].Name.Substring(0, 16) + "...";
                            else Ip = datahen[tNum].Ip;
                        }
                    }
                    lblInfo.Text = strTraduction("selectedLbl") + " " + Name + " / " + Ip;
                };

                // Create our label
                lblInfo.AutoSize = true;
                lblInfo.Location = new Point(12, 234);
                lblInfo.Name = "lblInfo";
                lblInfo.Size = new Size(158, 13);
                lblInfo.TabIndex = 3;
                lblInfo.Text = strTraduction("selectGrid");

                // Create our form
                formList.MinimizeBox = false;
                formList.MaximizeBox = false;
                formList.ClientSize = new Size(314, 285);
                formList.AutoScaleDimensions = new SizeF(6F, 13F);
                formList.AutoScaleMode = AutoScaleMode.Font;
                formList.FormBorderStyle = FormBorderStyle.FixedSingle;
                formList.StartPosition = FormStartPosition.CenterScreen;
                formList.Text = strTraduction("formTitle");
                formList.Controls.Add(listView);
                formList.Controls.Add(lblInfo);
                formList.Controls.Add(btnConnect);
                formList.Controls.Add(btnRefresh);

                // Start to update our list
                ImageList imgL = new();
                imgL.Images.Add(Resources.ps3);
                listView.SmallImageList = imgL;
                if (data != null)
                {
                    int sizeData = data.Count();
                    for (int i = 0; i < sizeData; i++)
                    {
                        ListViewItem item = new(" " + data[i].Name + " - " + data[i].Ip)
                        {
                            ImageIndex = 0
                        };
                        listView.Items.Add(item);
                    }
                    // If there are more than 0 targets we show the form
                    // Else we inform the user to create a console.
                    if (sizeData > 0)
                        formList.ShowDialog();
                    else
                    {
                        Result = false;
                        formList.Close();
                        MessageBox.Show(strTraduction("noConsole"), strTraduction("noConsoleTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                if (datahen != null)
                {
                    int sizeDatahen = datahen.Count();
                    for (int i = 0; i < sizeDatahen; i++)
                    {
                        ListViewItem item = new(" " + datahen[i].Name + " - " + datahen[i].Ip)
                        {
                            ImageIndex = 0
                        };
                        listView.Items.Add(item);
                    }
                    // If there are more than 0 targets we show the form
                    // Else we inform the user to create a console.
                    if (sizeDatahen > 0)
                        formList.ShowDialog();
                    else
                    {
                        Result = false;
                        formList.Close();
                        MessageBox.Show(strTraduction("noConsole"), strTraduction("noConsoleTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                return Result;
            }
        }
    }
}
