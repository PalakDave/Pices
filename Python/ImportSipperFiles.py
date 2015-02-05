import mysql.connector
from mysql.connector import errorcode
import datetime
import os



def  ToFloat(s):
  f=0.0
  try:
    f=float(s)
  except  ValueError:
    f=0.0
  return f



def  ToInt(s):
  i=0;
  try:
    i=int(s)
  except  ValueError:
    i=0
  return i



def  DateFromMDYStr(s):
  d = datetime.date(1,1,1)
  try:
    d=datetime.datetime.strptime(s.strip(),"%m/%d/%Y").date()
  except  ValueError:
    d=datetime.date(1900,1,1)
  return d




def  DateFromYMDStr(s):
  d = datetime.date(1,1,1)
  try:
    d=datetime.datetime.strptime(s.strip(),"%Y/%m/%d").date()
  except  ValueError:
    d=datetime.date(1900,1,1)
  return d




def  DateFromStr(s):
  d = datetime.date(1,1,1)
  try:
    d=datetime.datetime.strptime(s.strip(),"%m/%d/%Y").date()
  except  ValueError:
    try:
      d=datetime.datetime.strptime(s.strip(),"%m/%d/%Y").date()
    except  ValueError:
      try:
        d=datetime.datetime.strptime(s.strip(),"%Y/%m/%d").date()
      except  ValueError:
        try:
          d=datetime.datetime.strptime(s.strip(),"%Y-%m-%d").date()
        except  ValueError:
          try:
            d=datetime.datetime.strptime(s.strip(),"%d-%m-%Y").date()
          except  ValueError:
            d=datetime.date(1900,1,1)
  return d




def  TimeFromStr(s):
  t=datetime.time(0,0,0)
  try:
    t=datetime.datetime.strptime(s.strip(),"%H:%M:%S").time()
  except  ValueError:
    t=datetime.time(0,0,0)
  return t




def  DateTimeFromStr (s):
  s = s.strip("\"")
  d = datetime.date (1,1,1)
  t = datetime.time (0,0,0)
  fields = s.split(' ')
  if  len(fields) > 1:
    d=DateFromStr(fields[0])
    t=TimeFromStr(fields[1])
  dt=datetime.datetime(d.year, d.month, d.day, t.hour, t.minute, t.second)
  return dt



def  TimeFromHHMMSS(s):
  i=int(ToFloat(s))
  secs = i % 100
  i = int (i / 100)
  mins = i % 100
  i = int (i / 100)
  hours = i
  return datetime.time(hours,mins,secs)





def  SipperFileExist (db, c, _sipperFileIdExt, _sipperFileName):
  sipperFileIdOF = -1
  sipperFileNameOF = ""
  sipperFileInDB = False

  _sipperFileName = _sipperFileName.strip('"')

  sipperFileIdOur = -1

  sqlStr = ('select SipperFileId, SipperFileName  from  SipperFiles  where  SipperFileName = "' + _sipperFileName + '"')
  c.execute(sqlStr)
  for  (sipperFileIdOF, sipperFileNameOF)  in  c:
    if  (sipperFileNameOF == _sipperFileName):
      sipperFileInDB = True
      sipperFileIdOur = sipperFileIdOF

  return  sipperFileIdOur





#Server	Description:Default	Embedded:No	HostName:localhost	UserName:root	PassWord:Dasani30!	PortNum: 3306	MySqlDataDir:	DataBaseName:pices_new



