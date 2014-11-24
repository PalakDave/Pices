/* PicesApplication.cpp -- Generic PicesApplication class.  
 * Copyright (C) 1994-2011 Kurt Kramer
 * For conditions of distribution and use, see copyright notice in KKU.h
 */
#include "FirstIncludes.h"

#include <stdlib.h>
#include <stdio.h>
#include <string>
#include <iostream>
#include <fstream>
#include <vector>

#include "MemoryDebug.h"
using  namespace  std;

#include "OSservices.h"
using  namespace  KKU;


#include "PicesApplication.h"
#include "DataBase.h"
#include "DataBaseServer.h"
#include "FeatureFileIOPices.h"
#include "FileDesc.h"
#include "TrainingConfiguration2.h"
using namespace MLL;



PicesApplication::PicesApplication (RunLog&  _log):
  Application (_log),
  config             (NULL),
  configFileName     (),
  configFileFullPath (),
  db                 (NULL),
  dbServer           (NULL),
  dataBaseRequired   (false),
  fileDesc           (NULL)
{
}



PicesApplication::PicesApplication ():
  Application (),
  config             (NULL),
  configFileName     (),
  configFileFullPath (),
  db                 (NULL),
  dbServer           (NULL),
  dataBaseRequired   (false),
  fileDesc           (NULL)
{
  fileDesc = FeatureFileIOPices::NewPlanktonFile (log);
}




PicesApplication::PicesApplication (const PicesApplication&  _application):
  Application (_application),
  config             (NULL),
  configFileName     (_application.configFileName),
  configFileFullPath (_application.configFileFullPath),
  db                 (NULL),
  dbServer           (NULL),
  dataBaseRequired   (_application.dataBaseRequired),
  fileDesc           (_application.fileDesc)
{
  if  (_application.db)
    db = new DataBase (_application.db->Server (), log);

  if  (_application.dbServer)
    dbServer = new DataBaseServer (*_application.dbServer);

  if  (_application.config)
    config = new TrainingConfiguration2 (*config);
}



PicesApplication::~PicesApplication ()
{
  delete  config;    config   = NULL;
  delete  dbServer;  dbServer = NULL;
  delete  db;        db       = NULL;
}




void  PicesApplication::InitalizeApplication (int32   argc,
                                              char**  argv
                                             )
{
  Application::InitalizeApplication (argc, argv);

  if  (!configFileName.Empty ())
  {
    config = new TrainingConfiguration2 (fileDesc, configFileFullPath, log, true);
    if  (!(config->FormatGood ()))
    {
      log.Level (-1) << endl
                     << "PicesApplication::InitalizeApplication   ***ERROR***   Configuration file[" << configFileName << "] is not valid." << endl
                     << endl;
      Abort (true);
    }
  }

  if  (dataBaseRequired)
  {
    if  (!db)
    {
      db = new DataBase (log);
    }

    if  (!db->Valid ())
    {
      log.Level (-1) << endl
        << "PicesApplication::InitalizeApplication   ***ERROR***   A valid database connection is required for this application." << endl
        << endl;
      Abort (true);
    }
    else
    {
      if  (!dbServer)
        dbServer = new DataBaseServer (*(db->Server ()));
    }
  }
}



const char*  PicesApplication::ApplicationName ()  const
{
  return  "PicesApplication";
}  /* ApplicationName */




bool  PicesApplication::ProcessCmdLineParameter (const KKStr&  parmSwitch, 
                                                 const KKStr&  parmValue
                                                )
{
  if  (parmSwitch.EqualIgnoreCase ("-DataBase")  ||  
       parmSwitch.EqualIgnoreCase ("-DB")
      )
    return ProcessDataBaseParameter (parmSwitch, parmValue);

  else if  
      (parmSwitch.EqualIgnoreCase ("-C")       ||  
       parmSwitch.EqualIgnoreCase ("-CONFIG")  ||
       parmSwitch.EqualIgnoreCase ("-CONFIGFILE")
      )
    return ProcessConfigFileParameter (parmSwitch, parmValue);

  else
    return Application::ProcessCmdLineParameter (parmSwitch, parmValue);
}



