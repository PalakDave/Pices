﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Threading;
using System.IO;

using SipperFile;
using PicesInterface;

namespace PicesCommander
{
  public partial class ChartSizeDistribution : Form
  {
    PicesClassList  classes       = null;
    PicesClassList  activeClasses = null;
    String          rootDir       = "";

    private  String  cruise       = null;
    private  String  station      = null;
    private  String  deployment   = null;

    private  String  infinityStr = ((char)8734).ToString ();

    private  PicesDataBase    mainWinConn = null;
    private  PicesDataBase    threadConn  = null;  /**< This connection will only be use by the background thread. */

    private  PicesMsgQueue    statusMsgs = null;
    private  PicesMsgQueue    msgQueue   = null;
    private  PicesRunLog      runLog     = null;

    private  String           criteriaStr = "";

    private  PicesClass       classToPlot = null;
    private  bool             includeSubClasses = false;
    private  bool             weightByVolume    = false;

    private  PicesImageSizeDistribution     downCast = null;
    private  PicesImageSizeDistribution     upCast   = null;
    private  PicesImageSizeDistribution     bucketsDisplayed = null;  // Will contain the data thatis actually displayed in the Chart; depending of Cast options.
    private  PicesImageSizeDistributionRow  integratedDepth = null;
    private  uint[]                         integratedDepthDistribution = null;


    private  String  cast              = "Down";
    private  char    statistic         = '0';
    private  float   growthRate        = 1.1f;
    private  float   initialSizeValue = 0.01f;

    private  PicesGoalKeeper  goalie = new PicesGoalKeeper ("ChartSizeDistribution");

    private  bool  buildPlotDataRunning = false;
    private  bool  buildPlotDataDone    = false;
    private  bool  cancelRequested      = false;

    private  int  heightLast = 0;
    private  int  widthLast  = 0;

    private  Color[]  colorTable = {Color.Black,      Color.Red,        Color.Green,      
                                    Color.Blue,       Color.Crimson,    Color.DarkCyan,   
                                    Color.DarkOrange, Color.Lime,       Color.SeaGreen
                                   };

    private  String  configFileName = "";
    private  bool    formIsMaximized = false;


    private  float[]  depthVolumeProfile = null;

    private  int      selectedSizeBucket = -1;



    public ChartSizeDistribution (String          _cruise,
                                  String          _station,
                                  String          _deployment,
                                  PicesClass      _classToPlot,
                                  PicesClassList  _classes,
                                  PicesClassList  _activeClasses,
                                  String          _rootDir
                                 )
    {
      cruise        = _cruise;
      station       = _station;
      deployment    = _deployment;
      classToPlot   = _classToPlot;
      classes       = _classes;
      activeClasses = _activeClasses;
      rootDir       = _rootDir;

      statusMsgs = new PicesMsgQueue ("ChartSizeDistribution-StatusMsgs");
      msgQueue   = new PicesMsgQueue ("ChartSizeDistribution-RunLog");
      runLog     = new PicesRunLog (msgQueue);

      mainWinConn = PicesDataBase.GetGlobalDatabaseManagerNewInstance (runLog);

      configFileName = OSservices.AddSlash (PicesSipperVariables.PicesConfigurationDirectory ()) + "ChartSizeDistribution.cfg";

      InitializeComponent ();
    }

   

    private  void  AddToStr (ref String existingStr, String  addOn)
    {
      if  (String.IsNullOrEmpty (addOn))
        return;

      if  (!String.IsNullOrEmpty (existingStr))
        existingStr += ", ";
      existingStr += addOn;
    }



    private void ChartAbundanceByDeployment_Load (object sender, EventArgs e)
    {
      heightLast = Height;
      widthLast  = Width;

      Cruise_.Text     = cruise;
      Station_.Text    = station;
      Deployment_.Text = deployment;

      if  (classToPlot != null)
        ClassToPlot.Text = classToPlot.Name;

      ContextMenuStrip cms = new ContextMenuStrip ();
      cms.Items.Add ("Copy Chart to clipboard",               null, CopyChartToClipboard);
      cms.Items.Add ("Save Chart to Disk",                    null, SaveChartToDisk);
      cms.Items.Add ("Copy Data Tab-Delimited to Clipboard",  null, SaveTabDelToClipBoard);
      cms.Items.Add ("Save Data Tab-Delimited to Disk",       null, SaveTabDelToDisk);

      ProfileChart.ContextMenuStrip = cms;

      CastField.SelectedItem = CastField.Items[0];
      CastField.Text         = (String)CastField.Items[0];

      SizeStatisticField.SelectedItem = SizeStatisticField.Items[0];
      SizeStatisticField.Text         = (String)SizeStatisticField.Items[0];


      LoadConfigurationFile ();
    }


