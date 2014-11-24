#if  !defined(_PICESAPPLICATION_)
#define  _PICESAPPLICATION_
/**
 *@class  MLL::PicesApplication
 *@brief Base class to All PICES applications.
 *@author  Kurt Kramer
 */

#include "Application.h"
#include "BasicTypes.h"
#include "Str.h"


namespace MLL
{
#if  !defined(_DataBase_Defined_)
  class  DataBase;
  typedef  DataBase*  DataBasePtr;
  typedef  DataBase const *  DataBaseConstPtr;
#endif

#if  !defined(_DataBaseServer_Defined_)
  class  DataBaseServer;
  typedef  DataBaseServer*        DataBaseServerPtr;
  typedef  DataBaseServer const * DataBaseServerConstPtr;
#endif

  /** 
   *@class PicesApplication
   *@brief The base class for all standalone application.
   *@details  This class is meant to be a general class that all standalone applications should be inherited 
   *          from.  It supports command line processing, and logging facilities.
   */

  class  PicesApplication: public  Application
  {
  public:
    /** 
     *@brief  Constructor for Application class taht will start with a default logger(RunLog),
     *@details After creating an instance of this class you intialize it by calling InitalizeApplication.
     */
    PicesApplication ();


    /** 
     *@brief  Copy Constructor for PicesApplication class.
     *@param[in]  _application  PicesApplication instance to copy.
     */
    PicesApplication (const PicesApplication&  _application);


    /**
     *@brief  Constructor for PicesApplication class with no parameters.
     *@details This constructor is not interested in any command line parameters.
     *@param[in]  _log  A reference to a RunLog object.
     */
    PicesApplication (RunLog&  _log);



    virtual
    ~PicesApplication ();


    /** Specify the name of the application */
    virtual 
    const char*  ApplicationName () = 0;

    /** 
     *@brief  Initialized PicesApplication Instance; 1st method to be called after instance construction.
     *@see  RunLog
     *@see  ProcessCmdLineParameters
     *@param[in]  argc  Number of arguments in argv.
     *@param[in]  argv  List of asciiz strings; one string for each argument.
     */
    virtual
    void     InitalizeApplication (int32   argc,
                                   char**  argv
                                  );


    DataBasePtr   DB ();

    DataBaseServerPtr  DbServer ()  const  {return dbServer;}


    void  PrintStandardHeaderInfo (ostream&  o);


  protected:

    /**
     *@brief This method will get called once for each parameter specified in the command line.
     *@details  Derived classes should define this method to intercept parameters that they are interested in.
     *          Parameters are treated as pairs, Switched and Values where switches have a leading dash("-").
     *          The CmdLineExpander class will be used to expand any "-CmdFile" parameters.
     *@param[in] parmSwitch      The fill switch parameter.
     *@param[in] parmValue       Any value parameter that followed the switch parameter.
     */
    virtual 
    bool     ProcessCmdLineParameter (const KKStr&   parmSwitch, 
                                      const KKStr&   parmValue
                                     );

    bool  ProcessDataBaseParameter (const KKStr&  parmSwitch, 
                                    const KKStr&  parmValue
                                   );


  private:
    DataBasePtr        db;
    DataBaseServerPtr  dbServer;

  };  /* PicesApplication */
}  /* NameSpace KKU */

#endif