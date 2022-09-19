        IMyBeacon beacon;
		IMyGyro gyro;
        IMyTextPanel textPanel;
        IMyProgrammableBlock sam;
        IMyProgrammableBlock pbmy;
		//List<IMyShipDrill> drills = new List<IMyShipDrill>();
		List<IMyTerminalBlock> drills = new List<IMyTerminalBlock>();
		string drills_name = "remotedrill";
		string beacon_name = "maindrill";
        string gyroname = "drillgyro";
        string displayname = "drilldisplay";
        string pbsam = "pb [SAM]";
        string pbself = "pb my";
        int counter = 0;
        float areasize = 75;
        int _timer1 = 0;
        int action_drill = 0;
        int xzdim = 75;
        int drillworkdelay = 10;
		int[,] areas = new int[27,3];
		int[] drillprevpos = new int[6];


        public string direction(string axis)
        {
            if (axis == "x") return "LeftRight";
            if (axis == "y") return "UpDown";
            if (axis == "z") return "FrontBack";
            return "";
        }

        public void setpos(float v, string axis, int drillnum)
        {
			IMyTerminalBlock drill = drills[drillnum];
			v = v - get_drill_offset(axis, drillnum)*2;
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

		public int get_drill_offset(string axis, int drill_num)
		{
			if (axis == "y"){
			  return (beacon.Position.X - drills[drill_num].Position.X);
			}
			if (axis == "x"){
			  return (beacon.Position.Y - drills[drill_num].Position.Y);
			}
			if (axis == "z"){
			  return (beacon.Position.Z - drills[drill_num].Position.Z);
			}
			return 0;
		}

        public void wdisp(string msg, bool noclear = false)
        {
            textPanel = GridTerminalSystem.GetBlockWithName(displayname) as IMyTextPanel;
            textPanel.ContentType = ContentType.TEXT_AND_IMAGE;
            textPanel.FontSize = 2;
            textPanel.Alignment = VRage.Game.GUI.TextPanel.TextAlignment.CENTER;
            textPanel.WriteText(msg, noclear);
        }

        public Program() {
            gyro = GridTerminalSystem.GetBlockWithName(gyroname) as IMyGyro;
            sam = GridTerminalSystem.GetBlockWithName(pbsam) as IMyProgrammableBlock;
            pbmy = GridTerminalSystem.GetBlockWithName(pbself) as IMyProgrammableBlock;
			beacon = GridTerminalSystem.GetBlockWithName(beacon_name) as IMyBeacon;
			
			GridTerminalSystem.SearchBlocksOfName(drills_name, drills);
			
            Runtime.UpdateFrequency = UpdateFrequency.Update100;
            //throw new Exception("Block Actions: " + "");
			
            action_drill = Int32.Parse(beacon.CustomData);
            wdisp("");
			areas = get_areas();
			//Echo("--");
			//Random rnd = new Random();
			//Echo(rnd.Next(0,100).ToString());
			//Echo(areas[rnd.Next(0, 5),1].ToString());
			//Echo("--");
			
		}
		
		public int get_areas_index_next_point(int[,]areas){
			int drills_min_index = 0;
			int drills_min = 999;
			for (int i=0; i < areas.GetLength(0); i++){
				if (areas[i,3] <= drills_min){
					drills_min = areas[i,3];
					drills_min_index = i;
				}
			}
			return drills_min_index;
		}
		
		public int[,] get_areas(){
			int areas_count = 0;
		    for (int x = xzdim*-1; x <= xzdim;){
				for (int z = xzdim*-1; z <= xzdim;){
					for (int y=34; y <= 250;){
						areas_count++;
                        y = y + (int)areasize;
					}
				    z = z + (int)areasize;
				}
				x = x + (int)areasize;
			}
			
			int[,] areas_local = new int[areas_count,4];
			areas_count = 0;
			
			for (int x = xzdim*-1; x <= xzdim;){
				for (int z = xzdim*-1; z <= xzdim;){
					for (int y=34; y <= 250;){
						areas_local[areas_count,0] = x;
						areas_local[areas_count,1] = y;
						areas_local[areas_count,2] = z;
						areas_local[areas_count,3] = 0;
						Echo(areas_local[areas_count,0] + " " + areas_local[areas_count,1] + " " + areas_local[areas_count,2] + " " + areas_local[areas_count,3]);
						areas_count++;
                        y = y + (int)areasize;
					}
				    z = z + (int)areasize;
				}
				x = x + (int)areasize;
			}
			return areas_local;
		}

        public void Save()
        {

        }

        public float getoffset(string axis)
        {
            float offset = drills[0].GetValueFloat("Drill.AreaOffset" + direction(axis));
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
                return System.Text.RegularExpressions.Regex.IsMatch(pbtext, ".+?approaching.+");
            }
            catch
            {
            }
            return false;
        }

        public void travelnext()
        {
			Echo("Travel to next point...");
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

        //public void drillstartposition()
        //{
        //    setoffset((xzdim * -1) - 1, "x");
        //    setoffset(34, "y");
        //    setoffset(xzdim * -1, "z");
        //}
		//
        //public void drillendposition()
        //{
        //    setoffset(xzdim, "x");
        //    setoffset(250, "y");
        //    setoffset(xzdim, "z");
        //}
		
		public bool drillarea(int drill_num)
        {
            string result = "Nothing to drill...";
            try
            {
                object tgt = drills[drill_num].GetValue<object>("Drill.CurrentDrillTarget");
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
		
		public void flush_drillprevpos(){
			for ( int i = 0; i< drills.Count; i++){
			  drillprevpos[i] = 999;
			}
			
		}
		
        public void Main(string argument, UpdateType updateSource)
        {
            switch (argument)
            {
                case "addgps":
                    addgps();
                    break;
                case "drill":
                    gyro.SetValue("Override", true);
                    beacon.CustomData = "1";
                    action_drill = 1;
                    break;
                case "nodrill":
                    gyro.SetValue("Override", false);
                    beacon.CustomData = "0";
                    action_drill = 0;
                    break;
                //case "drillstartposition":
                //    drillstartposition();
                //    break;
                //case "drillendposition":
                //    drillendposition();
                //    break;
				case "test":
				    int nextpos_index = get_areas_index_next_point(areas);
			        areas[nextpos_index,3]++;
					Echo(nextpos_index + " " + areas[nextpos_index, 0] + " " + areas[nextpos_index, 1] + " " + areas[nextpos_index, 2] + " " + areas[nextpos_index, 3]);
				    break;
                default:
                    break;
            }
            

            if (action_drill == 0)
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
				wdisp("\nTraveling...", true);
                return;
            }
            else
            {
				wdisp("\nIdle...", true);
                gyro.SetValue("Override", true);
                
            }
			
			if (timer1(drillworkdelay) == true){
			    for (int drillnum = 0; drillnum < drills.Count; drillnum++) {
			    	Echo("Drill " + drillnum);
			    	if (drillarea(drillnum) == true){
			    	}else{
				    
						if (drillprevpos[drillnum] != 999){
							areas[drillprevpos[drillnum], 3] = 999;
						}
						Echo("Drill " + drillnum + " next position...");
				        int nextpos_index = get_areas_index_next_point(areas);
						Echo("Next position for drill " + drillnum + " is " + nextpos_index);
						if (areas[nextpos_index, 3] < 999){
						  drillprevpos[drillnum] = nextpos_index;
			              areas[nextpos_index,3]++;
				          int x = areas[nextpos_index,0];
				          int y = areas[nextpos_index,1];
				          int z = areas[nextpos_index,2];
				          setpos(x,"x",drillnum); setpos(y,"y",drillnum); setpos(z,"z",drillnum);
						} else {
							areas = get_areas();
							travelnext();
						}
					}
				}
			}
			
			for (int i=0; i < areas.GetLength(0); i++){
				Echo(areas[i,0] + " " + areas[i,1] + " " + areas[i,2] + " " + areas[i,3]);
			}
			
        }