    private  void  DisableControls ()
    {
      PlotButton.Enabled         = false;
      IncludeSubClasses.Enabled  = false;
      WeightByVolume.Enabled     = false;
      InitialSizeField.Enabled   = false;
      MaxSizeField.Enabled       = false;
      GrowthRateField.Enabled    = false;
      SizeStatisticField.Enabled = false;
    }


    private  void  EnableControls ()
    {
      PlotButton.Enabled         = true;
      IncludeSubClasses.Enabled  = true;
      WeightByVolume.Enabled     = true;
      InitialSizeField.Enabled   = true;
      MaxSizeField.Enabled       = true;
      GrowthRateField.Enabled    = true;
      SizeStatisticField.Enabled = true;
    }


    private  void  SaveConfiguration ()
    {
      System.IO.StreamWriter  o = null;
      try{o = new System.IO.StreamWriter (configFileName);}  catch  (Exception){o = null; return;}
      
      o.WriteLine ("//ChartSizeDistribution Configuration File");
      o.WriteLine ("//");
      o.WriteLine ("//DateTime Saved" + "\t" + DateTime.Now.ToString ("F"));
      o.WriteLine ("//");
      o.WriteLine ("WidthLast"          + "\t" + widthLast);
      o.WriteLine ("HeightLast"         + "\t" + heightLast);
      o.WriteLine ("Maximized"          + "\t" + (formIsMaximized ? "YES":"NO"));
      o.WriteLine ("ClassToPlot"        + "\t" + ClassToPlot.Text);
      o.WriteLine ("IncludeSubClasses"  + "\t" + IncludeSubClasses.Checked);
      o.WriteLine ("WeightByVolume"     + "\t" + WeightByVolume.Checked);
      o.WriteLine ("SizeStatistic"      + "\t" + SizeStatisticField.Text);
      o.WriteLine ("InitialSize"        + "\t" + InitialSizeField.Value.ToString ("###0.000"));
      o.WriteLine ("MaxSize"            + "\t" + MaxSizeField.Value);
      o.WriteLine ("GrowthRate"         + "\t" + GrowthRateField.Value);
      if  (SizeStatisticField.SelectedItem != null)
        o.WriteLine ("SizeStatistic"    + "\t" + SizeStatisticField.SelectedItem.ToString ());
      o.Close ();
      o = null;
    }


    private  void  SetSizeStatistic (String  s)
    {
      foreach  (Object o in SizeStatisticField.Items)
      {
        if  (o.ToString ().Equals (s, StringComparison.InvariantCultureIgnoreCase))
        {
          SizeStatisticField.SelectedItem = o;
          return;
        }
      }
      SizeStatisticField.SelectedIndex = 0;
    } /* SetSizeStatistic */



