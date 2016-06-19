ALTER TABLE `pices_new`.`deployments` 
    ADD COLUMN `SyncTimeStampActual` DATETIME NULL DEFAULT NULL  AFTER `Longitude`,
    ADD COLUMN `SyncTimeStampCTD`    DATETIME NULL DEFAULT NULL  AFTER `SyncTimeStampActual`,
    ADD COLUMN `SyncTimeStampGPS`    DATETIME NULL DEFAULT NULL  COMMENT 'The three SyncTimeStamp fields will hold example timestamps that indicate the relative differences between the clocks.  This will allow PICES to sycronize clocks.	'  AFTER `SyncTimeStampCTD` ;






ALTER TABLE `pices_new`.`featuredata` 
     CHANGE COLUMN `Size` `Size` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `Moment1` `Moment1` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `Moment2` `Moment2` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `Moment3` `Moment3` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `Moment4` `Moment4` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `Moment5` `Moment5` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `Moment6` `Moment6` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `Moment7` `Moment7` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `EdgeSize` `EdgeSize` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `EdgeMoment1` `EdgeMoment1` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `EdgeMoment2` `EdgeMoment2` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `EdgeMoment3` `EdgeMoment3` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `EdgeMoment4` `EdgeMoment4` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `EdgeMoment5` `EdgeMoment5` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `EdgeMoment6` `EdgeMoment6` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `EdgeMoment7` `EdgeMoment7` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `TransparancyConvexHull` `TransparancyConvexHull` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `TransparancyPixelCount` `TransparancyPixelCount` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `TransparancyOpen3` `TransparancyOpen3` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `TransparancyOpen5` `TransparancyOpen5` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `TransparancyOpen7` `TransparancyOpen7` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `TransparancyOpen9` `TransparancyOpen9` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `TransparancyClose3` `TransparancyClose3` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `TransparancyClose5` `TransparancyClose5` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `TransparancyClose7` `TransparancyClose7` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `EigenRatio` `EigenRatio` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `EigenHead` `EigenHead` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `ConvexArea` `ConvexArea` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `TransparancySize` `TransparancySize` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `TransparancyWtd` `TransparancyWtd` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `WeighedMoment0` `WeighedMoment0` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `WeighedMoment1` `WeighedMoment1` double NULL DEFAULT NULL  , 
     CANGE COLUMN `WeighedMoment2` `WeighedMoment2` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `WeighedMoment3` `WeighedMoment3` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `WeighedMoment4` `WeighedMoment4` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `WeighedMoment5` `WeighedMoment5` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `WeighedMoment6` `WeighedMoment6` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `WeighedMoment7` `WeighedMoment7` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `Fourier0` `Fourier0` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `Fourier1` `Fourier1` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `Fourier2` `Fourier2` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `Fourier3` `Fourier3` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `Fourier4` `Fourier4` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `ContourFourierDescriptor0` `ContourFourierDescriptor0` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `ContourFourierDescriptor1` `ContourFourierDescriptor1` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `ContourFourierDescriptor2` `ContourFourierDescriptor2` double NULL DEFAULT NULL  ,
     CHANGE COLUMN `ContourFourierDescriptor3` `ContourFourierDescriptor3` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `ContourFourierDescriptor4` `ContourFourierDescriptor4` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `FourierDescRight1` `FourierDescRight1` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `    FourierDescLeft2` `FourierDescLeft2` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `FourierDescRight2` `FourierDescRight2` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `FourierDescLeft3` `FourierDescLeft3` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `FourierDescRight3` `FourierDescRight3` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `FourierDescLeft4` `FourierDescLeft4` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `FourierDescRight4` `FourierDescRight4` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `FourierDescLeft5` `FourierDescLeft5` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `FourierDescRight5` `FourierDescRight5` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `FourierDescLeft6` `FourierDescLeft6` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `FourierDescRight6` `FourierDescRight6` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `FourierDescLeft7` `FourierDescLeft7` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `FourierDescRight7` `FourierDescRight7` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `FourierDescLeft8` `FourierDescLeft8` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `FourierDescRight8` `FourierDescRight8` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `IntensityHist1` `IntensityHist1` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `IntensityHist2` `IntensityHist2` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `IntensityHist3` `IntensityHist3` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `IntensityHist4` `IntensityHist4` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `IntensityHist5` `IntensityHist5` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `IntensityHist6` `IntensityHist6` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `IntensityHist7` `IntensityHist7` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `HeightVsWidth`  `HeightVsWidth` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `Length`          `Length` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `Width` `Width` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `FilledArea` `FilledArea` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `FlowRate1` `FlowRate1` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `FlowRate2` `FlowRate2` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `IntensityHistB0` `IntensityHistB0` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `IntensityHistB1` `IntensityHistB1` double NULL DEFAULT NULL  ,
     CHANGE COLUMN `IntensityHistB2` `IntensityHistB2` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `IntensityHistB3` `IntensityHistB3` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `IntensityHistB4` `IntensityHistB4` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `IntensityHistB5` `IntensityHistB5` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `IntensityHistB6` `IntensityHistB6` double NULL DEFAULT NULL  ,
     CHANGE COLUMN `IntensityHistB7` `IntensityHistB7` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `Depth` `Depth` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `Salinity` `Salinity` double NULL DEFAULT NULL  , 
     CHANGE COLUMN `Oxygen` `Oxygen` double NULL DEFAULT NULL  ;
     change COLUMN 'Florences' Florences  double NULL DEFAULT NULL  ;

