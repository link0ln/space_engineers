        IMyShipDrill drill1;
        IMyShipDrill drill2;
        IMyShipDrill drill3;
        IMyShipDrill drill4;
        IMyShipDrill drill5;
        IMyShipDrill drill6;
        IMyGyro gyro1;
        IMyTextPanel textPanel;
        IMyProgrammableBlock sam;
        IMyProgrammableBlock pbmy;
        string drillname1 = "drill1";
        string drillname2 = "drill2";
        string drillname3 = "drill3";
        string drillname4 = "drill4";
        string drillname5 = "drill5";
        string drillname6 = "drill6";
        string gyroname1 = "gyro1";
        string displayname = "drilldisplay";
        string pbsam = "pb [SAM]";
        string pbself = "pb my";
        int counter = 0;
        float areasize = 75;
        int _timer1 = 0;
        int drill = 0;
        int xzdim = 75;
        int drillworkdelay = 10;

        int d2x_offset;
        int d2y_offset;
        int d2z_offset;
        int d3x_offset;
        int d3y_offset;
        int d3z_offset;
        int d4x_offset;
        int d4y_offset;
        int d4z_offset;
        int d5x_offset;
        int d5y_offset;
        int d5z_offset;
        int d6x_offset;
        int d6y_offset;
        int d6z_offset;



        public string direction(string axis)
        {
            if (axis == "x") return "LeftRight";
            if (axis == "y") return "UpDown";
            if (axis == "z") return "FrontBack";
            return "";
        }

        public void setpos(float v, string axis, IMyShipDrill drill)
        {
            try
            {
                float k = drill.GetValueFloat("Drill.AreaOffset" + direction(axis));
                if (k < v)
                {
                    while (k < v & k < 250)
                    {
                        drill.ApplyAction("AreaOffset" + direction(axis) + "_Increase");
                        k = drill.GetValueFloat("Drill.AreaOffset" + direction(axis));
                    }
                }
                else if (k > v)
                {
                    while (k > v & k > -1 * 250)
                    {
                        drill.ApplyAction("AreaOffset" + direction(axis) + "_Decrease");
                        k = drill.GetValueFloat("Drill.AreaOffset" + direction(axis));
                    }
                }
            }
            catch { }
        }

        public void setoffset(float v, string axis)
        {
            //wdisp(v.ToString() + " " + axis + " ", true);
            setpos(v, axis, drill1);   // -76 -9 15   9.5 81.5 0.5

            if (axis == "x")
            {
                v = v - 3;
            }
            if (axis == "y")
            {
                v = v + 10;
            }
			
            setpos(v, axis, drill2);
			setpos(v, axis, drill3);
			setpos(v, axis, drill4);
			setpos(v, axis, drill5);
			setpos(v, axis, drill6);
			
            //if (axis == "y")
            //{
            //    axis = "z";
            //    v = v - v - v;
            //}
            //else if (axis == "z")
            //{
            //    axis = "x";
            //    //v = v - v - v;
            //}
            //else if (axis == "x")
            //{
            //    axis = "y";
            //    v = v - v - v;
            //}
            ////wdisp(v.ToString() + " " + axis + "\n", true);
			//
            //if (axis == "x")
            //{
            //    v = v - 3;
            //}
            //if (axis == "y")
            //{
            //    v = v + 3;
            //}
            //setpos(v, axis, drill3);
			//
            //if (axis == "y")
            //{
            //    v = v - 2;
            //}
            //setpos(v, axis, drill4);
        }

        public bool drillarea()
        {
            drill1.ApplyAction("OnOff_On");
            drill2.ApplyAction("OnOff_On");
            string result = "Nothing to drill...";
            try
            {
                object tgt = drill1.GetValue<object>("Drill.CurrentDrillTarget");
                result = tgt.ToString();
                result = "Drilling...";
                wdisp(result);
                return true;
            }
            catch
            {
            }
            wdisp(result);
            return false;
        }

        public void wdisp(string msg, bool noclear = false)
        {
            textPanel = GridTerminalSystem.GetBlockWithName(displayname) as IMyTextPanel;
            textPanel.ContentType = ContentType.TEXT_AND_IMAGE;
            textPanel.FontSize = 3;
            textPanel.Alignment = VRage.Game.GUI.TextPanel.TextAlignment.CENTER;
            textPanel.WriteText(msg, noclear);
        }

        public Program()
        {
            drill1 = GridTerminalSystem.GetBlockWithName(drillname1) as IMyShipDrill;
            drill2 = GridTerminalSystem.GetBlockWithName(drillname2) as IMyShipDrill;
            drill3 = GridTerminalSystem.GetBlockWithName(drillname3) as IMyShipDrill;
            drill4 = GridTerminalSystem.GetBlockWithName(drillname4) as IMyShipDrill;
            drill5 = GridTerminalSystem.GetBlockWithName(drillname5) as IMyShipDrill;
            drill6 = GridTerminalSystem.GetBlockWithName(drillname6) as IMyShipDrill;
            gyro1 = GridTerminalSystem.GetBlockWithName(gyroname1) as IMyGyro;
            sam = GridTerminalSystem.GetBlockWithName(pbsam) as IMyProgrammableBlock;
            pbmy = GridTerminalSystem.GetBlockWithName(pbself) as IMyProgrammableBlock;
            Runtime.UpdateFrequency = UpdateFrequency.Update100;
            //throw new Exception("Block Actions: " + "");
            drill = Int32.Parse(drill1.CustomData);
            wdisp("");
            drillendposition();
            set_drills_offset();
        }

        public void set_drills_offset()
        {
            var drillpos1 = pbmy.CubeGrid.GridIntegerToWorld(drill1.Position);
            var drillpos2 = pbmy.CubeGrid.GridIntegerToWorld(drill2.Position);
            var drillpos3 = pbmy.CubeGrid.GridIntegerToWorld(drill3.Position);
            var drillpos4 = pbmy.CubeGrid.GridIntegerToWorld(drill4.Position);
            var drillpos5 = pbmy.CubeGrid.GridIntegerToWorld(drill5.Position);
            var drillpos6 = pbmy.CubeGrid.GridIntegerToWorld(drill6.Position);

            double d1x = Math.Floor(drillpos1.GetDim(0));
            double d1y = Math.Floor(drillpos1.GetDim(1));
            double d1z = Math.Floor(drillpos1.GetDim(2));
            d2x_offset = (int)(Math.Floor(drillpos2.GetDim(0)) - Math.Floor(drillpos1.GetDim(0)));
            d2y_offset = (int)(Math.Floor(drillpos2.GetDim(1)) - Math.Floor(drillpos1.GetDim(1)));
            d2z_offset = (int)(Math.Floor(drillpos2.GetDim(2)) - Math.Floor(drillpos1.GetDim(2)));
            d3x_offset = (int)(Math.Floor(drillpos3.GetDim(0)) - Math.Floor(drillpos1.GetDim(0)));
            d3y_offset = (int)(Math.Floor(drillpos3.GetDim(1)) - Math.Floor(drillpos1.GetDim(1)));
            d3z_offset = (int)(Math.Floor(drillpos3.GetDim(2)) - Math.Floor(drillpos1.GetDim(2)));
            d4x_offset = (int)(Math.Floor(drillpos4.GetDim(0)) - Math.Floor(drillpos1.GetDim(0)));
            d4y_offset = (int)(Math.Floor(drillpos4.GetDim(1)) - Math.Floor(drillpos1.GetDim(1)));
            d4z_offset = (int)(Math.Floor(drillpos4.GetDim(2)) - Math.Floor(drillpos1.GetDim(2)));
            d5x_offset = (int)(Math.Floor(drillpos5.GetDim(0)) - Math.Floor(drillpos1.GetDim(0)));
            d5y_offset = (int)(Math.Floor(drillpos5.GetDim(1)) - Math.Floor(drillpos1.GetDim(1)));
            d5z_offset = (int)(Math.Floor(drillpos5.GetDim(2)) - Math.Floor(drillpos1.GetDim(2)));
            d6x_offset = (int)(Math.Floor(drillpos6.GetDim(0)) - Math.Floor(drillpos1.GetDim(0)));
            d6y_offset = (int)(Math.Floor(drillpos6.GetDim(1)) - Math.Floor(drillpos1.GetDim(1)));
            d6z_offset = (int)(Math.Floor(drillpos6.GetDim(2)) - Math.Floor(drillpos1.GetDim(2)));
            
            Echo("d2 " + d2x_offset.ToString() + " " + d2y_offset.ToString() + " " + d2z_offset.ToString());
            //Echo("d3 " + (d3x - d1x).ToString() + " " + (d3y - d1y).ToString() + " " + (d3z - d1z).ToString());
            //Echo("d4 " + (d4x - d1x).ToString() + " " + (d4y - d1y).ToString() + " " + (d4z - d1z).ToString());
        }

        public void Save()
        {

        }

        public float getoffset(string axis)
        {
            float offset = drill1.GetValueFloat("Drill.AreaOffset" + direction(axis));
            return offset;
        }

        public bool timer1(int tseconds)
        {
            _timer1++;
            if (_timer1 < tseconds)
            {
                return false;
            }
            else
            {
                _timer1 = 0;
                return true;
            }
        }

        public bool samnottravel()
        {
            try
            {
                string pbtext = sam.DetailedInfo;
                wdisp("\nDebugsam " + pbtext, true);
                return System.Text.RegularExpressions.Regex.IsMatch(pbtext, ".+?disabled");
            }
            catch
            {
            }
            return false;
        }

        public void travelnext()
        {
            string gps_string = pbmy.CustomData;
            string[] gps_list = gps_string.Split('\n');
            string gpsarg = gps_list[0];
            wdisp("\ngps_list " + gps_list[0]);
            gps_list = gps_list.Skip(1).ToArray();
            pbmy.CustomData = String.Join("\n", gps_list);
            sam.TryRun(gpsarg);
            wdisp("\nTravelnext " + gpsarg, true);
        }

        public void addgps()
        {
            var pos = pbmy.CubeGrid.GridIntegerToWorld(pbmy.Position);

            double x = Math.Floor(pos.GetDim(0));
            double y = Math.Floor(pos.GetDim(1));
            double z = Math.Floor(pos.GetDim(2));
            string gpscord = "START GPS:ore:" + x.ToString() + ":" + y.ToString() + ":" + z.ToString() + ":" + "#FF75C9F1";
            pbmy.CustomData = pbmy.CustomData + gpscord + "\n";
        }



        public void drillstartposition()
        {
            setoffset((xzdim * -1) - 1, "x");
            setoffset(34, "y");
            setoffset(xzdim * -1, "z");
        }
        public void drillendposition()
        {
            setoffset(xzdim, "x");
            setoffset(250, "y");
            setoffset(xzdim, "z");
        }

        public void Main(string argument, UpdateType updateSource)
        {
            switch (argument)
            {
                case "addgps":
                    addgps();
                    break;
                case "drill":
                    gyro1.SetValue("Override", true);
                    drill1.CustomData = "1";
                    drill = 1;
                    break;
                case "nodrill":
                    gyro1.SetValue("Override", false);
                    drill1.CustomData = "0";
                    drill = 0;
                    break;
                case "drillstartposition":
                    drillstartposition();
                    break;
                case "setdrillsoffset":
                    set_drills_offset();
                    break;
                case "drillendposition":
                    drillendposition();
                    break;
                default:
                    break;
            }

            if (drill == 0)
            {
                Echo("Drilling off");
                return;
            }
            else
            {
                Echo("Drilling on");
            }

            counter++;
            wdisp("\n" + counter.ToString());
            if (samnottravel() == true)
            {
                wdisp("\nIdle...", true);
                gyro1.SetValue("Override", true);
            }
            else
            {
                wdisp("\nTraveling...", true);
                return;
            }

            if (getoffset("x") >= xzdim & getoffset("y") == 250 & getoffset("z") >= xzdim)
            {
                wdisp("\nTravel next...", true);
                travelnext();
                drillstartposition();
            }

            if (drillarea() == true)
            {
            }
            else
            {
                if (timer1(drillworkdelay) == true)
                {
                    setoffset(getoffset("x") + areasize, "x");
                }
                else
                if (getoffset("x") >= xzdim & getoffset("y") < 250)
                {
                    setoffset(-1 * xzdim - 1, "x");
                    setoffset(getoffset("y") + areasize, "y");
                }
                else
                if (getoffset("x") >= xzdim & getoffset("y") == 250 & getoffset("z") < xzdim)
                {
                    setoffset(-1 * xzdim - 1, "x");
                    setoffset(34, "y");
                    setoffset(getoffset("z") + areasize, "z");
                }
            }
        }