    private  void   LoadConfigurationFile ()
    {
      System.IO.StreamReader i = null;

      try {i = new System.IO.StreamReader (configFileName);}  catch  (Exception) {i = null;}
      if  (i == null)
        return;

      int  savedWidth  = 0;
      int  savedHeight = 0;
      bool screenWasMaximized = false;

      String  nextLine = null;

      while  (true)
      {
        try  {nextLine = i.ReadLine ();}  catch (Exception) {break;}
        if  (nextLine == null)
          break;

        nextLine = nextLine.Trim ();
        
        if  ((nextLine.Length < 3)  ||  (nextLine.Substring (0, 2) == "//"))
          continue;

        String[] fields = nextLine.Split ('\t');
        if  (fields.Length < 2)
          continue;

        String  fieldName  = fields[0];
        String  fieldValue = fields[1];

        switch  (fieldName)
        { 
          case  "WidthLast":
            savedWidth = PicesKKStr.StrToInt (fieldValue);
            break;

          case  "HeightLast":
            savedHeight = PicesKKStr.StrToInt (fieldValue);
            break;

          case  "Maximized":
            screenWasMaximized  = (fieldValue.ToUpper () == "YES");
            break;

          case  "ClassToPlot":
            ClassToPlot.Text = fieldValue;
            break;

          case  "IncludeSubClasses":
            IncludeSubClasses.Checked = PicesKKStr.StrToBool (fieldValue);
            break;
            
          case  "WeightByVolume":
            WeightByVolume.Checked = PicesKKStr.StrToBool (fieldValue);
            break;
            
          case  "SizeStatistic":
            SetSizeStatistic (fieldValue);
            break;

          case  "InitialSize":
            float  initialSize = PicesKKStr.StrToFloat (fieldValue);
            if  ((initialSize <= 0.0)  ||  (initialSize >= 100.0f))
              initialSize = 0.10f;
            InitialSizeField.Value = (decimal)initialSize;
            break;

          case  "MaxSize":
            float  maxSize = PicesKKStr.StrToFloat (fieldValue);
            if  ((maxSize <= 0.0)  ||  (maxSize < (float)InitialSizeField.Value))
              maxSize = Math.Max (1000.0f, (float)InitialSizeField.Value);

            MaxSizeField.Value = (decimal)maxSize;
            break;
          
          case  "GrowthRate":
            float  growthRate = PicesKKStr.StrToFloat (fieldValue);
            if  (growthRate <= 1.0f)
              growthRate = 1.1f;
            GrowthRateField.Value = (decimal)growthRate;
            break;
        }
      }

      i.Close ();

      if  (savedWidth > Screen.PrimaryScreen.Bounds.Width)
        savedWidth = Screen.PrimaryScreen.Bounds.Width;

      if  (savedHeight > Screen.PrimaryScreen.Bounds.Height)
        savedHeight = Screen.PrimaryScreen.Bounds.Height;

      if  (screenWasMaximized)
      {
        //savedWidth = Screen.PrimaryScreen.Bounds.Width;
        //savedHeight = savedHeight = Screen.PrimaryScreen.Bounds.Height;
        this.WindowState = FormWindowState.Maximized; 
      }
      else
      {
        Height = Math.Max (savedHeight, MinimumSize.Height);
        Width  = Math.Max (savedWidth,  MinimumSize.Width);
      }

      if  (SizeStatisticField.SelectedItem == null)
        SizeStatisticField.SelectedItem = SizeStatisticField.Items[0];

      OnResizeEnd (new EventArgs ());
    }  /* LoadConfigurationFile */



    private  List<String>  ValidateFields ()
    {
      List<String>  errors = new List<string> ();
      switch  (SizeStatisticField.Text)
      {
      case "Pixel Area":   statistic = '0';   break;
      case "Diameter":     statistic = '1';   break;
      case "Volume (SBv)": statistic = '2';   break;
      case "Volume (EBv)": statistic = '3';   break;
      }

      cast = CastField.Text;

      growthRate       = (float)GrowthRateField.Value;
      initialSizeValue = (float)InitialSizeField.Value;

      weightByVolume = WeightByVolume.Checked;
      includeSubClasses = IncludeSubClasses.Checked;

      if  (String.IsNullOrEmpty (ClassToPlot.Text))
        errors.Add ("You must specify class to plot.");
      else
        classToPlot = PicesClassList.GetUniqueClass (ClassToPlot.Text, "");

      if  (errors.Count < 1)
        return null;
      else
        return errors;
    }

    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="cast">'U'=Up-Cast, 'D'=Down-Cast, 'B'-Combined Up/Down Cast</param>
    /// <returns></returns>

    private  float[]  GetDepthVolumeProfile  (char  cast)
    {
      String  msg = "Retrieving Depth-Volume profile Station[" + cruise + "]  Station[" + station + "]  Deployment[" + deployment + "].";
      msgQueue.AddMsg (msg);
      statusMsgs.AddMsg (msg);
      PicesVolumeSampledStatList 
        volStats = threadConn.InstrumentDataGetVolumePerMeterDepth (cruise, station, deployment, 1.0f);
      if  (volStats == null)
        return  null;

      return  volStats.ToArray ();
    }