bool  PicesApplication::ProcessDataBaseParameter (const KKStr&  parmSwitch, 
                                                  const KKStr&  parmValue
                                                 )
{
  delete  dbServer;  dbServer = NULL;
  delete  db;        db       = NULL;

  bool  validDataBase = false;

  if  (parmValue.Empty ())
  {
    // User wants to us default database server
    db = new DataBase (log);
  }

  else
  {
    dbServer = DataBase::GetServer (parmValue, log);
    if  (!dbServer)
    {
      log.Level (-1) << endl
        << "PicesApplication::ProcessDataBaseParameter  ***ERROR***    Invalid DataBase Sepcified Name [" << parmValue << "]." << endl
        << endl;
      return  false;
    }
    else
    {
      db = new DataBase (dbServer, log);
      if  (db->Valid ())
      {
        validDataBase = true;
      }
      else
      {
        log.Level (-1) << endl
          << "PicesApplication::ProcessDataBaseParameter  ***ERROR***    Could not connect to DataBase [" << parmValue << "]." << endl
          << "     : " << db->LastErrorDesc () << endl
          << endl;
      }
    }
  }

  return  validDataBase;
}  /* ProcessDataBaseParameter */



void  PicesApplication::DisplayCommandLineParameters ()
{
  Application::DisplayCommandLineParameters ();

  cout << endl
       << "    -DataBase  <DB-Description>  Specify the specific DB Server to connect to; 'DB-Description' will specify"  << endl
       << "                                 which entry in the 'MySql.cfg' configuration file to get parameters from."    << endl
       << "                                 This file is located in the ${PicesHomDir}\\Configurations directory."        << endl
       << "                                 If parameter not specifoed will default to the last DataBase sertver"         << endl
       << "                                 specified."                                                                   << endl
       << endl
       << "    -Config   <Config File>      Specify the name of teh Training Model Configuration file to use. These"      << endl
       << "                                 files can be found in the   ${PicesHomeDir}\\DataFiles\\TrainingModels"       << endl
       << "                                 subdirectory."                                                                << endl
       << endl;
}



bool  PicesApplication::ProcessConfigFileParameter  (const KKStr&  parmSwitch, 
                                                     const KKStr&  parmValue
                                                    )
{
  configFileName = parmValue;
  configFileFullPath = TrainingConfiguration2::GetEffectiveConfigFileName (configFileName);

  bool  valid = TrainingConfiguration2::ConfigFileExists (configFileFullPath);

  if  (!valid)
  {
    log.Level (-1) << endl
      << "PicesApplication::ProcessConfigFileParameter   ***ERROR***  Configuration[" << configFileName << "]  Does not exist." << endl
      << endl;
  }

  return  valid;
}  /* ProcessConfigFileParameter*/




DataBasePtr   PicesApplication::DB ()
{
  if  (db == NULL)
  {
    db = new DataBase (log);
  }
  return  db;
}



void  PicesApplication::PrintStandardHeaderInfo (ostream&  o)
{
  o << "Application:"     << "\t" << ApplicationName    () << endl;
  o << "Build Date/Time"  << "\t" << BuildDate          () << endl;
  o << "Run Date/Time"    << "\t" << osGetLocalDateTime () << endl; 
  o << "Host Name"        << "\t" << osGetHostName      () << endl;
  o << "User Name"        << "\t" << osGetUserName      () << endl;
  if  (db)
    o << "DataBase"       << "\t" << db->ServerDescription () << endl;

  if  (!configFileName.Empty ())
    o << "ConfigFileName" << "\t" << configFileName << endl;

  if  (!configFileFullPath.Empty ())
  {
    o << "ConfigFileFullPath"    << "\t" << configFileFullPath << endl;
    o << "Config File TimeStamp" << "\t" << osGetFileDateTime (configFileFullPath) << endl;
  }
  if  (config)
  {
    o << "Model Type"              << "\t" << config->ModelTypeStr          ()  << endl;
    o << "SVM Parameters"          << "\t" << config->ModelParameterCmdLine ()  << endl;
    o << "Num Hierarchial Levels"  << "\t" << config->NumHierarchialLevels  ()  << endl;
    if  (config->OtherClass ())
      o << "Other Class"  << "\t" << config->OtherClass ()->Name () << endl;
  }
}  /* PrintStandardHeaderInfo */