def  ImportLogEntries(dirName):
  try:
       #db = mysql.connector.Connect(user='kkramer',
       #                             password="tree10peach",
       #                             host='sipper-db2.marine.usf.edu',
       #                             database='pices_new'
       #                          )
              
       #db = mysql.connector.Connect(user='kkramer',
       #                             password="tree10peach",
       #                             host='sipper-db2.marine.usf.edu',
       #                             database='pices_new'
       #                           )
       db = mysql.connector.Connect(user='kkramer',
                                    password="tree10peach",
                                    host='localhost',
                                    database='pices_new'
                                  )
  except  mysql.connector.Error as err:
    db = None
    if  err.errno == errorcode.ER_ACCESS_DENIED_ERROR:
        print("Cound not conect to \"Sipper-d-marine.usf.edu\"")
    elif  err.errno == errorcode.ER_BAD_DB_ERROR:
        print("Database does not exists")
    else:
        print(err)

  if db is None:
    return

  c = db.cursor()

  sipperFileName = os.path.join(dirName, "SipperFiles") + ".txt"
  sipperFileData = open(sipperFileName)

  count = 0

  for  l in sipperFileData:
    print(l)
    fields=l.split('\t')
    fieldCount = len(fields)
    if  len(fields) > 13:
      sipperFileIdExt     = ToInt(fields[0])
      sipperFileName      = fields[1]
      cruiseName          = fields[2]
      stationName         = fields[3]
      deploymentNum       = fields[4]  # eval('"' + fields[4].replace('"','') + '"')
      description         = fields[5]
      latitude            = fields[6]
      longitude           = fields[7]
      dateTimeStart       = fields[8]

      sp0Instrument       = fields[9]
      sp1Instrument       = fields[10]
      sp2Instrument       = fields[11]
      sp3Instrument       = fields[12]
      
      ctdExt0             = ToInt(fields[13])
      ctdExt1             = ToInt(fields[14])
      ctdExt2             = ToInt(fields[15])
      ctdExt3             = ToInt(fields[16])

      sizeInBytes             = ToInt(fields[17])
      numScanLines            = ToInt(fields[18])
      scanRate                = ToFloat(fields[19])
      depth                   = ToFloat(fields[20])
      extractionStatus        = fields[21]
      extractionScanLineStart = ToInt(fields[22])
      extractionScanLineEnd   = ToInt(fields[23])

      #Make sure that this entry is not already in the database.
      sipperFileIdOur = SipperFileExist (db, c, sipperFileIdExt, sipperFileName.strip("\""))
      if  (sipperFileIdOur >= 0):
        print(sipperFileName + " already in database  ExternalId :" + str(sipperFileIdExt) + "  OurId :" + str(sipperFileIdOur))
      else:
        sqlStr = ("insert into SipperFiles(sipperFileName, ProgName, cruiseName, stationName, deploymentNum, " +
                  "description, latitude, longitude, dateTimeStart, sp0Instrument, sp1Instrument, sp2Instrument, sp3Instrument, " +
                  "ctdExt0, ctdExt1, ctdExt2, ctdExt3, sizeInBytes, numScanLines, scanRate, depth, extractionStatus, " +
                  " extractionScanLineStart, extractionScanLineEnd)" + " " +
           "values(" +
                    sipperFileName + ", " +
                    cruiseName     + ", " +
                    stationName    + ", " +
                    deploymentNum  + ", " +
                    description    + ", " +
                    latitude       + ", " +
                    longitude      + ", " +
                    dateTimeStart  + ", " +

                    sp0Instrument  + ", " +
                    sp1Instrument  + ", " +
                    sp2Instrument  + ", " +
                    sp3Instrument  + ", " +
      
                    ctdExt0        + ", " +
                    ctdExt1        + ", " +
                    ctdExt2        + ", " +
                    ctdExt3        + ", " +

                    sizeInBytes    + ", " +
                    numScanLines   + ", " +
                    scanRate       + ", " +
                    depth          + ", " +
                    extractionStatus  + ", " +
                    extractionScanLineStart  + ", " +
                    extractionScanLineEnd +
                  ")"
                  )
        print (sqlStr)
        try:
          c.execute(sqlStr)
          db.commit()
        except  mysql.connector.Error  as err:
          print(err)
  sipperFileData.close ()



def  main():
  #rootDir="E:\\Users\\kkramer\\Dropbox\\Sipper\\FromAndrewToKurt\\Validation\\2014-09-16\\"
  #rootDir="F:\\Pices\\UpdatesFromOtherServers\\FromAndrews"
  rootDir="C:\\Pices\\UpdatesFromOtherServers"
  ImportLogEntries(rootDir)
  print("\n\n    *** Import Completed ***\n\n")


main()