    private  void  SizeDistributionForAClass (PicesClass                      c,
                                              bool                            includeChildren,
                                              ref PicesImageSizeDistribution  downCastAcumulated,
                                              ref PicesImageSizeDistribution  upCastAcumulated
                                             )
    {
      statusMsgs.AddMsg ("Extracting for Class[" + c.Name + "]");
      sbyte  ch = (sbyte)statistic;
      PicesImageSizeDistribution  classDownCast = null;
      PicesImageSizeDistribution  classUpCast = null;
      threadConn.ImagesSizeDistributionByDepth (this.cruise, this.station, this.deployment, c.Name,
                                                1.0f,
                                                ch,
                                                (float)InitialSizeField.Value,
                                                (float)GrowthRateField.Value,
                                                (float)MaxSizeField.Value,
                                                ref classDownCast,
                                                ref classUpCast
                                               );

      if  (classDownCast != null)
      {
        if  (downCastAcumulated == null)
          downCastAcumulated = classDownCast;
        else
          downCastAcumulated.AddIn (classDownCast, runLog);
      }

      if  (classUpCast != null)
      {
        if  (upCastAcumulated == null)
          upCastAcumulated = classUpCast;
        else
          upCastAcumulated.AddIn (classUpCast, runLog);
      }

      if  (includeChildren)
      {
        foreach  (PicesClass  pc in  c.Children)
        {
          if  (cancelRequested)
            break;
          SizeDistributionForAClass (pc, includeChildren, ref downCastAcumulated, ref upCastAcumulated);
        }
      }
    }  /* SizeDistributionForAClass */
    
  



    
    /// <summary>
    /// This method will be ran as a separate thread; it is respnable for collecting all the data needed to generate the plot.
    /// </summary>
    private  void  BuildPlotData ()
    {
      if  (buildPlotDataRunning)
        return;

      PicesDataBase.ThreadInit ();

      threadConn = PicesDataBase.GetGlobalDatabaseManagerNewInstance (runLog);

      buildPlotDataRunning = true;

      classToPlot = PicesClassList.GetUniqueClass (ClassToPlot.Text, "");

      depthVolumeProfile = GetDepthVolumeProfile ('D');

      sbyte  ch = (sbyte)statistic;
      downCast = null;
      upCast = null;

      goalie.StartBlock ();
      
      SizeDistributionForAClass (classToPlot, 
                                 IncludeSubClasses.Checked, 
                                 ref downCast, 
                                 ref upCast
                                );

      if  (cast == "Down")
        bucketsDisplayed = downCast;

      else if  (cast == "Up")
        bucketsDisplayed = upCast;

      else
      {
        bucketsDisplayed = new PicesImageSizeDistribution (downCast);
        bucketsDisplayed.AddIn (upCast, runLog);
      }

      integratedDepth = bucketsDisplayed.AllDepths ();
      integratedDepthDistribution = integratedDepth.Distribution ();

      threadConn.Close ();
      threadConn = null;
      GC.Collect ();
      PicesDataBase.ThreadEnd ();

      goalie.EndBlock ();
      
      if  (cancelRequested)
        statusMsgs.AddMsg ("Plotting of data CANCELLED!!!");
      else
        statusMsgs.AddMsg ("Building of plot data completed !!!");

      buildPlotDataRunning = false;
      buildPlotDataDone = true;
    }  /* BuildPlotData */




    private  void  CancelBackGroundThread ()
    {
      cancelRequested = true;
      if  (threadConn != null)
        threadConn.CancelFlag = true;
      statusMsgs.AddMsg ("Cancel Requested");
    }




    private int  FindBestInetrvaleSize (int minSize, int maxSize, int range)
    {
      int  smallestInterval = range % maxSize;
      int  intervalThatIsSmallest = maxSize;

      int zed = maxSize - 1;
      while  (zed >= minSize)
      {
        int  mod = range % zed;
        if  (mod <= smallestInterval)
        {
          smallestInterval = mod;
          intervalThatIsSmallest = zed;
        }
        --zed;
      }
      return  intervalThatIsSmallest;
    }



    private  String  SubInSuperScriptExponent (String s)
    {
      int x = s.IndexOf("-3");
      while  (x >= 0)
      {
        s = s.Substring (0, x) + "\u00B3" + s.Substring (x + 2);
        x = s.IndexOf("-3");
      }

      x = s.IndexOf("-2");
      while  (x >= 0)
      {
        s = s.Substring (0, x) + "\u00B2" + s.Substring (x + 2);
        x = s.IndexOf("-2");
      }

      return s;
    }  /* SuperScriptExponent */




