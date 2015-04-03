﻿namespace PicesCommander
{
  partial class ChartGPSByStation
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose (bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose ();
      }
      base.Dispose (disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent ()
    {
      this.components = new System.ComponentModel.Container();
      System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea4 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
      System.Windows.Forms.DataVisualization.Charting.Legend legend4 = new System.Windows.Forms.DataVisualization.Charting.Legend();
      System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
      System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint13 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(-87D, 29D);
      System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint14 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(-86.8D, 28.2D);
      System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint15 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(-86.9D, 29.1D);
      System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint16 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(-86.9D, 29.3D);
      System.Windows.Forms.DataVisualization.Charting.Title title4 = new System.Windows.Forms.DataVisualization.Charting.Title();
      this.SelectionCriteriaPanel = new System.Windows.Forms.Panel();
      this.Station = new System.Windows.Forms.TextBox();
      this.StationLabel = new System.Windows.Forms.Label();
      this.PlotButton = new System.Windows.Forms.Button();
      this.CancelPlotButton = new System.Windows.Forms.Button();
      this.ProfileChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
      this.TimeInterval = new System.Windows.Forms.NumericUpDown();
      this.timer1 = new System.Windows.Forms.Timer(this.components);
      this.TimeIntervalLabel = new System.Windows.Forms.Label();
      this.Status = new System.Windows.Forms.TextBox();
      this.DeploymentsToPlot = new System.Windows.Forms.CheckedListBox();
      this.CurGPSLocation = new System.Windows.Forms.TextBox();
      this.CurrentGPSLocationLabel = new System.Windows.Forms.Label();
      this.ClearAllButton = new System.Windows.Forms.Button();
      this.SelectAllButton = new System.Windows.Forms.Button();
      this.COGLabel = new System.Windows.Forms.Label();
      this.COGField = new System.Windows.Forms.TextBox();
      this.SOGLabel = new System.Windows.Forms.Label();
      this.SOGField = new System.Windows.Forms.TextBox();
      this.CruiseHighlightedLabel = new System.Windows.Forms.Label();
      this.DeploymentHighlighted = new System.Windows.Forms.TextBox();
      this.SelectionCriteriaPanel.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ProfileChart)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.TimeInterval)).BeginInit();
      this.SuspendLayout();
      // 
      // SelectionCriteriaPanel
      // 
      this.SelectionCriteriaPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
      this.SelectionCriteriaPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.SelectionCriteriaPanel.Controls.Add(this.Station);
      this.SelectionCriteriaPanel.Controls.Add(this.StationLabel);
      this.SelectionCriteriaPanel.Location = new System.Drawing.Point(12, 12);
      this.SelectionCriteriaPanel.Name = "SelectionCriteriaPanel";
      this.SelectionCriteriaPanel.Size = new System.Drawing.Size(353, 72);
      this.SelectionCriteriaPanel.TabIndex = 24;
      // 
      // Station
      // 
      this.Station.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.Station.Location = new System.Drawing.Point(9, 30);
      this.Station.Name = "Station";
      this.Station.ReadOnly = true;
      this.Station.Size = new System.Drawing.Size(103, 23);
      this.Station.TabIndex = 6;
      this.Station.TabStop = false;
      // 
      // StationLabel
      // 
      this.StationLabel.AutoSize = true;
      this.StationLabel.Location = new System.Drawing.Point(12, 14);
      this.StationLabel.Name = "StationLabel";
      this.StationLabel.Size = new System.Drawing.Size(40, 13);
      this.StationLabel.TabIndex = 5;
      this.StationLabel.Text = "Station";
      // 
      // PlotButton
      // 
      this.PlotButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.PlotButton.Location = new System.Drawing.Point(11, 502);
      this.PlotButton.Name = "PlotButton";
      this.PlotButton.Size = new System.Drawing.Size(75, 23);
      this.PlotButton.TabIndex = 28;
      this.PlotButton.Text = "Plot";
      this.PlotButton.UseVisualStyleBackColor = true;
      this.PlotButton.Click += new System.EventHandler(this.PlotButton_Click);
      // 
      // CancelPlotButton
      // 
      this.CancelPlotButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.CancelPlotButton.Location = new System.Drawing.Point(284, 502);
      this.CancelPlotButton.Name = "CancelPlotButton";
      this.CancelPlotButton.Size = new System.Drawing.Size(75, 23);
      this.CancelPlotButton.TabIndex = 29;
      this.CancelPlotButton.Text = "Cancel";
      this.CancelPlotButton.UseVisualStyleBackColor = true;
      this.CancelPlotButton.Click += new System.EventHandler(this.CancelPlotButton_Click);
      // 
      // ProfileChart
      // 
      this.ProfileChart.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
      this.ProfileChart.CausesValidation = false;
      chartArea4.AlignmentOrientation = ((System.Windows.Forms.DataVisualization.Charting.AreaAlignmentOrientations)((System.Windows.Forms.DataVisualization.Charting.AreaAlignmentOrientations.Vertical | System.Windows.Forms.DataVisualization.Charting.AreaAlignmentOrientations.Horizontal)));
      chartArea4.AlignWithChartArea = "ChartArea1";
      chartArea4.AxisX.Enabled = System.Windows.Forms.DataVisualization.Charting.AxisEnabled.True;
      chartArea4.AxisX.LabelAutoFitMaxFontSize = 12;
      chartArea4.AxisX.LabelStyle.Format = "##0.000";
      chartArea4.AxisX.MajorGrid.Enabled = false;
      chartArea4.AxisX.Title = "Longitude";
      chartArea4.AxisX2.LabelStyle.Format = "##0.00";
      chartArea4.AxisX2.MajorGrid.Enabled = false;
      chartArea4.AxisY.IsStartedFromZero = false;
      chartArea4.AxisY.LabelAutoFitMaxFontSize = 12;
      chartArea4.AxisY.LabelStyle.Format = "#0.000";
      chartArea4.AxisY.MajorGrid.Enabled = false;
      chartArea4.AxisY.MajorTickMark.Interval = 0D;
      chartArea4.AxisY.MajorTickMark.IntervalOffset = 0D;
      chartArea4.AxisY.MajorTickMark.IntervalOffsetType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Number;
      chartArea4.AxisY.MajorTickMark.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Number;
      chartArea4.AxisY.Title = "Latitude";
      chartArea4.AxisY2.MajorGrid.Enabled = false;
      chartArea4.BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
      chartArea4.Name = "ChartArea1";
      this.ProfileChart.ChartAreas.Add(chartArea4);
      legend4.Alignment = System.Drawing.StringAlignment.Far;
      legend4.DockedToChartArea = "ChartArea1";
      legend4.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Bottom;
      legend4.IsDockedInsideChartArea = false;
      legend4.Name = "Legend1";
      this.ProfileChart.Legends.Add(legend4);
      this.ProfileChart.Location = new System.Drawing.Point(380, 12);
      this.ProfileChart.Name = "ProfileChart";
      series4.BorderWidth = 2;
      series4.ChartArea = "ChartArea1";
      series4.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
      series4.Legend = "Legend1";
      series4.LegendText = "A (2/2/13 12:32)";
      series4.Name = "Deployment(XXX)";
      dataPoint13.Label = "12:32";
      dataPoint13.LegendText = "";
      series4.Points.Add(dataPoint13);
      series4.Points.Add(dataPoint14);
      series4.Points.Add(dataPoint15);
      series4.Points.Add(dataPoint16);
      this.ProfileChart.Series.Add(series4);
      this.ProfileChart.Size = new System.Drawing.Size(575, 513);
      this.ProfileChart.TabIndex = 30;
      this.ProfileChart.Text = "GPS Plot";
      title4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      title4.Name = "GPS";
      title4.Text = "GPS";
      this.ProfileChart.Titles.Add(title4);
      this.ProfileChart.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ProfileChart_MouseClick);
      // 
      // TimeInterval
      // 
      this.TimeInterval.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.TimeInterval.Location = new System.Drawing.Point(12, 381);
      this.TimeInterval.Maximum = new decimal(new int[] {
            120,
            0,
            0,
            0});
      this.TimeInterval.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            65536});
      this.TimeInterval.Name = "TimeInterval";
      this.TimeInterval.Size = new System.Drawing.Size(63, 23);
      this.TimeInterval.TabIndex = 31;
      this.TimeInterval.Value = new decimal(new int[] {
            10,
            0,
            0,
            65536});
      // 
      // timer1
      // 
      this.timer1.Interval = 50;
      this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
      // 
      // TimeIntervalLabel
      // 
      this.TimeIntervalLabel.AutoSize = true;
      this.TimeIntervalLabel.Location = new System.Drawing.Point(9, 365);
      this.TimeIntervalLabel.Name = "TimeIntervalLabel";
      this.TimeIntervalLabel.Size = new System.Drawing.Size(68, 13);
      this.TimeIntervalLabel.TabIndex = 32;
      this.TimeIntervalLabel.Text = "Time Interval";
      // 
      // Status
      // 
      this.Status.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.Status.Location = new System.Drawing.Point(11, 472);
      this.Status.Name = "Status";
      this.Status.ReadOnly = true;
      this.Status.Size = new System.Drawing.Size(355, 23);
      this.Status.TabIndex = 35;
      this.Status.TabStop = false;
      // 
      // DeploymentsToPlot
      // 
      this.DeploymentsToPlot.CheckOnClick = true;
      this.DeploymentsToPlot.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.DeploymentsToPlot.FormattingEnabled = true;
      this.DeploymentsToPlot.Location = new System.Drawing.Point(12, 125);
      this.DeploymentsToPlot.Name = "DeploymentsToPlot";
      this.DeploymentsToPlot.Size = new System.Drawing.Size(353, 232);
      this.DeploymentsToPlot.TabIndex = 46;
      this.DeploymentsToPlot.Format += new System.Windows.Forms.ListControlConvertEventHandler(this.DeploymentsToPlot_Format);
      // 
      // CurGPSLocation
      // 
      this.CurGPSLocation.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
      this.CurGPSLocation.Location = new System.Drawing.Point(181, 383);
      this.CurGPSLocation.Name = "CurGPSLocation";
      this.CurGPSLocation.ReadOnly = true;
      this.CurGPSLocation.Size = new System.Drawing.Size(186, 23);
      this.CurGPSLocation.TabIndex = 48;
      this.CurGPSLocation.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      // 
      // CurrentGPSLocationLabel
      // 
      this.CurrentGPSLocationLabel.AutoSize = true;
      this.CurrentGPSLocationLabel.Location = new System.Drawing.Point(184, 366);
      this.CurrentGPSLocationLabel.Name = "CurrentGPSLocationLabel";
      this.CurrentGPSLocationLabel.Size = new System.Drawing.Size(73, 13);
      this.CurrentGPSLocationLabel.TabIndex = 49;
      this.CurrentGPSLocationLabel.Text = "GPS Location";
      // 
      // ClearAllButton
      // 
      this.ClearAllButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ClearAllButton.Location = new System.Drawing.Point(12, 93);
      this.ClearAllButton.Name = "ClearAllButton";
      this.ClearAllButton.Size = new System.Drawing.Size(75, 22);
      this.ClearAllButton.TabIndex = 50;
      this.ClearAllButton.Text = "Clear All";
      this.ClearAllButton.UseVisualStyleBackColor = true;
      this.ClearAllButton.Click += new System.EventHandler(this.ClearAllButton_Click);
      // 
      // SelectAllButton
      // 
      this.SelectAllButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.SelectAllButton.Location = new System.Drawing.Point(93, 93);
      this.SelectAllButton.Name = "SelectAllButton";
      this.SelectAllButton.Size = new System.Drawing.Size(75, 22);
      this.SelectAllButton.TabIndex = 51;
      this.SelectAllButton.Text = "Select All";
      this.SelectAllButton.UseVisualStyleBackColor = true;
      this.SelectAllButton.Click += new System.EventHandler(this.SelectAllButton_Click);
      // 
      // COGLabel
      // 
      this.COGLabel.AutoSize = true;
      this.COGLabel.Location = new System.Drawing.Point(205, 415);
      this.COGLabel.Name = "COGLabel";
      this.COGLabel.Size = new System.Drawing.Size(30, 13);
      this.COGLabel.TabIndex = 53;
      this.COGLabel.Text = "COG";
      // 
      // COGField
      // 
      this.COGField.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
      this.COGField.Location = new System.Drawing.Point(202, 431);
      this.COGField.Name = "COGField";
      this.COGField.ReadOnly = true;
      this.COGField.Size = new System.Drawing.Size(79, 23);
      this.COGField.TabIndex = 52;
      this.COGField.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      // 
      // SOGLabel
      // 
      this.SOGLabel.AutoSize = true;
      this.SOGLabel.Location = new System.Drawing.Point(290, 415);
      this.SOGLabel.Name = "SOGLabel";
      this.SOGLabel.Size = new System.Drawing.Size(30, 13);
      this.SOGLabel.TabIndex = 55;
      this.SOGLabel.Text = "SOG";
      // 
      // SOGField
      // 
      this.SOGField.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
      this.SOGField.Location = new System.Drawing.Point(287, 431);
      this.SOGField.Name = "SOGField";
      this.SOGField.ReadOnly = true;
      this.SOGField.Size = new System.Drawing.Size(79, 23);
      this.SOGField.TabIndex = 54;
      this.SOGField.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      // 
      // CruiseHighlightedLabel
      // 
      this.CruiseHighlightedLabel.AutoSize = true;
      this.CruiseHighlightedLabel.Location = new System.Drawing.Point(14, 415);
      this.CruiseHighlightedLabel.Name = "CruiseHighlightedLabel";
      this.CruiseHighlightedLabel.Size = new System.Drawing.Size(92, 13);
      this.CruiseHighlightedLabel.TabIndex = 57;
      this.CruiseHighlightedLabel.Text = "Cruise Highlighted";
      // 
      // DeploymentHighlighted
      // 
      this.DeploymentHighlighted.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
      this.DeploymentHighlighted.Location = new System.Drawing.Point(11, 431);
      this.DeploymentHighlighted.Name = "DeploymentHighlighted";
      this.DeploymentHighlighted.ReadOnly = true;
      this.DeploymentHighlighted.Size = new System.Drawing.Size(183, 23);
      this.DeploymentHighlighted.TabIndex = 56;
      this.DeploymentHighlighted.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      // 
      // ChartGPSByStation
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(967, 544);
      this.Controls.Add(this.CruiseHighlightedLabel);
      this.Controls.Add(this.DeploymentHighlighted);
      this.Controls.Add(this.SOGLabel);
      this.Controls.Add(this.SOGField);
      this.Controls.Add(this.COGLabel);
      this.Controls.Add(this.COGField);
      this.Controls.Add(this.SelectAllButton);
      this.Controls.Add(this.ClearAllButton);
      this.Controls.Add(this.CurrentGPSLocationLabel);
      this.Controls.Add(this.CurGPSLocation);
      this.Controls.Add(this.DeploymentsToPlot);
      this.Controls.Add(this.Status);
      this.Controls.Add(this.TimeIntervalLabel);
      this.Controls.Add(this.TimeInterval);
      this.Controls.Add(this.ProfileChart);
      this.Controls.Add(this.CancelPlotButton);
      this.Controls.Add(this.PlotButton);
      this.Controls.Add(this.SelectionCriteriaPanel);
      this.MinimumSize = new System.Drawing.Size(983, 572);
      this.Name = "ChartGPSByStation";
      this.Text = "Chart GPS by Station";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ChartGPS_FormClosing);
      this.Load += new System.EventHandler(this.ChartGPS_Load);
      this.SizeChanged += new System.EventHandler(this.ChartGPS_SizeChanged);
      this.Resize += new System.EventHandler(this.ChartGPS_Resize);
      this.SelectionCriteriaPanel.ResumeLayout(false);
      this.SelectionCriteriaPanel.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ProfileChart)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.TimeInterval)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Panel SelectionCriteriaPanel;
    private System.Windows.Forms.TextBox Station;
    private System.Windows.Forms.Label StationLabel;
    private System.Windows.Forms.Button PlotButton;
    private System.Windows.Forms.Button CancelPlotButton;
    private System.Windows.Forms.DataVisualization.Charting.Chart ProfileChart;
    private System.Windows.Forms.NumericUpDown TimeInterval;
    private System.Windows.Forms.Timer timer1;
    private System.Windows.Forms.Label TimeIntervalLabel;
    private System.Windows.Forms.TextBox Status;
    private System.Windows.Forms.CheckedListBox DeploymentsToPlot;
    private System.Windows.Forms.TextBox CurGPSLocation;
    private System.Windows.Forms.Label CurrentGPSLocationLabel;
    private System.Windows.Forms.Button ClearAllButton;
    private System.Windows.Forms.Button SelectAllButton;
    private System.Windows.Forms.Label COGLabel;
    private System.Windows.Forms.TextBox COGField;
    private System.Windows.Forms.Label SOGLabel;
    private System.Windows.Forms.TextBox SOGField;
    private System.Windows.Forms.Label CruiseHighlightedLabel;
    private System.Windows.Forms.TextBox DeploymentHighlighted;


  }
}