    private  void  UpdateChartAreas ()
    {
      if  (integratedDepth == null)
        return;

      goalie.StartBlock ();

      Font titleFont     = new Font (FontFamily.GenericSansSerif, 12);
      Font axisLabelFont = new Font (FontFamily.GenericSansSerif, 12);

      String  t1 = "Abundance by Size";
      switch  (statistic)
      {
      case '0': t1 = "Abundance by Size";                      break;
      case '1': t1 = "Abundance by Estimated Diamter";         break;
      case '2': t1 = "Abundance by Estimated Spheroid Volume"; break;
      case '3': t1 = "Abundance by Estimated Elipsoid Volume"; break;
      }

      String  t2 = "Cruise: " + cruise + "  Station: " + station;
      if  (String.IsNullOrEmpty (deployment))
        t2 += "  Deployment: " + deployment;

      t2 += "  Class: " + classToPlot.Name;

      ProfileChart.Titles.Clear ();
      ProfileChart.Titles.Add (new Title (t1, Docking.Top, titleFont, Color.Black));
      ProfileChart.Titles.Add (new Title (t2, Docking.Top, titleFont, Color.Black));

      if  (!String.IsNullOrEmpty (criteriaStr))
        ProfileChart.Titles.Add (new Title (criteriaStr, Docking.Top, titleFont, Color.Black));

      ProfileChart.Series.Clear ();

      ChartArea ca = ProfileChart.ChartAreas[0];

      Series s = new Series ("Size Distribution");
      s.ChartArea = "ChartArea1";
      s.ChartType = SeriesChartType.Column;

      float[]  startValues  = bucketsDisplayed.SizeStartValues ();
      float[]  endValues    = bucketsDisplayed.SizeEndValues   ();

      float  minX = float.MaxValue;
      float  minY = float.MaxValue;
      float  maxX = float.MinValue;
      float  maxY = float.MinValue;

      for  (int x = 0;  x < integratedDepthDistribution.Length;  ++x)
      {
        float  sv = startValues[x];
        float  ev = sv * growthRate;
        float  midPoint = (sv +ev) / 2.0f;
        if  (sv <= 0.0f)
          midPoint = initialSizeValue / 2.0f;

        double  d = (double)integratedDepthDistribution[x];
        if  (weightByVolume)
        {
          if ((depthVolumeProfile.Length > x) && (depthVolumeProfile[x] != 0.0))
            d = d / depthVolumeProfile[x];
          else
            d = 0.0;
        }
        float log10D = (float)Math.Log10 (d + 1.0);

        minY = Math.Min (minY, log10D);
        maxY = Math.Max (maxY, log10D);
        minX = Math.Min (minX, midPoint);
        maxX = Math.Max (maxX, midPoint);
        DataPoint dp = new DataPoint (midPoint, log10D);
        s.Points.Add (dp);
      }
      s.XAxisType = AxisType.Primary;
      s.YAxisType = AxisType.Primary;
      ProfileChart.ChartAreas[0].AxisY.LogarithmBase = 10.0;
      ProfileChart.ChartAreas[0].AxisY.LabelStyle.Format = "##,###,##0";

      if  (weightByVolume)
        ca.AxisY.Title = SubInSuperScriptExponent ("Log 10  Abundance");
      else
        ca.AxisY.Title = SubInSuperScriptExponent ("Log 10  Count");

      switch  (statistic)
      {
      case '0':
        ca.AxisX.Title = SubInSuperScriptExponent ("area(m-2)");
        ca.AxisX.LabelStyle.Format =  "##,##0.000";
        break;

      case '1':
        ca.AxisX.Title = SubInSuperScriptExponent ("Length(mili-meters)");
        ca.AxisX.LabelStyle.Format =  "##00.00";
        break;

      case '2':
        ca.AxisX.Title = SubInSuperScriptExponent ("Volume(mm-3)");
        ca.AxisX.LabelStyle.Format =  "##0.0000";
        break;

      case '3':
        ca.AxisX.Title = SubInSuperScriptExponent ("Volume(mm-3)");
        ca.AxisX.LabelStyle.Format =  "##0.0000";
        break;
      }

      ca.AxisX.IsLogarithmic    = true;
      ca.AxisX.Maximum          = maxX;
      ca.AxisX.LabelStyle.Angle = 45;
      ca.AxisX.LabelStyle.Font  = axisLabelFont;
      ca.AxisX.TitleFont        = axisLabelFont;

      ca.AxisY.Minimum          = 0.0;
      ca.AxisY.IsLogarithmic    = false;
      ca.AxisY.LabelStyle.Font  = axisLabelFont;
      ca.AxisY.TitleFont        = axisLabelFont;
      //ca.AxisX.IsLogarithmic = true;
 
      s.BorderWidth = 2;
      ProfileChart.Series.Add (s);

      try
      {
        ProfileChart.ChartAreas[0].RecalculateAxesScale ();
      }
      catch (Exception e)
      {
        runLog.Writeln (e.ToString ());
      }

      goalie.EndBlock ();
    }  /* UpdateChartAreas */



    private void PlotButton_Click (object sender, EventArgs e)
    {
      List<String>  errors = ValidateFields ();
      if  (errors != null)
      {
        String  errMsg = "Errors is class selection";
        foreach (String s in errors)
          errMsg += "\n" + s;
        MessageBox.Show (this, errMsg, "Validating Class Selection", MessageBoxButtons.OK);
      }
      else
      {
        DisableControls ();
        SaveConfiguration ();
        PlotButton.Enabled = false;
        if  (buildPlotDataRunning)
          return;

        timer1.Enabled = true;
        Thread t = new Thread (new ThreadStart (BuildPlotData));
        t.Start ();
      }
    }


    int ticks = 0;
    private void timer1_Tick (object sender, EventArgs e)
    {
      ++ticks;

      String msg = statusMsgs.GetNextMsg ();
      while  (msg != null)
      {
        Status.AppendText (msg);
        msg = statusMsgs.GetNextMsg ();
      }

      if   (buildPlotDataRunning)
      {
        if  ((ticks % 20) == 0)
          UpdateChartAreas ();
      }

      if  (buildPlotDataDone)
      {
        UpdateChartAreas ();
        PlotButton.Enabled = true;
        buildPlotDataDone = false;
        cancelRequested = false;
        EnableControls ();
      }
    }


    private void ChartAbundanceByDeployment_Resize (object sender, EventArgs e)
    {
      int  heightDelta = Height - heightLast;
      int  widthDelta  = Width  - widthLast;

      ProfileChart.Height += heightDelta;
      ProfileChart.Width  += widthDelta;

      Status.Height        += heightDelta;
      PlotButton.Top       += heightDelta;
      CancelPlotButton.Top += heightDelta;
   
      heightLast = Height;
      widthLast  = Width;
    }



    private void ChartAbundanceByDeployment_SizeChanged (object sender, EventArgs e)
    {
      if  (WindowState == FormWindowState.Maximized)
      {
        // Looks like user has pressed the Maximized button.  We have to trap it here because
        // the ResizeEnd envent does not trap when form is Maximized.
        //PicesCommanderFormResized ();
        ChartAbundanceByDeployment_Resize (sender, e);
        formIsMaximized = true;
      }
      else if  (WindowState == FormWindowState.Normal)
      {
        if  (formIsMaximized)
        {
          // We normally trap the ResizeEnd event;  but when the form was already maximized and the user
          // presses the button to unmaximize.  the ResizeEnd does not trap that.  So we check to 
          // see if the form was already maximize.  If so then we resized the form.
          //PicesCommanderFormResized ();
          ChartAbundanceByDeployment_Resize (sender, e);
          formIsMaximized = false;
        }
      }
    }



    private  void  CopyChartToClipboard (Object sender, EventArgs e)
    {
      using (System.IO.MemoryStream ms = new System.IO.MemoryStream ())
      {
        ProfileChart.SaveImage (ms, ChartImageFormat.Bmp);
        Bitmap bm = new Bitmap(ms);
        Clipboard.SetImage(bm);
      }
    }


    private  void  SaveChartToDisk (Object sender, EventArgs e)
    {
      SaveFileDialog sfd = new SaveFileDialog ();
      sfd.Filter = "JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif|Tab-Del|*.txt";
      sfd.Title = "Save chart as Image File";
      sfd.DefaultExt = "jpg";
      sfd.AddExtension = true;
      sfd.OverwritePrompt = true;
      //sfd.CheckFileExists = true;
      DialogResult dr = sfd.ShowDialog (this);
      if  (dr == DialogResult.OK)
      {
        String  fn = sfd.FileName;
        System.IO.FileInfo fi = new System.IO.FileInfo (fn);
        String ext = fi.Extension.ToLower ();

        ChartImageFormat cif = ChartImageFormat.Jpeg;
        if  (ext == ".jpg")
          cif = ChartImageFormat.Jpeg;

        else if  (ext == ".bmp")
          cif = ChartImageFormat.Bmp;

        else if  (ext == ".gif")
          cif = ChartImageFormat.Gif;
        
        else if  (ext == ".txt")
        {
          SaveTabDelToDisk (fn);
          return;
        }
        
        ProfileChart.SaveImage (fn, cif);
      }
    }


    
    private  void  SaveTabDelToDisk (Object sender, EventArgs e)
    {
      SaveFileDialog sfd = new SaveFileDialog ();
      sfd.Filter = "Tab delimited data to disk|*.txt";
      sfd.Title = "Save Abundance info as tab-delimited data to disk.";
      sfd.DefaultExt = "txt";
      sfd.AddExtension = true;
      sfd.OverwritePrompt = true;

      String fn = cruise + "_AbundanceByDeployment";
      if  (classToPlot != null)
        fn += ("_" + classToPlot.Name);
      fn += ".txt";
      sfd.FileName = fn;

      //sfd.CheckFileExists = true;
      DialogResult dr = sfd.ShowDialog (this);
      if  (dr == DialogResult.OK)
      {
        fn = sfd.FileName;
        SaveTabDelToDisk (fn);
      }
    }



    private  void  SaveTabDelToDisk (String fileName)
    {
      System.IO.StreamWriter sw = null;
      try  {sw = new System.IO.StreamWriter (fileName, false, System.Text.Encoding.ASCII);}
      catch (Exception) {sw = null;}

      if  (sw == null)
      {
        MessageBox.Show (this, "Could not open file[" + fileName + "]", "Save Tab-Delimited", MessageBoxButtons.OK);
        return;
      }
      WriteTabDelToStream (sw);
      sw.Close ();
      sw = null;

      MessageBox.Show (this, "GPS Data written to \"" + fileName + "\".", "", MessageBoxButtons.OK);
    }


    private  void  SaveTabDelToClipBoard (Object sender, EventArgs e)
    {
      StringWriter  sw = new StringWriter ();
      WriteTabDelToStream (sw);
      Clipboard.SetText (sw.ToString ());
      sw.Close ();
      sw = null;
    }


    
    private  void  WriteTabDelToStream (TextWriter tw)
    {
      DateTime  curTime = DateTime.Now;
      tw.WriteLine ("ChartSizeDistribution"); 
      tw.WriteLine ("DateTime" + "\t" + curTime.ToString ("yyyy/MM/dd HH:mm:ss")); 
      tw.WriteLine ();
      tw.WriteLine ("Cruise"               + "\t" + cruise);
      tw.WriteLine ("Station"              + "\t" + station);
      tw.WriteLine ("Deployment"           + "\t" + deployment);
      tw.WriteLine ("Class"                + "\t" + classToPlot.Name);
      tw.WriteLine ("Weight-by-Volume"     + "\t" + (weightByVolume    ? "Yes" : "No"));
      tw.WriteLine ("Include-Sub-Classes"  + "\t" + (includeSubClasses ? "Yes" : "No"));
      tw.WriteLine ();
      tw.WriteLine ("There will be two tables below: 1-Density, 2-Counts.");
      tw.WriteLine ();
      tw.WriteLine ();
      tw.WriteLine ();

      tw.WriteLine ("Density");
      tw.WriteLine ();
      int  largestIdx = 0;
      tw.Write ("Depth" + "\t" + "Vol-Sampled");
      /*
      foreach (DataSeriesToPlot dstp in series)
      {
        String s = dstp.legend;
        tw.Write ("\t" + s);
        if  (dstp.density.Length > largestIdx)
          largestIdx = dstp.density.Length;
      }
      tw.WriteLine ();

      for  (int  depthIdx = 0;  depthIdx < largestIdx;  ++depthIdx)
      {
        tw.Write (depthIdx * depthIncrement);
        tw.Write ("\t");
        if  (depthIdx < depthVolumeProfile.Length)
          tw.Write (depthVolumeProfile[depthIdx]);

        foreach  (DataSeriesToPlot dstp in series)
        {
          tw.Write ("\t");
          if  (depthIdx < dstp.density.Length)
            tw.Write (dstp.density[depthIdx]);
        }
        tw.WriteLine ();
      }

      tw.WriteLine ();
      tw.WriteLine ();
      tw.WriteLine ();

      tw.WriteLine ("Counts");
      tw.WriteLine ();
      largestIdx = 0;
      tw.Write ("Depth" + "\t" + "Vol-Sampled");
      foreach (DataSeriesToPlot dstp in series)
      {
        String s = dstp.legend;
        tw.Write ("\t" + s);
        if  (dstp.countsByDepth.Length > largestIdx)
          largestIdx = dstp.countsByDepth.Length;
      }
      tw.WriteLine ();

      for  (int  depthIdx = 0;  depthIdx < largestIdx;  ++depthIdx)
      {
        tw.Write (depthIdx * depthIncrement);

        tw.Write ("\t");
        if  (depthIdx < depthVolumeProfile.Length)
          tw.Write (depthVolumeProfile[depthIdx]);
        
        foreach  (DataSeriesToPlot dstp in series)
        {
          tw.Write ("\t");
          if  (depthIdx < dstp.countsByDepth.Length)
            tw.Write (dstp.countsByDepth[depthIdx]);
        }
        tw.WriteLine ();
      }
      */

      tw.WriteLine ();
      tw.WriteLine ("End of ChartSizeDistribution");
    }  /* WriteTabDelToStream */


    private void ChartSizeDistribution_FormClosing (object sender, FormClosingEventArgs e)
    {
      if  (buildPlotDataRunning)
      {
        if (e.CloseReason == CloseReason.WindowsShutDown)
        {
          CancelBackGroundThread ();
          while  (buildPlotDataRunning)
          {
            Thread.Sleep (100);
          }
        }
        else
        {
          MessageBox.Show (this, "Plot data is being collected; press \"Cancel Button\" to Stop before exiting.", "Form Closing", MessageBoxButtons.OK);
          e.Cancel = true;
          return;
        }
      }

      SaveConfiguration ();
      
      mainWinConn.Close ();
      mainWinConn = null;
      GC.Collect ();
    }



    private void CancelPlotButton_Click (object sender, EventArgs e)
    {
      if  (buildPlotDataRunning)
        CancelBackGroundThread ();
    }


    private void DeploymentsToPlot_Format (object sender, ListControlConvertEventArgs e)
    {
      if  (e.ListItem == null)
        return;
      PicesSipperDeployment  d = (PicesSipperDeployment)e.ListItem;
      String  m = d.StationName;
      if (!String.IsNullOrEmpty (d.DeploymentNum))
        m += "-" + d.DeploymentNum + "  " + d.Description;
      e.Value = m;
    }


    private void SelectClassButton_Click (object sender, EventArgs e)
    {
      SelectAPicesClass  sapc = new SelectAPicesClass (classes, rootDir, activeClasses);

      if  (!String.IsNullOrEmpty (ClassToPlot.Text))
      sapc.SelectedClass = PicesClassList.GetUniqueClass (ClassToPlot.Text, "");
      sapc.ShowDialog (this);
      if  (sapc.SelectionMade)
      {
        PicesClass pc = sapc.SelectedClass;
        if  (pc != null)
          ClassToPlot.Text = pc.Name;
      }
    }


    private  void  ClearHighLightedBucket ()
    {
      if  ((ProfileChart.Series.Count < 1)  ||  (selectedSizeBucket < 0))
        return;

      Series  s = ProfileChart.Series[0];
      if  (selectedSizeBucket >= s.Points.Count)
        return;

      s.Points[selectedSizeBucket].Label = "";
      s.Points[selectedSizeBucket].Color = s.Color;

      selectedSizeBucket = -1;
    }  /* ClearHighLightedBucket*/




    private  void  HighLightSizeBucket (int  sizeBucket)
    {
      if  ((ProfileChart.Series.Count < 1)  ||  (sizeBucket < 0))
        return;

      if (sizeBucket >= integratedDepthDistribution.Length)
        return;

      Series  s = ProfileChart.Series[0];
      if  (sizeBucket >= s.Points.Count)
        return;

      ClearHighLightedBucket ();

      DataPoint dp = s.Points[sizeBucket];
      double  d = (double)integratedDepthDistribution[sizeBucket];
      if  (weightByVolume)
      {
        if ((depthVolumeProfile.Length > sizeBucket) && (depthVolumeProfile[sizeBucket] != 0.0))
          d = d / depthVolumeProfile[sizeBucket];
        else
          d = 0.0;
      }
      
      s.Points[sizeBucket].Label = d.ToString("###,##0.0") + "(" + dp.XValue.ToString("##0.000") + ")";
      s.Points[sizeBucket].Color = Color.Red;

      selectedSizeBucket = sizeBucket;
    }  /* HighLightSizeBucket */



    private void ProfileChart_MouseClick (object sender, MouseEventArgs e)
    {
      Point  p = e.Location;
      ChartArea  ca = ProfileChart.ChartAreas[0];
      double yValue = ca.AxisY.PixelPositionToValue ((double)p.Y);
      double xValue = ca.AxisX.PixelPositionToValue ((double)p.X);
      
      // Because the x-axis is Log10 of the actiual value we need to raise 10 to the power of xValue.
      xValue = Math.Pow (10, xValue);

      int  sizeBucket = bucketsDisplayed.IdentifySizeBucket ((float)xValue);
      if  (sizeBucket < 0)
        return;
      HighLightSizeBucket (sizeBucket);
    }  /* ProfileChart_MouseClick */
  }
}
